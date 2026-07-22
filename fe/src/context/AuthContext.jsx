import { createContext, useCallback, useContext, useMemo, useState } from 'react'
import { clearAuthSession, getStoredUser, setAuthSession } from '../lib/api'
import { loginUser, logoutUser, registerUser } from '../services/authService'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => getStoredUser())

  const login = useCallback(async (email, password) => {
    const result = await loginUser({ email, password })
    const tokens = result.data ?? result
    setAuthSession({
      accessToken: tokens.accessToken,
      refreshToken: tokens.refreshToken,
      user: tokens.user,
    })
    setUser(tokens.user)
    return tokens
  }, [])

  const register = useCallback(async (fullName, email, password) => {
    const result = await registerUser({ fullName, email, password })
    const tokens = result.data ?? result
    setAuthSession({
      accessToken: tokens.accessToken,
      refreshToken: tokens.refreshToken,
      user: tokens.user,
    })
    setUser(tokens.user)
    return tokens
  }, [])

  const logout = useCallback(async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken')
      if (refreshToken) await logoutUser(refreshToken)
    } catch {
      // ignore logout API errors
    }
    clearAuthSession()
    setUser(null)
  }, [])

  const value = useMemo(
    () => ({
      user,
      isAuthenticated: Boolean(user),
      isAdmin: user?.role === 'Admin',
      login,
      register,
      logout,
    }),
    [user, login, register, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
