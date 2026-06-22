# Kết nối Supabase — điền sau

## Mỗi service dùng connection string riêng trong appsettings.Development.json

| Service             | Database    |
|---------------------|-------------|
| AuthService         | auth_db     |
| PaperService        | paper_db    |
| TrendService        | trend_db    |
| NotificationService | notify_db   |
| SyncService         | sync_db     |

## Ví dụ format (PostgreSQL / Supabase)

```
Host=db.xxxx.supabase.co;Port=5432;Database=auth_db;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```
