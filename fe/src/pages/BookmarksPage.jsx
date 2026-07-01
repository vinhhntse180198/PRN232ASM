import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { Bookmark, Loader2 } from 'lucide-react'
import AppNavbar from '../components/app/AppNavbar'
import PageBookCard from '../components/PageBook/PageBookCard'
import ScrollReveal from '../components/ui/ScrollReveal'
import { fetchBookmarks } from '../services/paperService'

export default function BookmarksPage() {
  const [bookmarks, setBookmarks] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let cancelled = false
    async function load() {
      setLoading(true)
      setError('')
      try {
        const data = await fetchBookmarks()
        if (!cancelled) setBookmarks(data)
      } catch (err) {
        if (!cancelled) setError(err.message || 'Could not load bookmarks')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    load()
    return () => { cancelled = true }
  }, [])

  const papers = useMemo(() => bookmarks.map((b) => b.paper).filter(Boolean), [bookmarks])

  return (
    <div className="min-h-screen bg-base">
      <AppNavbar />

      <main className="mx-auto max-w-4xl px-4 pb-20 pt-28 sm:px-6 lg:px-8">
        <ScrollReveal>
          <div className="flex items-center gap-2 text-accent-glow">
            <Bookmark className="h-5 w-5" />
            <span className="text-sm font-medium">Saved papers</span>
          </div>
          <h1 className="mt-4 font-display text-3xl font-extrabold text-primary">Bookmarks</h1>
          <p className="mt-2 text-muted">Các bài báo bạn đã lưu để xem lại.</p>
        </ScrollReveal>

        {loading && (
          <div className="mt-16 flex justify-center">
            <Loader2 className="h-6 w-6 animate-spin text-accent-primary" />
          </div>
        )}

        {error && !loading && (
          <div className="mt-8 rounded-2xl border border-red-500/30 bg-red-500/10 px-6 py-8 text-center text-sm text-red-300">
            {error}
          </div>
        )}

        {!loading && !error && papers.length === 0 && (
          <div className="mt-12 rounded-2xl border border-border bg-surface px-6 py-16 text-center">
            <p className="text-muted">Chưa có bookmark nào.</p>
            <Link to="/papers" className="mt-4 inline-block text-sm text-accent-glow hover:underline">
              ← Duyệt papers
            </Link>
          </div>
        )}

        <div className="mt-8 space-y-4">
          {papers.map((paper) => (
            <PageBookCard key={paper.id} paper={paper} />
          ))}
        </div>
      </main>
    </div>
  )
}
