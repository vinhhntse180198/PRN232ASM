import {
  BarChart3,
  Bookmark,
  BookOpen,
  LayoutDashboard,
  Search,
  TrendingUp,
} from 'lucide-react'
import { useCountUp } from '../../hooks/useCountUp'
import ScrollReveal from '../ui/ScrollReveal'
import { APP_SHORT_NAME } from '../../config/app'

const navItems = [
  { icon: LayoutDashboard, label: 'Papers', active: true },
  { icon: Search, label: 'Search' },
  { icon: Bookmark, label: 'Bookmarks' },
  { icon: TrendingUp, label: 'Trends' },
  { icon: BarChart3, label: 'Analytics' },
]

const stats = [
  { label: 'Papers in Library', value: 303, suffix: '', change: '+24', positive: true },
  { label: 'Open Access', value: 186, suffix: '', change: '+12%', positive: true },
  { label: 'Bookmarks', value: 18, suffix: '', change: '+3', positive: true },
  { label: 'Avg. Citations', value: 12, suffix: '', change: '+4.2%', positive: true },
]

const recentPapers = [
  { name: 'Intestinal Parasitic Infection…', value: '2024', status: 'Open Access', priority: 'Free' },
  { name: 'Machine Learning in Healthcare', value: '2023', status: 'PDF', priority: 'Saved' },
  { name: 'Climate Change Impact Study', value: '2022', status: 'Read', priority: 'Done' },
]

function StatCard({ stat }) {
  const { ref, formatted } = useCountUp(stat.value, 1400)

  return (
    <div className="rounded-xl border border-border bg-base/60 p-4">
      <p className="text-xs text-muted">{stat.label}</p>
      <p ref={ref} className="mt-1 font-mono text-xl font-semibold text-primary">
        {Number(formatted).toLocaleString()}{stat.suffix}
      </p>
      <p className={`mt-1 text-xs font-medium ${stat.positive ? 'text-accent-green' : 'text-red-400'}`}>
        {stat.change}
      </p>
    </div>
  )
}

export default function DashboardPreview() {
  return (
    <section className="relative px-4 py-16 sm:px-6 lg:px-8">
      <ScrollReveal>
        <div className="relative mx-auto max-w-6xl">
          <div className="pointer-events-none absolute -inset-4 rounded-3xl bg-halo opacity-80" />

          <div className="animate-float relative overflow-hidden rounded-2xl border border-border bg-surface shadow-glow">
            <div className="flex min-h-[480px] flex-col md:flex-row">
              <aside className="hidden w-52 shrink-0 border-r border-border bg-base/80 p-4 md:block">
                <div className="mb-6 flex items-center gap-2 font-display text-sm font-bold text-primary">
                  {APP_SHORT_NAME}
                  <span className="h-1.5 w-1.5 rounded-full bg-accent-primary" />
                </div>
                <nav className="space-y-1">
                  {navItems.map(({ icon: Icon, label, active }) => (
                    <div
                      key={label}
                      className={`flex items-center gap-2 rounded-lg px-3 py-2 text-xs ${
                        active
                          ? 'bg-accent-primary/15 text-accent-glow'
                          : 'text-muted hover:text-primary'
                      }`}
                    >
                      <Icon className="h-3.5 w-3.5" />
                      {label}
                    </div>
                  ))}
                </nav>
              </aside>

              <div className="flex-1 p-4 sm:p-6">
                <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border pb-4">
                  <div>
                    <p className="text-xs text-muted">Library</p>
                    <h3 className="font-display text-lg font-bold text-primary">Research Papers</h3>
                  </div>
                  <div className="flex items-center gap-3">
                    <button
                      type="button"
                      className="rounded-lg bg-accent-primary px-3 py-1.5 text-xs font-semibold text-white"
                    >
                      Search
                    </button>
                    <div className="flex h-8 w-8 items-center justify-center rounded-full bg-elevated text-xs font-bold text-accent-glow">
                      T
                    </div>
                  </div>
                </div>

                <div className="mt-4 grid grid-cols-2 gap-3 lg:grid-cols-4">
                  {stats.map((stat) => (
                    <StatCard key={stat.label} stat={stat} />
                  ))}
                </div>

                <div className="mt-4 rounded-xl border border-border bg-base/40 p-4">
                  <p className="mb-3 text-sm font-semibold text-primary">Recent Papers</p>
                  <div className="space-y-2">
                    {recentPapers.map((row) => (
                      <div
                        key={row.name}
                        className="flex flex-wrap items-center justify-between gap-2 rounded-lg bg-surface/80 px-3 py-2 text-xs"
                      >
                        <span className="font-medium text-primary">{row.name}</span>
                        <span className="font-mono text-muted">{row.value}</span>
                        <span className="rounded-full bg-elevated px-2 py-0.5 text-muted">{row.status}</span>
                        <span
                          className={`rounded-full px-2 py-0.5 ${
                            row.priority === 'Free'
                              ? 'bg-accent-green/15 text-accent-green'
                              : row.priority === 'Done'
                                ? 'bg-accent-primary/10 text-accent-glow'
                                : 'bg-accent-amber/15 text-accent-amber'
                          }`}
                        >
                          {row.priority}
                        </span>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="absolute -bottom-6 -right-2 z-10 hidden rounded-xl border border-border bg-surface p-3 shadow-card sm:block lg:-right-8">
            <div className="flex items-center gap-2">
              <BookOpen className="h-4 w-4 text-accent-primary" />
              <div>
                <p className="text-xs font-semibold text-primary">PDF Ready</p>
                <p className="text-[10px] text-muted">Read in-app</p>
              </div>
            </div>
          </div>

          <div className="absolute -left-2 top-1/3 z-10 hidden rounded-xl border border-accent-green/30 bg-surface p-3 shadow-card sm:block lg:-left-10">
            <p className="text-xs font-semibold text-accent-green">Open Access found</p>
            <p className="mt-1 text-[10px] text-muted">12 free papers this week</p>
          </div>
        </div>
      </ScrollReveal>
    </section>
  )
}
