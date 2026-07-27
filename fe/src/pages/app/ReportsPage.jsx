import { useEffect, useState } from 'react'
import { FileText, Loader2, Plus, X, Download, TrendingUp, BookOpen, Tag, RefreshCw } from 'lucide-react'
import { generateReport, getReports, getDashboard } from '../../services/trendService'

const STATIC_REPORT_STATS = {
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

export default function ReportsPage() {
  const [reports, setReports] = useState([])
  const [loading, setLoading] = useState(true)
  const [generating, setGenerating] = useState(false)
  const [error, setError] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [latestStats, setLatestStats] = useState(null)
  const [modalReport, setModalReport] = useState(null)
  const [usingFallback, setUsingFallback] = useState(false)

  const load = async () => {
    setError('')
    setLoading(true)
    try {
      const data = await getReports()
      setReports(data || [])
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
  }, [])

  const handleGenerate = async () => {
    setGenerating(true)
    setError('')
    try {
      const report = await generateReport()
      await load()
      setModalReport(report)
      setUsingFallback(false)

      try {
        const dash = await getDashboard()
        setLatestStats(dash)
      } catch {
        setLatestStats(null)
      }

      setShowModal(true)
    } catch (err) {
      setError(err.message)
    } finally {
      setGenerating(false)
    }
  }

  const handleViewStats = async (report) => {
    setModalReport(report)
    setUsingFallback(false)

    try {
      const dash = await getDashboard()
      setLatestStats(dash)
    } catch {
      setLatestStats(null)
    }

    setShowModal(true)
  }

  const downloadJson = (data, filename) => {
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = filename
    a.click()
    URL.revokeObjectURL(url)
  }

  const exportReport = () => {
    const stats = latestStats || STATIC_REPORT_STATS
    const data = {
      reportDate: modalReport?.reportDate,
      totalPapers: modalReport?.totalPapers ?? stats.totalPapers,
      topKeyword: modalReport?.topKeyword ?? stats.topKeywords?.[0]?.keyword ?? 'N/A',
      generatedAt: modalReport?.generatedAt,
      statistics: stats,
    }
    downloadJson(data, `report-${modalReport?.reportDate || new Date().toISOString().split('T')[0]}.json`)
  }

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-10 w-10 animate-spin text-accent-primary" />
          <p className="text-sm text-muted">Loading reports...</p>
        </div>
      </div>
    )
  }

  const effectiveStats = latestStats || (usingFallback ? STATIC_REPORT_STATS : null)

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Analytical Reports</h1>
          <p className="text-sm text-muted">
            {reports.length > 0
              ? `${reports.length} report${reports.length !== 1 ? 's' : ''} generated`
              : 'Generate publication trend reports from trend data'}
          </p>
        </div>
        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={load}
            disabled={loading}
            className="btn-secondary flex items-center gap-2 text-sm"
          >
            <RefreshCw className="h-4 w-4" />
            Refresh
          </button>
          <button
            type="button"
            onClick={handleGenerate}
            disabled={generating}
            className="btn-primary flex items-center gap-2"
          >
            {generating ? <Loader2 className="h-4 w-4 animate-spin" /> : <Plus className="h-4 w-4" />}
            {generating ? 'Generating...' : 'Generate Report'}
          </button>
        </div>
      </div>

      {error && (
        <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400">
          {error}
        </div>
      )}

      {!reports.length ? (
        <div className="rounded-xl border border-dashed border-border bg-surface p-12 text-center">
          <FileText className="mx-auto h-12 w-12 text-muted/30" />
          <p className="mt-4 text-muted">No reports generated yet.</p>
          <p className="mt-1 text-xs text-muted">
            Click &quot;Generate Report&quot; to create a trend analysis report.
          </p>
        </div>
      ) : (
        <div className="space-y-3">
          {reports.map((r) => (
            <div key={r.id} className="flex items-center gap-4 rounded-xl border border-border bg-surface p-5">
              <FileText className="h-8 w-8 shrink-0 text-accent-primary" />
              <div className="flex-1">
                <p className="font-medium text-primary">
                  Daily Report — {r.reportDate || (r.generatedAt ? new Date(r.generatedAt).toLocaleDateString() : 'Unknown date')}
                </p>
                <p className="text-sm text-muted">
                  {r.totalPapers} papers
                  {r.topKeyword ? (
                    <>
                      {' '}&middot; Top keyword:{' '}
                      <span className="text-accent-glow">{r.topKeyword}</span>
                    </>
                  ) : null}
                </p>
                <p className="text-xs text-muted">
                  Generated {r.generatedAt ? new Date(r.generatedAt).toLocaleString() : 'Unknown'}
                </p>
              </div>
              <button
                type="button"
                onClick={() => handleViewStats(r)}
                className="btn-secondary text-sm shrink-0"
              >
                View Stats
              </button>
            </div>
          ))}
        </div>
      )}

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="max-w-lg w-full rounded-2xl border border-border bg-surface p-6 shadow-xl max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-6">
              <h2 className="font-display text-xl font-bold text-primary">Report Statistics</h2>
              <button
                type="button"
                onClick={() => { setShowModal(false); setModalReport(null); setLatestStats(null) }}
                className="rounded-lg p-1 text-muted hover:bg-elevated hover:text-primary"
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            {effectiveStats ? (
              <div className="space-y-4">
                {usingFallback && !latestStats && (
                  <div className="rounded-lg border border-accent-primary/20 bg-accent-primary/5 px-3 py-2 text-xs text-accent-glow flex items-center gap-2">
                    <TrendingUp className="h-3 w-3 shrink-0" />
                    Showing sample data — real data will appear after sync completes.
                  </div>
                )}

                <div className="grid grid-cols-2 gap-3">
                  <StatCard
                    icon={BookOpen}
                    label="Total Papers"
                    value={modalReport?.totalPapers ?? effectiveStats.totalPapers}
                  />
                  <StatCard
                    icon={Tag}
                    label="Top Keyword"
                    value={modalReport?.topKeyword ?? (effectiveStats.topKeywords?.[0]?.keyword || 'N/A')}
                    sub={
                      modalReport?.topKeyword
                        ? null
                        : effectiveStats.topKeywords?.[0]?.count
                          ? `(${effectiveStats.topKeywords[0].count} papers)`
                          : null
                    }
                  />
                </div>

                {(effectiveStats.topKeywords?.length > 0) && (
                  <div className="rounded-lg border border-border bg-elevated p-4">
                    <h3 className="mb-3 flex items-center gap-2 text-sm font-semibold text-primary">
                      <TrendingUp className="h-4 w-4" />
                      Top Keywords
                    </h3>
                    <div className="space-y-2">
                      {effectiveStats.topKeywords.slice(0, 10).map((k, i) => {
                        const maxCount = effectiveStats.topKeywords[0]?.count || 1
                        return (
                          <div key={k.keyword} className="flex items-center gap-3">
                            <span className="w-5 text-xs text-muted">{i + 1}</span>
                            <div className="flex-1">
                              <div className="h-1.5 rounded-full bg-base overflow-hidden">
                                <div
                                  className="h-full rounded-full bg-accent-primary"
                                  style={{ width: `${Math.min(100, (k.count / maxCount) * 100)}%` }}
                                />
                              </div>
                            </div>
                            <span className="text-xs font-medium text-muted whitespace-nowrap">
                              {k.keyword} ({k.count})
                            </span>
                          </div>
                        )
                      })}
                    </div>
                  </div>
                )}

                {(effectiveStats.papersByYear?.length > 0) && (
                  <div className="rounded-lg border border-border bg-elevated p-4">
                    <h3 className="mb-3 text-sm font-semibold text-primary">Papers by Year</h3>
                    <div className="flex items-end gap-1 h-20">
                      {effectiveStats.papersByYear.map((y) => {
                        const maxCount = Math.max(...effectiveStats.papersByYear.map(p => p.count), 1)
                        return (
                          <div key={y.year} className="flex flex-col items-center gap-1 flex-1">
                            <span className="text-xs text-muted">{y.count}</span>
                            <div
                              className="w-full rounded-t bg-accent-primary/60"
                              style={{ height: `${Math.max(4, (y.count / maxCount) * 60)}px` }}
                            />
                            <span className="text-xs text-muted">{y.year}</span>
                          </div>
                        )
                      })}
                    </div>
                  </div>
                )}
              </div>
            ) : (
              <div className="flex flex-col items-center justify-center py-8 gap-3">
                <Loader2 className="h-6 w-6 animate-spin text-accent-primary" />
                <p className="text-sm text-muted">Loading statistics...</p>
              </div>
            )}

            <div className="mt-6 flex gap-3">
              <button type="button" onClick={exportReport} className="btn-primary flex items-center gap-2 flex-1 justify-center">
                <Download className="h-4 w-4" />
                Download JSON
              </button>
              <button
                type="button"
                onClick={() => { setShowModal(false); setModalReport(null); setLatestStats(null) }}
                className="btn-secondary flex-1"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

function StatCard({ icon: Icon, label, value, sub }) {
  return (
    <div className="rounded-lg border border-border bg-elevated p-4">
      <div className="flex items-center gap-2 mb-2">
        <Icon className="h-4 w-4 text-accent-primary" />
        <span className="text-xs text-muted">{label}</span>
      </div>
      <p className="text-lg font-bold text-primary">{value}</p>
      {sub && <p className="text-xs text-muted">{sub}</p>}
    </div>
  )
}
