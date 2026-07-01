import {
  BookOpen,
  Download,
  ExternalLink,
  FileText,
  Loader2,
  Quote,
} from 'lucide-react'
import PageBookReaderNavbar from './PageBookReaderNavbar'
import PageBookPaperMeta from './PageBookPaperMeta'
import PageBookAnalyticsPanel from './PageBookAnalyticsPanel'
import PageBookPdfViewer from './PageBookPdfViewer'
import { formatApaCitation } from '../../utils/citation'

const SECTIONS = [
  { id: 'abstract', label: 'Abstract', icon: FileText },
  { id: 'pdf', label: 'Full Text', icon: BookOpen },
]

function authorNames(paper) {
  if (!paper?.authors?.length) return []
  return paper.authors.map((a) => (typeof a === 'string' ? a : a?.name)).filter(Boolean)
}

export default function PageBookReaderLayout({
  paper,
  loading,
  error,
  activeSection,
  onSectionChange,
  hasPdf,
  pdfProxyUrl,
  pdfDownloadUrl,
  publisherUrl,
  isFree = true,
  onSave,
  saving,
  saved,
  resolvingDoi = false,
}) {
  const citation = paper ? formatApaCitation(paper) : ''
  const sections = SECTIONS.filter((s) => (s.id === 'pdf' ? hasPdf : true))
  const authors = authorNames(paper)
  const year = paper?.publishedYear || (paper?.publishedDate ? new Date(paper.publishedDate).getFullYear() : null)
  const isPdfView = activeSection === 'pdf' && hasPdf

  const handleCite = async () => {
    if (!citation) return
    try {
      await navigator.clipboard.writeText(citation)
    } catch {
      /* ignore */
    }
  }

  return (
    <div className="min-h-screen bg-base">
      <PageBookReaderNavbar theme="dark" />

      {loading && (
        <div className="flex justify-center py-32 pt-40">
          <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
        </div>
      )}

      {error && !loading && (
        <div className="mx-auto mt-28 max-w-lg rounded-xl border border-red-500/30 bg-red-500/10 px-6 py-8 text-center text-sm text-red-300">
          {error}
        </div>
      )}

      {paper && !loading && (
        <div
          className={
            isPdfView
              ? 'mx-auto grid max-w-[100%] grid-cols-1 gap-3 px-2 pt-20 lg:grid-cols-[minmax(220px,250px)_1fr] lg:gap-4 lg:px-4 lg:pt-24'
              : 'mx-auto grid max-w-[1600px] grid-cols-1 gap-5 p-4 pt-24 lg:grid-cols-[minmax(240px,280px)_1fr_minmax(260px,300px)] lg:gap-6 lg:p-6 lg:pt-28'
          }
        >
          <aside className="space-y-4">
            <PageBookPaperMeta paper={paper} publisherUrl={publisherUrl} isFree={isFree} />

            <div className="rounded-xl border border-border/60 bg-surface p-3 shadow-card">
              <p className="mb-2 text-[10px] font-bold uppercase tracking-widest text-muted">
                Research Explorer
              </p>
              <p className="mb-3 text-[11px] text-muted">Current Paper Context</p>
              <nav className="space-y-0.5">
                {sections.map(({ id, label, icon: Icon }) => (
                  <button
                    key={id}
                    type="button"
                    onClick={() => onSectionChange(id)}
                    className={`flex w-full items-center gap-2 rounded-md px-3 py-2 text-left text-sm transition-colors ${
                      activeSection === id
                        ? 'bg-accent-primary/15 font-medium text-accent-glow'
                        : 'text-muted hover:bg-elevated/60 hover:text-primary'
                    }`}
                  >
                    <Icon className="h-4 w-4 opacity-60" />
                    {label}
                  </button>
                ))}
              </nav>
              <div className="mt-4 space-y-2">
                <button
                  type="button"
                  onClick={handleCite}
                  className="flex w-full items-center justify-center gap-2 rounded-md bg-accent-primary px-3 py-2.5 text-sm font-medium text-white shadow-glow-sm hover:bg-accent-glow"
                >
                  <Quote className="h-4 w-4" />
                  Cite Paper
                </button>
                {pdfDownloadUrl && (
                  <a
                    href={pdfDownloadUrl}
                    download
                    className="flex w-full items-center justify-center gap-2 rounded-md border border-border px-3 py-2 text-sm font-medium text-primary hover:bg-elevated"
                  >
                    <Download className="h-4 w-4" />
                    Tải PDF
                  </a>
                )}
                {onSave && (
                  <button
                    type="button"
                    onClick={onSave}
                    disabled={saving || saved}
                    className="flex w-full items-center justify-center gap-2 rounded-md border border-border px-3 py-2 text-sm text-primary hover:bg-elevated disabled:opacity-50"
                  >
                    {saving ? <Loader2 className="h-4 w-4 animate-spin" /> : null}
                    {saved ? 'Đã lưu' : 'Lưu thư viện'}
                  </button>
                )}
              </div>
            </div>
          </aside>

          <main className="min-w-0">
            <div className="mb-4 flex gap-2 lg:hidden">
              {sections.map(({ id, label }) => (
                <button
                  key={id}
                  type="button"
                  onClick={() => onSectionChange(id)}
                  className={`rounded-full px-3 py-1 text-xs font-medium ${
                    activeSection === id ? 'bg-accent-primary text-white' : 'bg-surface text-muted'
                  }`}
                >
                  {label}
                </button>
              ))}
            </div>

            {isPdfView ? (
              <div className="flex min-h-[calc(100vh-5.5rem)] flex-col">
                <div className="mb-2 flex shrink-0 flex-wrap items-center justify-between gap-2 px-1">
                  <p className="line-clamp-2 flex-1 font-serif text-sm font-semibold leading-snug text-primary sm:text-base">
                    {paper.title}
                  </p>
                  {pdfDownloadUrl && (
                    <a
                      href={pdfDownloadUrl}
                      download
                      className="shrink-0 text-xs font-medium text-accent-glow hover:underline"
                    >
                      Tải PDF về
                    </a>
                  )}
                </div>
                <PageBookPdfViewer
                  proxyUrl={pdfProxyUrl}
                  title={paper.title}
                  className="min-h-0 flex-1"
                />
              </div>
            ) : (
              <article className="overflow-hidden rounded-xl bg-surface shadow-card">
                <div className="px-6 pb-2 pt-8 sm:px-10 sm:pt-10">
                  <div className="mb-5 flex flex-wrap items-center gap-2 text-sm">
                    {isFree && (
                      <span className="rounded-full border border-accent-green/30 bg-accent-green/10 px-2.5 py-0.5 text-xs text-accent-green">
                        Open Access
                      </span>
                    )}
                    {paper.journalName && (
                      <span className="text-accent-glow">{paper.journalName}</span>
                    )}
                    {year && (
                      <span className="rounded-full border border-border px-2.5 py-0.5 text-xs text-muted">
                        {year}
                      </span>
                    )}
                  </div>

                  <h1 className="font-serif text-2xl font-bold leading-snug text-primary sm:text-[1.65rem]">
                    {paper.title}
                  </h1>

                  {authors.length > 0 && (
                    <p className="mt-4 text-sm text-muted">{authors.join(' · ')}</p>
                  )}
                </div>

                {resolvingDoi && !hasPdf && (
                  <div className="flex items-center gap-2 px-6 pb-4 text-sm text-muted sm:px-10">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Đang tải thông tin bài viết…
                  </div>
                )}

                <section className="px-6 pb-10 sm:px-10">
                  <h2 className="mt-6 flex items-center gap-2 font-serif text-lg font-semibold text-primary">
                    <FileText className="h-5 w-5 text-accent-primary" />
                    Abstract
                  </h2>
                  {paper.abstract ? (
                    <p className="mt-5 font-serif text-base leading-[1.9] text-muted">{paper.abstract}</p>
                  ) : (
                    <p className="mt-4 text-sm text-muted">
                      Chưa có tóm tắt cho bài này.
                      {hasPdf ? ' Chọn Full Text để đọc PDF.' : ''}
                    </p>
                  )}
                  {publisherUrl && (
                    <div className="mt-10 border-t border-border/40 pt-6">
                      <p className="text-xs font-medium text-muted">Trang nhà xuất bản</p>
                      <a
                        href={publisherUrl}
                        target="_blank"
                        rel="noreferrer"
                        className="mt-2 inline-flex items-center gap-1.5 text-sm font-medium text-accent-glow hover:underline"
                      >
                        Mở trang bài viết
                        <ExternalLink className="h-3.5 w-3.5" />
                      </a>
                    </div>
                  )}
                </section>
              </article>
            )}
          </main>

          {!isPdfView && (
            <aside>
              <PageBookAnalyticsPanel paper={paper} isFree={isFree} />
            </aside>
          )}
        </div>
      )}
    </div>
  )
}
