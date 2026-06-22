import {
  BarChart3,
  Briefcase,
  LayoutDashboard,
  MessageSquare,
  Sparkles,
  Users,
} from 'lucide-react'
import { useCountUp } from '../../hooks/useCountUp'
import ScrollReveal from '../ui/ScrollReveal'

const navItems = [
  { icon: LayoutDashboard, label: 'Overview', active: true },
  { icon: Users, label: 'Partners' },
  { icon: Briefcase, label: 'Projects' },
  { icon: MessageSquare, label: 'Messages' },
  { icon: BarChart3, label: 'Analytics' },
]

const stats = [
  { label: 'Total Revenue', value: 284500, prefix: '$', suffix: '', change: '+12.5%', positive: true },
  { label: 'Active Partners', value: 500, suffix: 'k', change: '+8.1%', positive: true, format: 'k' },
  { label: 'Open Opportunities', value: 23, suffix: '', change: '-2%', positive: false },
  { label: 'Win Rate', value: 88, suffix: '%', change: '+4.2%', positive: true },
]

const opportunities = [
  { name: 'Brand Refresh — NovaTech', value: '$42,000', status: 'Proposal', priority: 'High' },
  { name: 'SEO Sprint — Lumina', value: '$18,500', status: 'Discovery', priority: 'Medium' },
  { name: 'App Redesign — Orbit', value: '$24,500', status: 'Won', priority: 'Won' },
]

function StatCard({ stat, index }) {
  const displayValue = stat.format === 'k' ? stat.value : stat.value
  const { ref, formatted } = useCountUp(displayValue, 1400)

  return (
    <div className="rounded-xl border border-border bg-base/60 p-4">
      <p className="text-xs text-muted">{stat.label}</p>
      <p ref={ref} className="mt-1 font-mono text-xl font-semibold text-primary">
        {stat.prefix}
        {stat.format === 'k' ? `${formatted}k` : stat.suffix === '%' ? `${formatted}%` : Number(formatted).toLocaleString()}
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
              {/* Sidebar */}
              <aside className="hidden w-52 shrink-0 border-r border-border bg-base/80 p-4 md:block">
                <div className="mb-6 flex items-center gap-2 font-display text-sm font-bold text-primary">
                  Collective OS
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

              {/* Main */}
              <div className="flex-1 p-4 sm:p-6">
                <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border pb-4">
                  <div>
                    <p className="text-xs text-muted">Dashboard</p>
                    <h3 className="font-display text-lg font-bold text-primary">Welcome back, Rahim</h3>
                  </div>
                  <div className="flex items-center gap-3">
                    <button
                      type="button"
                      className="rounded-lg bg-accent-primary px-3 py-1.5 text-xs font-semibold text-white"
                    >
                      New Deal
                    </button>
                    <div className="flex h-8 w-8 items-center justify-center rounded-full bg-elevated text-xs font-bold text-accent-glow">
                      R
                    </div>
                  </div>
                </div>

                <div className="mt-4 grid grid-cols-2 gap-3 lg:grid-cols-4">
                  {stats.map((stat, i) => (
                    <StatCard key={stat.label} stat={stat} index={i} />
                  ))}
                </div>

                <div className="mt-4 rounded-xl border border-border bg-base/40 p-4">
                  <p className="mb-3 text-sm font-semibold text-primary">Recent Opportunities</p>
                  <div className="space-y-2">
                    {opportunities.map((row) => (
                      <div
                        key={row.name}
                        className="flex flex-wrap items-center justify-between gap-2 rounded-lg bg-surface/80 px-3 py-2 text-xs"
                      >
                        <span className="font-medium text-primary">{row.name}</span>
                        <span className="font-mono text-muted">{row.value}</span>
                        <span className="rounded-full bg-elevated px-2 py-0.5 text-muted">{row.status}</span>
                        <span
                          className={`rounded-full px-2 py-0.5 ${
                            row.priority === 'High'
                              ? 'bg-accent-amber/15 text-accent-amber'
                              : row.priority === 'Won'
                                ? 'bg-accent-green/15 text-accent-green'
                                : 'bg-accent-primary/10 text-accent-glow'
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

          {/* Floating cards */}
          <div className="absolute -bottom-6 -right-2 z-10 hidden rounded-xl border border-border bg-surface p-3 shadow-card sm:block lg:-right-8">
            <div className="flex items-center gap-2">
              <Sparkles className="h-4 w-4 text-accent-primary" />
              <div>
                <p className="text-xs font-semibold text-primary">AI Matched</p>
                <p className="text-[10px] text-muted">3 partners found</p>
              </div>
            </div>
            <div className="mt-2 flex -space-x-2">
              {['A', 'B', 'C'].map((l) => (
                <div
                  key={l}
                  className="flex h-6 w-6 items-center justify-center rounded-full border border-base bg-elevated text-[9px] font-bold text-accent-glow"
                >
                  {l}
                </div>
              ))}
            </div>
          </div>

          <div className="absolute -left-2 top-1/3 z-10 hidden rounded-xl border border-accent-green/30 bg-surface p-3 shadow-card sm:block lg:-left-10">
            <p className="text-xs font-semibold text-accent-green">Deal won: $24,500</p>
            <p className="mt-1 text-[10px] text-muted">🎉 Confetti sent to team</p>
          </div>
        </div>
      </ScrollReveal>
    </section>
  )
}
