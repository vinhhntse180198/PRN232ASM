import { apiFetch, unwrap } from '../lib/api'

export async function getNotifications(userId) {
  const res = await apiFetch(`/api/notifications?userId=${userId}`)
  return unwrap(res)
}

export async function markNotificationRead(id) {
  return apiFetch(`/api/notifications/${id}/read`, { method: 'PUT' })
}

export async function markAllNotificationsRead(userId) {
  return apiFetch(`/api/notifications/read-all?userId=${userId}`, { method: 'PUT' })
}

export async function getFollows(userId) {
  const res = await apiFetch(`/api/follows?userId=${userId}`)
  return unwrap(res)
}

export async function followTopic(userId, topicId) {
  return apiFetch('/api/follows/topic', {
    method: 'POST',
    body: JSON.stringify({ userId, topicId }),
  })
}

export async function followKeyword(userId, keywordId) {
  return apiFetch('/api/follows/keyword', {
    method: 'POST',
    body: JSON.stringify({ userId, keywordId }),
  })
}

export async function unfollowTopic(userId, topicId) {
  return apiFetch(`/api/follows/topic/${topicId}?userId=${userId}`, { method: 'DELETE' })
}
