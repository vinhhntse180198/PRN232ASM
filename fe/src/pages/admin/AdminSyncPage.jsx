import { useEffect, useState } from 'react'
import { Loader2, RefreshCw } from 'lucide-react'
import { getSyncLogs, getSyncStatus, triggerSync } from '../../services/syncService'

export default function AdminSyncPage() {
  const [status, setStatus] = useState(null)
  const [logs, setLogs] = useState([])
  const [loading, setLoading] = useState(true)
  const [syncing, setSyncing] = useState(false)
  const [error, setError] = useState('')

  const load = async () => {
    try {
      const [s, l] = await Promise.all([getSyncStatus(), getSyncLogs()])
      setStatus(s)
      setLogs(l || [])
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
  }, [])

  const handleSync = async () => {
    setSyncing(true)
    setError('')
    try {
      await triggerSync()
      await load()
    } catch (err) {
      setError(err.message)
    } finally {
      setSyncing(false)
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
          <h1 className="font-display text-2xl font-bold text-primary">Synchronization</h1>
          <p className="text-sm text-muted">
            Status: {status?.isRunning ? 'Running' : 'Idle'}
          </p>
        </div>
        <button
          type="button"
          onClick={handleSync}
          disabled={syncing}
          className="btn-primary flex items-center gap-2"
        >
          {syncing ? <Loader2 className="h-4 w-4 animate-spin" /> : <RefreshCw className="h-4 w-4" />}
          Trigger Sync
        </button>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {status?.latestLog && (
        <div className="rounded-xl border border-border bg-surface p-4">
          <p className="text-sm font-medium text-primary">Latest sync</p>
          <p className="text-xs text-muted">
            {status.latestLog.status} · {status.latestLog.papersImported} papers ·{' '}
            {status.latestLog.message}
          </p>
        </div>
      )}

      <div className="overflow-hidden rounded-xl border border-border">
        <table className="w-full text-sm">
          <thead className="bg-elevated text-left text-muted">
            <tr>
              <th className="px-4 py-3">Source</th>
              <th className="px-4 py-3">Status</th>
              <th className="px-4 py-3">Imported</th>
              <th className="px-4 py-3">Started</th>
            </tr>
          </thead>
          <tbody>
            {logs.map((log) => (
              <tr key={log.id} className="border-t border-border bg-surface">
                <td className="px-4 py-3 text-primary">{log.dataSourceName}</td>
                <td className="px-4 py-3 text-muted">{log.status}</td>
                <td className="px-4 py-3 text-muted">{log.papersImported}</td>
                <td className="px-4 py-3 text-muted">{new Date(log.startedAt).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
