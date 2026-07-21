import { apiFetch, unwrap } from '../lib/api'

export async function getDataSources() {
  const res = await apiFetch('/api/datasources')
  return unwrap(res)
}

export async function updateDataSource(id, payload) {
  const res = await apiFetch(`/api/datasources/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  })
  return unwrap(res)
}

export async function triggerSync() {
  const res = await apiFetch('/api/sync/trigger', { method: 'POST' })
  return unwrap(res)
}

export async function getSyncLogs() {
  const res = await apiFetch('/api/sync/logs')
  return unwrap(res)
}

export async function getSyncStatus() {
  const res = await apiFetch('/api/sync/status')
  return unwrap(res)
}

export async function getHealth() {
  const res = await fetch(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/health`)
  return res.json()
}
