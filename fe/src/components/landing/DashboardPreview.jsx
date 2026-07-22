import {
  BarChart3,
  FileText,
  LayoutDashboard,
  Sparkles,
  Tags,
  TrendingUp,
} from 'lucide-react'
import { useCountUp } from '../../hooks/useCountUp'
import ScrollReveal from '../ui/ScrollReveal'

const navItems = [
  { icon: LayoutDashboard, label: 'Dashboard', active: true },
  { icon: FileText, label: 'Papers' },
  { icon: TrendingUp, label: 'Trends' },
  { icon: Tags, label: 'Topics' },
  { icon: BarChart3, label: 'Reports' },
]

const stats = [
  { label: 'Total Papers', value: 12480, change: '+12.5%', positive: true },
  { label: 'Keywords Tracked', value: 1240, change: '+8.1%', positive: true },
  { label: 'Emerging Topics', value: 36, change: '+6', positive: true },
  { label: 'YoY Growth', value: 18, suffix: '%', change: '+4.2%', positive: true },
]

const papers = [
  { name: 'Attention Is All You Need', value: '2017', status: 'NeurIPS', trend: 'Hot' },
  { name: 'Deep Residual Learning', value: '2016', status: 'CVPR', trend: 'Rising' },
  { name: 'Language Models are Few-Shot Learners', value: '2020', status: 'NeurIPS', trend: 'Hot' },
]

function StatCard({ stat }) {
  const { ref, formatted } = useCountUp(stat.value, 1400)

  return (
    <div className="rounded-xl border border-border bg-base/60 p-4">
      <p className="text-xs text-muted">{stat.label}</p>
      <p ref={ref} className="mt-1 font-mono text-xl font-semibold text-primary">
        {stat.suffix === '%' ? `${formatted}%` : Number(formatted).toLocaleString()}
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
                  Paper Trend Tracker
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
                    <h3 className="font-display text-lg font-bold text-primary">Welcome back, Researcher</h3>
                  </div>
                  <div className="flex items-center gap-3">
                    <button
                      type="button"
                      className="rounded-lg bg-accent-primary px-3 py-1.5 text-xs font-semibold text-white"
                    >
                      New Report
                    </button>
                    <div className="flex h-8 w-8 items-center justify-center rounded-full bg-elevated text-xs font-bold text-accent-glow">
                      R
                    </div>
                  </div>
                </div>

                <div className="mt-4 grid grid-cols-2 gap-3 lg:grid-cols-4">
                  {stats.map((stat) => (
                    <StatCard key={stat.label} stat={stat} />
                  ))}
                </div>

                <div className="mt-4 rounded-xl border border-border bg-base/40 p-4">
                  <p className="mb-3 text-sm font-semibold text-primary">Trending Papers</p>
                  <div className="space-y-2">
                    {papers.map((row) => (
                      <div
                        key={row.name}
                        className="flex flex-wrap items-center justify-between gap-2 rounded-lg bg-surface/80 px-3 py-2 text-xs"
                      >
                        <span className="font-medium text-primary">{row.name}</span>
                        <span className="font-mono text-muted">{row.value}</span>
                        <span className="rounded-full bg-elevated px-2 py-0.5 text-muted">{row.status}</span>
                        <span
                          className={`rounded-full px-2 py-0.5 ${
                            row.trend === 'Hot'
                              ? 'bg-accent-amber/15 text-accent-amber'
                              : 'bg-accent-green/15 text-accent-green'
                          }`}
                        >
                          {row.trend}
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
                <p className="text-xs font-semibold text-primary">Emerging Topic</p>
                <p className="text-[10px] text-muted">Graph Neural Networks</p>
              </div>
            </div>
            <div className="mt-2 flex -space-x-2">
              {['AI', 'ML', 'DL'].map((l) => (
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
            <p className="text-xs font-semibold text-accent-green">Trend detected</p>
            <p className="mt-1 text-[10px] text-muted">📈 +42% YoY on “LLM”</p>
          </div>
        </div>
      </ScrollReveal>
    </section>
  )
}
