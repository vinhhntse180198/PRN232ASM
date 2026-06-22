import { Link } from 'react-router-dom'
import { Bookmark, ExternalLink, Quote } from 'lucide-react'

export default function PaperCard({ paper, onToggleBookmark, bookmarkLoading }) {
  const abstractPreview =
    paper.abstract && paper.abstract.length > 180
      ? `${paper.abstract.slice(0, 180)}...`
      : paper.abstract

  return (
    <article className="rounded-2xl border border-border bg-surface p-6 transition-colors hover:border-accent-primary/30">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <Link
            to={`/papers/${paper.id}`}
            className="font-display text-lg font-semibold text-primary hover:text-accent-glow"
          >
            {paper.title}
          </Link>
          <div className="mt-2 flex flex-wrap gap-2 text-xs text-muted">
            {paper.publishedYear && <span>{paper.publishedYear}</span>}
            {paper.journalName && (
              <>
                <span>•</span>
                <span>{paper.journalName}</span>
              </>
            )}
            <span>•</span>
            <span>{paper.citationCount} citations</span>
          </div>
          {paper.authors?.length > 0 && (
            <p className="mt-2 text-sm text-muted">{paper.authors.join(', ')}</p>
          )}
        </div>

        <button
          type="button"
          onClick={() => onToggleBookmark?.(paper)}
          disabled={bookmarkLoading}
          className={`shrink-0 rounded-lg border p-2 transition-colors ${
            paper.isBookmarked
              ? 'border-accent-primary/50 bg-accent-primary/10 text-accent-glow'
              : 'border-border text-muted hover:border-accent-primary/40 hover:text-primary'
          }`}
          title={paper.isBookmarked ? 'Remove bookmark' : 'Add bookmark'}
        >
          <Bookmark className={`h-4 w-4 ${paper.isBookmarked ? 'fill-current' : ''}`} />
        </button>
      </div>

      {abstractPreview && (
        <p className="mt-4 flex gap-2 text-sm leading-relaxed text-muted">
          <Quote className="mt-0.5 h-4 w-4 shrink-0 text-accent-primary/60" />
          <span>{abstractPreview}</span>
        </p>
      )}

      {paper.keywords?.length > 0 && (
        <div className="mt-4 flex flex-wrap gap-2">
          {paper.keywords.map((kw) => (
            <span
              key={kw}
              className="rounded-full border border-border bg-elevated px-2.5 py-1 text-xs text-muted"
            >
              {kw}
            </span>
          ))}
        </div>
      )}

      <div className="mt-4">
        <Link
          to={`/papers/${paper.id}`}
          className="inline-flex items-center gap-1 text-sm font-medium text-accent-glow hover:underline"
        >
          View details
          <ExternalLink className="h-3.5 w-3.5" />
        </Link>
      </div>
    </article>
  )
}
