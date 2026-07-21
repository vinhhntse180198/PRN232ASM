import { useEffect, useState } from 'react'
import { AlertTriangle, Loader2, Save } from 'lucide-react'
import { getDataSources, updateDataSource } from '../../services/syncService'

export default function AdminDataSourcesPage() {
  const [sources, setSources] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(null)

  const load = async () => {
    try {
      const data = await getDataSources()
      setSources(data || [])
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
  }, [])

  const handleSave = async (source) => {
    setSaving(source.id)
    setError('')
    try {
      await updateDataSource(source.id, {
        isEnabled: source.isEnabled,
        maxImportCount: source.maxImportCount,
      })
      await load()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(null)
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
      <h1 className="font-display text-2xl font-bold text-primary">Academic API Sources</h1>

      <div className="flex items-start gap-3 rounded-xl border border-accent-amber/30 bg-accent-amber/10 p-4 text-sm text-accent-amber">
        <AlertTriangle className="mt-0.5 h-5 w-5 shrink-0" />
        <div>
          <p className="font-semibold">OpenAlex warning</p>
          <p className="mt-1 text-accent-amber/80">
            Keep OpenAlex disabled on localhost. Max import is capped at 303 papers. Enabling sync
            with millions of records may inflate your database and incur Supabase costs.
          </p>
        </div>
      </div>

      {error && <div className="alert-error">{error}</div>}

      <div className="space-y-4">
        {sources.map((s) => (
          <div key={s.id} className="rounded-xl border border-border bg-surface p-5">
            <div className="flex items-center justify-between">
              <div>
                <p className="font-semibold text-primary">{s.name}</p>
                <p className="text-xs text-muted">{s.baseUrl}</p>
              </div>
              <label className="flex items-center gap-2 text-sm text-muted">
                <input
                  type="checkbox"
                  checked={s.isEnabled}
                  onChange={(e) =>
                    setSources((prev) =>
                      prev.map((x) => (x.id === s.id ? { ...x, isEnabled: e.target.checked } : x)),
                    )
                  }
                  className="accent-accent-primary"
                />
                Enabled
              </label>
            </div>
            <div className="mt-4 flex items-end gap-4">
              <div>
                <label className="text-xs text-muted">Max import count</label>
                <input
                  type="number"
                  min={1}
                  max={303}
                  value={s.maxImportCount}
                  onChange={(e) =>
                    setSources((prev) =>
                      prev.map((x) =>
                        x.id === s.id ? { ...x, maxImportCount: Number(e.target.value) } : x,
                      ),
                    )
                  }
                  className="input-field mt-1 w-32"
                />
              </div>
              <button
                type="button"
                onClick={() => handleSave(sources.find((x) => x.id === s.id))}
                disabled={saving === s.id}
                className="btn-primary flex items-center gap-2"
              >
                {saving === s.id ? <Loader2 className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />}
                Save
              </button>
            </div>
            {s.lastSyncedAt && (
              <p className="mt-2 text-xs text-muted">
                Last synced: {new Date(s.lastSyncedAt).toLocaleString()}
              </p>
            )}
          </div>
        ))}
      </div>
    </div>
  )
}
