import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Loader2, Tag } from 'lucide-react'
import { getKeywords, searchPapers } from '../../services/paperService'

export default function KeywordsPage() {
  const [keywords, setKeywords] = useState([])
  const [search, setSearch] = useState('')
  const [selectedKeyword, setSelectedKeyword] = useState(null)

  const [page, setPage] = useState(1)
  const [papers, setPapers] = useState(null)
  const [loadingKeywords, setLoadingKeywords] = useState(true)
  const [loadingPapers, setLoadingPapers] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    getKeywords()
      .then((data) => setKeywords(data || []))
      .catch((err) => setError(err.message))
      .finally(() => setLoadingKeywords(false))
  }, [])

  const loadPapers = async (p = page, kwName = selectedKeyword) => {
    if (!kwName) return
    setLoadingPapers(true)
    setError('')
    try {
      const data = await searchPapers({ page: p, pageSize: 12, keyword: kwName })
      setPapers(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoadingPapers(false)
    }
  }

  useEffect(() => {
    if (selectedKeyword) {
      setPage(1)
      loadPapers(1, selectedKeyword)
    } else {
      setPapers(null)
    }
  }, [selectedKeyword])

  const selectKeyword = (kw) => {
    setSelectedKeyword(kw?.name || null)
    setPage(1)
  }

  const filteredKeywords = keywords.filter((k) =>
    k.name.toLowerCase().includes(search.toLowerCase())
  )

  const totalPages = papers ? Math.ceil(papers.totalCount / papers.pageSize) : 0

  return (
    <div className="flex gap-6">
      {/* Sidebar */}
      <aside className="w-64 shrink-0 space-y-4">
        <div>
          <h3 className="mb-2 flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-muted">
            <Tag className="h-3.5 w-3.5" />
            Keywords ({filteredKeywords.length})
          </h3>
          <div className="relative mb-2">
            <Tag className="absolute left-2.5 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted" />
            <input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Filter keywords..."
              className="w-full rounded-lg border border-border bg-elevated py-1.5 pl-8 pr-3 text-xs text-primary placeholder-muted/50 focus:border-accent-primary/50 focus:outline-none"
            />
          </div>
          <div className="max-h-[calc(100vh-280px)] space-y-1 overflow-y-auto">
            {loadingKeywords ? (
              <div className="flex justify-center py-4">
                <Loader2 className="h-4 w-4 animate-spin text-accent-primary" />
              </div>
            ) : filteredKeywords.length === 0 ? (
              <p className="px-3 py-2 text-xs text-muted">No keywords found.</p>
            ) : (
              filteredKeywords.map((k) => (
                <button
                  key={k.id}
                  type="button"
                  onClick={() => selectKeyword(k)}
                  className={`block w-full rounded-lg px-3 py-2 text-left text-sm transition-colors ${
                    selectedKeyword === k.name
                      ? 'bg-accent-primary/15 text-accent-glow font-medium'
                      : 'text-muted hover:bg-elevated hover:text-primary'
                  }`}
                >
                  <span className="block truncate">{k.name}</span>
                </button>
              ))
            )}
          </div>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 space-y-6">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Keywords</h1>
          <p className="mt-1 text-sm text-muted">
            {selectedKeyword
              ? `Papers matching: "${selectedKeyword}"`
              : 'Select a keyword to view related papers'}
          </p>
        </div>

        {error && <div className="alert-error">{error}</div>}

        {selectedKeyword && (
          <div className="flex items-center gap-3">
            <span className="rounded-full bg-accent-primary/10 px-4 py-1.5 text-sm font-medium text-accent-glow">
              {selectedKeyword}
            </span>
            <button
              type="button"
              onClick={() => selectKeyword(null)}
              className="text-xs text-muted hover:text-primary hover:underline"
            >
              Clear
            </button>
          </div>
        )}

        {loadingPapers ? (
          <div className="flex justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
          </div>
        ) : !selectedKeyword ? (
          <div className="rounded-xl border border-border bg-surface p-12 text-center">
            <Tag className="mx-auto h-12 w-12 text-muted/30" />
            <p className="mt-4 text-muted">Select a keyword from the sidebar to view related papers.</p>
          </div>
        ) : (
          <>
            <p className="text-sm text-muted">
              {papers?.totalCount ?? 0} papers found for "{selectedKeyword}"
            </p>

            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
              {(papers?.items || []).map((paper) => (
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
                </Link>
              ))}
            </div>

            {papers?.items?.length === 0 && (
              <div className="rounded-xl border border-border bg-surface p-12 text-center">
                <p className="text-muted">No papers found for this keyword.</p>
              </div>
            )}

            {totalPages > 1 && (
              <div className="flex items-center justify-center gap-3">
                <button
                  type="button"
                  disabled={page <= 1}
                  onClick={() => { const p = page - 1; setPage(p); loadPapers(p) }}
                  className="btn-secondary"
                >
                  Previous
                </button>
                <span className="text-sm text-muted">Page {page} / {totalPages}</span>
                <button
                  type="button"
                  disabled={page >= totalPages}
                  onClick={() => { const p = page + 1; setPage(p); loadPapers(p) }}
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
