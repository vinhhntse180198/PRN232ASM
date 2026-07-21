import { useEffect, useState } from 'react'
import { FileText, Loader2, Plus } from 'lucide-react'
import { generateReport, getReports } from '../../services/trendService'

export default function ReportsPage() {
  const [reports, setReports] = useState([])
  const [loading, setLoading] = useState(true)
  const [generating, setGenerating] = useState(false)
  const [error, setError] = useState('')

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
      await generateReport()
      await load()
    } catch (err) {
      setError(err.message)
    } finally {
      setGenerating(false)
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
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Analytical Reports</h1>
          <p className="text-sm text-muted">Publication trend reports generated from local data</p>
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
        <p className="text-muted">No reports yet. Click Generate Report to create one.</p>
      ) : (
        <div className="space-y-3">
          {reports.map((r) => (
            <div key={r.id} className="flex items-center gap-4 rounded-xl border border-border bg-surface p-5">
              <FileText className="h-8 w-8 text-accent-primary" />
              <div>
                <p className="font-medium text-primary">
                  Daily Report — {r.reportDate || new Date(r.generatedAt).toLocaleDateString()}
                </p>
                <p className="text-sm text-muted">
                  {r.totalPapers} papers · Top keyword: {r.topKeyword}
                </p>
                <p className="text-xs text-muted">Generated {new Date(r.generatedAt).toLocaleString()}</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
