import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Eye, EyeOff, Loader2 } from 'lucide-react'
import AuthLayout from '../components/auth/AuthLayout'
import { registerUser } from '../services/authService'

export default function RegisterPage() {
  const navigate = useNavigate()
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')

    if (password !== confirmPassword) {
      setError('Passwords do not match')
      return
    }
    if (password.length < 8) {
      setError('Password must be at least 8 characters')
      return
    }

    setLoading(true)
    try {
      const result = await registerUser({ fullName, email, password })
      if (result.data?.accessToken) {
        localStorage.setItem('accessToken', result.data.accessToken)
      }
      navigate('/login')
    } catch (err) {
      setError(err.message || 'Registration failed. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <AuthLayout
      title="Start free today"
      subtitle="Create your agency account on Collective OS"
    >
      <form onSubmit={handleSubmit} className="mt-8 space-y-5">
        {error && (
          <div className="rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-300">
            {error}
          </div>
        )}

        <div>
          <label htmlFor="fullName" className="block text-sm font-medium text-primary">
            Full name
          </label>
          <input
            id="fullName"
            type="text"
            required
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            placeholder="Rahim Hassan"
            className="mt-2 w-full rounded-xl border border-border bg-elevated px-4 py-3 text-sm text-primary placeholder:text-muted outline-none transition-colors focus:border-accent-primary/50 focus:ring-1 focus:ring-accent-primary/30"
          />
        </div>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-primary">
            Work email
          </label>
          <input
            id="email"
            type="email"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="you@agency.com"
            className="mt-2 w-full rounded-xl border border-border bg-elevated px-4 py-3 text-sm text-primary placeholder:text-muted outline-none transition-colors focus:border-accent-primary/50 focus:ring-1 focus:ring-accent-primary/30"
          />
        </div>

        <div>
          <label htmlFor="password" className="block text-sm font-medium text-primary">
            Password
          </label>
          <div className="relative mt-2">
            <input
              id="password"
              type={showPassword ? 'text' : 'password'}
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Min. 8 characters"
              className="w-full rounded-xl border border-border bg-elevated px-4 py-3 pr-11 text-sm text-primary placeholder:text-muted outline-none transition-colors focus:border-accent-primary/50 focus:ring-1 focus:ring-accent-primary/30"
            />
            <button
              type="button"
              onClick={() => setShowPassword(!showPassword)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-muted hover:text-primary"
            >
              {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
            </button>
          </div>
        </div>

        <div>
          <label htmlFor="confirmPassword" className="block text-sm font-medium text-primary">
            Confirm password
          </label>
          <input
            id="confirmPassword"
            type={showPassword ? 'text' : 'password'}
            required
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            placeholder="Repeat password"
            className="mt-2 w-full rounded-xl border border-border bg-elevated px-4 py-3 text-sm text-primary placeholder:text-muted outline-none transition-colors focus:border-accent-primary/50 focus:ring-1 focus:ring-accent-primary/30"
          />
        </div>

        <label className="flex items-start gap-2 text-xs text-muted">
          <input type="checkbox" required className="mt-0.5 rounded border-border bg-elevated accent-accent-primary" />
          I agree to the Terms of Service and Privacy Policy
        </label>

        <button
          type="submit"
          disabled={loading}
          className="flex w-full items-center justify-center gap-2 rounded-xl bg-accent-primary py-3.5 text-sm font-semibold text-white shadow-glow-sm transition-all hover:bg-accent-glow disabled:opacity-60"
        >
          {loading && <Loader2 className="h-4 w-4 animate-spin" />}
          Create account
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-muted">
        Already have an account?{' '}
        <Link to="/login" className="font-medium text-accent-glow hover:underline">
          Sign in
        </Link>
      </p>
    </AuthLayout>
  )
}
