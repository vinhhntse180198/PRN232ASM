const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

let isRefreshing = false
let refreshSubscribers = []

function subscribeTokenRefresh(callback) {
  refreshSubscribers.push(callback)
}

function onTokenRefreshed(newAccessToken) {
  refreshSubscribers.forEach((cb) => cb(newAccessToken))
  refreshSubscribers = []
}

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

async function tryRefreshToken() {
  const refreshToken = getRefreshToken()
  if (!refreshToken) return null

  try {
    const res = await fetch(`${API_URL}/api/auth/refresh-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    })

    if (!res.ok) {
      clearAuthSession()
      window.location.href = '/login'
      return null
    }

    const data = await res.json()
    const newAccessToken = data.data?.accessToken
    if (!newAccessToken) return null

    setAuthSession({
      accessToken: newAccessToken,
      refreshToken: data.data?.refreshToken || refreshToken,
      user: getStoredUser(),
    })
    return newAccessToken
  } catch {
    clearAuthSession()
    window.location.href = '/login'
    return null
  }
}

export async function apiFetch(path, options = {}) {
  const token = getAccessToken()
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {}),
  }
  if (token) headers.Authorization = `Bearer ${token}`

  let res = await fetch(`${API_URL}${path}`, { ...options, headers })
  const data = await res.json().catch(() => ({}))

  if (res.status === 401 && !options._retry) {
    if (isRefreshing) {
      return new Promise((resolve) => {
        subscribeTokenRefresh((newToken) => {
          const retryHeaders = { ...headers, Authorization: `Bearer ${newToken}` }
          resolve(fetch(`${API_URL}${path}`, { ...options, headers: retryHeaders }).then((r) => r.json()))
        })
      })
    }

    isRefreshing = true
    const newToken = await tryRefreshToken()
    isRefreshing = false

    if (newToken) {
      onTokenRefreshed(newToken)
      const retryHeaders = { ...headers, Authorization: `Bearer ${newToken}` }
      res = await fetch(`${API_URL}${path}`, { ...options, headers: retryHeaders })
      const retryData = await res.json().catch(() => ({}))
      if (!res.ok) {
        throw new Error(retryData.message || retryData.title || `Request failed (${res.status})`)
      }
      return retryData
    }

    throw new Error(data.message || data.title || `Request failed (401)`)
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
