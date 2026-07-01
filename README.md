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

Cấu hình `.env` (bắt buộc)

- Backend dùng file `be/.env` (không commit).
- Frontend dùng file `fe/.env` (không commit).

### Backend: `be/.env`

Tạo file `be/.env` theo mẫu `be/.env.example` (nếu chưa có thì tạo mới dựa trên nội dung đang dùng local). Các biến quan trọng:

- **DB**: `ConnectionStrings__DefaultConnection`
- **JWT**: `JWT__Secret`, `JWT__Issuer`, `JWT__Audience`, `JWT__AccessTokenExpirationMinutes`
- **OpenAlex** (tuỳ chọn / có thể tắt): `OpenAlex__Enabled=false`
- **PaperService URL** (để SyncService import): `PaperService__BaseUrl=http://localhost:5002`

### Frontend: `fe/.env`

Copy từ `fe/.env.example` → `fe/.env`. Các biến quan trọng:

- **Auth API**: `VITE_API_URL=http://localhost:5131`
- **Paper API**: `VITE_PAPER_API_URL=http://localhost:5002`
- **Trend API**: `VITE_TREND_API_URL=http://localhost:5003` (hoặc để trống để dùng Vite proxy `/api/trends`)
- **Tắt OpenAlex (free-only)**: `VITE_OPENALEX_ENABLED=false`

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
