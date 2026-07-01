import { Link } from 'react-router-dom'
import { ExternalLink, Lock, Quote } from 'lucide-react'

export default function PageBookPaidCard({ paper, returnTo }) {
  const authors = paper.authors?.length ? paper.authors.join(', ') : null
  const linkPath = paper.openAlexId ? `/link/${paper.openAlexId}` : null
  const linkState = returnTo ? { from: returnTo } : undefined
  const externalUrl = paper.url || (paper.doi ? `https://doi.org/${paper.doi}` : null)

  const cardInner = (
    <>
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <span className="inline-flex items-center gap-1 rounded-full bg-amber-500/15 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wider text-amber-400">
              <Lock className="h-3 w-3" />
              Trả phí
            </span>
            {paper.publishedYear && (
              <span className="text-[10px] font-medium uppercase tracking-wider text-muted">{paper.publishedYear}</span>
            )}
          </div>
          <h3 className="mt-2 font-display text-lg font-bold leading-snug text-primary transition-colors group-hover:text-amber-300">
            {paper.title}
          </h3>
        </div>
        <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg border border-amber-500/30 bg-amber-500/10 text-amber-400">
          <Lock className="h-4 w-4" />
        </span>
      </div>

      {paper.abstract && (
        <p className="mt-3 line-clamp-2 text-sm leading-relaxed text-muted">{paper.abstract}</p>
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
    <article className="group rounded-2xl border border-amber-500/20 bg-surface transition-all duration-300 hover:border-amber-500/40 hover:bg-elevated">
      {linkPath ? (
        <Link to={linkPath} state={linkState} className="block p-6">
          {cardInner}
        </Link>
      ) : (
        <div className="p-6">{cardInner}</div>
      )}

      <div className="flex flex-wrap gap-2 border-t border-border/60 px-6 pb-4 pt-3">
        {linkPath && (
          <Link
            to={linkPath}
            state={linkState}
            className="inline-flex items-center gap-2 rounded-lg border border-amber-500/40 bg-amber-500/10 px-3 py-1.5 text-xs font-semibold text-amber-300 transition-colors hover:bg-amber-500/20"
          >
            <ExternalLink className="h-3.5 w-3.5" />
            Trang liên kết
          </Link>
        )}
        {externalUrl && (
          <a
            href={externalUrl}
            target="_blank"
            rel="noreferrer"
            onClick={(e) => e.stopPropagation()}
            className="inline-flex items-center gap-2 rounded-lg border border-border bg-elevated px-3 py-1.5 text-xs font-medium text-primary transition-colors hover:border-amber-500/40"
          >
            <ExternalLink className="h-3.5 w-3.5" />
            Mở nhà xuất bản
          </a>
        )}
      </div>
    </article>
  )
}
