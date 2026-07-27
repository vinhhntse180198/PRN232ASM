import { apiFetch, unwrap } from '../lib/api'

export async function getNotifications() {
  const res = await apiFetch('/api/notifications')
  return unwrap(res)
}

export async function markNotificationRead(id) {
  return apiFetch(`/api/notifications/${id}/read`, { method: 'PUT' })
}

export async function markAllNotificationsRead() {
  return apiFetch('/api/notifications/read-all', { method: 'PUT' })
}

export async function getFollows() {
  const res = await apiFetch('/api/follows')
  return unwrap(res)
}

export async function followTopic(topicId) {
  return apiFetch('/api/follows/topic', {
    method: 'POST',
    body: JSON.stringify({ topicId }),
  })
}

export async function unfollowTopic(topicId) {
  return apiFetch(`/api/follows/topic/${topicId}`, { method: 'DELETE' })
}

export async function followKeyword(keywordId) {
  return apiFetch('/api/follows/keyword', {
    method: 'POST',
    body: JSON.stringify({ keywordId }),
  })
}

export async function unfollowKeyword(keywordId) {
  return apiFetch(`/api/follows/keyword/${keywordId}`, { method: 'DELETE' })
}

export async function followJournal(journalId) {
  return apiFetch('/api/follows/journal', {
    method: 'POST',
    body: JSON.stringify({ journalId }),
  })
}

export async function unfollowJournal(journalId) {
  return apiFetch(`/api/follows/journal/${journalId}`, { method: 'DELETE' })
}
