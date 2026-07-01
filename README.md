# PRN232ASM — Scientific Journal Publication Trend Tracking System

Kiến trúc: **Microservices** (Monorepo)

## Cấu trúc dự án

```
PRN232ASM/
├── fe/                         # Frontend (React + Vite + Tailwind)
│   ├── src/
│   ├── public/
│   └── package.json
│
├── be/                         # Backend (Microservices)
│   ├── Gateway/ApiGateway/
│   ├── BuildingBlocks/
│   ├── Services/
│   │   ├── AuthService/
│   │   ├── PaperService/
│   │   ├── TrendService/
│   │   ├── NotificationService/
│   │   └── SyncService/
│   ├── tests/
│   └── backend.sln
│
├── docker/
├── docs/
└── README.md
```

## Services & Database (Supabase)

| Service              | Port | Database    |
|----------------------|------|-------------|
| ApiGateway           | 5000 | —           |
| AuthService          | 5001 | auth_db     |
| PaperService         | 5002 | paper_db    |
| TrendService         | 5003 | trend_db    |
| NotificationService  | 5004 | notify_db   |
| SyncService          | 5005 | sync_db     |

## Chạy local

Cần **3 terminal** để trang Papers hoạt động đầy đủ:

```bash
# Terminal 1 — AuthService (port 5131)
cd be/Services/AuthService/AuthService.Api
dotnet run

# Terminal 2 — SyncService (port 5005) — bắt buộc cho danh sách / tìm kiếm OpenAlex
cd be/Services/SyncService/SyncService.Api
dotnet run

# Terminal 3 — Frontend (port 5173)
cd fe
npm install
npm run dev
```

Sau khi sửa code SyncService, **restart** terminal 2 (`Ctrl+C` rồi `dotnet run` lại).

## Mỗi service gồm 4 layers

```
ServiceX.Api/
ServiceX.Application/     ← IUnitOfWork, Interfaces, Services
ServiceX.Domain/          ← Entities
ServiceX.Infrastructure/  ← DbContext, UnitOfWork, Repositories
```
