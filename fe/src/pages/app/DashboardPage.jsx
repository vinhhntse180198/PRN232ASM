import { useEffect, useState } from 'react'
import { Loader2 } from 'lucide-react'
import SimpleLineChart from '../../components/ui/SimpleLineChart'
import SimpleBarChart from '../../components/ui/SimpleBarChart'
import { getAnalytics, getDashboard } from '../../services/trendService'

export default function DashboardPage() {
  const [dashboard, setDashboard] = useState(null)
  const [analytics, setAnalytics] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    Promise.all([getDashboard(), getAnalytics()])
      .then(([dash, stats]) => {
        setDashboard(dash)
        setAnalytics(stats)
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-red-300">
        {error}
      </div>
    )
  }

  const papersByYear = (dashboard?.papersByYear || []).map((y) => ({
    label: String(y.year),
    count: y.count,
  }))

  const topKeywords = (dashboard?.topKeywords || []).map((k) => ({
    label: k.keyword,
    count: k.count,
  }))

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl font-bold text-primary">Publication Trend Dashboard</h1>
        <p className="mt-1 text-sm text-muted">
          Analyze publication trends from local dataset (303 papers)
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard label="Total Papers" value={dashboard?.totalPapers ?? analytics?.totalPapers ?? 0} />
        <StatCard
          label="Keywords Tracked"
          value={analytics?.totalKeywords ?? topKeywords.length}
        />
        <StatCard label="Years Covered" value={papersByYear.length || ((analytics?.yearFrom && analytics?.yearTo) ? analytics.yearTo - analytics.yearFrom + 1 : 0)} />
        <StatCard label="Top Keyword" value={analytics?.topKeyword || topKeywords[0]?.label || '—'} small />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <ChartCard title="Publications Over Time">
          <SimpleLineChart data={papersByYear} labelKey="year" valueKey="count" color="#818CF8" />
        </ChartCard>
        <ChartCard title="Top Keywords">
          <SimpleBarChart data={topKeywords} labelKey="keyword" valueKey="count" color="#10B981" />
        </ChartCard>
      </div>
    </div>
  )
}

function StatCard({ label, value, small }) {
  return (
    <div className="rounded-xl border border-border bg-surface p-5 shadow-card">
      <p className="text-xs uppercase tracking-wide text-muted">{label}</p>
      <p className={`mt-2 font-display font-bold text-primary ${small ? 'text-lg' : 'text-3xl'}`}>
        {value}
      </p>
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
