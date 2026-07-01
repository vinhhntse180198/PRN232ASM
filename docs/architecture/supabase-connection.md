# Kết nối Supabase — Microservices (schema per service)

Trên **một Supabase project** dùng **PostgreSQL schemas**:

| Service             | Schema     | Search Path |
|---------------------|------------|-------------|
| AuthService         | `auth_svc` | `auth_svc`  |
| SyncService         | `sync`     | `sync`      |
| PaperService        | `paper`    | `paper`     |
| TrendService        | `trend`    | `trend`     |
| NotificationService | `notify`   | `notify`    |

> **Quan trọng:** Không dùng schema `auth` — Supabase đã dùng schema này cho hệ thống đăng nhập nội bộ (GoTrue). Tạo bảng trong `auth.*` sẽ lỗi `permission denied`.

## Setup

1. Chạy `docs/architecture/microservices-database-split.sql` trong Supabase SQL Editor
2. Đặt `USE_SUPABASE_DB=true` trong `be/.env`

## Connection string mẫu (AuthService)

```
Host=db.xxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;Search Path=auth_svc
```

Đổi `Search Path=` theo từng service.

## Dev local (SQLite)

`USE_SUPABASE_DB=false` — mỗi service dùng file `.db` riêng.
