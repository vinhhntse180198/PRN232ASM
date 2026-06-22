import { apiRequest } from './apiClient'

export async function searchPapers(params = {}) {
  const query = new URLSearchParams()
  if (params.keyword) query.set('keyword', params.keyword)
  if (params.author) query.set('author', params.author)
  if (params.journal) query.set('journal', params.journal)
  if (params.topicId) query.set('topicId', params.topicId)
  query.set('page', String(params.page || 1))
  query.set('pageSize', String(params.pageSize || 10))
  return apiRequest(`/api/papers/search?${query.toString()}`)
}

export async function getPaperById(id) {
  return apiRequest(`/api/papers/${id}`)
}

export async function getJournals() {
  return apiRequest('/api/journals')
}

export async function getTopics() {
  return apiRequest('/api/topics')
}

export async function getKeywords() {
  return apiRequest('/api/keywords')
}

export async function getBookmarks() {
  return apiRequest('/api/bookmarks', { auth: true })
}

export async function addBookmark(paperId) {
  return apiRequest(`/api/bookmarks/${paperId}`, { method: 'POST', auth: true })
}

export async function removeBookmark(paperId) {
  return apiRequest(`/api/bookmarks/${paperId}`, { method: 'DELETE', auth: true })
}
