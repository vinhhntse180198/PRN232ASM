const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export function getStoredUser() {
  try {
    const raw = localStorage.getItem('user')
    return raw ? JSON.parse(raw) : null
  } catch {
    return null
  }
}

export function getAccessToken() {
  return localStorage.getItem('accessToken')
}

export function setAuthSession({ accessToken, refreshToken, user }) {
  if (accessToken) localStorage.setItem('accessToken', accessToken)
  if (refreshToken) localStorage.setItem('refreshToken', refreshToken)
  if (user) localStorage.setItem('user', JSON.stringify(user))
}

export function clearAuthSession() {
  localStorage.removeItem('accessToken')
  localStorage.removeItem('refreshToken')
  localStorage.removeItem('user')
}

export async function apiFetch(path, options = {}) {
  const token = getAccessToken()
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {}),
  }
  if (token) headers.Authorization = `Bearer ${token}`

  const res = await fetch(`${API_URL}${path}`, { ...options, headers })
  const data = await res.json().catch(() => ({}))

  if (!res.ok) {
    throw new Error(data.message || data.title || `Request failed (${res.status})`)
  }

  return data
}

export function unwrap(data) {
  return data?.data ?? data
}

export { API_URL }
