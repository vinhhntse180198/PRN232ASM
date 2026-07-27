import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Loader2, Users, Search } from 'lucide-react'
import { getAuthors, searchPapers } from '../../services/paperService'

export default function AuthorsPage() {
  const [authors, setAuthors] = useState([])
  const [search, setSearch] = useState('')
  const [selectedAuthor, setSelectedAuthor] = useState(null)

  const [page, setPage] = useState(1)
  const [papers, setPapers] = useState(null)
  const [loadingAuthors, setLoadingAuthors] = useState(true)
  const [loadingPapers, setLoadingPapers] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    getAuthors()
      .then((data) => {
        const seen = new Map()
        for (const a of data || []) {
          if (!seen.has(a.name.toLowerCase())) {
            seen.set(a.name.toLowerCase(), a)
          }
        }
        setAuthors([...seen.values()])
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoadingAuthors(false))
  }, [])

  const loadPapers = async (p = page, authorName = selectedAuthor) => {
    if (!authorName) return
    setLoadingPapers(true)
    setError('')
    try {
      const data = await searchPapers({ page: p, pageSize: 12, author: authorName })
      setPapers(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoadingPapers(false)
    }
  }

  useEffect(() => {
    if (selectedAuthor) {
      setPage(1)
      loadPapers(1, selectedAuthor)
    } else {
      setPapers(null)
    }
  }, [selectedAuthor])

  const selectAuthor = (author) => {
    setSelectedAuthor(author?.name || null)
    setPage(1)
  }

  const filteredAuthors = authors.filter((a) =>
    a.name.toLowerCase().includes(search.toLowerCase())
  )

  const totalPages = papers ? Math.ceil(papers.totalCount / papers.pageSize) : 0

  return (
    <div className="flex gap-6">
      {/* Sidebar */}
      <aside className="w-64 shrink-0 space-y-4">
        <div>
          <h3 className="mb-2 flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-muted">
            <Users className="h-3.5 w-3.5" />
            Authors ({filteredAuthors.length})
          </h3>
          <div className="relative mb-2">
            <Search className="absolute left-2.5 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted" />
            <input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Filter authors..."
              className="w-full rounded-lg border border-border bg-elevated py-1.5 pl-8 pr-3 text-xs text-primary placeholder-muted/50 focus:border-accent-primary/50 focus:outline-none"
            />
          </div>
          <div className="max-h-[calc(100vh-280px)] space-y-1 overflow-y-auto">
            {loadingAuthors ? (
              <div className="flex justify-center py-4">
                <Loader2 className="h-4 w-4 animate-spin text-accent-primary" />
              </div>
            ) : filteredAuthors.length === 0 ? (
              <p className="px-3 py-2 text-xs text-muted">No authors found.</p>
            ) : (
              filteredAuthors.map((a) => (
                <button
                  key={a.id}
                  type="button"
                  onClick={() => selectAuthor(a)}
                  className={`block w-full rounded-lg px-3 py-2 text-left text-sm transition-colors ${
                    selectedAuthor === a.name
                      ? 'bg-accent-primary/15 text-accent-glow font-medium'
                      : 'text-muted hover:bg-elevated hover:text-primary'
                  }`}
                >
                  <span className="block truncate">{a.name}</span>
                  {a.affiliation && (
                    <span className="block truncate text-xs text-muted/60">{a.affiliation}</span>
                  )}
                </button>
              ))
            )}
          </div>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 space-y-6">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Authors</h1>
          <p className="mt-1 text-sm text-muted">
            {selectedAuthor
              ? `Papers by "${selectedAuthor}"`
              : 'Select an author to view their papers'}
          </p>
        </div>

        {error && <div className="alert-error">{error}</div>}

        {selectedAuthor && (
          <div className="flex items-center gap-3">
            <span className="rounded-full bg-accent-primary/10 px-4 py-1.5 text-sm font-medium text-accent-glow">
              {selectedAuthor}
            </span>
            <button
              type="button"
              onClick={() => selectAuthor(null)}
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
        ) : !selectedAuthor ? (
          <div className="rounded-xl border border-border bg-surface p-12 text-center">
            <Users className="mx-auto h-12 w-12 text-muted/30" />
            <p className="mt-4 text-muted">Select an author from the sidebar to view their papers.</p>
          </div>
        ) : (
          <>
            <p className="text-sm text-muted">
              {papers?.totalCount ?? 0} papers by this author
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
                <p className="text-muted">No papers found for this author.</p>
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
