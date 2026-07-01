import { useEffect, useMemo, useState } from 'react'
import { Navigate, useLocation, useParams } from 'react-router-dom'
import PageBookReaderLayout from '../components/PageBook/PageBookReaderLayout'
import { fetchPaperById } from '../services/paperService'
import { fetchOpenAlexWork, resolvePaperDoi } from '../services/syncService'
import { OPENALEX_ENABLED } from '../config/openalex'
import { resolvePaperReaderUrls } from '../utils/paperReader'

export default function PageBookDetail() {
  const { id } = useParams()
  const location = useLocation()
  const backTo = location.state?.from || '/papers'

  const [paper, setPaper] = useState(null)
  const [openAlexExtra, setOpenAlexExtra] = useState(null)
  const [doiResolved, setDoiResolved] = useState(null)
  const [resolvingDoi, setResolvingDoi] = useState(false)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [section, setSection] = useState('abstract')

  useEffect(() => {
    let cancelled = false
    async function load() {
      setLoading(true)
      setError('')
      setDoiResolved(null)
      setOpenAlexExtra(null)
      try {
        const data = await fetchPaperById(id)
        if (cancelled) return
        setPaper(data)

        if (OPENALEX_ENABLED && data.externalId) {
          try {
            const oa = await fetchOpenAlexWork(data.externalId)
            if (!cancelled) setOpenAlexExtra(oa)
          } catch {
            /* optional */
          }
        }

        if (data.doi) {
          setResolvingDoi(true)
          try {
            const resolved = await resolvePaperDoi(data.doi)
            if (!cancelled) setDoiResolved(resolved)
          } catch {
            /* optional */
          } finally {
            if (!cancelled) setResolvingDoi(false)
          }
        }
      } catch (err) {
        if (!cancelled) setError(err.message || 'Không tìm thấy bài viết')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    load()
    return () => { cancelled = true }
  }, [id])

  const { pdfEmbedUrl, pdfProxyUrl, pdfDownloadUrl, publisherUrl } = useMemo(
    () => resolvePaperReaderUrls(paper, openAlexExtra, doiResolved),
    [paper, openAlexExtra, doiResolved],
  )

  const hasPdf = Boolean(pdfEmbedUrl)

  const readerPaper = useMemo(() => {
    if (!paper) return null
    return {
      ...paper,
      abstract: paper.abstract || openAlexExtra?.abstract || null,
      keywords: paper.keywords?.length ? paper.keywords : (openAlexExtra?.keywords ?? []),
      journalName: paper.journalName || openAlexExtra?.journalName,
      citationCount: paper.citationCount ?? openAlexExtra?.citationCount ?? 0,
    }
  }, [paper, openAlexExtra])

  /* eslint-disable react-hooks/set-state-in-effect */
  useEffect(() => {
    if (!readerPaper) return
    if (readerPaper.abstract) setSection('abstract')
    else if (hasPdf) setSection('pdf')
  }, [readerPaper, hasPdf])
  /* eslint-enable react-hooks/set-state-in-effect */

  const isPaywalled = Boolean(openAlexExtra?.isPaywalled && !hasPdf && !resolvingDoi)

  if (!loading && paper && isPaywalled && paper.externalId) {
    return <Navigate to={`/link/${paper.externalId}`} state={{ from: backTo }} replace />
  }

  return (
    <PageBookReaderLayout
      paper={readerPaper}
      loading={loading}
      error={error}
      backTo={backTo}
      activeSection={section}
      onSectionChange={setSection}
      hasPdf={hasPdf}
      pdfProxyUrl={pdfProxyUrl}
      pdfDownloadUrl={pdfDownloadUrl}
      publisherUrl={publisherUrl}
      isFree={!openAlexExtra?.isPaywalled && (hasPdf || Boolean(readerPaper?.abstract))}
      resolvingDoi={resolvingDoi}
    />
  )
}
