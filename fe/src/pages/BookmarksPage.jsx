import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Bookmark, Loader2 } from 'lucide-react'
import AppLayout from '../layouts/AppLayout'
import ProtectedRoute from '../routes/ProtectedRoute'
import { getBookmarks, removeBookmark } from '../services/paperService'

function BookmarksContent() {
  const [bookmarks, setBookmarks] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [removingId, setRemovingId] = useState(null)

  const load = async () => {
    setLoading(true)
    setError('')
    try {
      const res = await getBookmarks()
      setBookmarks(res.data || [])
    } catch (err) {
      setError(err.message || 'Failed to load bookmarks')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
  }, [])

  const handleRemove = async (paperId) => {
    setRemovingId(paperId)
    try {
      await removeBookmark(paperId)
      setBookmarks((prev) => prev.filter((b) => b.paperId !== paperId))
    } catch (err) {
      setError(err.message || 'Failed to remove bookmark')
    } finally {
      setRemovingId(null)
    }
  }

  return (
    <AppLayout>
      <div className="space-y-6">
        <div>
          <h1 className="font-display text-3xl font-bold text-primary">My Bookmarks</h1>
          <p className="mt-2 text-muted">Papers you saved for later reading.</p>
        </div>

        {error && (
          <div className="rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-300">
            {error}
          </div>
        )}

        {loading ? (
          <div className="flex justify-center py-16">
            <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
          </div>
        ) : bookmarks.length === 0 ? (
          <div className="rounded-2xl border border-border bg-surface px-6 py-16 text-center">
            <Bookmark className="mx-auto h-10 w-10 text-muted" />
            <p className="mt-4 text-muted">No bookmarks yet.</p>
            <Link to="/papers" className="mt-4 inline-block text-sm text-accent-glow hover:underline">
              Browse papers
            </Link>
          </div>
        ) : (
          <div className="space-y-3">
            {bookmarks.map((b) => (
              <div
                key={b.id}
                className="flex items-center justify-between gap-4 rounded-xl border border-border bg-surface p-4"
              >
                <div className="min-w-0">
                  <Link
                    to={`/papers/${b.paperId}`}
                    className="font-medium text-primary hover:text-accent-glow"
                  >
                    {b.title}
                  </Link>
                  <p className="mt-1 text-sm text-muted">
                    {[b.journalName, b.publishedYear].filter(Boolean).join(' • ')}
                  </p>
                </div>
                <button
                  type="button"
                  onClick={() => handleRemove(b.paperId)}
                  disabled={removingId === b.paperId}
                  className="shrink-0 rounded-lg border border-border px-3 py-2 text-sm text-muted hover:border-red-500/40 hover:text-red-300"
                >
                  Remove
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </AppLayout>
  )
}

export default function BookmarksPage() {
  return (
    <ProtectedRoute>
      <BookmarksContent />
    </ProtectedRoute>
  )
}
