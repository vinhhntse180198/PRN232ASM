import { Link } from 'react-router-dom'

const footerLinks = {
  Product: ['Features', 'Research Topics', 'Dashboard', 'Reports'],
  Resources: ['Paper Search', 'Trend Analytics', 'API Sources', 'Help Center'],
  Company: ['About', 'Research', 'Privacy Policy', 'Contact'],
}

export default function Footer() {
  return (
    <footer className="border-t border-border bg-base px-4 py-16 sm:px-6 lg:px-8">
      <div className="mx-auto grid max-w-7xl gap-10 sm:grid-cols-2 lg:grid-cols-4">
        <div>
          <Link to="/" className="flex items-center gap-2 font-display text-lg font-bold text-primary">
            SciTrend
            <span className="h-2 w-2 rounded-full bg-accent-primary" />
          </Link>
          <p className="mt-3 text-sm leading-relaxed text-muted">
            Track and analyze scientific publication trends. Built for researchers, lecturers, and students.
          </p>
        </div>

        {Object.entries(footerLinks).map(([title, links]) => (
          <div key={title}>
            <p className="text-sm font-semibold text-primary">{title}</p>
            <ul className="mt-4 space-y-2">
              {links.map((link) => (
                <li key={link}>
                  <a href="#" className="text-sm text-muted transition-colors hover:text-primary">
                    {link}
                  </a>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>

      <div className="mx-auto mt-12 max-w-7xl border-t border-border pt-8">
        <p className="text-center text-xs text-muted">
          © {new Date().getFullYear()} SciTrend. Scientific Publication Trend Tracking.
        </p>
      </div>
    </footer>
  )
}
