const PAPER_API_URL = import.meta.env.VITE_PAPER_API_URL || 'http://localhost:5002'

function authHeaders() {
  const token = localStorage.getItem('accessToken')
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  }
}

export async function fetchPapers(params = {}) {
  const result = await fetchPapersPage(params)
  return result.items
}

export async function fetchPapersPage(params = {}) {
  const qs = new URLSearchParams()
  if (params.query) qs.set('query', params.query)
  if (params.keyword) qs.set('keyword', params.keyword)
  if (params.doi) qs.set('doi', params.doi)
  if (params.year) qs.set('year', String(params.year))
  qs.set('page', String(params.page ?? 1))
  qs.set('pageSize', String(params.pageSize ?? 10))

  const url = `${PAPER_API_URL}/api/papers?${qs}`
  const res = await fetch(url, { headers: authHeaders() })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to load papers')

  const payload = data.data
  if (payload?.items) {
    return {
      items: payload.items,
      total: payload.total ?? payload.items.length,
      page: payload.page ?? params.page ?? 1,
      pageSize: payload.pageSize ?? params.pageSize ?? 10,
    }
  }
  const items = Array.isArray(payload) ? payload : []
  return { items, total: items.length, page: 1, pageSize: items.length }
}

export async function fetchPaperById(id) {
  const res = await fetch(`${PAPER_API_URL}/api/papers/${id}`, {
    headers: authHeaders(),
  })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Paper not found')
  return data.data
}

export async function addBookmark(paperId) {
  const res = await fetch(`${PAPER_API_URL}/api/bookmarks`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({ paperId }),
  })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to bookmark')
  return data.data
}

export async function removeBookmark(paperId) {
  const res = await fetch(`${PAPER_API_URL}/api/bookmarks/${paperId}`, {
    method: 'DELETE',
    headers: authHeaders(),
  })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to remove bookmark')
  return data
}

export async function fetchBookmarks() {
  const res = await fetch(`${PAPER_API_URL}/api/bookmarks`, {
    headers: authHeaders(),
  })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to load bookmarks')
  return data.data ?? []
}
