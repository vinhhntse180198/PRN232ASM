import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from 'lucide-react'

const OPEN_ALEX_MAX_PAGE = 400

function pageRange(current, total, windowSize = 10) {
  if (total <= windowSize) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }
  let start = Math.max(1, current - Math.floor(windowSize / 2))
  let end = start + windowSize - 1
  if (end > total) {
    end = total
    start = Math.max(1, end - windowSize + 1)
  }
  return Array.from({ length: end - start + 1 }, (_, i) => start + i)
}

export default function PageBookSearchPagination({
  page,
  total,
  pageSize,
  onPageChange,
  showOpenAlexLimit = true,
}) {
  const rawTotalPages = Math.max(1, Math.ceil(total / pageSize))
  const totalPages = Math.min(rawTotalPages, OPEN_ALEX_MAX_PAGE)
  const capped = rawTotalPages > OPEN_ALEX_MAX_PAGE

  if (total <= pageSize) return null

  const pages = pageRange(page, totalPages)

  const go = (p) => {
    const next = Math.min(Math.max(1, p), totalPages)
    onPageChange(next)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const handleJump = (e) => {
    e.preventDefault()
    const val = parseInt(e.target.pageJump.value, 10)
    if (!Number.isNaN(val)) go(val)
  }

  return (
    <nav className="mt-10 space-y-4" aria-label="Phân trang kết quả">
      <div className="flex flex-wrap items-center justify-center gap-1">
        <PageBtn disabled={page <= 1} onClick={() => go(1)} title="Trang đầu">
          <ChevronsLeft className="h-4 w-4" />
        </PageBtn>
        <PageBtn disabled={page <= 1} onClick={() => go(page - 1)} title="Trang trước">
          <ChevronLeft className="h-4 w-4" />
        </PageBtn>

        {pages.map((p) => (
          <button
            key={p}
            type="button"
            onClick={() => go(p)}
            className={`min-w-[2.25rem] rounded-lg px-2.5 py-2 text-sm font-medium transition-colors ${
              p === page
                ? 'bg-accent-primary text-white shadow-glow-sm'
                : 'border border-border text-primary hover:border-accent-primary/40 hover:bg-elevated'
            }`}
          >
            {p}
          </button>
        ))}

        <PageBtn disabled={page >= totalPages} onClick={() => go(page + 1)} title="Trang sau">
          <ChevronRight className="h-4 w-4" />
        </PageBtn>
        <PageBtn disabled={page >= totalPages} onClick={() => go(totalPages)} title="Trang cuối">
          <ChevronsRight className="h-4 w-4" />
        </PageBtn>
      </div>

      <form onSubmit={handleJump} className="flex items-center justify-center gap-2 text-sm">
        <label htmlFor="pageJump" className="text-muted">Đến trang</label>
        <input
          id="pageJump"
          name="pageJump"
          type="number"
          min={1}
          max={totalPages}
          defaultValue={page}
          key={page}
          className="w-16 rounded-lg border border-border bg-elevated px-2 py-1.5 text-center text-primary outline-none focus:border-accent-primary/50"
        />
        <span className="text-muted">/ {totalPages.toLocaleString()}</span>
        <button
          type="submit"
          className="rounded-lg border border-border px-3 py-1.5 text-primary hover:border-accent-primary/40"
        >
          Đi
        </button>
      </form>

      {capped && showOpenAlexLimit && (
        <p className="text-center text-xs text-muted">
          OpenAlex giới hạn tối đa {OPEN_ALEX_MAX_PAGE} trang (~{(OPEN_ALEX_MAX_PAGE * pageSize).toLocaleString()} bài).
          Thu hẹp bằng từ khóa cụ thể hoặc lọc năm.
        </p>
      )}
    </nav>
  )
}

function PageBtn({ disabled, onClick, children, title }) {
  return (
    <button
      type="button"
      disabled={disabled}
      onClick={onClick}
      title={title}
      className="flex h-9 w-9 items-center justify-center rounded-lg border border-border text-primary transition-colors hover:border-accent-primary/40 disabled:opacity-35"
    >
      {children}
    </button>
  )
}
