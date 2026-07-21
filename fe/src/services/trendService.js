import { apiFetch, unwrap } from '../lib/api'

export async function getTrends({ keyword, year } = {}) {
  const params = new URLSearchParams()
  if (keyword) params.set('keyword', keyword)
  if (year) params.set('year', year)
  const qs = params.toString()
  const res = await apiFetch(`/api/trends${qs ? `?${qs}` : ''}`)
  return unwrap(res)
}

export async function getAnalytics() {
  const res = await apiFetch('/api/analytics')
  return unwrap(res)
}

export async function getDashboard() {
  const res = await apiFetch('/api/dashboard')
  return unwrap(res)
}

export async function getReports() {
  const res = await apiFetch('/api/reports')
  return unwrap(res)
}

export async function generateReport() {
  const res = await apiFetch('/api/reports/generate', { method: 'POST' })
  return unwrap(res)
}
