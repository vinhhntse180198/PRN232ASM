import { useEffect, useState } from 'react'
import { Loader2 } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { getKeywords, getTopics } from '../../services/paperService'
import { followKeyword, followTopic, getFollows, unfollowTopic } from '../../services/notificationService'

export default function TopicsPage() {
  const { user } = useAuth()
  const [topics, setTopics] = useState([])
  const [keywords, setKeywords] = useState([])
  const [follows, setFollows] = useState({ topicIds: [], keywordIds: [] })
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const load = async () => {
    if (!user?.id) return
    try {
      const [t, k, f] = await Promise.all([getTopics(), getKeywords(), getFollows(user.id)])
      setTopics(t || [])
      setKeywords(k || [])
      setFollows({ topicIds: f?.topicIds || [], keywordIds: f?.keywordIds || [] })
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.id])

  const toggleTopic = async (topicId) => {
    try {
      if (follows.topicIds.includes(topicId)) {
        await unfollowTopic(user.id, topicId)
      } else {
        await followTopic(user.id, topicId)
      }
      await load()
    } catch (err) {
      setError(err.message)
    }
  }

  const toggleKeyword = async (keywordId) => {
    try {
      if (follows.keywordIds.includes(keywordId)) {
        setError('Unfollow keyword via API not implemented on backend — follow only.')
        return
      }
      await followKeyword(user.id, keywordId)
      await load()
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
      <div>
        <h1 className="font-display text-2xl font-bold text-primary">Follow Topics & Keywords</h1>
        <p className="text-sm text-muted">Get notified when new papers match your interests</p>
      </div>
      {error && <div className="alert-error">{error}</div>}

      <section>
        <h2 className="mb-3 font-semibold text-primary">Research Topics</h2>
        <div className="grid gap-3 md:grid-cols-2">
          {topics.map((t) => (
            <button
              key={t.id}
              type="button"
              onClick={() => toggleTopic(t.id)}
              className={`rounded-xl border p-4 text-left transition-colors ${
                follows.topicIds.includes(t.id)
                  ? 'border-accent-green/50 bg-accent-green/10'
                  : 'border-border bg-surface hover:border-accent-primary/40'
              }`}
            >
              <p className="font-medium text-primary">{t.name}</p>
              <p className="mt-1 text-xs text-muted">{t.description}</p>
            </button>
          ))}
        </div>
      </section>

      <section>
        <h2 className="mb-3 font-semibold text-primary">Keywords</h2>
        <div className="flex flex-wrap gap-2">
          {keywords.map((k) => (
            <button
              key={k.id}
              type="button"
              onClick={() => toggleKeyword(k.id)}
              className={`rounded-full px-4 py-2 text-sm ${
                follows.keywordIds.includes(k.id)
                  ? 'bg-accent-primary text-white'
                  : 'bg-elevated text-muted hover:text-primary'
              }`}
            >
              {k.name}
            </button>
          ))}
        </div>
      </section>
    </div>
  )
}
