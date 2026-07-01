export function formatApaCitation(paper) {
  if (!paper?.title) return ''
  const authors = paper.authors
  const authorStr = Array.isArray(authors)
    ? authors
        .slice(0, 3)
        .map((a) => (typeof a === 'string' ? a : a?.name))
        .filter(Boolean)
        .join(', ')
    : ''
  const year = paper.publishedYear || paper.publishedDate?.getFullYear?.() || 'n.d.'
  const journal = paper.journalName ? ` *${paper.journalName}*` : ''
  const doi = paper.doi ? ` https://doi.org/${paper.doi.replace(/^https?:\/\/(dx\.)?doi\.org\//i, '')}` : ''
  return `${authorStr || 'Unknown'} (${year}). ${paper.title}.${journal}${doi}`
}

export function extractKeyTakeaways(abstract, limit = 3) {
  if (!abstract?.trim()) return []
  return abstract
    .split(/(?<=[.!?])\s+/)
    .map((s) => s.trim())
    .filter((s) => s.length > 40)
    .slice(0, limit)
}

export function formatPublishDate(paper) {
  if (paper?.publishedDate) {
    const d = new Date(paper.publishedDate)
    if (!Number.isNaN(d.getTime())) {
      return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
    }
  }
  if (paper?.publishedYear) return `Jan 1, ${paper.publishedYear}`
  return '—'
}

export function citationBars(citationCount = 0) {
  const base = Math.max(1, Math.min(citationCount, 200))
  return [0.35, 0.55, 0.45, 0.7, 0.85, 1].map((m) => Math.round(base * m * 0.4 + 8))
}
