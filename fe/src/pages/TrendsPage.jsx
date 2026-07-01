import { useCallback, useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { Loader2, RefreshCw, TrendingUp } from 'lucide-react'
import {
  Bar,
  BarChart,
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import AppNavbar from '../components/app/AppNavbar'
import ScrollReveal from '../components/ui/ScrollReveal'
import {
  TREND_BADGE_LABELS,
  TREND_BADGE_STYLES,
  fetchTrendsDashboard,
  refreshTrends,
} from '../services/trendService'

const YEAR_OPTIONS = [
  { value: 'all', label: 'Tất cả năm' },
  ...Array.from({ length: 7 }, (_, i) => {
    const y = 2026 - i
    return { value: String(y), label: String(y) }
  }),
]

function PapersByYearChart({ data }) {
  return (
    <div className="mt-4 h-64">
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data} margin={{ top: 8, right: 12, left: 0, bottom: 8 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.06)" />
          <XAxis dataKey="year" tick={{ fill: 'rgba(255,255,255,0.65)', fontSize: 12 }} />
          <YAxis tick={{ fill: 'rgba(255,255,255,0.65)', fontSize: 12 }} />
          <Tooltip
            contentStyle={{
              background: 'rgba(17, 24, 39, 0.92)',
              border: '1px solid rgba(255,255,255,0.12)',
              borderRadius: 12,
              color: 'white',
            }}
            labelStyle={{ color: 'rgba(255,255,255,0.75)' }}
            formatter={(value) => [value, 'Papers']}
          />
          <Bar dataKey="count" fill="rgba(99, 102, 241, 0.85)" radius={[8, 8, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </div>
  )
}

function OpenAccessChart({ data }) {
  return (
    <div className="mt-4 h-64">
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={data} margin={{ top: 8, right: 12, left: 0, bottom: 8 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.06)" />
          <XAxis dataKey="year" tick={{ fill: 'rgba(255,255,255,0.65)', fontSize: 12 }} />
          <YAxis
            domain={[0, 100]}
            tick={{ fill: 'rgba(255,255,255,0.65)', fontSize: 12 }}
            tickFormatter={(v) => `${v}%`}
          />
          <Tooltip
            contentStyle={{
              background: 'rgba(17, 24, 39, 0.92)',
              border: '1px solid rgba(255,255,255,0.12)',
              borderRadius: 12,
              color: 'white',
            }}
            labelStyle={{ color: 'rgba(255,255,255,0.75)' }}
            formatter={(value) => [`${value}%`, 'Open Access']}
          />
          <Line type="monotone" dataKey="openAccessPercent" stroke="rgba(34, 211, 238, 0.9)" strokeWidth={2.5} dot />
        </LineChart>
      </ResponsiveContainer>
    </div>
  )
}

function TrendBadge({ badge }) {
  const key = badge || 'stable'
  return (
    <span className={`rounded-full border px-2 py-0.5 text-xs font-medium ${TREND_BADGE_STYLES[key] ?? TREND_BADGE_STYLES.stable}`}>
      {TREND_BADGE_LABELS[key] ?? TREND_BADGE_LABELS.stable}
    </span>
  )
}

function KeywordRow({ item }) {
  const q = encodeURIComponent(item.keyword)
  return (
    <li className="flex items-center justify-between gap-2 text-sm">
      <Link to={`/papers?q=${q}`} className="capitalize text-accent-glow hover:underline">
        {item.keyword}
      </Link>
      <div className="flex shrink-0 items-center gap-2">
        <TrendBadge badge={item.trendBadge} />
        <span className="text-muted">{item.paperCount}</span>
      </div>
    </li>
  )
}

export default function TrendsPage() {
  const [year, setYear] = useState('all')
  const [dashboard, setDashboard] = useState(null)
  const [loading, setLoading] = useState(true)
  const [refreshing, setRefreshing] = useState(false)
  const [error, setError] = useState('')

  const load = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      const data = await fetchTrendsDashboard({ year })
      setDashboard(data)
    } catch (err) {
      setError(err.message || 'Không tải được trends')
    } finally {
      setLoading(false)
    }
  }, [year])

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      if (cancelled) return
      await load()
    })()
    return () => { cancelled = true }
  }, [load])

  const papersByYear = useMemo(
    () => (dashboard?.papersByYear ?? []).map((x) => ({ year: x.year, count: x.count })),
    [dashboard],
  )
  const openAccessByYear = useMemo(
    () => (dashboard?.openAccessByYear ?? []).map((x) => ({ ...x, year: x.year })),
    [dashboard],
  )

  const handleRefresh = async () => {
    setRefreshing(true)
    setError('')
    try {
      await refreshTrends()
      await load()
    } catch (err) {
      setError(err.message || 'Refresh thất bại')
    } finally {
      setRefreshing(false)
    }
  }

  return (
    <div className="min-h-screen bg-base">
      <AppNavbar />

      <main className="mx-auto max-w-5xl px-4 pb-20 pt-28 sm:px-6 lg:px-8">
        <ScrollReveal>
          <div className="flex flex-wrap items-start justify-between gap-4">
            <div>
              <div className="flex items-center gap-2 text-accent-glow">
                <TrendingUp className="h-5 w-5" />
                <span className="text-sm font-medium">Publication trends</span>
              </div>
              <h1 className="mt-4 font-display text-3xl font-extrabold text-primary">Trends</h1>
              <p className="mt-2 text-muted">
                Thống kê từ thư viện local — không dùng OpenAlex.
                {dashboard?.totalPapers != null && (
                  <span className="ml-1 text-primary">({dashboard.totalPapers} papers)</span>
                )}
              </p>
            </div>

            <div className="flex flex-wrap items-center gap-2">
              <select
                value={year}
                onChange={(e) => setYear(e.target.value)}
                className="rounded-lg border border-border bg-surface px-3 py-2 text-sm text-primary"
              >
                {YEAR_OPTIONS.map((opt) => (
                  <option key={opt.value} value={opt.value}>{opt.label}</option>
                ))}
              </select>
              <button
                type="button"
                onClick={handleRefresh}
                disabled={refreshing || loading}
                className="inline-flex items-center gap-2 rounded-lg border border-border bg-surface px-3 py-2 text-sm text-primary hover:bg-surface/80 disabled:opacity-50"
              >
                <RefreshCw className={`h-4 w-4 ${refreshing ? 'animate-spin' : ''}`} />
                Refresh
              </button>
            </div>
          </div>
        </ScrollReveal>

        {error && (
          <p className="mt-6 rounded-lg border border-rose-500/30 bg-rose-500/10 px-4 py-3 text-sm text-rose-300">
            {error}
          </p>
        )}

        {loading ? (
          <div className="mt-16 flex justify-center">
            <Loader2 className="h-6 w-6 animate-spin text-accent-primary" />
          </div>
        ) : (
          <div className="mt-10 grid gap-6 lg:grid-cols-2">
            <div className="rounded-2xl border border-border bg-surface p-6">
              <h2 className="font-display font-bold text-primary">Top keywords</h2>
              {(dashboard?.topKeywords?.length ?? 0) === 0 ? (
                <p className="mt-4 text-sm text-muted">
                  Chưa đủ dữ liệu.{' '}
                  <Link to="/papers" className="text-accent-glow hover:underline">Mở Library</Link>
                </p>
              ) : (
                <ul className="mt-4 space-y-2">
                  {dashboard.topKeywords.map((k) => (
                    <KeywordRow key={k.keyword} item={k} />
                  ))}
                </ul>
              )}
            </div>

            <div className="rounded-2xl border border-border bg-surface p-6">
              <h2 className="font-display font-bold text-primary">Papers by year (2020–2026)</h2>
              {papersByYear.length === 0 ? (
                <p className="mt-4 text-sm text-muted">Chưa có dữ liệu.</p>
              ) : (
                <PapersByYearChart data={papersByYear} />
              )}
            </div>

            <div className="rounded-2xl border border-border bg-surface p-6">
              <h2 className="font-display font-bold text-primary">Citations by keyword</h2>
              {(dashboard?.citationsByKeyword?.length ?? 0) === 0 ? (
                <p className="mt-4 text-sm text-muted">Chưa có dữ liệu.</p>
              ) : (
                <ul className="mt-4 space-y-2">
                  {dashboard.citationsByKeyword.map((k) => (
                    <li key={k.keyword} className="flex justify-between text-sm">
                      <Link to={`/papers?q=${encodeURIComponent(k.keyword)}`} className="capitalize text-accent-glow hover:underline">
                        {k.keyword}
                      </Link>
                      <span className="font-mono text-muted">{k.citationSum}</span>
                    </li>
                  ))}
                </ul>
              )}
            </div>

            <div className="rounded-2xl border border-border bg-surface p-6">
              <h2 className="font-display font-bold text-primary">Open Access % by year</h2>
              {(dashboard?.openAccessByYear?.length ?? 0) === 0 ? (
                <p className="mt-4 text-sm text-muted">Chưa có dữ liệu OA.</p>
              ) : (
                <OpenAccessChart data={openAccessByYear} />
              )}
            </div>

            {(dashboard?.bookmarkKeywords?.length ?? 0) > 0 && (
              <div className="rounded-2xl border border-border bg-surface p-6 lg:col-span-2">
                <h2 className="font-display font-bold text-primary">Bookmark trends</h2>
                <p className="mt-1 text-sm text-muted">Keywords từ papers bạn đã bookmark.</p>
                <ul className="mt-4 grid gap-2 sm:grid-cols-2">
                  {dashboard.bookmarkKeywords.map((k) => (
                    <li key={k.keyword} className="flex justify-between text-sm">
                      <Link to={`/papers?q=${encodeURIComponent(k.keyword)}`} className="capitalize text-accent-glow hover:underline">
                        {k.keyword}
                      </Link>
                      <span className="text-muted">{k.paperCount} papers</span>
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </div>
        )}
      </main>
    </div>
  )
}
