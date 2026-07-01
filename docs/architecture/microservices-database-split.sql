-- ============================================================
-- PRN232ASM — Tách database monolithic → Microservices (schemas)
-- Chạy trong Supabase Dashboard → SQL Editor
--
-- CÁCH DÙNG:
--   A) Database MỚI (chưa có bảng): chạy phần "1" rồi "2"
--   B) Đã chạy script monolithic (bảng trong public): chạy "3" rồi "2"
-- ============================================================

-- LƯU Ý SUPABASE:
-- Schema "auth" là schema HỆ THỐNG của Supabase (GoTrue) — KHÔNG tạo bảng trong auth.*
-- AuthService dùng schema "auth_svc" thay thế.

-- ============================================================
-- 1. TẠO SCHEMA (mỗi microservice một schema)
-- ============================================================

CREATE SCHEMA IF NOT EXISTS auth_svc;
CREATE SCHEMA IF NOT EXISTS sync;
CREATE SCHEMA IF NOT EXISTS paper;
CREATE SCHEMA IF NOT EXISTS trend;
CREATE SCHEMA IF NOT EXISTS notify;

-- ============================================================
-- 2. TẠO BẢNG THEO TỪNG SERVICE (không có FK cross-schema)
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ---------- auth_svc (AuthService) ----------
CREATE TABLE IF NOT EXISTS auth_svc.roles (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(50)  NOT NULL UNIQUE,
    description TEXT,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS auth_svc.users (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    role_id       UUID         NOT NULL REFERENCES auth_svc.roles(id),
    email         VARCHAR(255) NOT NULL UNIQUE,
    password_hash TEXT         NOT NULL,
    full_name     VARCHAR(255) NOT NULL,
    avatar_url    TEXT,
    is_active     BOOLEAN      NOT NULL DEFAULT true,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at    TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS auth_svc.refresh_tokens (
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID        NOT NULL REFERENCES auth_svc.users(id) ON DELETE CASCADE,
    token      TEXT        NOT NULL UNIQUE,
    expires_at TIMESTAMPTZ NOT NULL,
    is_revoked BOOLEAN     NOT NULL DEFAULT false,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ---------- sync (SyncService) ----------
CREATE TABLE IF NOT EXISTS sync.data_sources (
    id             UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name           VARCHAR(100) NOT NULL UNIQUE,
    base_url       TEXT         NOT NULL,
    api_key        TEXT,
    is_active      BOOLEAN      NOT NULL DEFAULT true,
    last_synced_at TIMESTAMPTZ,
    created_at     TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at     TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS sync.sync_logs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    data_source_id  UUID         NOT NULL REFERENCES sync.data_sources(id),
    status          VARCHAR(20)  NOT NULL CHECK (status IN ('RUNNING', 'SUCCESS', 'FAILED')),
    papers_imported INTEGER      NOT NULL DEFAULT 0,
    errors          TEXT,
    started_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),
    finished_at     TIMESTAMPTZ
);

-- ---------- paper (PaperService) ----------
CREATE TABLE IF NOT EXISTS paper.journals (
    id             UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name           VARCHAR(500) NOT NULL,
    issn           VARCHAR(20),
    publisher      VARCHAR(255),
    impact_factor  DECIMAL(6, 3),
    website_url    TEXT,
    created_at     TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at     TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS paper.authors (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name            VARCHAR(255) NOT NULL,
    affiliation     VARCHAR(500),
    email           VARCHAR(255),
    orcid           VARCHAR(50),
    external_id     VARCHAR(255),
    data_source_id  UUID,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS paper.keywords (
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name       VARCHAR(255) NOT NULL UNIQUE,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS paper.research_topics (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(255) NOT NULL UNIQUE,
    description TEXT,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at  TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS paper.research_papers (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    journal_id      UUID REFERENCES paper.journals(id),
    data_source_id  UUID,
    external_id     VARCHAR(255),
    title           TEXT         NOT NULL,
    abstract        TEXT,
    doi             VARCHAR(255) UNIQUE,
    published_year  SMALLINT,
    published_date  DATE,
    citation_count  INTEGER      NOT NULL DEFAULT 0,
    url             TEXT,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS paper.paper_authors (
    paper_id     UUID    NOT NULL REFERENCES paper.research_papers(id) ON DELETE CASCADE,
    author_id    UUID    NOT NULL REFERENCES paper.authors(id) ON DELETE CASCADE,
    author_order SMALLINT,
    PRIMARY KEY (paper_id, author_id)
);

CREATE TABLE IF NOT EXISTS paper.paper_keywords (
    paper_id   UUID NOT NULL REFERENCES paper.research_papers(id) ON DELETE CASCADE,
    keyword_id UUID NOT NULL REFERENCES paper.keywords(id) ON DELETE CASCADE,
    PRIMARY KEY (paper_id, keyword_id)
);

CREATE TABLE IF NOT EXISTS paper.paper_topics (
    paper_id UUID NOT NULL REFERENCES paper.research_papers(id) ON DELETE CASCADE,
    topic_id UUID NOT NULL REFERENCES paper.research_topics(id) ON DELETE CASCADE,
    PRIMARY KEY (paper_id, topic_id)
);

CREATE TABLE IF NOT EXISTS paper.bookmarks (
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID NOT NULL,
    paper_id   UUID NOT NULL REFERENCES paper.research_papers(id) ON DELETE CASCADE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    UNIQUE (user_id, paper_id)
);

-- ---------- trend (TrendService) ----------
CREATE TABLE IF NOT EXISTS trend.publication_trends (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    keyword_id    UUID,
    topic_id      UUID,
    year          SMALLINT     NOT NULL,
    paper_count   INTEGER      NOT NULL DEFAULT 0,
    citation_sum  INTEGER      NOT NULL DEFAULT 0,
    growth_rate   DECIMAL(8, 4),
    calculated_at TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS trend.dashboard_reports (
    id           UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    generated_by UUID,
    report_type  VARCHAR(50) NOT NULL CHECK (report_type IN ('DAILY', 'WEEKLY', 'CUSTOM')),
    report_date  DATE        NOT NULL,
    data         JSONB       NOT NULL DEFAULT '{}',
    created_at   TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ---------- notify (NotificationService) ----------
CREATE TABLE IF NOT EXISTS notify.notifications (
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID         NOT NULL,
    paper_id   UUID,
    type       VARCHAR(50)  NOT NULL CHECK (type IN ('NEW_PAPER', 'TREND_UPDATE', 'SYSTEM')),
    title      VARCHAR(500) NOT NULL,
    message    TEXT         NOT NULL,
    is_read    BOOLEAN      NOT NULL DEFAULT false,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS notify.follow_topics (
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID NOT NULL,
    topic_id   UUID NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    UNIQUE (user_id, topic_id)
);

CREATE TABLE IF NOT EXISTS notify.follow_keywords (
    id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID NOT NULL,
    keyword_id UUID NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    UNIQUE (user_id, keyword_id)
);

-- ---------- Indexes ----------
CREATE INDEX IF NOT EXISTS idx_auth_svc_users_email ON auth_svc.users(email);
CREATE INDEX IF NOT EXISTS idx_paper_papers_doi     ON paper.research_papers(doi);
CREATE INDEX IF NOT EXISTS idx_paper_papers_year    ON paper.research_papers(published_year);
CREATE INDEX IF NOT EXISTS idx_trend_pub_kw_year    ON trend.publication_trends(keyword_id, year);
CREATE INDEX IF NOT EXISTS idx_notify_user_read     ON notify.notifications(user_id, is_read);

-- ---------- Seed ----------
INSERT INTO auth_svc.roles (name, description) VALUES
    ('Admin',      'Full system access'),
    ('Researcher', 'Analyze trends and manage bookmarks'),
    ('Student',    'Search papers and follow topics'),
    ('Lecturer',   'Search papers and view statistics')
ON CONFLICT (name) DO NOTHING;

INSERT INTO sync.data_sources (name, base_url) VALUES
    ('OpenAlex',         'https://api.openalex.org'),
    ('Semantic Scholar', 'https://api.semanticscholar.org'),
    ('Crossref',         'https://api.crossref.org')
ON CONFLICT (name) DO NOTHING;

-- ============================================================
-- 3. MIGRATE TỪ public (nếu đã chạy script monolithic trước đó)
--    Chỉ chạy MỘT LẦN. Backup data trước khi chạy.
-- ============================================================

/*
ALTER TABLE IF EXISTS public.bookmarks DROP CONSTRAINT IF EXISTS bookmarks_user_id_fkey;
ALTER TABLE IF EXISTS public.notifications DROP CONSTRAINT IF EXISTS notifications_user_id_fkey;
ALTER TABLE IF EXISTS public.notifications DROP CONSTRAINT IF EXISTS notifications_paper_id_fkey;
ALTER TABLE IF EXISTS public.follow_topics DROP CONSTRAINT IF EXISTS follow_topics_user_id_fkey;
ALTER TABLE IF EXISTS public.follow_topics DROP CONSTRAINT IF EXISTS follow_topics_topic_id_fkey;
ALTER TABLE IF EXISTS public.follow_keywords DROP CONSTRAINT IF EXISTS follow_keywords_user_id_fkey;
ALTER TABLE IF EXISTS public.follow_keywords DROP CONSTRAINT IF EXISTS follow_keywords_keyword_id_fkey;
ALTER TABLE IF EXISTS public.publication_trends DROP CONSTRAINT IF EXISTS publication_trends_keyword_id_fkey;
ALTER TABLE IF EXISTS public.publication_trends DROP CONSTRAINT IF EXISTS publication_trends_topic_id_fkey;
ALTER TABLE IF EXISTS public.dashboard_reports DROP CONSTRAINT IF EXISTS dashboard_reports_generated_by_fkey;
ALTER TABLE IF EXISTS public.authors DROP CONSTRAINT IF EXISTS authors_data_source_id_fkey;
ALTER TABLE IF EXISTS public.research_papers DROP CONSTRAINT IF EXISTS research_papers_data_source_id_fkey;

ALTER TABLE IF EXISTS public.roles SET SCHEMA auth_svc;
ALTER TABLE IF EXISTS public.users SET SCHEMA auth_svc;
ALTER TABLE IF EXISTS public.refresh_tokens SET SCHEMA auth_svc;
ALTER TABLE IF EXISTS public.data_sources SET SCHEMA sync;
ALTER TABLE IF EXISTS public.sync_logs SET SCHEMA sync;
ALTER TABLE IF EXISTS public.journals SET SCHEMA paper;
ALTER TABLE IF EXISTS public.authors SET SCHEMA paper;
ALTER TABLE IF EXISTS public.keywords SET SCHEMA paper;
ALTER TABLE IF EXISTS public.research_topics SET SCHEMA paper;
ALTER TABLE IF EXISTS public.research_papers SET SCHEMA paper;
ALTER TABLE IF EXISTS public.paper_authors SET SCHEMA paper;
ALTER TABLE IF EXISTS public.paper_keywords SET SCHEMA paper;
ALTER TABLE IF EXISTS public.paper_topics SET SCHEMA paper;
ALTER TABLE IF EXISTS public.bookmarks SET SCHEMA paper;
ALTER TABLE IF EXISTS public.publication_trends SET SCHEMA trend;
ALTER TABLE IF EXISTS public.dashboard_reports SET SCHEMA trend;
ALTER TABLE IF EXISTS public.notifications SET SCHEMA notify;
ALTER TABLE IF EXISTS public.follow_topics SET SCHEMA notify;
ALTER TABLE IF EXISTS public.follow_keywords SET SCHEMA notify;
*/

-- ============================================================
-- Phân bổ service ↔ schema ↔ Search Path (EF Core)
--   AuthService         → auth_svc   (KHÔNG dùng "auth" trên Supabase)
--   SyncService         → sync
--   PaperService        → paper
--   TrendService        → trend
--   NotificationService → notify
-- ============================================================
