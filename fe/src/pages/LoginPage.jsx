import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Eye, EyeOff, Loader2 } from 'lucide-react'
import AuthLayout from '../components/auth/AuthLayout'
import { loginUser } from '../services/authService'

export default function LoginPage() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)

    try {
      const result = await loginUser({ email, password })
      if (result.data?.accessToken) {
        localStorage.setItem('accessToken', result.data.accessToken)
      }
      navigate('/')
    } catch (err) {
      setError(err.message || 'Invalid email or password')
    } finally {
      setLoading(false)
    }
  }

  return (
    <AuthLayout
      title="Welcome back"
      subtitle="Sign in to your Collective OS account"
    >
      <form onSubmit={handleSubmit} className="mt-8 space-y-5">
        {error && (
          <div className="rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-300">
            {error}
          </div>
        )}

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-primary">
            Email
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
              placeholder="••••••••"
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

        <div className="flex items-center justify-between text-sm">
          <label className="flex items-center gap-2 text-muted">
            <input type="checkbox" className="rounded border-border bg-elevated accent-accent-primary" />
            Remember me
          </label>
          <a href="#" className="text-accent-glow hover:underline">
            Forgot password?
          </a>
        </div>

        <button
          type="submit"
          disabled={loading}
          className="flex w-full items-center justify-center gap-2 rounded-xl bg-accent-primary py-3.5 text-sm font-semibold text-white shadow-glow-sm transition-all hover:bg-accent-glow disabled:opacity-60"
        >
          {loading && <Loader2 className="h-4 w-4 animate-spin" />}
          Sign in
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-muted">
        Don&apos;t have an account?{' '}
        <Link to="/register" className="font-medium text-accent-glow hover:underline">
          Create one free
        </Link>
      </p>
    </AuthLayout>
  )
}
