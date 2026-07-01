import { useMemo } from 'react'
import { Link } from 'react-router-dom'
import { BarChart3, Bookmark, Flame, TrendingUp } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

function topKeywords(papers, limit = 5) {
  const counts = {}
  for (const p of papers) {
    for (const kw of p.keywords ?? []) {
      const key = kw.toLowerCase()
      counts[key] = (counts[key] || 0) + 1
    }
  }
  return Object.entries(counts)
    .sort((a, b) => b[1] - a[1])
    .slice(0, limit)
    .map(([name, count]) => ({ name, count }))
}

export default function PageBookSidebar({
  papers,
  query,
  searchTotal = null,
  searchPage = null,
  pageSize = 25,
  isSearchMode = false,
}) {
  const total = papers.length
  const filtered = query
    ? papers.filter((p) => {
        const q = query.toLowerCase()
        return (
          p.title?.toLowerCase().includes(q) ||
          p.abstract?.toLowerCase().includes(q) ||
          p.doi?.toLowerCase().includes(q)
        )
      }).length
    : total

  const years = papers.map((p) => p.publishedYear).filter(Boolean)
  const latestYear = years.length ? Math.max(...years) : '—'
  const totalCitations = papers.reduce((sum, p) => sum + (p.citationCount || 0), 0)
  const topics = useMemo(() => topKeywords(papers), [papers])

  return (
    <aside className="space-y-4">
      <ScrollReveal>
        <div className="rounded-2xl border border-border bg-surface p-5">
          <div className="flex items-center gap-2 text-sm font-medium text-primary">
            <BarChart3 className="h-4 w-4 text-accent-primary" />
            Overview
          </div>
          <dl className="mt-4 space-y-3">
            {searchTotal != null ? (
              <>
                <div className="flex justify-between text-sm">
                  <dt className="text-muted">{isSearchMode ? 'Kết quả tìm' : 'Tác phẩm hiện có'}</dt>
                  <dd className="font-mono font-medium text-primary">{searchTotal.toLocaleString()}</dd>
                </div>
                {isSearchMode && (
                  <>
                    <div className="flex justify-between text-sm">
                      <dt className="text-muted">Trang hiện tại</dt>
                      <dd className="font-mono font-medium text-primary">
                        {searchPage ?? 1}
                        {searchTotal > pageSize && (
                          <span className="text-muted"> / {Math.min(Math.ceil(searchTotal / pageSize), 400).toLocaleString()}</span>
                        )}
                      </dd>
                    </div>
                    <div className="flex justify-between text-sm">
                      <dt className="text-muted">Trên trang này</dt>
                      <dd className="font-mono font-medium text-primary">{papers.length}</dd>
                    </div>
                  </>
                )}
              </>
            ) : (
              <>
                <div className="flex justify-between text-sm">
                  <dt className="text-muted">Showing</dt>
                  <dd className="font-mono font-medium text-primary">{filtered}</dd>
                </div>
                <div className="flex justify-between text-sm">
                  <dt className="text-muted">Library saved</dt>
                  <dd className="font-mono font-medium text-primary">{total}</dd>
                </div>
              </>
            )}
            <div className="flex justify-between text-sm">
              <dt className="text-muted">Latest year</dt>
              <dd className="font-mono font-medium text-primary">{latestYear}</dd>
            </div>
            <div className="flex justify-between text-sm">
              <dt className="text-muted">Citations</dt>
              <dd className="font-mono font-medium text-accent-green">{totalCitations.toLocaleString()}</dd>
            </div>
          </dl>
        </div>
      </ScrollReveal>

      <ScrollReveal delay={80}>
        <div className="rounded-2xl border border-border bg-surface p-5">
          <div className="flex items-center gap-2 text-sm font-medium text-primary">
            <TrendingUp className="h-4 w-4 text-accent-primary" />
            Top keywords
          </div>
          {topics.length === 0 ? (
            <p className="mt-3 text-xs text-muted">Chưa có từ khóa trên trang này.</p>
          ) : (
            <ul className="mt-4 space-y-3">
              {topics.map((topic) => (
                <li key={topic.name} className="flex items-center justify-between rounded-lg bg-elevated/50 px-3 py-2">
                  <p className="text-sm font-medium capitalize text-primary">{topic.name}</p>
                  <span className="text-xs text-muted">{topic.count} papers</span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </ScrollReveal>

      <ScrollReveal delay={160}>
        <div className="rounded-2xl border border-border bg-gradient-to-br from-accent-primary/10 to-transparent p-5">
          <div className="flex items-center gap-2 text-sm font-medium text-primary">
            <Bookmark className="h-4 w-4 text-accent-primary" />
            Bookmarks
          </div>
          <p className="mt-2 text-sm text-muted">
            Lưu paper để xem lại.{' '}
            <Link to="/bookmarks" className="text-accent-glow hover:underline">
              Xem bookmarks →
            </Link>
          </p>
        </div>
      </ScrollReveal>

      <ScrollReveal delay={240}>
        <div className="rounded-2xl border border-accent-primary/20 bg-accent-primary/5 p-5">
          <div className="flex items-center gap-2 text-sm font-medium text-accent-glow">
            <Flame className="h-4 w-4" />
            Nguồn dữ liệu
          </div>
          <p className="mt-2 text-xs leading-relaxed text-muted">
            Thư viện lưu các bài báo khoa học từ nhiều nguồn mở. Bạn có thể tìm kiếm, đọc PDF và lưu lại ngay trong ứng dụng.
          </p>
        </div>
      </ScrollReveal>
    </aside>
  )
}
