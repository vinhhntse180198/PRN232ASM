import { apiFetch, API_URL, getRefreshToken, clearAuthSession } from '../lib/api'

export async function loginUser({ email, password }) {
  return apiFetch('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  })
}

export async function registerUser({ fullName, email, password }) {
  return apiFetch('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify({ fullName, email, password }),
  })
}

export async function logoutUser() {
  const refreshToken = getRefreshToken()
  try {
    await apiFetch('/api/auth/logout', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    })
  } catch {
    // Ignore errors — always clear session locally
  } finally {
    clearAuthSession()
  }
}

export async function getUsers() {
  return apiFetch('/api/users')
}

export async function updateUserRole(userId, role) {
  return apiFetch(`/api/users/${userId}/role`, {
    method: 'PUT',
    body: JSON.stringify({ role }),
  })
}

export async function changePassword(currentPassword, newPassword) {
  return apiFetch('/api/auth/change-password', {
    method: 'POST',
    body: JSON.stringify({ currentPassword, newPassword }),
  })
}

export { API_URL }
