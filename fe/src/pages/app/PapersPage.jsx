import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Loader2, Search } from 'lucide-react'
import { searchPapers } from '../../services/paperService'

export default function PapersPage() {
  const [keyword, setKeyword] = useState('')
  const [author, setAuthor] = useState('')
  const [journal, setJournal] = useState('')
  const [page, setPage] = useState(1)
  const [result, setResult] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const load = async (p = page) => {
    setLoading(true)
    setError('')
    try {
      const data = await searchPapers({ page: p, pageSize: 12, keyword, author, journal })
      setResult(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load(1)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const handleSearch = (e) => {
    e.preventDefault()
    setPage(1)
    load(1)
  }

  const totalPages = result ? Math.ceil(result.totalCount / result.pageSize) : 0

  return (
    <div className="space-y-6">
      <div>
        <h1 className="font-display text-2xl font-bold text-primary">Search Papers</h1>
        <p className="text-sm text-muted">Search by keyword, author, or journal</p>
      </div>

      <form onSubmit={handleSearch} className="grid gap-3 rounded-xl border border-border bg-surface p-4 md:grid-cols-4">
        <input
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
          placeholder="Keyword"
          className="input-field"
        />
        <input
          value={author}
          onChange={(e) => setAuthor(e.target.value)}
          placeholder="Author"
          className="input-field"
        />
        <input
          value={journal}
          onChange={(e) => setJournal(e.target.value)}
          placeholder="Journal"
          className="input-field"
        />
        <button type="submit" className="btn-primary flex items-center justify-center gap-2">
          <Search className="h-4 w-4" />
          Search
        </button>
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
          </p>
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
            {(result?.items || []).map((paper) => (
              <Link
                key={paper.id}
                to={`/papers/${paper.id}`}
                className="rounded-xl border border-border bg-surface p-5 transition-colors hover:border-accent-primary/40"
              >
                <h3 className="line-clamp-2 font-semibold text-primary">{paper.title}</h3>
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
  )
}
