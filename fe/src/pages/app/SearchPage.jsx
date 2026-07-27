import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Loader2, Search, X } from 'lucide-react'
import { searchPapers, getTopics } from '../../services/paperService'

export default function SearchPage() {
  const [keyword, setKeyword] = useState('')
  const [topics, setTopics] = useState([])
  const [page, setPage] = useState(1)
  const [result, setResult] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getTopics().then(setTopics).catch(() => {})
  }, [])

  const load = async (p = page, kw = keyword) => {
    setLoading(true)
    setError('')
    try {
      const data = await searchPapers({ page: p, pageSize: 12, keyword: kw || undefined })
      setResult(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load(1, keyword)
  }, [keyword])

  const handleSearch = (e) => {
    e.preventDefault()
    setPage(1)
    load(1, keyword)
  }

  const clearAll = () => {
    setKeyword('')
    setPage(1)
    load(1, '')
  }

  const totalPages = result ? Math.ceil(result.totalCount / result.pageSize) : 0

  return (
    <div className="flex gap-6">
      {/* Sidebar */}
      <aside className="hidden w-56 shrink-0 space-y-6 lg:block">
        <div>
          <h3 className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted">Topics</h3>
          <div className="space-y-1 max-h-60 overflow-y-auto">
            {topics.map((t) => (
              <Link
                key={t.id}
                to={`/topics?topic=${encodeURIComponent(t.name)}`}
                className="block rounded-lg px-3 py-2 text-sm text-muted transition-colors hover:bg-elevated hover:text-primary"
              >
                {t.name}
              </Link>
            ))}
          </div>
        </div>

        <div>
          <h3 className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted">Browse</h3>
          <div className="space-y-1">
            <Link
              to="/keywords"
              className="block rounded-lg px-3 py-2 text-sm text-muted transition-colors hover:bg-elevated hover:text-primary"
            >
              Browse Keywords
            </Link>
            <Link
              to="/authors"
              className="block rounded-lg px-3 py-2 text-sm text-muted transition-colors hover:bg-elevated hover:text-primary"
            >
              Browse Authors
            </Link>
          </div>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 space-y-6">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Search Papers</h1>
          <p className="mt-1 text-sm text-muted">
            {keyword ? `Results for "${keyword}"` : 'All papers'}
          </p>
        </div>

        <form onSubmit={handleSearch} className="flex gap-2">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted" />
            <input
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
              placeholder="Search by paper title..."
              className="w-full rounded-lg border border-border bg-elevated py-2.5 pl-10 pr-4 text-sm text-primary placeholder-muted/50 focus:border-accent-primary/50 focus:outline-none"
            />
          </div>
          <button type="submit" className="btn-primary px-6">Search</button>
          {keyword && (
            <button type="button" onClick={clearAll} className="btn-secondary flex items-center gap-1 px-3">
              <X className="h-4 w-4" />
            </button>
          )}
        </form>

        {error && <div className="alert-error">{error}</div>}

        {loading ? (
          <div className="flex justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
          </div>
        ) : (
          <>
            <p className="text-sm text-muted">
              {result?.totalCount ?? 0} papers found
              {keyword && <> for "{keyword}"</>}
            </p>

            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
              {(result?.items || []).map((paper) => (
                <Link
                  key={paper.id}
                  to={`/papers/${paper.id}`}
                  className="rounded-xl border border-border bg-surface p-5 transition-all hover:-translate-y-0.5 hover:border-accent-primary/40 hover:shadow-card"
                >
                  <h3 className="line-clamp-2 text-sm font-semibold text-primary">{paper.title}</h3>
                  <p className="mt-2 line-clamp-2 text-xs text-muted">{paper.abstract}</p>
                  <div className="mt-3 flex flex-wrap gap-2 text-xs text-muted">
                    <span>{paper.publicationYear}</span>
                    <span>·</span>
                    <span>{paper.journalName}</span>
                    <span>·</span>
                    <span>{paper.citationCount} citations</span>
                  </div>
                  {paper.authors?.length > 0 && (
                    <p className="mt-2 truncate text-xs text-muted">
                      {[...new Set(paper.authors)].join(', ')}
                    </p>
                  )}
                </Link>
              ))}
            </div>

            {result?.items?.length === 0 && (
              <div className="rounded-xl border border-border bg-surface p-12 text-center">
                <Search className="mx-auto h-10 w-10 text-muted/30" />
                <p className="mt-4 text-muted">No papers found.</p>
              </div>
            )}

            {totalPages > 1 && (
              <div className="flex items-center justify-center gap-3">
                <button
                  type="button"
                  disabled={page <= 1}
                  onClick={() => { const p = page - 1; setPage(p); load(p) }}
                  className="btn-secondary"
                >
                  Previous
                </button>
                <span className="text-sm text-muted">Page {page} / {totalPages}</span>
                <button
                  type="button"
                  disabled={page >= totalPages}
                  onClick={() => { const p = page + 1; setPage(p); load(p) }}
                  className="btn-secondary"
                >
                  Next
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}
