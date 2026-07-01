const SYNC_API_URL = (
  import.meta.env.VITE_SYNC_API_URL ??
  (import.meta.env.DEV ? '' : 'http://localhost:5005')
).replace(/\/$/, '')

export function getDoiUrl(doi) {
  if (!doi?.trim()) return null
  const normalized = doi.trim().replace(/^https?:\/\/(dx\.)?doi\.org\//i, '')
  return `https://doi.org/${normalized}`
}

export function getProxiedPdfUrl(pdfUrl, { download = false } = {}) {
  if (!pdfUrl) return null
  const qs = new URLSearchParams({ url: pdfUrl })
  if (download) qs.set('download', 'true')
  return `${SYNC_API_URL}/api/openalex/pdf?${qs}`
}

/** Chrome/Edge PDF viewer: fit page width so users don't need manual zoom */
export function buildPdfEmbedSrc(proxyUrl) {
  if (!proxyUrl) return null
  const base = proxyUrl.split('#')[0]
  return `${base}#toolbar=1&navpanes=0&scrollbar=1&view=FitH&zoom=page-width`
}

function isPublisherUrl(url) {
  return Boolean(url?.trim() && !url.includes('doi.org'))
}

function endsWithPdf(url) {
  return Boolean(url?.trim() && url.trimEnd().toLowerCase().endsWith('.pdf'))
}

export function resolvePaperReaderUrls(paper, openAlexExtra = null, doiResolved = null) {
  const rawUrl = paper?.url?.trim() || null
  const pdfFromOa = openAlexExtra?.pdfUrl?.trim() || null
  const oaUrl = openAlexExtra?.oaUrl?.trim() || null
  const pdfFromDoi = doiResolved?.pdfUrl?.trim() || null
  const landingFromDoi = doiResolved?.landingUrl?.trim() || null

  const pdfEmbedUrl =
    pdfFromDoi ||
    pdfFromOa ||
    (endsWithPdf(rawUrl) ? rawUrl : null) ||
    (endsWithPdf(oaUrl) ? oaUrl : null)

  const publisherUrl =
    [landingFromDoi, oaUrl, rawUrl, openAlexExtra?.url].find(isPublisherUrl) || null

  const doiUrl = getDoiUrl(paper?.doi)

  return {
    pdfEmbedUrl,
    pdfProxyUrl: pdfEmbedUrl ? getProxiedPdfUrl(pdfEmbedUrl) : null,
    pdfDownloadUrl: pdfEmbedUrl ? getProxiedPdfUrl(pdfEmbedUrl, { download: true }) : null,
    publisherUrl,
    doiUrl,
  }
}

export function resolveOpenAlexPaperUrls(paper) {
  if (!paper) {
    return {
      pdfEmbedUrl: null,
      pdfProxyUrl: null,
      pdfDownloadUrl: null,
      publisherUrl: null,
    }
  }
  return resolvePaperReaderUrls(
    { url: paper.url || paper.readUrl, doi: paper.doi },
    { pdfUrl: paper.pdfUrl, oaUrl: paper.oaUrl, url: paper.url },
  )
}

export function getOpenAlexPublisherUrl(paper) {
  const { publisherUrl } = resolveOpenAlexPaperUrls(paper)
  return publisherUrl || getDoiUrl(paper?.doi) || paper?.openAlexUrl || null
}

export function splitPapersByAccess(papers = []) {
  const free = []
  const paid = []
  for (const paper of papers) {
    if (paper.isPaywalled) paid.push(paper)
    else free.push(paper)
  }
  return { free, paid }
}
