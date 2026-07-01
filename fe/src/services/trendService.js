// Prefer Vite proxy (relative /api/*) when env is unset.
// If you set VITE_TREND_API_URL, we call that absolute origin instead.
const TREND_API_URL = import.meta.env.VITE_TREND_API_URL || ''

function authHeaders() {
  const token = localStorage.getItem('accessToken')
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  }
}

export async function fetchTrendsDashboard({ year } = {}) {
  const qs = new URLSearchParams()
  if (year && year !== 'all') qs.set('year', String(year))

  const url = `${TREND_API_URL}/api/trends/dashboard${qs.toString() ? `?${qs}` : ''}`
  const res = await fetch(url, { headers: authHeaders() })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to load trends dashboard')
  return data.data
}

export async function refreshTrends() {
  const url = `${TREND_API_URL}/api/trends/refresh`
  const res = await fetch(url, { method: 'POST', headers: authHeaders() })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to refresh trends')
  return data.data
}

export const TREND_BADGE_LABELS = {
  rising: 'Đang lên',
  stable: 'Ổn định',
  falling: 'Giảm',
}

export const TREND_BADGE_STYLES = {
  rising: 'bg-emerald-500/15 text-emerald-400 border-emerald-500/30',
  stable: 'bg-sky-500/15 text-sky-400 border-sky-500/30',
  falling: 'bg-rose-500/15 text-rose-400 border-rose-500/30',
}
