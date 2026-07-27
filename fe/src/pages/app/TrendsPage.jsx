import { useEffect, useState } from 'react'
import { Loader2, TrendingUp } from 'lucide-react'
import SimpleLineChart from '../../components/ui/SimpleLineChart'
import SimpleBarChart from '../../components/ui/SimpleBarChart'
import { getTrends } from '../../services/trendService'

export default function TrendsPage() {
  const [trends, setTrends] = useState([])
  const [selectedKeyword, setSelectedKeyword] = useState('')
  const [selectedYear, setSelectedYear] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getTrends()
      .then((data) => setTrends(data || []))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  const filtered = trends.filter((t) => {
    if (selectedKeyword && !t.keyword?.toLowerCase().includes(selectedKeyword.toLowerCase())) return false
    if (selectedYear && String(t.year) !== selectedYear) return false
    return true
  })

  const years = [...new Set(trends.map((t) => t.year))].sort()

  const keywordCounts = filtered.reduce((acc, t) => {
    const kw = (t.keyword || 'unknown').trim()
    acc[kw] = (acc[kw] || 0) + t.paperCount
    return acc
  }, {})

  const topKeywords = Object.entries(keywordCounts)
    .map(([keyword, count]) => ({ keyword, count }))
    .sort((a, b) => b.count - a.count)
    .slice(0, 15)

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl font-bold text-primary">Research Trends</h1>
        <p className="mt-1 text-sm text-muted">
          Explore publication trends by keyword and year
        </p>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-end gap-4 rounded-xl border border-border bg-surface p-4">
        <div className="flex flex-col gap-1">
          <label className="text-xs text-muted">Keyword</label>
          <input
            type="text"
            value={selectedKeyword}
            onChange={(e) => setSelectedKeyword(e.target.value)}
            placeholder="e.g. machine learning"
            className="rounded-lg border border-border bg-elevated px-3 py-2 text-sm text-primary placeholder-muted/50 focus:border-accent-primary/50 focus:outline-none"
          />
        </div>
        <div className="flex flex-col gap-1">
          <label className="text-xs text-muted">Year</label>
          <select
            value={selectedYear}
            onChange={(e) => setSelectedYear(e.target.value)}
            className="rounded-lg border border-border bg-elevated px-3 py-2 text-sm text-primary focus:border-accent-primary/50 focus:outline-none"
          >
            <option value="">All years</option>
            {years.map((y) => (
              <option key={y} value={y}>{y}</option>
            ))}
          </select>
        </div>
        <button
          type="button"
          onClick={() => { setSelectedKeyword(''); setSelectedYear('') }}
          className="rounded-lg border border-border bg-elevated px-4 py-2 text-sm text-muted transition-colors hover:border-accent-primary/40 hover:text-primary"
        >
          Clear filters
        </button>
      </div>

      {error && (
        <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400">
          {error}
        </div>
      )}

      {/* Stats */}
      <div className="grid gap-4 sm:grid-cols-3">
        <StatCard label="Matching Records" value={filtered.length} />
        <StatCard label="Keywords Found" value={topKeywords.length} />
        <StatCard label="Years Covered" value={years.length} />
      </div>

      {/* Line Chart */}
      {filtered.length > 0 && (
        <div className="rounded-xl border border-border bg-surface p-6 shadow-card">
          <h2 className="mb-1 flex items-center gap-2 font-display text-lg font-semibold text-primary">
            <TrendingUp className="h-4 w-4 text-accent-primary" />
            Publications Over Time
          </h2>
          <p className="mb-4 text-xs text-muted">
            {selectedKeyword ? `Trend for: "${selectedKeyword}"` : 'All keywords'}
            {selectedYear ? ` · Year ${selectedYear}` : ''}
          </p>
          <SimpleLineChart
            data={filtered.map((t) => ({ year: t.year, count: t.paperCount }))}
            labelKey="year"
            valueKey="count"
            color="#818CF8"
          />
        </div>
      )}

      {/* Top Keywords */}
      {topKeywords.length > 0 && (
        <div className="rounded-xl border border-border bg-surface p-6 shadow-card">
          <h2 className="mb-4 font-display text-lg font-semibold text-primary">
            Top Keywords
            {selectedYear ? ` in ${selectedYear}` : ' (all years)'}
          </h2>
          <SimpleBarChart data={topKeywords} labelKey="keyword" valueKey="count" color="#10B981" />
        </div>
      )}

      {filtered.length === 0 && !error && (
        <div className="rounded-xl border border-border bg-surface p-12 text-center">
          <TrendingUp className="mx-auto h-12 w-12 text-muted/30" />
          <p className="mt-4 text-muted">No trends found. Try adjusting your filters.</p>
        </div>
      )}
    </div>
  )
}

function StatCard({ label, value }) {
  return (
    <div className="rounded-xl border border-border bg-surface p-5 shadow-card">
      <p className="text-xs uppercase tracking-wide text-muted">{label}</p>
      <p className="mt-2 font-display text-3xl font-bold text-primary">{value}</p>
    </div>
  )
}
