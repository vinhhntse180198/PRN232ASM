import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { Loader2, Tags } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { getTopics, searchPapers } from '../../services/paperService'
import {
  followTopic, unfollowTopic,
  getFollows,
} from '../../services/notificationService'

export default function TopicsPage() {
  const { user } = useAuth()
  const [searchParams, setSearchParams] = useSearchParams()

  const [topics, setTopics] = useState([])
  const [followIds, setFollowIds] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const [selectedTopic, setSelectedTopic] = useState(() => {
    const t = searchParams.get('topic')
    return t ? topics.find(x => x.name === t) || null : null
  })
  const [page, setPage] = useState(1)
  const [papers, setPapers] = useState(null)
  const [loadingPapers, setLoadingPapers] = useState(false)

  const load = async () => {
    try {
      const [t, f] = await Promise.all([
        getTopics(),
        user?.id ? getFollows() : Promise.resolve({ topicIds: [] }),
      ])
      setTopics(t || [])
      setFollowIds(f?.topicIds || [])
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.id])

  const loadPapers = async (p = page, topicObj = selectedTopic) => {
    if (!topicObj) return
    setLoadingPapers(true)
    try {
      const data = await searchPapers({ page: p, pageSize: 12, topicId: topicObj.id })
      setPapers(data)
    } catch {
      setPapers(null)
    } finally {
      setLoadingPapers(false)
    }
  }

  useEffect(() => {
    if (selectedTopic) {
      setPage(1)
      loadPapers(1, selectedTopic)
    } else {
      setPapers(null)
    }
  }, [selectedTopic])

  const toggleFollow = async (topicId, topicObj) => {
    try {
      if (followIds.includes(topicId)) {
        await unfollowTopic(topicId)
      } else {
        await followTopic(topicId)
      }
      await load()
    } catch (err) {
      setError(err.message)
    }
  }

  const selectTopic = (topicObj) => {
    setSelectedTopic(topicObj)
    if (topicObj) setSearchParams({ topic: topicObj.name })
    else setSearchParams({})
  }

  const totalPages = papers ? Math.ceil(papers.totalCount / papers.pageSize) : 0

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  return (
    <div className="flex gap-6">
      {/* Sidebar */}
      <aside className="w-64 shrink-0 space-y-6">
        <section>
          <h3 className="mb-2 flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-muted">
            <Tags className="h-3.5 w-3.5" />
            Topics ({topics.length})
          </h3>
          <div className="max-h-[calc(100vh-280px)] space-y-1 overflow-y-auto">
            {topics.map((t) => (
              <div key={t.id} className="flex items-center gap-1">
                <button
                  type="button"
                  onClick={() => selectTopic(t)}
                  className={`flex-1 rounded-lg px-3 py-2 text-left text-sm transition-colors ${
                    selectedTopic?.id === t.id
                      ? 'bg-accent-primary/15 text-accent-glow font-medium'
                      : 'text-muted hover:bg-elevated hover:text-primary'
                  }`}
                >
                  <span className="block truncate">{t.name}</span>
                </button>
                <button
                  type="button"
                  onClick={() => toggleFollow(t.id, t)}
                  className={`shrink-0 rounded px-2 py-1 text-xs font-medium transition-colors ${
                    followIds.includes(t.id)
                      ? 'bg-accent-green/20 text-accent-green'
                      : 'border border-border text-muted hover:border-accent-primary/40 hover:text-primary'
                  }`}
                >
                  {followIds.includes(t.id) ? '✓' : '+'}
                </button>
              </div>
            ))}
          </div>
        </section>
      </aside>

      {/* Main */}
      <div className="flex-1 space-y-6">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Research Topics</h1>
          <p className="mt-1 text-sm text-muted">
            {selectedTopic
              ? `Papers in: "${selectedTopic.name}"`
              : 'Click a topic to view papers'}
          </p>
        </div>

        {selectedTopic && (
          <div className="flex items-center gap-3">
            <span className="rounded-full bg-accent-primary/10 px-4 py-1.5 text-sm font-medium text-accent-glow">
              {selectedTopic.name}
            </span>
            <button
              type="button"
              onClick={() => selectTopic(null)}
              className="text-xs text-muted hover:text-primary hover:underline"
            >
              Clear
            </button>
          </div>
        )}

        {error && <div className="alert-error">{error}</div>}

        {loadingPapers ? (
          <div className="flex justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
          </div>
        ) : !selectedTopic ? (
          <div className="rounded-xl border border-border bg-surface p-12 text-center">
            <Tags className="mx-auto h-12 w-12 text-muted/30" />
            <p className="mt-4 text-muted">Click a topic from the sidebar to see related papers.</p>
          </div>
        ) : (
          <>
            <p className="text-sm text-muted">
              {papers?.totalCount ?? 0} papers found for "{selectedTopic.name}"
            </p>

            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
              {(papers?.items || []).map((paper) => (
                <Link
                  key={paper.id}
                  to={`/papers/${paper.id}`}
                  className="rounded-xl border border-border bg-surface p-5 transition-all hover:-translate-y-0.5 hover:border-accent-primary/40 hover:shadow-card"
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

            {papers?.items?.length === 0 && (
              <div className="rounded-xl border border-border bg-surface p-12 text-center">
                <p className="text-muted">No papers found for this topic.</p>
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
