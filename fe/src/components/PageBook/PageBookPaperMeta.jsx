import { useState } from 'react'
import { ExternalLink } from 'lucide-react'
import { formatPublishDate } from '../../utils/citation'

function MetaRow({ label, children, light }) {
  if (!children) return null
  return (
    <div className={`grid grid-cols-[110px_1fr] gap-3 py-3 text-sm last:border-0 ${
      light ? 'border-b border-stone-100' : 'border-b border-border/60'
    }`}>
      <dt className={`font-semibold ${light ? 'text-stone-800' : 'text-primary'}`}>{label}</dt>
      <dd className={light ? 'text-stone-600' : 'text-muted'}>{children}</dd>
    </div>
  )
}

export default function PageBookPaperMeta({ paper, publisherUrl, isFree, variant = 'dark' }) {
  const light = variant === 'light'
  const [abstractOpen, setAbstractOpen] = useState(false)
  const authors = Array.isArray(paper?.authors)
    ? paper.authors.map((a) => (typeof a === 'string' ? a : a?.name)).filter(Boolean)
    : []
  const keywords = paper?.keywords ?? []
  const abstract = paper?.abstract?.trim()
  const shortAbstract = abstract && abstract.length > 220 && !abstractOpen
    ? `${abstract.slice(0, 220)}…`
    : abstract
  const linkClass = light ? 'text-stone-800 hover:underline' : 'text-accent-glow hover:underline'

  return (
    <div className={`rounded-lg shadow-sm ${light ? 'bg-white' : 'rounded-xl border border-border bg-surface shadow-card'}`}>
      <div className={`px-4 py-3 ${light ? 'border-b border-stone-100' : 'border-b border-border/60'}`}>
        <p className={`text-[10px] font-bold uppercase tracking-widest ${light ? 'text-stone-400' : 'text-muted'}`}>
          Công việc
        </p>
        <p className={`mt-0.5 line-clamp-2 text-xs font-medium ${light ? 'text-stone-700' : 'text-primary'}`}>
          {paper?.title}
        </p>
      </div>

      <dl className="px-4 py-1">
        <MetaRow label="Năm" light={light}>{paper?.publishedYear || formatPublishDate(paper).split(',').pop()?.trim()}</MetaRow>
        <MetaRow label="Loại" light={light}>bài báo</MetaRow>
        {abstract && (
          <MetaRow label="Tóm tắt" light={light}>
            <span className="leading-relaxed">{shortAbstract}</span>
            {abstract.length > 220 && (
              <button type="button" onClick={() => setAbstractOpen((v) => !v)} className={`ml-1 ${linkClass}`}>
                {abstractOpen ? 'thu gọn' : 'thêm'}
              </button>
            )}
          </MetaRow>
        )}
        <MetaRow label="Nguồn" light={light}>
          {publisherUrl ? (
            <a href={publisherUrl} target="_blank" rel="noreferrer" className={linkClass}>
              {paper?.journalName || 'Trang nhà xuất bản'}
              <ExternalLink className="ml-1 inline h-3 w-3" />
            </a>
          ) : (
            paper?.journalName
          )}
        </MetaRow>
        {authors.length > 0 && (
          <MetaRow label="Tác giả" light={light}>
            <span className={linkClass}>{authors.join(' · ')}</span>
          </MetaRow>
        )}
        {paper?.doi && (
          <MetaRow label="DOI" light={light}>
            <a
              href={`https://doi.org/${paper.doi.replace(/^https?:\/\/(dx\.)?doi\.org\//i, '')}`}
              target="_blank"
              rel="noreferrer"
              className={`break-all font-mono text-xs ${linkClass}`}
            >
              {paper.doi}
            </a>
          </MetaRow>
        )}
        <MetaRow label="Trích dẫn" light={light}>
          <span className={`font-semibold ${light ? 'text-stone-900' : 'text-accent-glow'}`}>
            {(paper?.citationCount ?? 0).toLocaleString()}
          </span>
        </MetaRow>
        {keywords.length > 0 && (
          <MetaRow label="Chủ đề" light={light}>
            <div className="flex flex-wrap gap-1">
              {keywords.slice(0, 4).map((kw) => (
                <span key={kw} className={light ? 'text-stone-700' : 'text-accent-glow'}>{kw}</span>
              ))}
            </div>
          </MetaRow>
        )}
        <MetaRow label="Truy cập mở" light={light}>
          <span className={isFree ? (light ? 'text-emerald-700' : 'text-emerald-400') : (light ? 'text-amber-700' : 'text-amber-400')}>
            {isFree ? 'Miễn phí' : 'Trả phí'}
          </span>
        </MetaRow>
      </dl>
    </div>
  )
}
