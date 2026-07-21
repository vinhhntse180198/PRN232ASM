import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Loader2 } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { getBookmarks } from '../../services/paperService'

export default function BookmarksPage() {
  const { user } = useAuth()
  const [bookmarks, setBookmarks] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!user?.id) return
    getBookmarks(user.id)
      .then(setBookmarks)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
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
