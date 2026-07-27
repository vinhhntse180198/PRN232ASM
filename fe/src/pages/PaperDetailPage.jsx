import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Bookmark, ExternalLink, Loader2 } from 'lucide-react'
import AppLayout from '../layouts/AppLayout'
import { getAccessToken } from '../services/apiClient'
import { addBookmark, getPaperById, removeBookmark } from '../services/paperService'

export default function PaperDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [paper, setPaper] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [bookmarkLoading, setBookmarkLoading] = useState(false)

  useEffect(() => {
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const res = await getPaperById(id)
        setPaper(res.data)
      } catch (err) {
        setError(err.message || 'Failed to load paper')
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [id])

  const handleToggleBookmark = async () => {
    if (!getAccessToken()) {
      navigate('/login', { state: { from: `/papers/${id}` } })
      return
    }

    setBookmarkLoading(true)
    try {
      if (paper.isBookmarked) {
        await removeBookmark(paper.id)
        setPaper((p) => ({ ...p, isBookmarked: false }))
      } else {
        await addBookmark(paper.id)
        setPaper((p) => ({ ...p, isBookmarked: true }))
      }
    } catch (err) {
      setError(err.message || 'Bookmark action failed')
    } finally {
      setBookmarkLoading(false)
    }
  }

  return (
    <AppLayout>
      <Link
        to="/papers"
        className="mb-6 inline-flex items-center gap-2 text-sm text-muted hover:text-primary"
      >
        <ArrowLeft className="h-4 w-4" />
        Back to papers
      </Link>

      {loading ? (
        <div className="flex justify-center py-16">
          <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
        </div>
      ) : error ? (
        <div className="rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-300">
          {error}
        </div>
      ) : paper ? (
        <article className="rounded-2xl border border-border bg-surface p-8">
          <div className="flex items-start justify-between gap-4">
            <h1 className="font-display text-2xl font-bold leading-tight text-primary sm:text-3xl">
              {paper.title}
            </h1>
            <button
              type="button"
              onClick={handleToggleBookmark}
              disabled={bookmarkLoading}
              className={`shrink-0 rounded-lg border p-3 transition-colors ${
                paper.isBookmarked
                  ? 'border-accent-primary/50 bg-accent-primary/10 text-accent-glow'
                  : 'border-border text-muted hover:border-accent-primary/40'
              }`}
            >
              <Bookmark className={`h-5 w-5 ${paper.isBookmarked ? 'fill-current' : ''}`} />
            </button>
          </div>

          <div className="mt-4 flex flex-wrap gap-3 text-sm text-muted">
            {paper.publishedYear && <span>Year: {paper.publishedYear}</span>}
            {paper.journal?.name && <span>Journal: {paper.journal.name}</span>}
            <span>Citations: {paper.citationCount}</span>
            {paper.doi && <span>DOI: {paper.doi}</span>}
          </div>

          {paper.authors?.length > 0 && (
            <div className="mt-6">
              <h2 className="text-sm font-semibold uppercase tracking-wide text-muted">Authors</h2>
              <ul className="mt-2 space-y-1">
                {paper.authors.map((a) => (
                  <li key={a.id} className="text-sm text-primary">
                    {a.name}
                    {a.affiliation && <span className="text-muted"> — {a.affiliation}</span>}
                  </li>
                ))}
              </ul>
            </div>
          )}

          {paper.abstract && (
            <div className="mt-6">
              <h2 className="text-sm font-semibold uppercase tracking-wide text-muted">Abstract</h2>
              <p className="mt-2 leading-relaxed text-primary/90">{paper.abstract}</p>
            </div>
          )}

          {paper.keywords?.length > 0 && (
            <div className="mt-6">
              <h2 className="text-sm font-semibold uppercase tracking-wide text-muted">Keywords</h2>
              <div className="mt-2 flex flex-wrap gap-2">
                {paper.keywords.map((kw) => (
                  <span
                    key={kw}
                    className="rounded-full border border-border bg-elevated px-3 py-1 text-xs text-muted"
                  >
                    {kw}
                  </span>
                ))}
              </div>
            </div>
          )}

          {paper.topics?.length > 0 && (
            <div className="mt-6">
              <h2 className="text-sm font-semibold uppercase tracking-wide text-muted">Topics</h2>
              <div className="mt-2 flex flex-wrap gap-2">
                {paper.topics.map((t) => (
                  <span
                    key={t.id}
                    className="rounded-full border border-accent-primary/30 bg-accent-primary/10 px-3 py-1 text-xs text-accent-glow"
                  >
                    {t.name}
                  </span>
                ))}
              </div>
            </div>
          )}

          {paper.url && (
            <a
              href={paper.url}
              target="_blank"
              rel="noreferrer"
              className="mt-8 inline-flex items-center gap-2 text-sm font-medium text-accent-glow hover:underline"
            >
              Open source link
              <ExternalLink className="h-4 w-4" />
            </a>
          )}
        </article>
      ) : null}
    </AppLayout>
  )
}
