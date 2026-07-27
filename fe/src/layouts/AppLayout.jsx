import { Link, NavLink, useNavigate } from 'react-router-dom'
import { BookOpen, Bookmark, FileText, LogOut } from 'lucide-react'
import { getAccessToken } from '../services/apiClient'

const navItems = [
  { to: '/papers', label: 'Papers', icon: FileText },
  { to: '/bookmarks', label: 'Bookmarks', icon: Bookmark },
]

export default function AppLayout({ children }) {
  const navigate = useNavigate()
  const isLoggedIn = Boolean(getAccessToken())

  const handleLogout = () => {
    localStorage.removeItem('accessToken')
    navigate('/login')
  }

  return (
    <div className="min-h-screen bg-base text-primary">
      <header className="glass-nav sticky top-0 z-40 border-b border-border">
        <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
          <Link to="/papers" className="flex items-center gap-2 font-display text-lg font-bold">
            <BookOpen className="h-5 w-5 text-accent-primary" />
            Paper Trend Tracker
          </Link>

          <nav className="flex items-center gap-1">
            {navItems.map(({ to, label, icon: Icon }) => (
              <NavLink
                key={to}
                to={to}
                className={({ isActive }) =>
                  `flex items-center gap-2 rounded-lg px-3 py-2 text-sm transition-colors ${
                    isActive ? 'bg-elevated text-accent-glow' : 'text-muted hover:text-primary'
                  }`
                }
              >
                <Icon className="h-4 w-4" />
                <span className="hidden sm:inline">{label}</span>
              </NavLink>
            ))}
            {isLoggedIn ? (
              <button
                onClick={handleLogout}
                className="ml-2 flex items-center gap-2 rounded-lg px-3 py-2 text-sm text-muted hover:text-primary"
              >
                <LogOut className="h-4 w-4" />
                <span className="hidden sm:inline">Logout</span>
              </button>
            ) : (
              <Link
                to="/login"
                className="ml-2 rounded-lg bg-accent-primary px-4 py-2 text-sm font-medium text-white shadow-glow-sm hover:bg-accent-glow"
              >
                Sign in
              </Link>
            )}
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">{children}</main>
    </div>
  )
}
