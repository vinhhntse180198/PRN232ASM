import { Link } from 'react-router-dom'
import { ArrowUpRight, BookOpen, Calendar, Quote, Sparkles } from 'lucide-react'

export default function PageBookCard({ paper }) {
  const year = paper.publishedYear || (paper.publishedDate ? new Date(paper.publishedDate).getFullYear() : null)
  const readPath = paper.externalId ? `/read/${paper.externalId}` : `/papers/${paper.id}`

  return (
    <article className="group rounded-2xl border border-border bg-surface p-6 transition-all duration-300 hover:-translate-y-1 hover:border-accent-primary/25 hover:bg-elevated hover:shadow-card">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            {paper.externalId && (
              <span className="inline-flex items-center gap-1 rounded-full bg-emerald-500/15 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wider text-emerald-400">
                <Sparkles className="h-3 w-3" />
                Miễn phí
              </span>
            )}
            {paper.doi && (
              <p className="font-mono text-[10px] uppercase tracking-wider text-accent-glow">{paper.doi}</p>
            )}
          </div>
          <h3 className="mt-2 font-display text-lg font-bold leading-snug text-primary group-hover:text-accent-glow">
            <Link to={readPath} className="hover:underline">
              {paper.title}
            </Link>
          </h3>
        </div>
        <Link
          to={readPath}
          className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg border border-border bg-base/60 text-muted transition-colors group-hover:border-accent-primary/40 group-hover:text-accent-glow"
        >
          {paper.externalId ? <BookOpen className="h-4 w-4" /> : <ArrowUpRight className="h-4 w-4" />}
        </Link>
      </div>

      {paper.abstract && (
        <p className="mt-3 line-clamp-3 text-sm leading-relaxed text-muted">{paper.abstract}</p>
      )}

      <div className="mt-5 flex flex-wrap items-center gap-4 border-t border-border/60 pt-4 text-xs text-muted">
        {year && (
          <span className="inline-flex items-center gap-1.5">
            <Calendar className="h-3.5 w-3.5" />
            {year}
          </span>
        )}
        {paper.journalName && <span>{paper.journalName}</span>}
        {paper.authors?.length > 0 && (
          <span className="max-w-[200px] truncate">{paper.authors.map((a) => a.name).join(', ')}</span>
        )}
        <span className="inline-flex items-center gap-1.5">
          <Quote className="h-3.5 w-3.5" />
          {paper.citationCount ?? 0} citations
        </span>
      </div>
    </article>
  )
}
