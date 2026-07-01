import { Link } from 'react-router-dom'
import { useNavbarScroll } from '../../hooks/useNavbarScroll'
import { APP_NAME } from '../../config/app'

const links = ['Features', 'Papers', 'Bookmarks', 'FAQ']

export default function Navbar() {
  const scrolled = useNavbarScroll()
  const isLoggedIn = Boolean(localStorage.getItem('accessToken'))

  return (
    <header
      className={`fixed inset-x-0 top-0 z-50 transition-all duration-300 ${
        scrolled ? 'glass-nav shadow-lg shadow-black/20' : 'bg-transparent'
      }`}
    >
      <nav className="relative mx-auto flex h-16 max-w-7xl items-center justify-center px-4 sm:px-6 lg:px-8">
        <Link
          to="/"
          className="absolute left-4 flex items-center gap-2 font-display text-sm font-extrabold tracking-tight text-primary sm:left-6 sm:text-base lg:left-8 lg:text-lg"
        >
          <span className="whitespace-nowrap">{APP_NAME}</span>
          <span className="h-2 w-2 shrink-0 rounded-full bg-accent-primary shadow-glow-sm" />
        </Link>

        <ul className="hidden items-center gap-8 md:flex">
          {links.map((item) => (
            <li key={item}>
              <a
                href={`#${item.toLowerCase().replace(' ', '-')}`}
                className="text-sm font-medium text-muted transition-colors hover:text-primary"
              >
                {item}
              </a>
            </li>
          ))}
        </ul>

        <div className="absolute right-4 flex items-center gap-3 sm:right-6 lg:right-8">
          {isLoggedIn ? (
            <Link
              to="/papers"
              className="rounded-lg bg-accent-primary px-4 py-2 text-sm font-medium text-white shadow-glow-sm transition-all hover:bg-accent-glow"
            >
              Go to Papers
            </Link>
          ) : (
            <>
              <Link
                to="/login"
                className="rounded-lg px-4 py-2 text-sm font-medium text-primary transition-colors hover:text-accent-glow"
              >
                Sign in
              </Link>
              <Link
                to="/register"
                className="rounded-lg bg-accent-primary px-4 py-2 text-sm font-medium text-white shadow-glow-sm transition-all hover:bg-accent-glow"
              >
                Start free
              </Link>
            </>
          )}
        </div>
      </nav>
    </header>
  )
}
