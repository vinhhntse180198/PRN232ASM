import { useEffect, useState } from 'react'
import { Navigate, useLocation, useParams } from 'react-router-dom'
import PageBookReaderLayout from '../components/PageBook/PageBookReaderLayout'
import { OPENALEX_ENABLED } from '../config/openalex'
import { fetchOpenAlexWork, importOpenAlexPaper } from '../services/syncService'
import { resolveOpenAlexPaperUrls } from '../utils/paperReader'

export default function PageBookReader() {
  const { openAlexId } = useParams()
  const location = useLocation()
  const backTo = location.state?.from || '/papers'

  const [paper, setPaper] = useState(null)
  const [loading, setLoading] = useState(OPENALEX_ENABLED)
  const [error, setError] = useState('')
  const [section, setSection] = useState('abstract')
  const [saving, setSaving] = useState(false)
  const [saved, setSaved] = useState(false)

  useEffect(() => {
    if (!OPENALEX_ENABLED) return

    let cancelled = false
    async function load() {
      setLoading(true)
      setError('')
      try {
        const data = await fetchOpenAlexWork(openAlexId)
        if (!cancelled) {
          setPaper(data)
          setSection(data.abstract ? 'abstract' : (data.pdfUrl || data.hasPdf ? 'pdf' : 'abstract'))
        }
      } catch (err) {
        if (!cancelled) setError(err.message || 'Không tải được bài viết')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    load()
    return () => { cancelled = true }
  }, [openAlexId])

  const handleSave = async () => {
    if (!OPENALEX_ENABLED || !paper?.openAlexId || saving || saved) return
    setSaving(true)
    try {
      await importOpenAlexPaper(paper.openAlexId)
      setSaved(true)
    } catch {
      /* ignore */
    } finally {
      setSaving(false)
    }
  }

  if (!OPENALEX_ENABLED) {
    return <Navigate to="/papers" replace />
  }

  const { pdfEmbedUrl, pdfProxyUrl, pdfDownloadUrl, publisherUrl } = resolveOpenAlexPaperUrls(paper)
  const hasPdf = Boolean(pdfEmbedUrl)

  if (!loading && paper?.isPaywalled) {
    return <Navigate to={`/link/${openAlexId}`} state={{ from: backTo }} replace />
  }

  return (
    <PageBookReaderLayout
      paper={paper}
      loading={loading}
      error={error}
      backTo={backTo}
      activeSection={section}
      onSectionChange={setSection}
      hasPdf={hasPdf}
      pdfProxyUrl={pdfProxyUrl}
      pdfDownloadUrl={pdfDownloadUrl}
      publisherUrl={publisherUrl}
      isFree
      onSave={handleSave}
      saving={saving}
      saved={saved}
    />
  )
}
