import { useEffect, useState } from 'react'
import { Loader2 } from 'lucide-react'
import { getJournals } from '../../services/paperService'

export default function AdminJournalsPage() {
  const [journals, setJournals] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getJournals()
      .then(setJournals)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <h1 className="font-display text-2xl font-bold text-primary">Journal Management</h1>
      <p className="text-sm text-muted">{journals.length} journals in local database</p>
      {error && <div className="alert-error">{error}</div>}
      <div className="grid gap-3 md:grid-cols-2">
        {journals.map((j) => (
          <div key={j.id} className="rounded-xl border border-border bg-surface p-4">
            <p className="font-medium text-primary">{j.name}</p>
            <p className="text-xs text-muted">ISSN: {j.issn} · {j.publisher}</p>
          </div>
        ))}
      </div>
    </div>
  )
}
