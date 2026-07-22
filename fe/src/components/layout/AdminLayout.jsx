import { Link, NavLink, Outlet } from 'react-router-dom'
import {
  Activity,
  ArrowLeft,
  BookOpen,
  Database,
  FileBarChart,
  RefreshCw,
  Tags,
  Users,
} from 'lucide-react'

const adminNav = [
  { to: '/admin/users', icon: Users, label: 'Users' },
  { to: '/admin/journals', icon: BookOpen, label: 'Journals' },
  { to: '/admin/topics', icon: Tags, label: 'Topics' },
  { to: '/admin/datasources', icon: Database, label: 'API Sources' },
  { to: '/admin/sync', icon: RefreshCw, label: 'Sync' },
  { to: '/admin/monitoring', icon: Activity, label: 'Monitoring' },
  { to: '/admin/reports', icon: FileBarChart, label: 'System Reports' },
]

export default function AdminLayout() {
  return (
    <div className="flex min-h-screen bg-base">
      <aside className="w-56 border-r border-border bg-surface p-4">
        <Link
          to="/dashboard"
          className="mb-6 flex items-center gap-2 text-sm text-muted hover:text-primary"
        >
          <ArrowLeft className="h-4 w-4" />
          Back to app
        </Link>
        <h2 className="mb-4 font-display text-sm font-bold uppercase tracking-wide text-accent-amber">
          Admin
        </h2>
        <nav className="space-y-1">
          {adminNav.map(({ to, icon: Icon, label }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `flex items-center gap-2 rounded-lg px-3 py-2 text-sm ${
                  isActive
                    ? 'bg-accent-amber/15 text-accent-amber'
                    : 'text-muted hover:bg-elevated hover:text-primary'
                }`
              }
            >
              <Icon className="h-4 w-4" />
              {label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <main className="flex-1 p-8">
        <Outlet />
      </main>
    </div>
  )
}
