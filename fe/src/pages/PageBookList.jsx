import { useCallback, useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Loader2, Search, Sparkles } from 'lucide-react'

import PageBookReaderNavbar from '../components/PageBook/PageBookReaderNavbar'
import PageBookCard from '../components/PageBook/PageBookCard'
import PageBookSidebar from '../components/PageBook/PageBookSidebar'
import PageBookSearchPagination from '../components/PageBook/PageBookSearchPagination'
import ScrollReveal from '../components/ui/ScrollReveal'
import { fetchPapersPage } from '../services/paperService'

const PAGE_SIZE = 10
const CURRENT_YEAR = new Date().getFullYear()
const YEAR_OPTIONS = Array.from({ length: CURRENT_YEAR - 1999 }, (_, i) => CURRENT_YEAR - i)

export default function PageBookList() {
  const [searchParams, setSearchParams] = useSearchParams()

  const urlQuery = searchParams.get('q') ?? ''
  const currentPage = Math.max(1, parseInt(searchParams.get('page') ?? '1', 10) || 1)
  const yearFilter = searchParams.get('year') ?? 'all'

  const [papers, setPapers] = useState([])
  const [total, setTotal] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [query, setQuery] = useState(urlQuery)

  const patchParams = useCallback((patch) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev)
      for (const [key, value] of Object.entries(patch)) {
        if (value === null || value === undefined || value === '' || value === 'all' || value === false) {
          next.delete(key)
        } else if (value === true) {
          next.set(key, '1')
        } else {
          next.set(key, String(value))
        }
      }
      return next
    }, { replace: true })
  }, [setSearchParams])

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setQuery(urlQuery)
  }, [urlQuery])

  useEffect(() => {
    const timer = setTimeout(() => {
      const trimmed = query.trim()
      if (trimmed.length >= 2) {
        if (trimmed !== urlQuery) patchParams({ q: trimmed, page: null })
      } else if (urlQuery) {
        patchParams({ q: null, page: null })
      }
    }, 450)
    return () => clearTimeout(timer)
  }, [query, urlQuery, patchParams])

  useEffect(() => {
    let cancelled = false

    async function loadPapers() {
      setLoading(true)
      setError('')
      try {
        const data = await fetchPapersPage({
          page: currentPage,
          pageSize: PAGE_SIZE,
          query: urlQuery.trim().length >= 2 ? urlQuery.trim() : undefined,
          year: yearFilter !== 'all' ? yearFilter : undefined,
        })
        if (!cancelled) {
          setPapers(data.items ?? [])
          setTotal(data.total ?? 0)
        }
      } catch (err) {
        if (!cancelled) {
          setError(err.message || 'Không tải được thư viện. Chạy PaperService port 5002.')
          setPapers([])
          setTotal(0)
        }
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    loadPapers()
    return () => { cancelled = true }
  }, [urlQuery, yearFilter, currentPage])

  const yearOptions = useMemo(() => YEAR_OPTIONS, [])

  return (
    <div className="min-h-screen bg-base">
      <PageBookReaderNavbar />

      <section className="relative overflow-hidden pt-28 pb-10 sm:pt-32">
        <div className="dot-grid pointer-events-none absolute inset-0 opacity-[0.05]" />
        <div className="pointer-events-none absolute right-0 top-10 h-80 w-80 rounded-full bg-halo" />

        <div className="relative mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <ScrollReveal>
            <span className="inline-flex items-center gap-1.5 rounded-full border border-emerald-500/30 bg-emerald-500/10 px-4 py-1.5 text-xs font-medium text-emerald-300">
              <Sparkles className="h-3.5 w-3.5" />
              Thư viện · miễn phí
            </span>
          </ScrollReveal>

          <ScrollReveal delay={100}>
            <h1 className="mt-6 font-display text-3xl font-extrabold tracking-tight text-primary sm:text-5xl">
              Research <span className="text-gradient">Library</span>
            </h1>
          </ScrollReveal>

          <ScrollReveal delay={200}>
            <div className="mt-8 flex flex-col gap-3 sm:flex-row sm:items-center">
              <div className="relative flex-1">
                <Search className="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-muted" />
                <input
                  type="search"
                  value={query}
                  onChange={(e) => setQuery(e.target.value)}
                  placeholder="Tìm trong thư viện (tiêu đề, tác giả…)…"
                  className="w-full rounded-xl border border-border bg-elevated py-3.5 pl-11 pr-4 text-sm text-primary placeholder:text-muted outline-none transition-colors focus:border-accent-primary/50 focus:ring-1 focus:ring-accent-primary/30"
                />
              </div>
              <select
                value={yearFilter}
                onChange={(e) => patchParams({ year: e.target.value, page: null })}
                className="rounded-xl border border-border bg-elevated px-4 py-3.5 text-sm text-primary outline-none focus:border-accent-primary/50"
              >
                <option value="all">Tất cả năm</option>
                {yearOptions.map((y) => (
                  <option key={y} value={String(y)}>{y}</option>
                ))}
              </select>
            </div>

            {!loading && (
              <p className="mt-4 text-sm text-muted">
                <span className="font-medium text-primary">{total.toLocaleString()}</span> bài trong thư viện
                {urlQuery.trim().length >= 2 && (
                  <> · tìm &quot;<span className="font-medium text-primary">{urlQuery}</span>&quot;</>
                )}
                {yearFilter !== 'all' ? (
                  <> · năm <span className="font-medium text-primary">{yearFilter}</span></>
                ) : (
                  <> · mọi năm</>
                )}
              </p>
            )}
          </ScrollReveal>
        </div>
      </section>

      <main className="mx-auto max-w-7xl px-4 pb-20 sm:px-6 lg:px-8">
        <div className="grid gap-8 lg:grid-cols-[1fr_320px]">
          <div>
            {loading && (
              <div className="flex items-center justify-center gap-2 py-20 text-muted">
                <Loader2 className="h-5 w-5 animate-spin text-accent-primary" />
                Đang tải thư viện…
              </div>
            )}

            {error && !loading && (
              <div className="rounded-2xl border border-red-500/30 bg-red-500/10 px-6 py-8 text-center">
                <p className="text-sm text-red-300">{error}</p>
              </div>
            )}

            {!loading && !error && papers.length === 0 && (
              <div className="rounded-2xl border border-border bg-surface px-6 py-16 text-center">
                <p className="font-display text-lg font-bold text-primary">
                  {urlQuery.trim().length >= 2
                    ? `Không có bài cho "${urlQuery}"`
                    : yearFilter !== 'all'
                      ? `Chưa có bài trong thư viện cho năm ${yearFilter}`
                      : 'Thư viện trống'}
                </p>
              </div>
            )}

            {!loading && !error && papers.length > 0 && (
              <div className="space-y-4">
                {papers.map((paper, i) => (
                  <ScrollReveal key={paper.id} delay={Math.min(i * 30, 120)}>
                    <PageBookCard paper={paper} />
                  </ScrollReveal>
                ))}
              </div>
            )}

            {!loading && total > PAGE_SIZE && (
              <PageBookSearchPagination
                page={currentPage}
                total={total}
                pageSize={PAGE_SIZE}
                onPageChange={(p) => patchParams({ page: p === 1 ? null : p })}
              />
            )}
          </div>

          <PageBookSidebar
            papers={papers}
            query={query}
            searchTotal={total}
            searchPage={currentPage}
            pageSize={PAGE_SIZE}
            isSearchMode={urlQuery.trim().length >= 2}
          />
        </div>
      </main>
    </div>
  )
}
