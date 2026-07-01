import { BookOpen, Download, FileText } from 'lucide-react'

export function ReaderTabBar({ tab, onTabChange, hasAbstract, pdfEmbedUrl, pdfDownloadUrl, onDownload }) {
  if (!hasAbstract && !pdfEmbedUrl) return null

  return (
    <div className="flex flex-wrap items-center justify-between gap-2 border-b border-border px-4 pt-2 sm:px-6">
      <div className="flex gap-1">
        {hasAbstract && (
          <TabButton active={tab === 'abstract'} onClick={() => onTabChange('abstract')} icon={FileText}>
            Tóm tắt
          </TabButton>
        )}
        {pdfEmbedUrl && (
          <TabButton active={tab === 'pdf'} onClick={() => onTabChange('pdf')} icon={BookOpen}>
            Đọc PDF trong app
          </TabButton>
        )}
      </div>
      {pdfDownloadUrl && tab === 'pdf' && (
        <a
          href={pdfDownloadUrl}
          download
          onClick={onDownload}
          className="mb-1 inline-flex items-center gap-1.5 rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-3 py-1.5 text-xs font-semibold text-emerald-400 transition-colors hover:bg-emerald-500/20"
        >
          <Download className="h-3.5 w-3.5" />
          Tải PDF
        </a>
      )}
    </div>
  )
}

export function ReaderTabPanel({ tab, paper, pdfEmbedUrl, pdfProxyUrl, pdfDownloadUrl }) {
  return (
    <div className="p-6 sm:p-8">
      {tab === 'abstract' && (
        <div>
          {paper.abstract ? (
            <p className="text-base leading-relaxed text-primary/90">{paper.abstract}</p>
          ) : (
            <p className="text-sm text-muted">
              Chưa có tóm tắt. Nếu có PDF miễn phí, dùng tab &quot;Đọc PDF trong app&quot;.
            </p>
          )}
          {paper.keywords?.length > 0 && (
            <div className="mt-8">
              <h2 className="text-xs font-bold uppercase tracking-wider text-muted">Từ khóa</h2>
              <div className="mt-3 flex flex-wrap gap-2">
                {paper.keywords.map((kw) => (
                  <span key={kw} className="rounded-full border border-border bg-elevated px-3 py-1 text-xs text-primary">
                    {kw}
                  </span>
                ))}
              </div>
            </div>
          )}
        </div>
      )}

      {tab === 'pdf' && (pdfProxyUrl || pdfEmbedUrl) && (
        <div>
          <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
            <p className="text-xs text-muted">PDF miễn phí — đọc trực tiếp trong app.</p>
            {pdfDownloadUrl && (
              <a
                href={pdfDownloadUrl}
                download
                className="inline-flex items-center gap-1.5 rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-3 py-1.5 text-xs font-semibold text-emerald-400 transition-colors hover:bg-emerald-500/20"
              >
                <Download className="h-3.5 w-3.5" />
                Tải PDF về
              </a>
            )}
          </div>
          <iframe
            title={paper.title}
            src={pdfProxyUrl || pdfEmbedUrl}
            className="h-[75vh] w-full rounded-xl border border-border bg-white"
          />
        </div>
      )}
    </div>
  )
}

function TabButton({ active, onClick, icon: Icon, children }) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`inline-flex items-center gap-1.5 border-b-2 px-4 py-3 text-sm font-medium transition-colors ${
        active
          ? 'border-accent-primary text-accent-glow'
          : 'border-transparent text-muted hover:text-primary'
      }`}
    >
      <Icon className="h-4 w-4" />
      {children}
    </button>
  )
}
