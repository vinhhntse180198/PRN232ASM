# PRN232ASM — Scientific Journal Publication Trend Tracking System

## Chạy local

### 1. Backend (cần .NET 9 SDK + Docker cho RabbitMQ)

```bash
# RabbitMQ
cd docker
docker compose up rabbitmq -d

# Chạy từng service (hoặc dùng Visual Studio / Rider mở be/backend.sln)
dotnet run --project be/Services/AuthService/AuthService.Api
dotnet run --project be/Services/PaperService/PaperService.Api
dotnet run --project be/Services/TrendService/TrendService.Api
dotnet run --project be/Services/NotificationService/NotificationService.Api
dotnet run --project be/Services/SyncService/SyncService.Api
dotnet run --project be/Gateway/ApiGateway
```

Hoặc full stack Docker:

```bash
cd docker
docker compose up --build
```

### 2. Frontend

```bash
cd fe
npm install
npm run dev
```

Mở http://localhost:5173

### Tài khoản demo

| Email | Password | Role |
|-------|----------|------|
| admin@prn232.asm | Admin@123 | Admin |
| user@prn232.asm | User@123 | Student |

## Ports

| Service | Port |
|---------|------|
| ApiGateway | 5000 |
| AuthService | 5131 |
| PaperService | 5002 |
| TrendService | 5003 |
| NotificationService | 5004 |
| SyncService | 5005 |
| Frontend | 5173 |
| RabbitMQ UI | 15672 |

## Lưu ý OpenAlex

- **Local dev: chỉ dùng 303 bài seed** trong PaperService
- `OpenAlex__Enabled=false` — không bật khi dev local
- Bật nhầm OpenAlex sync → DB phình → **có thể mất phí Supabase**

## Yêu cầu môn học

- 5 Microservices + YARP Gateway
- JWT Authentication
- RabbitMQ (5 events)
- Hangfire (Sync daily, Trend aggregation) + Cleanup notifications
- EF Core SQLite (local) / PostgreSQL (Supabase)
- Docker Compose
- User Web + Admin Web (React + Vite)
