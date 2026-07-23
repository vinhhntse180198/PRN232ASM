const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

let refreshPromise = null

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

export function getRefreshToken() {
  return localStorage.getItem('refreshToken')
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

async function refreshAccessToken(refreshToken) {
  const res = await fetch(`${API_URL}/api/auth/refresh-token`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) {
    throw new Error(data.message || data.title || `Refresh failed (${res.status})`)
  }
  return data
}

async function tryRefreshSession() {
  const refreshToken = getRefreshToken()
  if (!refreshToken) return false

  if (!refreshPromise) {
    refreshPromise = refreshAccessToken(refreshToken)
      .then((result) => {
        const tokens = result.data ?? result
        setAuthSession({
          accessToken: tokens.accessToken,
          refreshToken: tokens.refreshToken,
          user: tokens.user,
        })
        return true
      })
      .catch(() => {
        clearAuthSession()
        return false
      })
      .finally(() => {
        refreshPromise = null
      })
  }

  return refreshPromise
}

export async function apiFetch(path, options = {}) {
  const doFetch = async () => {
    const token = getAccessToken()
    const headers = {
      'Content-Type': 'application/json',
      ...(options.headers || {}),
    }
    if (token) headers.Authorization = `Bearer ${token}`

    const res = await fetch(`${API_URL}${path}`, { ...options, headers })
    const data = await res.json().catch(() => ({}))
    return { res, data }
  }

  let { res, data } = await doFetch()

  if (res.status === 401 && !options._retried && !path.includes('/api/auth/')) {
    const refreshed = await tryRefreshSession()
    if (refreshed) {
      ;({ res, data } = await doFetch())
    }
  }

  if (!res.ok) {
    throw new Error(data.message || data.title || `Request failed (${res.status})`)
  }

  return data
}

export function unwrap(data) {
  return data?.data ?? data
}

export { API_URL }
