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

```bash
# Frontend
cd fe
npm install
npm run dev

# Backend — từng service trong be/Services/...
```

## Mỗi service gồm 4 layers

```
ServiceX.Api/
ServiceX.Application/     ← IUnitOfWork, Interfaces, Services
ServiceX.Domain/          ← Entities
ServiceX.Infrastructure/  ← DbContext, UnitOfWork, Repositories
```
