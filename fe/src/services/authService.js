import { apiFetch, API_URL } from '../lib/api'

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

export async function logoutUser(refreshToken) {
  return apiFetch('/api/auth/logout', {
    method: 'POST',
    body: JSON.stringify({ refreshToken }),
  })
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

export { API_URL }
