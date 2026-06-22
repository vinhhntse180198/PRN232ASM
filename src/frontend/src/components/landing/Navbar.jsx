import { Link } from 'react-router-dom'
import { useNavbarScroll } from '../../hooks/useNavbarScroll'

const links = ['Features', 'Partners', 'Pricing', 'Case Studies']

export default function Navbar() {
  const scrolled = useNavbarScroll()

  return (
    <header
      className={`fixed inset-x-0 top-0 z-50 transition-all duration-300 ${
        scrolled ? 'glass-nav shadow-lg shadow-black/20' : 'bg-transparent'
      }`}
    >
      <nav className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <Link to="/" className="flex items-center gap-2 font-display text-lg font-bold tracking-tight text-primary">
          Collective OS
          <span className="h-2 w-2 rounded-full bg-accent-primary shadow-glow-sm" />
        </Link>

        <ul className="hidden items-center gap-8 md:flex">
          {links.map((item) => (
            <li key={item}>
              <a
                href={`#${item.toLowerCase().replace(' ', '-')}`}
                className="text-sm text-muted transition-colors hover:text-primary"
              >
                {item}
              </a>
            </li>
          ))}
        </ul>

        <div className="flex items-center gap-3">
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
        </div>
      </nav>
    </header>
  )
}
