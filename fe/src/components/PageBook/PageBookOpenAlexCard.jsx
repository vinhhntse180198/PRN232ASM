import { Link } from 'react-router-dom'
import { useState } from 'react'
import { BookOpen, Bookmark, Download, Loader2, Quote, Sparkles } from 'lucide-react'
import { importOpenAlexPaper } from '../../services/syncService'
import { OPENALEX_ENABLED } from '../../config/openalex'
import { resolveOpenAlexPaperUrls } from '../../utils/paperReader'

export default function PageBookOpenAlexCard({ paper, returnTo, onSaved }) {
  const [saving, setSaving] = useState(false)
  const [saved, setSaved] = useState(false)

  const authors = paper.authors?.length ? paper.authors.join(', ') : null
  const readPath = paper.openAlexId ? `/read/${paper.openAlexId}` : null
  const linkState = returnTo ? { from: returnTo } : undefined
  const { pdfDownloadUrl } = resolveOpenAlexPaperUrls(paper)
  const hasPdf = paper.hasPdf || Boolean(pdfDownloadUrl)

  const handleSave = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    if (!OPENALEX_ENABLED || !paper.openAlexId || saving || saved) return
    setSaving(true)
    try {
      await importOpenAlexPaper(paper.openAlexId)
      setSaved(true)
      onSaved?.()
    } catch {
      setSaved(true)
    } finally {
      setSaving(false)
    }
  }

  const handleDownload = (e) => {
    e.preventDefault()
    e.stopPropagation()
    if (!pdfDownloadUrl) return
    window.open(pdfDownloadUrl, '_blank', 'noopener,noreferrer')
  }

  const cardInner = (
    <>
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <span className="inline-flex items-center gap-1 rounded-full bg-emerald-500/15 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wider text-emerald-400">
              <Sparkles className="h-3 w-3" />
              Miễn phí
            </span>
            {hasPdf && (
              <span className="rounded-full bg-accent-primary/15 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wider text-accent-glow">
                PDF
              </span>
            )}
            {paper.publishedYear && (
              <span className="text-[10px] font-medium uppercase tracking-wider text-muted">{paper.publishedYear}</span>
            )}
          </div>
          <h3 className="mt-2 font-display text-lg font-bold leading-snug text-primary transition-colors group-hover:text-accent-glow">
            {paper.title}
          </h3>
        </div>
        <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg border border-border bg-base/60 text-muted transition-colors group-hover:border-accent-primary/40 group-hover:text-accent-glow">
          <BookOpen className="h-4 w-4" />
        </span>
      </div>

      {paper.abstract && (
        <p className="mt-3 line-clamp-3 text-sm leading-relaxed text-muted">{paper.abstract}</p>
      )}

      <div className="mt-5 flex flex-wrap items-center gap-4 border-t border-border/60 pt-4 text-xs text-muted">
        {paper.journalName && <span>{paper.journalName}</span>}
        {authors && <span className="max-w-md truncate">{authors}</span>}
        <span className="inline-flex items-center gap-1.5">
          <Quote className="h-3.5 w-3.5" />
          {(paper.citationCount ?? 0).toLocaleString()} citations
        </span>
      </div>
    </>
  )

  return (
    <article className="group rounded-2xl border border-emerald-500/20 bg-surface transition-all duration-300 hover:border-emerald-500/35 hover:bg-elevated hover:shadow-card">
      {readPath ? (
        <Link to={readPath} state={linkState} className="block p-6">
          {cardInner}
        </Link>
      ) : (
        <div className="p-6">{cardInner}</div>
      )}

      <div className="flex flex-wrap gap-2 border-t border-border/60 px-6 pb-4 pt-3">
        {readPath && (
          <Link
            to={readPath}
            state={linkState}
            className="inline-flex items-center gap-2 rounded-lg bg-accent-primary px-3 py-1.5 text-xs font-semibold text-white shadow-glow-sm transition-colors hover:bg-accent-glow"
          >
            <BookOpen className="h-3.5 w-3.5" />
            Đọc bài
          </Link>
        )}
        {pdfDownloadUrl && (
          <button
            type="button"
            onClick={handleDownload}
            className="inline-flex items-center gap-2 rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-3 py-1.5 text-xs font-semibold text-emerald-400 transition-colors hover:bg-emerald-500/20"
          >
            <Download className="h-3.5 w-3.5" />
            Tải PDF
          </button>
        )}
        <button
          type="button"
          onClick={handleSave}
          disabled={saving || saved}
          className="inline-flex items-center gap-2 rounded-lg border border-border bg-elevated px-3 py-1.5 text-xs font-medium text-primary transition-colors hover:border-accent-primary/40 disabled:opacity-60"
        >
          {saving ? <Loader2 className="h-3.5 w-3.5 animate-spin" /> : <Bookmark className="h-3.5 w-3.5" />}
          {saved ? 'Đã lưu' : 'Lưu thư viện'}
        </button>
      </div>
    </article>
  )
}
