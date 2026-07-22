import { useEffect, useState } from 'react'
import { Loader2 } from 'lucide-react'
import { getKeywords, getTopics } from '../../services/paperService'

export default function AdminTopicsPage() {
  const [topics, setTopics] = useState([])
  const [keywords, setKeywords] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    Promise.all([getTopics(), getKeywords()])
      .then(([t, k]) => {
        setTopics(t || [])
        setKeywords(k || [])
      })
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
    <div className="space-y-8">
      <h1 className="font-display text-2xl font-bold text-primary">Topic & Keyword Management</h1>
      {error && <div className="alert-error">{error}</div>}

      <section>
        <h2 className="mb-3 font-semibold text-primary">Research Topics ({topics.length})</h2>
        <div className="grid gap-3 md:grid-cols-2">
          {topics.map((t) => (
            <div key={t.id} className="rounded-xl border border-border bg-surface p-4">
              <p className="font-medium text-primary">{t.name}</p>
              <p className="text-xs text-muted">{t.description}</p>
            </div>
          ))}
        </div>
      </section>

      <section>
        <h2 className="mb-3 font-semibold text-primary">Keywords ({keywords.length})</h2>
        <div className="flex flex-wrap gap-2">
          {keywords.map((k) => (
            <span key={k.id} className="rounded-full bg-elevated px-3 py-1 text-sm text-muted">
              {k.name}
            </span>
          ))}
        </div>
      </section>
    </div>
  )
}
