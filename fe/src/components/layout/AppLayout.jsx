import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import {
  BarChart3,
  Bell,
  Bookmark,
  FileText,
  LayoutDashboard,
  LogOut,
  Search,
  Settings,
  Tags,
} from 'lucide-react'
import { useAuth } from '../../context/AuthContext'

const navItems = [
  { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/papers', icon: Search, label: 'Search Papers' },
  { to: '/bookmarks', icon: Bookmark, label: 'Bookmarks' },
  { to: '/topics', icon: Tags, label: 'Follow Topics' },
  { to: '/notifications', icon: Bell, label: 'Notifications' },
  { to: '/reports', icon: FileText, label: 'Reports' },
]

export default function AppLayout() {
  const { user, logout, isAdmin } = useAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    navigate('/login')
  }

  return (
    <div className="flex min-h-screen bg-base">
      <aside className="flex w-64 flex-col border-r border-border bg-surface">
        <div className="border-b border-border px-5 py-5">
          <Link to="/dashboard" className="font-display text-lg font-bold text-primary">
            Paper Trend Tracker
          </Link>
          <p className="mt-1 text-xs text-muted">Scientific publication analytics</p>
        </div>

        <nav className="flex-1 space-y-1 p-3">
          {navItems.map(({ to, icon: Icon, label }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm transition-colors ${
                  isActive
                    ? 'bg-accent-primary/15 text-accent-glow'
                    : 'text-muted hover:bg-elevated hover:text-primary'
                }`
              }
            >
              <Icon className="h-4 w-4" />
              {label}
            </NavLink>
          ))}

          {isAdmin && (
            <NavLink
              to="/admin"
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm transition-colors ${
                  isActive
                    ? 'bg-accent-amber/15 text-accent-amber'
                    : 'text-muted hover:bg-elevated hover:text-primary'
                }`
              }
            >
              <Settings className="h-4 w-4" />
              Admin Panel
            </NavLink>
          )}
        </nav>

        <div className="border-t border-border p-4">
          <div className="mb-3 rounded-lg bg-elevated px-3 py-2">
            <p className="text-sm font-medium text-primary">{user?.fullName}</p>
            <p className="text-xs text-muted">{user?.role}</p>
          </div>
          <button
            type="button"
            onClick={handleLogout}
            className="flex w-full items-center gap-2 rounded-lg px-3 py-2 text-sm text-muted hover:bg-elevated hover:text-primary"
          >
            <LogOut className="h-4 w-4" />
            Logout
          </button>
        </div>
      </aside>

      <main className="flex-1 overflow-auto">
        <header className="border-b border-border bg-surface/50 px-8 py-4">
          <div className="flex items-center gap-2 text-sm text-muted">
            <BarChart3 className="h-4 w-4 text-accent-primary" />
            PRN232ASM — Research Trend System
          </div>
        </header>
        <div className="p-8">
          <Outlet />
        </div>
      </main>
    </div>
  )
}
