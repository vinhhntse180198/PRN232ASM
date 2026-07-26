import { apiFetch, unwrap } from '../lib/api'

export async function searchPapers({ page = 1, pageSize = 20, keyword, author, journal, topicId } = {}) {
  const params = new URLSearchParams({ page, pageSize })
  if (keyword) params.set('keyword', keyword)
  if (author) params.set('author', author)
  if (journal) params.set('journal', journal)
  if (topicId) params.set('topicId', topicId)
  const res = await apiFetch(`/api/papers?${params}`)
  return unwrap(res)
}

export async function getPaper(id) {
  const res = await apiFetch(`/api/papers/${id}`)
  return unwrap(res)
}

export async function getAuthors() {
  const res = await apiFetch('/api/authors')
  return unwrap(res)
}

export async function getJournals() {
  const res = await apiFetch('/api/journals')
  return unwrap(res)
}

export async function getKeywords() {
  const res = await apiFetch('/api/keywords')
  return unwrap(res)
}

export async function getTopics() {
  const res = await apiFetch('/api/topics')
  return unwrap(res)
}

export async function getBookmarks() {
  const res = await apiFetch('/api/bookmarks')
  return unwrap(res)
}

export async function addBookmark(paperId) {
  const res = await apiFetch('/api/bookmarks', {
    method: 'POST',
    body: JSON.stringify({ paperId }),
  })
  return unwrap(res)
}

export async function removeBookmark(paperId) {
  return apiFetch(`/api/bookmarks/${paperId}`, { method: 'DELETE' })
}
