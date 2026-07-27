import { useEffect, useState } from 'react'
import { FileText, Loader2, Plus, X, Download, TrendingUp, BookOpen, Tag } from 'lucide-react'
import { generateReport, getReports, getDashboard } from '../../services/trendService'

export default function ReportsPage() {
  const [reports, setReports] = useState([])
  const [loading, setLoading] = useState(true)
  const [generating, setGenerating] = useState(false)
  const [error, setError] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [latestStats, setLatestStats] = useState(null)
  const [modalReport, setModalReport] = useState(null)

  const load = async () => {
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
      // Get stats for modal
      try {
        const dash = await getDashboard()
        setLatestStats(dash)
      } catch { /* ignore */ }
      setModalReport(report)
      setShowModal(true)
    } catch (err) {
      setError(err.message)
    } finally {
      setGenerating(false)
    }
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
    const data = {
      reportDate: modalReport?.reportDate,
      totalPapers: modalReport?.totalPapers,
      topKeyword: modalReport?.topKeyword,
      generatedAt: modalReport?.generatedAt,
      statistics: latestStats,
    }
    downloadJson(data, `report-${modalReport?.reportDate || 'export'}.json`)
  }

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Analytical Reports</h1>
          <p className="text-sm text-muted">Generate publication trend reports from local data</p>
        </div>
        <button
          type="button"
          onClick={handleGenerate}
          disabled={generating}
          className="btn-primary flex items-center gap-2"
        >
          {generating ? <Loader2 className="h-4 w-4 animate-spin" /> : <Plus className="h-4 w-4" />}
          Generate Report
        </button>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {!reports.length ? (
        <div className="rounded-xl border border-dashed border-border bg-surface p-12 text-center">
          <FileText className="mx-auto h-12 w-12 text-muted/30" />
          <p className="mt-4 text-muted">No reports yet. Click Generate Report to create one.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {reports.map((r) => (
            <div key={r.id} className="flex items-center gap-4 rounded-xl border border-border bg-surface p-5">
              <FileText className="h-8 w-8 text-accent-primary" />
              <div className="flex-1">
                <p className="font-medium text-primary">
                  Daily Report — {r.reportDate || new Date(r.generatedAt).toLocaleDateString()}
                </p>
                <p className="text-sm text-muted">
                  {r.totalPapers} papers · Top keyword: <span className="text-accent-glow">{r.topKeyword}</span>
                </p>
                <p className="text-xs text-muted">Generated {new Date(r.generatedAt).toLocaleString()}</p>
              </div>
              <button
                type="button"
                onClick={async () => {
                  try {
                    const dash = await getDashboard()
                    setLatestStats(dash)
                  } catch { /* ignore */ }
                  setModalReport(r)
                  setShowModal(true)
                }}
                className="btn-secondary text-sm"
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
          <div className="max-w-lg w-full rounded-2xl border border-border bg-surface p-6 shadow-xl">
            <div className="flex items-center justify-between mb-6">
              <h2 className="font-display text-xl font-bold text-primary">Report Statistics</h2>
              <button
                type="button"
                onClick={() => setShowModal(false)}
                className="rounded-lg p-1 text-muted hover:bg-elevated hover:text-primary"
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            {latestStats ? (
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-3">
                  <StatCard icon={BookOpen} label="Total Papers" value={latestStats.totalPapers} />
                  <StatCard icon={Tag} label="Top Keyword" value={latestStats.topKeywords?.[0]?.keyword || 'N/A'} sub={latestStats.topKeywords?.[0]?.count ? `(${latestStats.topKeywords[0].count})` : ''} />
                </div>

                {latestStats.topKeywords?.length > 0 && (
                  <div className="rounded-lg border border-border bg-elevated p-4">
                    <h3 className="mb-3 flex items-center gap-2 text-sm font-semibold text-primary">
                      <TrendingUp className="h-4 w-4" />
                      Top Keywords
                    </h3>
                    <div className="space-y-2">
                      {latestStats.topKeywords.slice(0, 10).map((k, i) => (
                        <div key={k.keyword} className="flex items-center gap-3">
                          <span className="w-5 text-xs text-muted">{i + 1}</span>
                          <div className="flex-1">
                            <div className="h-1.5 rounded-full bg-base overflow-hidden">
                              <div
                                className="h-full rounded-full bg-accent-primary"
                                style={{ width: `${Math.min(100, (k.count / latestStats.topKeywords[0].count) * 100)}%` }}
                              />
                            </div>
                          </div>
                          <span className="text-xs font-medium text-muted">{k.keyword} ({k.count})</span>
                        </div>
                      ))}
                    </div>
                  </div>
                )}

                {latestStats.papersByYear?.length > 0 && (
                  <div className="rounded-lg border border-border bg-elevated p-4">
                    <h3 className="mb-3 text-sm font-semibold text-primary">Papers by Year</h3>
                    <div className="flex items-end gap-1 h-20">
                      {latestStats.papersByYear.map((y) => (
                        <div key={y.year} className="flex flex-col items-center gap-1 flex-1">
                          <span className="text-xs text-muted">{y.count}</span>
                          <div
                            className="w-full rounded-t bg-accent-primary/60"
                            style={{ height: `${Math.max(4, (y.count / (latestStats.papersByYear.reduce((a, b) => Math.max(a, b.count), 0) || 1)) * 60)}px` }}
                          />
                          <span className="text-xs text-muted">{y.year}</span>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            ) : (
              <div className="flex justify-center py-8">
                <Loader2 className="h-6 w-6 animate-spin text-accent-primary" />
              </div>
            )}

            <div className="mt-6 flex gap-3">
              <button type="button" onClick={exportReport} className="btn-primary flex items-center gap-2 flex-1 justify-center">
                <Download className="h-4 w-4" />
                Download JSON
              </button>
              <button type="button" onClick={() => setShowModal(false)} className="btn-secondary flex-1">
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
