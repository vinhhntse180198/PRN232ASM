import { Link, useLocation, useNavigate } from 'react-router-dom'
import { BookOpen, Bookmark, LogOut, Search } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'

import { APP_NAME, APP_SHORT_NAME } from '../../config/app'
import { t } from '../../config/i18n'

const appName = APP_NAME

const navItems = [
  { to: '/papers', labelKey: 'library' },
  { to: '/trends', labelKey: 'trends' },
  { to: '/bookmarks', labelKey: 'bookmarks' },
]

export default function PageBookReaderNavbar({ theme = 'dark' }) {
  const navigate = useNavigate()
  const location = useLocation()
  const [search, setSearch] = useState('')
  const userName = localStorage.getItem('userFullName') || 'Researcher'
  const initial = userName.charAt(0).toUpperCase()
  const isLight = theme === 'light'
  const [menuOpen, setMenuOpen] = useState(false)
  const menuRef = useRef(null)

  const handleSearch = (e) => {
    e.preventDefault()
    const q = search.trim()
    if (q.length >= 2) navigate(`/papers?q=${encodeURIComponent(q)}`)
    else navigate('/papers')
  }

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

  if (isLight) {
    return (
      <header className="sticky top-0 z-50 border-b border-stone-200/80 bg-[#f4f4f0]/95 backdrop-blur-sm">
        <div className="mx-auto flex h-14 max-w-[1600px] items-center gap-4 px-4 sm:px-6">
          <Link to="/papers" className="shrink-0 font-serif text-lg font-bold text-stone-900">
            <span className="hidden lg:inline">{appName}</span>
            <span className="lg:hidden">{APP_SHORT_NAME}</span>
          </Link>

          <form onSubmit={handleSearch} className="hidden flex-1 md:block">
            <div className="relative mx-auto max-w-lg">
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-stone-400" />
              <input
                type="search"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search papers, authors…"
                className="w-full rounded-full border-0 bg-white py-2 pl-10 pr-4 text-sm text-stone-800 shadow-sm outline-none ring-1 ring-stone-200/80 focus:ring-stone-300"
              />
            </div>
          </form>

          <nav className="hidden items-center gap-6 sm:flex">
            {navItems.map(({ to, labelKey }) => (
              <Link
                key={to}
                to={to}
                className={`text-sm transition-colors ${
                  isActive(to)
                    ? 'border-b-2 border-stone-900 pb-0.5 font-medium text-stone-900'
                    : 'text-stone-500 hover:text-stone-800'
                }`}
              >
                {t(labelKey)}
              </Link>
            ))}
          </nav>

          <div className="ml-auto flex items-center gap-2">
            <Link
              to="/bookmarks"
              className="flex h-9 w-9 items-center justify-center rounded-md text-stone-500 hover:bg-white/60 sm:hidden"
            >
              <Bookmark className="h-4 w-4" />
            </Link>
            <div className="relative" ref={menuRef}>
              <button
                type="button"
                onClick={() => setMenuOpen((v) => !v)}
                className="hidden rounded-md px-2 py-1 text-xs text-stone-600 hover:bg-white/60 hover:text-stone-800 sm:block"
              >
                {t('hi')},{' '}
                <span className="font-medium text-stone-900">{userName.split(' ')[0]}</span>
              </button>

              {menuOpen && (
                <div className="absolute right-0 mt-2 w-44 overflow-hidden rounded-xl border border-stone-200 bg-white shadow-lg">
                  <Link
                    to="/account"
                    onClick={() => setMenuOpen(false)}
                    className="block px-3 py-2 text-sm text-stone-900 hover:bg-stone-50"
                  >
                    {t('account')}
                  </Link>
                  <button
                    type="button"
                    onClick={handleLogout}
                    className="flex w-full items-center gap-2 px-3 py-2 text-sm text-stone-600 hover:bg-stone-50 hover:text-stone-900"
                  >
                    <LogOut className="h-4 w-4" />
                    {t('signOut')}
                  </button>
                </div>
              )}
            </div>
            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-stone-900 text-xs font-semibold text-white">
              {initial}
            </div>
          </div>
        </div>
      </header>
    )
  }

  return (
    <header className="sticky top-0 z-50 border-b border-border bg-base/95 backdrop-blur-sm">
      <nav className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <Link to="/papers" className="flex items-center gap-2 font-display text-lg font-bold tracking-tight text-primary">
          <BookOpen className="h-5 w-5 shrink-0 text-accent-primary" />
          <span className="max-w-[220px] truncate sm:max-w-none">{appName}</span>
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
