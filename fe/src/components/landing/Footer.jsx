import { Link } from 'react-router-dom'

const footerLinks = {
  Product: ['Features', 'Trends', 'Topics', 'Dashboard'],
  Explore: ['Papers', 'Journals', 'Keywords', 'Data Sources'],
  Company: ['About', 'Team', 'Contact', 'Privacy'],
}

export default function Footer() {
  return (
    <footer className="border-t border-border bg-base px-4 py-16 sm:px-6 lg:px-8">
      <div className="mx-auto grid max-w-7xl gap-10 sm:grid-cols-2 lg:grid-cols-4">
        <div>
          <Link to="/" className="flex items-center gap-2 font-display text-lg font-bold text-primary">
            Paper Trend Tracker
            <span className="h-2 w-2 rounded-full bg-accent-primary" />
          </Link>
          <p className="mt-3 text-sm leading-relaxed text-muted">
            Track scientific publication trends and discover emerging research topics from open academic data.
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
          © {new Date().getFullYear()} Paper Trend Tracker. All rights reserved.
        </p>
      </div>
    </footer>
  )
}
