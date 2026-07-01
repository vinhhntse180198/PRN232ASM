const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5131'

function authHeaders() {
  const token = localStorage.getItem('accessToken')
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  }
}

export function saveUserSession(result) {
  if (result.data?.accessToken) {
    localStorage.setItem('accessToken', result.data.accessToken)
  }
  if (result.data?.fullName) {
    localStorage.setItem('userFullName', result.data.fullName)
  }
  if (result.data?.email) {
    localStorage.setItem('userEmail', result.data.email)
  }
}

export async function loginUser({ email, password }) {
  const res = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  })

  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Login failed')
  return data
}

export async function registerUser({ fullName, email, password }) {
  const res = await fetch(`${API_URL}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ fullName, email, password }),
  })

  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Registration failed')
  return data
}

export async function changePassword({ currentPassword, newPassword }) {
  const res = await fetch(`${API_URL}/api/auth/change-password`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({ currentPassword, newPassword }),
  })
  const data = await res.json().catch(() => ({}))
  if (!res.ok) throw new Error(data.message || 'Failed to change password')
  return data
}
