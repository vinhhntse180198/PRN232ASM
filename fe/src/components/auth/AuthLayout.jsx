import { Link } from 'react-router-dom'
import { APP_NAME, APP_SHORT_NAME } from '../../config/app'

export default function AuthLayout({ children, title, subtitle }) {
  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-base px-4 py-12">
      <div className="dot-grid pointer-events-none absolute inset-0 opacity-[0.04]" />
      <div className="pointer-events-none absolute left-1/2 top-1/4 h-[400px] w-[400px] -translate-x-1/2 rounded-full bg-halo" />

      <div className="relative w-full max-w-md">
        <Link to="/" className="mb-8 flex items-center justify-center gap-2 text-center font-display text-lg font-bold text-primary sm:text-xl">
          <span className="hidden sm:inline">{APP_NAME}</span>
          <span className="sm:hidden">{APP_SHORT_NAME}</span>
          <span className="h-2 w-2 shrink-0 rounded-full bg-accent-primary shadow-glow-sm" />
        </Link>

        <div className="rounded-2xl border border-border bg-surface p-8 shadow-glow-sm">
          <h1 className="font-display text-2xl font-bold tracking-tight text-primary">{title}</h1>
          <p className="mt-2 text-sm text-muted">{subtitle}</p>
          {children}
        </div>

        <p className="mt-6 text-center text-xs text-muted">
          <Link to="/" className="text-accent-glow hover:underline">
            ← Back to home
          </Link>
        </p>
      </div>
    </div>
  )
}
