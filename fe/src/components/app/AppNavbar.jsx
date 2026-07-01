import { Link, useLocation, useNavigate } from 'react-router-dom'
import { BookOpen, LogOut } from 'lucide-react'
import { useNavbarScroll } from '../../hooks/useNavbarScroll'
import { useEffect, useRef, useState } from 'react'

import { APP_NAME } from '../../config/app'
import { t } from '../../config/i18n'

const appName = APP_NAME

const navItems = [
  { to: '/papers', labelKey: 'library' },
  { to: '/trends', labelKey: 'trends' },
  { to: '/bookmarks', labelKey: 'bookmarks' },
]

export default function AppNavbar() {
  const scrolled = useNavbarScroll()
  const navigate = useNavigate()
  const location = useLocation()
  const userName = localStorage.getItem('userFullName') || 'Researcher'
  const [menuOpen, setMenuOpen] = useState(false)
  const menuRef = useRef(null)

  const handleLogout = () => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('userEmail')
    localStorage.removeItem('userFullName')
    navigate('/login')
  }

  useEffect(() => {
    function onClick(e) {
      if (!menuRef.current) return
      if (!menuRef.current.contains(e.target)) setMenuOpen(false)
    }
    function onKey(e) {
      if (e.key === 'Escape') setMenuOpen(false)
    }
    document.addEventListener('mousedown', onClick)
    document.addEventListener('keydown', onKey)
    return () => {
      document.removeEventListener('mousedown', onClick)
      document.removeEventListener('keydown', onKey)
    }
  }, [])

  const isActive = (path) => {
    if (path === '/papers') {
      return location.pathname === '/papers'
        || /^\/papers\/[0-9a-f-]{36}$/i.test(location.pathname)
        || location.pathname.startsWith('/read/')
    }
    return location.pathname === path
  }

  return (
    <header
      className={`fixed inset-x-0 top-0 z-50 transition-all duration-300 ${
        scrolled ? 'glass-nav shadow-lg shadow-black/20' : 'bg-transparent'
      }`}
    >
      <nav className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <Link to="/papers" className="flex items-center gap-2 font-display text-lg font-bold tracking-tight text-primary">
          <BookOpen className="h-5 w-5 text-accent-primary" />
          <span className="max-w-[220px] truncate sm:max-w-none">{appName}</span>
          <span className="h-2 w-2 rounded-full bg-accent-primary shadow-glow-sm" />
        </Link>

        <ul className="hidden items-center gap-8 md:flex">
          {navItems.map(({ to, labelKey }) => (
            <li key={to}>
              <Link
                to={to}
                className={`text-sm transition-colors ${
                  isActive(to) ? 'font-medium text-primary' : 'text-muted hover:text-primary'
                }`}
              >
                {t(labelKey)}
              </Link>
            </li>
          ))}
        </ul>

        <div className="flex items-center gap-3">
          <div className="relative" ref={menuRef}>
            <button
              type="button"
              onClick={() => setMenuOpen((v) => !v)}
              className="hidden rounded-lg border border-border bg-surface/50 px-3 py-2 text-sm text-muted hover:text-primary sm:inline"
            >
              {t('hi')},{' '}
              <span className="font-medium text-primary">{userName.split(' ')[0]}</span>
            </button>

            {menuOpen && (
              <div className="absolute right-0 mt-2 w-44 overflow-hidden rounded-xl border border-border bg-surface shadow-card">
                <Link
                  to="/account"
                  onClick={() => setMenuOpen(false)}
                  className="block px-3 py-2 text-sm text-primary hover:bg-elevated"
                >
                  {t('account')}
                </Link>
                <button
                  type="button"
                  onClick={handleLogout}
                  className="flex w-full items-center gap-2 px-3 py-2 text-sm text-muted hover:bg-elevated hover:text-primary"
                >
                  <LogOut className="h-4 w-4" />
                  {t('signOut')}
                </button>
              </div>
            )}
          </div>
          <button
            type="button"
            onClick={handleLogout}
            className="inline-flex items-center gap-2 rounded-lg border border-border bg-surface/50 px-3 py-2 text-sm font-medium text-muted transition-all hover:border-accent-primary/40 hover:text-primary"
          >
            <LogOut className="h-4 w-4" />
            <span className="hidden sm:inline">{t('signOut')}</span>
          </button>
        </div>
      </nav>
    </header>
  )
}
