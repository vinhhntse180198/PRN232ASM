import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { ArrowLeft, Bookmark, BookmarkCheck, Loader2 } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { addBookmark, getPaper, removeBookmark } from '../../services/paperService'
import { getBookmarks } from '../../services/paperService'

export default function PaperDetailPage() {
  const { id } = useParams()
  const { user } = useAuth()
  const [paper, setPaper] = useState(null)
  const [bookmarked, setBookmarked] = useState(false)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!id) return
    setLoading(true)
    Promise.all([
      getPaper(id),
      user?.id ? getBookmarks(user.id) : Promise.resolve([]),
    ])
      .then(([p, bookmarks]) => {
        setPaper(p)
        setBookmarked((bookmarks || []).some((b) => b.paperId === id))
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [id, user?.id])

  const toggleBookmark = async () => {
    try {
      if (bookmarked) {
        await removeBookmark(id)
        setBookmarked(false)
      } else {
        await addBookmark(id)
        setBookmarked(true)
      }
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

  if (error || !paper) {
    return <div className="alert-error">{error || 'Paper not found'}</div>
  }

  return (
    <div className="mx-auto max-w-4xl space-y-6">
      <Link to="/papers" className="inline-flex items-center gap-2 text-sm text-muted hover:text-primary">
        <ArrowLeft className="h-4 w-4" />
        Back to search
      </Link>

      <div className="flex items-start justify-between gap-4">
        <h1 className="font-display text-2xl font-bold text-primary">{paper.title}</h1>
        <button type="button" onClick={toggleBookmark} className="btn-secondary flex items-center gap-2">
          {bookmarked ? <BookmarkCheck className="h-4 w-4 text-accent-green" /> : <Bookmark className="h-4 w-4" />}
          {bookmarked ? 'Saved' : 'Bookmark'}
        </button>
      </div>

      <div className="flex flex-wrap gap-3 text-sm text-muted">
        <span>Year: {paper.publicationYear}</span>
        <span>Journal: {paper.journalName}</span>
        <span>DOI: {paper.doi || 'N/A'}</span>
        <span>Citations: {paper.citationCount}</span>
      </div>

      <section className="rounded-xl border border-border bg-surface p-6">
        <h2 className="mb-2 font-semibold text-primary">Abstract</h2>
        <p className="text-sm leading-relaxed text-muted">{paper.abstract}</p>
      </section>

      <section className="grid gap-4 md:grid-cols-2">
        <TagList title="Authors" items={paper.authors} />
        <TagList title="Keywords" items={paper.keywords} />
        <TagList title="Topics" items={paper.topics} />
      </section>
    </div>
  )
}

function TagList({ title, items }) {
  return (
    <div className="rounded-xl border border-border bg-surface p-4">
      <h3 className="mb-2 text-sm font-semibold text-primary">{title}</h3>
      <div className="flex flex-wrap gap-2">
        {(items || []).map((item) => (
          <span key={item} className="rounded-full bg-elevated px-3 py-1 text-xs text-muted">
            {item}
          </span>
        ))}
      </div>
    </div>
  )
}
