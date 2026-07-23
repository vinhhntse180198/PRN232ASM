import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Loader2 } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { getBookmarks, getReadingProfile } from '../../services/paperService'
import { getFollows } from '../../services/notificationService'
import { getJournals, getKeywords, getTopics } from '../../services/paperService'

export default function BookmarksPage() {
  const { user } = useAuth()
  const [bookmarks, setBookmarks] = useState([])
  const [profile, setProfile] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!user?.id) return
    ;(async () => {
      try {
        const [bms, follows, keywords, topics, journals] = await Promise.all([
          getBookmarks(user.id),
          getFollows(user.id).catch(() => ({ keywordIds: [], topicIds: [], journalIds: [] })),
          getKeywords().catch(() => []),
          getTopics().catch(() => []),
          getJournals().catch(() => []),
        ])
        setBookmarks(bms || [])

        const kwMap = Object.fromEntries((keywords || []).map((k) => [k.id, k.name]))
        const topicMap = Object.fromEntries((topics || []).map((t) => [t.id, t.name]))
        const journalMap = Object.fromEntries((journals || []).map((j) => [j.id, j.name]))

        const reading = await getReadingProfile(user.id, {
          followedKeywords: (follows?.keywordIds || []).map((id) => kwMap[id]).filter(Boolean),
          followedTopics: (follows?.topicIds || []).map((id) => topicMap[id]).filter(Boolean),
          followedJournals: (follows?.journalIds || []).map((id) => journalMap[id]).filter(Boolean),
        }).catch(() => null)
        setProfile(reading)
      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    })()
  }, [user?.id])

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <h1 className="font-display text-2xl font-bold text-primary">Bookmarks</h1>
      {error && <div className="alert-error">{error}</div>}

      {profile && (
        <section className="rounded-xl border border-border bg-surface p-4">
          <h2 className="text-sm font-semibold text-primary">Reading profile (UserProfile gRPC)</h2>
          <p className="mt-1 text-lg font-medium text-accent-glow">{profile.personaLabel}</p>
          <p className="mt-1 text-xs text-muted">{profile.summary}</p>
          <div className="mt-3 flex flex-wrap gap-2">
            {(profile.topTopics || []).map((t) => (
              <span key={t.name} className="rounded-full bg-elevated px-2.5 py-1 text-xs text-muted">
                {t.name} · {t.weight}
              </span>
            ))}
          </div>
        </section>
      )}

      {!bookmarks.length ? (
        <p className="text-muted">No bookmarked papers yet.</p>
      ) : (
        <div className="space-y-3">
          {bookmarks.map((b) => (
            <Link
              key={b.paperId}
              to={`/papers/${b.paperId}`}
              className="block rounded-xl border border-border bg-surface p-4 hover:border-accent-primary/40"
            >
              <p className="font-medium text-primary">{b.paperTitle}</p>
              <p className="mt-1 text-xs text-muted">Saved {new Date(b.createdAt).toLocaleDateString()}</p>
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}
