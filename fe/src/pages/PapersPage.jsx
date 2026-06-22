import { useCallback, useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Loader2 } from 'lucide-react'
import AppLayout from '../layouts/AppLayout'
import PaperCard from '../components/papers/PaperCard'
import SearchFilters from '../components/papers/SearchFilters'
import { getAccessToken } from '../services/apiClient'
import {
  addBookmark,
  getTopics,
  removeBookmark,
  searchPapers,
} from '../services/paperService'

export default function PapersPage() {
  const navigate = useNavigate()
  const [filters, setFilters] = useState({ keyword: '', author: '', journal: '', topicId: '' })
  const [topics, setTopics] = useState([])
  const [papers, setPapers] = useState([])
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [bookmarkLoadingId, setBookmarkLoadingId] = useState(null)

  const loadTopics = useCallback(async () => {
    try {
      const res = await getTopics()
      setTopics(res.data || [])
    } catch {
      setTopics([])
    }
  }, [])

  const loadPapers = useCallback(async (pageNum = 1) => {
    setLoading(true)
    setError('')
    try {
      const res = await searchPapers({
        keyword: filters.keyword || undefined,
        author: filters.author || undefined,
        journal: filters.journal || undefined,
        topicId: filters.topicId || undefined,
        page: pageNum,
        pageSize: 10,
      })
      setPapers(res.data?.items || [])
      setPage(res.data?.page || 1)
      setTotalPages(res.data?.totalPages || 0)
    } catch (err) {
      setError(err.message || 'Failed to load papers')
      setPapers([])
    } finally {
      setLoading(false)
    }
  }, [filters])

  useEffect(() => {
    loadTopics()
    loadPapers(1)
  }, []) // eslint-disable-line react-hooks/exhaustive-deps

  const handleSearch = () => loadPapers(1)

  const handleToggleBookmark = async (paper) => {
    if (!getAccessToken()) {
      navigate('/login', { state: { from: '/papers' } })
      return
    }

    setBookmarkLoadingId(paper.id)
    try {
      if (paper.isBookmarked) {
        await removeBookmark(paper.id)
      } else {
        await addBookmark(paper.id)
      }
      setPapers((prev) =>
        prev.map((p) => (p.id === paper.id ? { ...p, isBookmarked: !p.isBookmarked } : p))
      )
    } catch (err) {
      setError(err.message || 'Bookmark action failed')
    } finally {
      setBookmarkLoadingId(null)
    }
  }

  return (
    <AppLayout>
      <div className="space-y-8">
        <div>
          <h1 className="font-display text-3xl font-bold text-primary">Research Papers</h1>
          <p className="mt-2 text-muted">
            Search scientific publications by keyword, author, journal, or topic.
          </p>
        </div>

        <SearchFilters
          filters={filters}
          topics={topics}
          onChange={setFilters}
          onSubmit={handleSearch}
          loading={loading}
        />

        {error && (
          <div className="rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-300">
            {error}
          </div>
        )}

        {loading ? (
          <div className="flex justify-center py-16">
            <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
          </div>
        ) : papers.length === 0 ? (
          <div className="rounded-2xl border border-border bg-surface px-6 py-16 text-center text-muted">
            No papers found. Try adjusting your search filters.
          </div>
        ) : (
          <div className="space-y-4">
            {papers.map((paper) => (
              <PaperCard
                key={paper.id}
                paper={paper}
                onToggleBookmark={handleToggleBookmark}
                bookmarkLoading={bookmarkLoadingId === paper.id}
              />
            ))}
          </div>
        )}

        {totalPages > 1 && (
          <div className="flex items-center justify-center gap-3">
            <button
              type="button"
              disabled={page <= 1 || loading}
              onClick={() => loadPapers(page - 1)}
              className="rounded-lg border border-border px-4 py-2 text-sm disabled:opacity-50"
            >
              Previous
            </button>
            <span className="text-sm text-muted">
              Page {page} of {totalPages}
            </span>
            <button
              type="button"
              disabled={page >= totalPages || loading}
              onClick={() => loadPapers(page + 1)}
              className="rounded-lg border border-border px-4 py-2 text-sm disabled:opacity-50"
            >
              Next
            </button>
          </div>
        )}
      </div>
    </AppLayout>
  )
}
