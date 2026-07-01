import { OPENALEX_ENABLED } from '../config/openalex'

/** Dev: dùng Vite proxy (cùng origin). Prod: URL đầy đủ từ .env */
const SYNC_API_URL = (
  import.meta.env.VITE_SYNC_API_URL ??
  (import.meta.env.DEV ? '' : 'http://localhost:5005')
).replace(/\/$/, '')

function assertOpenAlexEnabled() {
  if (!OPENALEX_ENABLED) {
    throw new Error('OpenAlex đã tắt — app chỉ dùng thư viện local miễn phí.')
  }
}

export async function syncFromOpenAlex({ years, year, perPage = 100 } = {}) {
  assertOpenAlexEnabled()
  const qs = new URLSearchParams()
  if (years) qs.set('years', years)
  if (year) qs.set('year', String(year))
  qs.set('perPage', String(perPage))

  const res = await fetch(`${SYNC_API_URL}/api/sync/run/OpenAlex?${qs}`, { method: 'POST' })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Sync failed')
  return data.data
}

export async function searchOpenAlex({ query, year, page = 1, perPage = 25, openAccessOnly = false, paywalledOnly = false } = {}) {
  assertOpenAlexEnabled()
  const qs = new URLSearchParams({ query, page: String(page), perPage: String(perPage) })
  if (year && year !== 'all') qs.set('year', String(year))
  if (openAccessOnly) qs.set('openAccessOnly', 'true')
  if (paywalledOnly) qs.set('paywalledOnly', 'true')

  const res = await fetch(`${SYNC_API_URL}/api/openalex/search?${qs}`)
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'OpenAlex search failed')
  return data.data
}

export async function browseOpenAlex({ year, page = 1, perPage = 10 } = {}) {
  assertOpenAlexEnabled()
  const qs = new URLSearchParams({ page: String(page), perPage: String(perPage) })
  if (year && year !== 'all') qs.set('year', String(year))

  const res = await fetch(`${SYNC_API_URL}/api/openalex/browse?${qs}`)
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || `Không tải được danh sách bài (HTTP ${res.status}).`)
  return data.data
}

export async function browsePaidOpenAlex({ year, page = 1, perPage = 10 } = {}) {
  assertOpenAlexEnabled()
  const qs = new URLSearchParams({ page: String(page), perPage: String(perPage) })
  if (year && year !== 'all') qs.set('year', String(year))

  const res = await fetch(`${SYNC_API_URL}/api/openalex/browse/paid?${qs}`)
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Không tải được bài trả phí.')
  return data.data
}

export async function resolvePaperDoi(doi) {
  const qs = new URLSearchParams({ doi })
  const res = await fetch(`${SYNC_API_URL}/api/openalex/resolve?${qs}`)
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Could not resolve DOI')
  return data.data
}

export async function fetchOpenAlexWork(openAlexId) {
  assertOpenAlexEnabled()
  const res = await fetch(`${SYNC_API_URL}/api/openalex/works/${encodeURIComponent(openAlexId)}`)
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to load paper')
  return data.data
}

export async function importOpenAlexPaper(openAlexId) {
  assertOpenAlexEnabled()
  const res = await fetch(`${SYNC_API_URL}/api/openalex/import/${encodeURIComponent(openAlexId)}`, {
    method: 'POST',
  })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Import failed')
  return data.data
}

export async function fetchSyncStatus() {
  const res = await fetch(`${SYNC_API_URL}/api/sync/status`)
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to load sync status')
  return data.data ?? []
}
