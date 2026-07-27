import {
  BarChart3,
  BookOpen,
  TrendingUp,
  Sparkles,
  Users,
  FileText,
} from 'lucide-react'
import { useCountUp } from '../../hooks/useCountUp'
import ScrollReveal from '../ui/ScrollReveal'

const navItems = [
  { icon: BarChart3, label: 'Dashboard', active: true },
  { icon: FileText, label: 'Papers' },
  { icon: TrendingUp, label: 'Trends' },
  { icon: Users, label: 'Topics' },
]

const stats = [
  { label: 'Total Papers', value: 12847, suffix: '', change: '+12.5%', positive: true },
  { label: 'Keywords Tracked', value: 342, suffix: '', change: '+8.1%', positive: true },
  { label: 'Research Topics', value: 56, suffix: '', change: '+3', positive: true },
  { label: 'Journals Covered', value: 189, suffix: '', change: '+12', positive: true },
]

const topKeywords = [
  { name: 'machine learning', papers: 847 },
  { name: 'deep learning', papers: 634 },
  { name: 'neural networks', papers: 521 },
  { name: 'natural language', papers: 398 },
]

function StatCard({ stat, index }) {
  const { ref, formatted } = useCountUp(stat.value, 1400)

  return (
    <div className="rounded-xl border border-border bg-base/60 p-4">
      <p className="text-xs text-muted">{stat.label}</p>
      <p ref={ref} className="mt-1 font-mono text-xl font-semibold text-primary">
        {Number(formatted).toLocaleString()}
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
                  SciTrend
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
                    <h3 className="font-display text-lg font-bold text-primary">Research Overview</h3>
                  </div>
                  <div className="flex items-center gap-3">
                    <div className="flex h-8 w-8 items-center justify-center rounded-full bg-elevated text-xs font-bold text-accent-glow">
                      DR
                    </div>
                  </div>
                </div>

                <div className="mt-4 grid grid-cols-2 gap-3 lg:grid-cols-4">
                  {stats.map((stat, i) => (
                    <StatCard key={stat.label} stat={stat} index={i} />
                  ))}
                </div>

                <div className="mt-4 rounded-xl border border-border bg-base/40 p-4">
                  <p className="mb-3 text-sm font-semibold text-primary">Top Keywords This Month</p>
                  <div className="space-y-2">
                    {topKeywords.map((row) => (
                      <div
                        key={row.name}
                        className="flex flex-wrap items-center justify-between gap-2 rounded-lg bg-surface/80 px-3 py-2 text-xs"
                      >
                        <span className="font-medium text-primary">{row.name}</span>
                        <div className="flex items-center gap-2">
                          <span className="font-mono text-muted">{row.papers.toLocaleString()} papers</span>
                          <span className="rounded-full bg-accent-primary/10 px-2 py-0.5 text-muted">+5%</span>
                        </div>
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
              <TrendingUp className="h-4 w-4 text-accent-primary" />
              <div>
                <p className="text-xs font-semibold text-primary">Trending Up</p>
                <p className="text-[10px] text-muted">+23% AI papers this week</p>
              </div>
            </div>
          </div>

          <div className="absolute -left-2 top-1/3 z-10 hidden rounded-xl border border-accent-green/30 bg-surface p-3 shadow-card sm:block lg:-left-10">
            <div className="flex items-center gap-2">
              <BookOpen className="h-4 w-4 text-accent-green" />
              <div>
                <p className="text-xs font-semibold text-accent-green">New Papers</p>
                <p className="text-[10px] text-muted">127 papers synced today</p>
              </div>
            </div>
          </div>
        </div>
      </ScrollReveal>
    </section>
  )
}
