# SYSTEM ARCHITECTURE DOCUMENT
## Paper Trend Tracker (PRN232ASM)

> Đối chiếu với mã nguồn và `docker/docker-compose.yml` hiện tại (gRPC Trend↔Paper, RabbitMQ Outbox, SQL Server, Hangfire SQL Server, queue theo service).

---

# 1. System Context View

**Paper Trend Tracker** là nền tảng theo dõi xu hướng nghiên cứu khoa học: người dùng tìm kiếm/bookmark paper, follow topic/keyword/journal, nhận thông báo, xem trend/report; Admin quản trị user, datasource và đồng bộ dữ liệu từ OpenAlex.

## Hệ thống phục vụ các đối tượng

| Actor | Vai trò |
|-------|---------|
| **Anonymous User** | Xem landing, đăng ký, đăng nhập |
| **Student / Researcher** | Dashboard, search & view papers, bookmark, follow topic/keyword/journal, notifications, trends & reports, profile |
| **Admin** | *(kế thừa toàn bộ chức năng Student/Researcher)* + quản lý user/role, journal/topic (xem), datasource, trigger/schedule sync OpenAlex, monitoring |
| **System / Background Job** | Hangfire: sync OpenAlex daily (SyncService); aggregate trend mỗi 6 giờ (TrendService); CleanupOldNotificationsJob; OutboxDispatcherHostedService |

## External System / Hạ tầng kết nối

| Thành phần | Mục đích | Ghi chú |
|------------|----------|---------|
| **OpenAlex API** (`https://api.openalex.org`) | Nguồn works khoa học để SyncService import | **Hệ thống bên ngoài duy nhất** đang kết nối thật (HTTPS REST) |
| **SQL Server (Docker)** | DB theo service: `AuthDb`, `PaperDb`, `TrendDb`, `NotificationDb`, `SyncDb` (+ Hangfire/Outbox cùng DB) | Container `sqlserver`, port `1433` |
| **RabbitMQ** | Message broker AMQP — integration events | Hạ tầng nội bộ (Docker Compose) |
| **Browser / React Frontend** | UI người dùng | `:5173` |
| **Docker Desktop / Compose** | Triển khai backend (+ optional FE) | Project name `prn232asm` |
| **Hangfire Dashboard** | Theo dõi job Trend/Sync | `/hangfire` |
| **RabbitMQ Management UI** | Queue/exchange | `http://localhost:15672` |

## Người dùng truy cập hệ thống thông qua thành phần nào?

Người dùng truy cập qua **React Frontend** (`http://localhost:5173`).  
Frontend gọi **API Gateway** tại `http://localhost:5000` (khai báo trong `fe/src/lib/api.js`).  
**Không** gọi thẳng từng microservice.

```js
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'
```

## Phạm vi hệ thống gồm

- Frontend React/Vite  
- API Gateway (YARP)  
- AuthService, PaperService, TrendService, NotificationService, SyncService  
- SQL Server (database-per-service trên Docker)  
- RabbitMQ + **Transactional Outbox**  
- Hangfire (TrendService, SyncService) — storage **SQL Server**  
- gRPC nội bộ: `paper_catalog.proto` (TrendService ↔ PaperService)  

## Context Diagram

```
┌──────────────┐  ┌───────────────────┐  ┌─────────────┐
│ Anonymous    │  │ Student /         │  │   Admin     │
│ User         │  │ Researcher        │  │             │
└──────┬───────┘  └─────────┬─────────┘  └──────┬──────┘
       │                    │                   │
       └────────────────────┼───────────────────┘
                            │ HTTP (Browser)
                            ▼
                  ┌─────────────────────┐
                  │ Frontend React/Vite │
                  │     :5173           │
                  └──────────┬──────────┘
                             │ HTTP REST + JWT
                             │ http://localhost:5000
                             ▼
     ┌───────────────────────────────────────────────┐
     │              PRN232ASM System                 │
     │  API Gateway (YARP)                           │
     │  Auth · Paper · Trend · Notification · Sync   │
     │  RabbitMQ · Outbox · SQL Server · Hangfire    │
     └───────────────────────┬───────────────────────┘
                             │ HTTPS REST
                             ▼
                  ┌─────────────────────┐
                  │    OpenAlex API     │
                  │ api.openalex.org    │
                  └─────────────────────┘
```

*(Chèn ảnh Context Diagram vào Word tại đây.)*

## Use Case Diagram (tóm tắt)

| Actor | Use cases |
|-------|-----------|
| Anonymous | Register / Login |
| Student / Researcher | Search & View Papers; Bookmark; Follow Topic/Keyword/Journal; View Notifications; View Trends & Reports; Dashboard / Profile |
| Admin | *(kế thừa Student/Researcher)* + Manage Users & Roles; Manage Data Sources; Trigger / Schedule OpenAlex Sync; Monitoring |

**Không có trong hệ thống:** Review Pending Papers, Approve/Reject Imported Papers.

*(Chèn ảnh Use Case Diagram vào Word tại đây.)*

### Minh chứng Context

| Evidence | Đường dẫn |
|----------|-----------|
| FE API URL | `fe/src/lib/api.js` |
| Routes / actors | `fe/src/App.jsx`, `AdminRoute.jsx` |
| Roles | `AuthService.Domain/Entities/Role.cs` |
| YARP | `be/Gateway/ApiGateway/Configuration/yarp.json` |
| OpenAlex | `docker/docker-compose.yml`, `OpenAlexClient.cs` |

---

# 2. Runtime View

## Số lượng Services

**6 component backend** (5 microservice nghiệp vụ + 1 API Gateway):

| Service | Chức năng chính |
|---------|-----------------|
| **ApiGateway** | Điểm vào HTTP duy nhất, validate JWT, inject `X-User-Id`, reverse proxy YARP |
| **AuthService** | Register/login, refresh token, logout, change-password, quản lý user/role |
| **PaperService** | Search/create/import papers; authors, journals, keywords, topics, bookmarks; **gRPC server** `PaperCatalog`; Outbox publish `PaperCreatedEvent` |
| **TrendService** | Trends, analytics, dashboard, reports; Hangfire aggregate; **gRPC client**; consume `PaperCreated` / `UserFollowedTopic`; Outbox publish `TrendUpdatedEvent` |
| **NotificationService** | Notifications + follows; consume paper/trend events; Outbox publish `UserFollowedTopicEvent` |
| **SyncService** | Datasources, trigger sync, logs/status; gọi OpenAlex; import paper qua REST; Hangfire daily; Outbox publish `NewPaperDetectedEvent` |

## Runtime Architecture Diagram

```
User → Frontend → API Gateway (YARP)
                      │ HTTP
        ┌─────────────┼─────────────┬──────────────┬────────────┐
        ▼             ▼             ▼              ▼            ▼
      Auth         Paper          Trend       Notification    Sync
        │             │             │              │            │
     auth.db      paper.db      trend.db      notify.db      sync.db
                      │             │              │            │
                      └────── OutboxMessages (per DB) ──────────┘
                                      │
                                      ▼ (dispatcher)
                                 RabbitMQ (AMQP)
                                      │
                    ┌─────────────────┴─────────────────┐
                    ▼                                   ▼
              Notification                         Trend

Sync ──HTTPS──► OpenAlex
Sync ──HTTP POST /api/papers/import──► Paper
Trend ──gRPC SearchPapers──► Paper
```

## Communication Matrix

| Nguồn | Đích | Giao thức | Mục đích |
|-------|------|-----------|----------|
| React Frontend | ApiGateway | REST/HTTP | Gọi API (`VITE_API_URL`) |
| ApiGateway | AuthService | REST proxy | `/api/auth`, `/api/users` |
| ApiGateway | PaperService | REST proxy | `/api/papers`, authors, journals, keywords, topics, bookmarks |
| ApiGateway | TrendService | REST proxy | `/api/trends`, analytics, dashboard, reports |
| ApiGateway | NotificationService | REST proxy | `/api/notifications`, `/api/follows` |
| ApiGateway | SyncService | REST proxy | `/api/sync`, `/api/datasources` |
| SyncService | OpenAlex API | HTTPS REST | Fetch works |
| SyncService | PaperService | REST | `POST /api/papers/import` |
| TrendService | PaperService | **gRPC** | `PaperCatalog.SearchPapers` (aggregate trend) |
| Paper / Sync / Notif / Trend | **Outbox (SQL Server)** | EF Core | Ghi event cùng transaction nghiệp vụ |
| Outbox Dispatcher | RabbitMQ | AMQP Publish | Gửi event từ Outbox (retry nếu fail) |
| PaperService | RabbitMQ | AMQP (qua Outbox) | `PaperCreatedEvent` |
| SyncService | RabbitMQ | AMQP (qua Outbox) | `NewPaperDetectedEvent` |
| NotificationService | RabbitMQ | AMQP (qua Outbox) | `UserFollowedTopicEvent` |
| TrendService | RabbitMQ | AMQP (qua Outbox) | `TrendUpdatedEvent` |
| RabbitMQ | NotificationService | **AMQP Consume** | Tạo notification từ paper/trend events |
| RabbitMQ | TrendService | **AMQP Consume** | `PaperCreatedEvent` → incremental trend; `UserFollowedTopicEvent` |
| Các service | SQL Server | EF Core | Lưu dữ liệu riêng (DB riêng từng service) |

**Không dùng gRPC** cho FE/Gateway.  
**Không dùng Kafka / Redis.** SQL Server chạy trong Docker Compose.

### Queue naming (tránh competing consumer)

Queue = `{exchange}_{QueuePrefix}_{EventName}`

- Notification: `prn232asm_event_bus_notification_PaperCreatedEvent`  
- Trend: `prn232asm_event_bus_trend_PaperCreatedEvent`  

→ Cả hai service đều nhận bản copy của `PaperCreatedEvent`.

## Transactional Outbox

```
Cùng 1 DB transaction:
  INSERT nghiệp vụ (paper / follow / …)
  INSERT OutboxMessages
  → COMMIT

OutboxDispatcherHostedService (mỗi ~2s):
  đọc Outbox chưa gửi → Publish RabbitMQ → ProcessedAt
  (fail → RetryCount++, gửi lại)
```

Áp dụng: PaperService, SyncService, NotificationService, TrendService.  
Minh chứng: `be/BuildingBlocks/EventBus/Outbox/`.

## Background Workers

| Service | Worker |
|---------|--------|
| **SyncService** | Hangfire SQL Server: `daily-openalex-sync` — `Cron.Daily` → `ScheduledSyncJob` |
| **TrendService** | Hangfire SQL Server: `trend-aggregation` — `0 */6 * * *` + enqueue startup → `TrendAggregationJob` (gRPC Paper) |
| **NotificationService** | `CleanupOldNotificationsJob` — mỗi **7 ngày**, xóa notification đã đọc > **30 ngày** |
| **Paper / Sync / Notif / Trend** | `EventBusInitializerHostedService` — kết nối RabbitMQ, đăng ký consumer |
| **Paper / Sync / Notif / Trend** | `OutboxDispatcherHostedService` — đẩy Outbox → RabbitMQ |

Hangfire Dashboard:  
- Trend: `http://localhost:5003/hangfire` (JWT Bearer)  
- Sync: `http://localhost:5005/hangfire` (Development)  

Storage: `hangfire-trend.db`, `hangfire-sync.db` (không dùng MemoryStorage).

## Event Publisher / Consumer

| Event | Publisher | Consumer |
|-------|-----------|----------|
| `PaperCreatedEvent` | PaperService (Outbox) | NotificationService, TrendService |
| `NewPaperDetectedEvent` | SyncService (Outbox) | NotificationService |
| `TrendUpdatedEvent` | TrendService (Outbox) | NotificationService |
| `UserFollowedTopicEvent` | NotificationService (Outbox) | TrendService |

**Đã loại bỏ:** `PaperImportedEvent`, `NotificationEvent` (orphan / không dùng).

**Broker:** RabbitMQ — exchange topic `prn232asm_event_bus`.

## Sequence Diagram 1: Login

```
User → Frontend: email/password
Frontend → ApiGateway: POST /api/auth/login
ApiGateway → AuthService: YARP proxy
AuthService → auth.db: verify user + issue JWT/refresh
AuthService → Frontend: TokenResponse
Frontend: lưu session → Dashboard / Admin
```

*(Chèn Sequence Login vào Word.)*

## Sequence Diagram 2: Sync OpenAlex → Import → Notify (có Outbox)

```
Admin/Hangfire → Gateway → SyncService: POST /api/sync/trigger
SyncService → OpenAlex: HTTPS GET works
SyncService → PaperService: HTTP POST /api/papers/import
PaperService: INSERT paper + Outbox(PaperCreatedEvent) → COMMIT
SyncService: Outbox(NewPaperDetectedEvent) → COMMIT (khi import thành công)
Outbox Dispatcher → RabbitMQ: AMQP publish
RabbitMQ → NotificationService: consume → INSERT notifications
RabbitMQ → TrendService: consume PaperCreated → cập nhật trend
(Định kỳ) TrendService ──gRPC SearchPapers──► PaperService → recalc trends
```

*(Chèn Sequence Sync/Import/Notify vào Word.)*

## API Contract

| Service | REST / gRPC chính |
|---------|-------------------|
| **AuthService** | `POST /api/auth/register`, `login`, `refresh-token`, `logout`, `change-password`; `GET /api/users`, `PUT /api/users/{id}/role` |
| **PaperService** | `GET/POST /api/papers`, `GET /api/papers/{id}`, `POST /api/papers/import`; `GET /api/authors\|journals\|keywords\|topics`; bookmarks CRUD |
| **PaperService (gRPC)** | `PaperCatalog.SearchPapers` — `be/BuildingBlocks/GrpcContracts/Protos/paper_catalog.proto` |
| **TrendService** | `GET /api/trends`, `/api/analytics`, `/api/dashboard`; `GET/POST /api/reports` |
| **NotificationService** | `GET /api/notifications`, `PUT .../read`, `PUT .../read-all`; follows topic/keyword/journal |
| **SyncService** | `POST /api/sync/trigger`, `GET /api/sync/logs`, `GET /api/sync/status`; `GET/PUT /api/datasources` |
| **ApiGateway** | `GET /health` + proxy toàn bộ `/api/**` |

Import paper trùng DOI → **HTTP 409 Conflict** (Sync coi là skip duplicate).

## Swagger / OpenAPI

| Service | URL (host port) |
|---------|-----------------|
| ApiGateway | `http://localhost:5000/swagger` |
| AuthService | `http://localhost:5131/swagger` |
| PaperService | `http://localhost:5002/swagger` |
| TrendService | `http://localhost:5003/swagger` |
| NotificationService | `http://localhost:5004/swagger` |
| SyncService | `http://localhost:5005/swagger` |

gRPC contract: `paper_catalog.proto` (không thay Swagger REST của Paper).

## Event Flow Diagram

```
PaperService ──Outbox──► PaperCreatedEvent ────────┐
SyncService  ──Outbox──► NewPaperDetectedEvent ────┼──► RabbitMQ
TrendService ──Outbox──► TrendUpdatedEvent ────────┤
Notification ──Outbox──► UserFollowedTopicEvent ───┘
                              │
              ┌───────────────┼───────────────┐
              ▼                               ▼
        NotificationService              TrendService
        (PaperCreated,                   (PaperCreated,
         NewPaperDetected,                UserFollowedTopic)
         TrendUpdated)
```

### Minh chứng Runtime

| Evidence | Đường dẫn |
|----------|-----------|
| YARP | `Gateway/ApiGateway/Configuration/yarp.json` |
| Event bus + QueuePrefix | `BuildingBlocks/EventBus/RabbitMQ/` |
| Outbox | `BuildingBlocks/EventBus/Outbox/` |
| gRPC proto | `BuildingBlocks/GrpcContracts/Protos/paper_catalog.proto` |
| Hangfire SQL Server | `TrendService.Api/Program.cs`, `SyncService.Api/Program.cs` |

---

# 3. Deployment View

Backend triển khai bằng **Docker Compose** (`docker/docker-compose.yml`).  
Project name: **`prn232asm`** → network mặc định **`prn232asm_default`**.  
**Không dùng Kubernetes / Kafka / Redis** trong compose này.

## Số lượng Containers

**8 containers** mặc định (gồm SQL Server).  
**9** nếu `docker compose --profile frontend up`.

| Container | Service / Image | Port mapping |
|-----------|-----------------|--------------|
| `prn232asm-sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | `1433:1433` |
| `prn232asm-rabbitmq` | `rabbitmq:3-management` | `5672:5672`, `15672:15672` |
| `prn232asm-api-gateway` | ApiGateway | `5000:8080` |
| `prn232asm-auth-service` | AuthService | `5131:8080` |
| `prn232asm-paper-service` | PaperService (REST + gRPC cùng 8080) | `5002:8080` |
| `prn232asm-trend-service` | TrendService | `5003:8080` |
| `prn232asm-notification-service` | NotificationService | `5004:8080` |
| `prn232asm-sync-service` | SyncService | `5005:8080` |
| `prn232asm-fe` *(profile `frontend`)* | Frontend | `5173:5173` |

## Docker Network

- Tên project Compose: `prn232asm` (`name: prn232asm` trong file)  
- Network bridge: **`prn232asm_default`**  
- DNS nội bộ: `sqlserver`, `auth-service`, `paper-service`, `trend-service`, `notification-service`, `sync-service`, `rabbitmq`  

Gateway / Trend trỏ: `http://paper-service:8080` (REST/gRPC), v.v.

## Database / Message queue

| Thành phần | Triển khai |
|------------|------------|
| **SQL Server** | Container `sqlserver` — DB `AuthDb` / `PaperDb` / `TrendDb` / `NotificationDb` / `SyncDb` (+ Hangfire tables cùng DB Trend/Sync) |
| **RabbitMQ** | Container `rabbitmq` |
| **OpenAlex** | Cloud HTTPS |
| Redis / Kafka | Không có |

## Docker Volumes

| Volume | Dùng cho |
|--------|----------|
| `sqlserver-data` | `sqlserver:/var/opt/mssql` — toàn bộ database SQL Server |

## Port Mapping (tóm tắt)

| Host | Service |
|------|---------|
| 1433 | SQL Server |
| 5000 | API Gateway |
| 5131 | Auth |
| 5002 | Paper |
| 5003 | Trend |
| 5004 | Notification |
| 5005 | Sync |
| 5672 / 15672 | RabbitMQ AMQP / UI |
| 5173 | Frontend (optional) |

JWT: `JWT__Secret: ${JWT_SECRET:-...}` — xem `docker/.env.example`.

## Deployment Diagram

```
[Browser] ──:5173──► [fe] ──:5000──► [api-gateway]
                                          │
     ┌────────────┬───────────┬───────────┼───────────┬────────────┐
     ▼            ▼           ▼           ▼           ▼            ▼
 auth-service paper-service trend    notification  sync-service  rabbitmq
   :5131        :5002       :5003       :5004        :5005      :5672/:15672
     │            │           │           │           │
  auth.db      paper.db   trend.db   notify.db    sync.db
                  ▲           │
                  │ gRPC      │
                  └───────────┘
 sync-service ──HTTPS──► OpenAlex
```

*(Chèn Deployment Diagram vào Word.)*

## Docker Compose Architecture

```
rabbitmq (healthy)
  ├── paper-service
  │     ├── trend-service
  │     └── sync-service
  ├── notification-service
  ├── auth-service
  └── (các service trên)
        └── api-gateway
              └── fe (profile: frontend)
```

Khởi động:

```bash
cd docker
docker compose up -d --build
# optional FE:
docker compose --profile frontend up -d
docker compose ps
```

Seed lần đầu: Auth users, ~303 papers (PaperService), OpenAlex datasource (`OpenAlex__Enabled=true` trên Compose).  
Thêm paper: Admin → Trigger Sync OpenAlex.

*(Chèn ảnh `docker compose ps` / Docker Desktop vào Word.)*

### Minh chứng Deployment

| Evidence | Đường dẫn |
|----------|-----------|
| Compose | `docker/docker-compose.yml` (`name: prn232asm`) |
| JWT env | `docker/.env.example` |
| Dockerfiles | `be/Gateway/.../Dockerfile`, `be/Services/*/.../Dockerfile`, `fe/Dockerfile` |

---

# Phụ lục — Checklist đề bài

| Yêu cầu | Có trong tài liệu? |
|---------|-------------------|
| Context + Actors + External (OpenAlex) | ✔ |
| Service Catalog (6 = 5 + Gateway) | ✔ |
| Communication Matrix (REST / gRPC / AMQP / Outbox) | ✔ |
| ≥ 2 Sequence Diagrams | ✔ Login + Sync/Import/Notify |
| Event Publisher/Consumer cập nhật | ✔ |
| Background Workers + Hangfire SQL Server + Outbox | ✔ |
| Swagger ports đúng + `.proto` | ✔ |
| Deployment 7 containers, volumes, network `prn232asm_default` | ✔ |

---

*Paper Trend Tracker (PRN232ASM) — SYSTEM ARCHITECTURE DOCUMENT*  
*Cập nhật theo codebase sau Outbox, QueuePrefix, gRPC Trend↔Paper, SQL Server + Hangfire SqlServer.*
