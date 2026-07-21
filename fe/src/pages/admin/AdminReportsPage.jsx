import { useEffect, useState } from 'react'
import { Loader2 } from 'lucide-react'
import { generateReport, getReports } from '../../services/trendService'
import { getSyncLogs } from '../../services/syncService'

export default function AdminReportsPage() {
  const [reports, setReports] = useState([])
  const [logs, setLogs] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    Promise.all([getReports(), getSyncLogs()])
      .then(([r, l]) => {
        setReports(r || [])
        setLogs(l || [])
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  const handleGenerate = async () => {
    try {
      await generateReport()
      const r = await getReports()
      setReports(r || [])
    } catch (err) {
      setError(err.message)
    }
  }

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <h1 className="font-display text-2xl font-bold text-primary">System Reports</h1>
        <button type="button" onClick={handleGenerate} className="btn-primary text-sm">
          Generate Daily Report
        </button>
      </div>
      {error && <div className="alert-error">{error}</div>}

      <section>
        <h2 className="mb-3 font-semibold text-primary">Trend Reports</h2>
        <div className="space-y-2">
          {reports.map((r) => (
            <div key={r.id} className="rounded-lg border border-border bg-surface px-4 py-3 text-sm">
              <span className="text-primary">{r.reportDate}</span>
              <span className="text-muted"> — {r.totalPapers} papers, top: {r.topKeyword}</span>
            </div>
          ))}
        </div>
      </section>

      <section>
        <h2 className="mb-3 font-semibold text-primary">Sync Logs</h2>
        <div className="space-y-2">
          {logs.slice(0, 10).map((l) => (
            <div key={l.id} className="rounded-lg border border-border bg-surface px-4 py-3 text-sm text-muted">
              {l.dataSourceName}: {l.status} ({l.papersImported} papers) — {new Date(l.startedAt).toLocaleString()}
            </div>
          ))}
        </div>
      </section>
    </div>
  )
}
