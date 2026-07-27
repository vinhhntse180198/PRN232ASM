import { useEffect, useState } from 'react'
import { Loader2, RefreshCw, BarChart3 } from 'lucide-react'
import SimpleLineChart from '../../components/ui/SimpleLineChart'
import SimpleBarChart from '../../components/ui/SimpleBarChart'
import { getAnalytics, getDashboard } from '../../services/trendService'

const STATIC_FALLBACK = {
  totalPapers: 303,
  topKeywords: [
    { keyword: 'Machine Learning', count: 127 },
    { keyword: 'Deep Learning', count: 118 },
    { keyword: 'AI', count: 112 },
    { keyword: 'NLP', count: 103 },
    { keyword: 'Neural Networks', count: 107 },
    { keyword: 'Computer Vision', count: 93 },
    { keyword: 'Data Mining', count: 80 },
    { keyword: 'Big Data', count: 70 },
    { keyword: 'Reinforcement Learning', count: 61 },
    { keyword: 'Robotics', count: 58 },
  ],
  papersByYear: [
    { year: 2020, count: 55 },
    { year: 2021, count: 63 },
    { year: 2022, count: 73 },
    { year: 2023, count: 66 },
    { year: 2024, count: 46 },
  ],
}

export default function DashboardPage() {
  const [dashboard, setDashboard] = useState(null)
  const [analytics, setAnalytics] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [refreshing, setRefreshing] = useState(false)

  const load = async () => {
    setError('')
    try {
      const [dash, stats] = await Promise.all([getDashboard(), getAnalytics()])
      setDashboard(dash)
      setAnalytics(stats)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
      setRefreshing(false)
    }
  }

  useEffect(() => {
    load()
  }, [])

  const handleRefresh = async () => {
    setRefreshing(true)
    await load()
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-10 w-10 animate-spin text-accent-primary" />
          <p className="text-sm text-muted">Loading dashboard data...</p>
        </div>
      </div>
    )
  }

  const hasData = dashboard && (dashboard.papersByYear?.length > 0 || dashboard.topKeywords?.length > 0)

  const effectiveDashboard = hasData ? dashboard : STATIC_FALLBACK
  const effectiveAnalytics = analytics || { totalPapers: effectiveDashboard.totalPapers, keywordCount: effectiveDashboard.topKeywords.length }

  const papersByYear = (effectiveDashboard.papersByYear || []).map((y) => ({
    label: String(y.year),
    count: y.count,
  }))

  const topKeywords = (effectiveDashboard.topKeywords || []).map((k) => ({
    label: k.keyword,
    count: k.count,
  }))

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Publication Trend Dashboard</h1>
          <p className="mt-1 text-sm text-muted">
            {hasData ? 'Analyzing publication trends from dataset' : 'Showing sample data — run sync to load real data'}
          </p>
        </div>
        <button
          type="button"
          onClick={handleRefresh}
          disabled={refreshing}
          className="btn-secondary flex items-center gap-2 text-sm"
        >
          <RefreshCw className={`h-4 w-4 ${refreshing ? 'animate-spin' : ''}`} />
          Refresh
        </button>
      </div>

      {error && (
        <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400 flex items-center gap-3">
          <span>{error}</span>
          <button onClick={handleRefresh} className="ml-auto underline underline-offset-2 hover:text-red-300">
            Retry
          </button>
        </div>
      )}

      {!hasData && !error && (
        <div className="rounded-xl border border-accent-primary/20 bg-accent-primary/5 px-4 py-3 text-sm text-accent-glow flex items-center gap-3">
          <BarChart3 className="h-4 w-4 shrink-0" />
          <span>
            No trend data in database yet. Showing sample preview — data will populate after the first sync.
          </span>
        </div>
      )}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard label="Total Papers" value={effectiveAnalytics?.totalPapers ?? 0} />
        <StatCard label="Keywords Tracked" value={effectiveAnalytics?.keywordCount ?? topKeywords.length} />
        <StatCard label="Years Covered" value={papersByYear.length} />
        <StatCard
          label="Top Keyword"
          value={effectiveDashboard.topKeywords?.[0]?.keyword ?? topKeywords[0]?.label ?? '—'}
          sub={effectiveDashboard.topKeywords?.[0]?.count ? `(${effectiveDashboard.topKeywords[0].count} papers)` : null}
          small
        />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <ChartCard title="Publications Over Time">
          {papersByYear.length > 0 ? (
            <SimpleLineChart data={papersByYear} labelKey="year" valueKey="count" color="#818CF8" />
          ) : (
            <EmptyChart message="No publication data yet" />
          )}
        </ChartCard>
        <ChartCard title="Top Keywords">
          {topKeywords.length > 0 ? (
            <SimpleBarChart data={topKeywords} labelKey="keyword" valueKey="count" color="#10B981" />
          ) : (
            <EmptyChart message="No keyword data yet" />
          )}
        </ChartCard>
      </div>

      {/* Analytics breakdown */}
      {hasData && effectiveAnalytics && (
        <div className="rounded-xl border border-border bg-surface p-6 shadow-card">
          <h2 className="mb-4 font-display text-lg font-semibold text-primary">Analytics Summary</h2>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {effectiveAnalytics.yearFrom > 0 && (
              <div>
                <p className="text-xs text-muted">Year Range</p>
                <p className="font-semibold text-primary">{effectiveAnalytics.yearFrom} — {effectiveAnalytics.yearTo}</p>
              </div>
            )}
            {effectiveAnalytics.topKeyword && (
              <div>
                <p className="text-xs text-muted">Leading Keyword</p>
                <p className="font-semibold text-primary">{effectiveAnalytics.topKeyword}</p>
                <p className="text-xs text-muted">{effectiveAnalytics.topKeywordCount} papers</p>
              </div>
            )}
            {effectiveAnalytics.totalKeywords > 0 && (
              <div>
                <p className="text-xs text-muted">Unique Keywords</p>
                <p className="font-semibold text-primary">{effectiveAnalytics.totalKeywords}</p>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  )
}

function StatCard({ label, value, sub, small }) {
  return (
    <div className="rounded-xl border border-border bg-surface p-5 shadow-card">
      <p className="text-xs uppercase tracking-wide text-muted">{label}</p>
      <p className={`mt-2 font-display font-bold text-primary ${small ? 'text-lg' : 'text-3xl'}`}>
        {value}
      </p>
      {sub && <p className="text-xs text-muted mt-1">{sub}</p>}
    </div>
  )
}

function ChartCard({ title, children }) {
  return (
    <div className="rounded-xl border border-border bg-surface p-6 shadow-card">
      <h2 className="mb-4 font-display text-lg font-semibold text-primary">{title}</h2>
      {children}
    </div>
  )
}

function EmptyChart({ message }) {
  return (
    <div className="flex h-40 items-center justify-center rounded-lg bg-elevated/50">
      <p className="text-sm text-muted">{message}</p>
    </div>
  )
}
