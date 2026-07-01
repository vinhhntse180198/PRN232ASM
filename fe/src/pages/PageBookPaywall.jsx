import { useEffect, useState } from 'react'
import { Link, Navigate, useLocation, useParams } from 'react-router-dom'
import { ArrowLeft, Calendar, ExternalLink, Loader2, Lock, Quote } from 'lucide-react'
import PageBookReaderNavbar from '../components/PageBook/PageBookReaderNavbar'
import { OPENALEX_ENABLED } from '../config/openalex'
import { extractKeyTakeaways, formatPublishDate } from '../utils/citation'
import { fetchOpenAlexWork } from '../services/syncService'
import { getOpenAlexPublisherUrl } from '../utils/paperReader'

export default function PageBookPaywall() {
  const { openAlexId } = useParams()
  const location = useLocation()
  const backTo = location.state?.from || '/papers'

  const [paper, setPaper] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!OPENALEX_ENABLED) return
    let cancelled = false
    async function load() {
      setLoading(true)
      setError('')
      try {
        const data = await fetchOpenAlexWork(openAlexId)
        if (!cancelled) setPaper(data)
      } catch (err) {
        if (!cancelled) setError(err.message || 'Không tải được bài viết')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    load()
    return () => { cancelled = true }
  }, [openAlexId])

  if (!OPENALEX_ENABLED) {
    return <Navigate to="/papers" replace />
  }

  const publisherUrl = paper ? getOpenAlexPublisherUrl(paper) : null
  const takeaways = extractKeyTakeaways(paper?.abstract, 2)

  return (
    <div className="min-h-screen bg-base">
      <PageBookReaderNavbar />

      <main className="mx-auto max-w-3xl px-4 pb-20 pt-28 sm:px-6">
        <Link
          to={backTo}
          className="inline-flex items-center gap-2 text-sm text-muted transition-colors hover:text-primary"
        >
          <ArrowLeft className="h-4 w-4" />
          Quay lại
        </Link>

        {loading && (
          <div className="mt-16 flex justify-center">
            <Loader2 className="h-6 w-6 animate-spin text-accent-primary" />
          </div>
        )}

        {error && !loading && (
          <div className="mt-8 rounded-xl border border-red-500/30 bg-red-500/10 px-6 py-8 text-center text-sm text-red-300">
            {error}
          </div>
        )}

        {paper && !loading && (
          <article className="mt-8 overflow-hidden rounded-2xl border border-amber-500/30 bg-surface shadow-card">
            <div className="border-b border-amber-500/20 bg-amber-500/10 p-6 sm:p-8">
              <span className="inline-flex items-center gap-1.5 rounded-md bg-amber-500/20 px-3 py-1 text-xs font-semibold uppercase tracking-wider text-amber-400">
                <Lock className="h-3.5 w-3.5" />
                Bài trả phí
              </span>

              <h1 className="mt-4 font-display text-2xl font-bold leading-tight text-primary sm:text-3xl">
                {paper.title}
              </h1>

              {paper.authors?.length > 0 && (
                <p className="mt-4 text-sm text-muted">{paper.authors.join(' · ')}</p>
              )}

              <div className="mt-5 flex flex-wrap gap-3 text-sm text-muted">
                {paper.publishedYear && (
                  <span className="inline-flex items-center gap-1.5 rounded-lg bg-elevated px-3 py-1.5">
                    <Calendar className="h-4 w-4" />
                    {paper.publishedYear}
                  </span>
                )}
                <span className="inline-flex items-center gap-1.5 rounded-lg bg-elevated px-3 py-1.5">
                  <Quote className="h-4 w-4" />
                  {(paper.citationCount ?? 0).toLocaleString()} citations
                </span>
              </div>
            </div>

            <div className="p-6 sm:p-8">
              <p className="text-sm leading-relaxed text-muted">
                Bài báo này nằm sau paywall — không có PDF miễn phí. Truy cập trang nhà xuất bản để đọc toàn văn.
              </p>

              {takeaways.length > 0 && (
                <div className="mt-6 rounded-xl border border-border bg-elevated/50 p-5">
                  <h2 className="text-xs font-bold uppercase tracking-wider text-muted">Tóm tắt</h2>
                  <ul className="mt-3 space-y-2 text-sm text-primary">
                    {takeaways.map((t, i) => (
                      <li key={i}>{t}</li>
                    ))}
                  </ul>
                </div>
              )}

              <div className="mt-6 rounded-xl border border-border bg-elevated/30 p-5 text-sm">
                <p className="text-xs font-bold uppercase tracking-wider text-muted">Publication Info</p>
                <p className="mt-2 text-muted">Published: {formatPublishDate(paper)}</p>
                {paper.doi && <p className="mt-1 font-mono text-xs text-primary">DOI: {paper.doi}</p>}
              </div>

              {publisherUrl ? (
                <a
                  href={publisherUrl}
                  target="_blank"
                  rel="noreferrer"
                  className="mt-8 flex w-full items-center justify-center gap-2 rounded-xl bg-accent-primary px-5 py-3.5 text-sm font-semibold text-white shadow-glow-sm transition-all hover:bg-accent-glow"
                >
                  <ExternalLink className="h-4 w-4" />
                  Mở trang nhà xuất bản
                </a>
              ) : (
                <p className="mt-6 text-sm text-muted">Chưa có liên kết nhà xuất bản.</p>
              )}
            </div>
          </article>
        )}
      </main>
    </div>
  )
}
