import { Search, TrendingUp, Bell } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

const steps = [
  {
    num: '01',
    icon: Search,
    title: 'Search & Discover Papers',
    desc: 'Filter thousands of scientific publications by keyword, author, journal, or publication year.',
    preview: (
      <div className="mt-4 space-y-2 rounded-lg border border-border bg-base/60 p-3">
        <div className="rounded-md bg-elevated px-2 py-1.5 text-[10px] text-muted">
          Search: &quot;machine learning + 2024&quot;
        </div>
        {[
          { title: 'Attention Is All You Need', journal: 'NeurIPS', year: 2024 },
          { title: 'Deep Learning for NLP', journal: 'ACL', year: 2024 },
          { title: 'Neural Architecture Search', journal: 'ICML', year: 2024 },
        ].map((r) => (
          <div key={r.title} className="flex flex-col text-[10px]">
            <span className="font-medium text-primary">{r.title}</span>
            <span className="text-muted">{r.journal} · {r.year}</span>
          </div>
        ))}
      </div>
    ),
  },
  {
    num: '02',
    icon: TrendingUp,
    title: 'Track Research Trends',
    desc: 'Follow keywords and topics to visualize publication trends over time with interactive charts.',
    preview: (
      <div className="mt-4 rounded-lg border border-border bg-base/60 p-3">
        <p className="mb-2 text-[10px] font-semibold text-primary">Papers per Year: &quot;Deep Learning&quot;</p>
        {[2021, 2022, 2023, 2024].map((year, i) => (
          <div key={year} className="flex items-center gap-2 text-[10px]">
            <span className="w-6 text-muted">{year}</span>
            <div className="flex-1 rounded bg-accent-primary/20" style={{ width: `${[30, 55, 75, 100][i]}%` }}>
              <span className="px-1 text-accent-glow">{[120, 220, 300, 400][i]}</span>
            </div>
          </div>
        ))}
      </div>
    ),
  },
  {
    num: '03',
    icon: Bell,
    title: 'Get Notified',
    desc: 'Follow journals and topics to receive alerts when new relevant papers are published.',
    preview: (
      <div className="mt-4 space-y-2">
        {[
          { icon: '📄', title: 'New paper in Machine Learning', time: '2 min ago' },
          { icon: '📄', title: 'Deep Learning paper trending', time: '15 min ago' },
          { icon: '📄', title: 'AI paper in Nature', time: '1 hour ago' },
        ].map((n) => (
          <div key={n.title} className="flex items-center gap-2 rounded-lg border border-border bg-base/60 px-3 py-2 text-[10px]">
            <span>{n.icon}</span>
            <div className="flex-1">
              <p className="font-medium text-primary">{n.title}</p>
              <p className="text-muted">{n.time}</p>
            </div>
          </div>
        ))}
      </div>
    ),
  },
]

export default function HowItWorks() {
  return (
    <section id="how-it-works" className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <p className="text-center text-sm font-medium text-accent-glow">How It Works</p>
          <h2 className="mt-2 text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            From Discovery to Insight
          </h2>
          <p className="mt-2 text-center text-sm text-muted">Three steps to stay ahead in your research field</p>
        </ScrollReveal>

        <div className="mt-14 grid gap-6 md:grid-cols-3">
          {steps.map((step, i) => (
            <ScrollReveal key={step.num} delay={i * 100}>
              <article className="group relative h-full rounded-2xl border border-border bg-surface p-6 transition-all duration-300 hover:-translate-y-1 hover:border-accent-primary/30 hover:shadow-glow-sm">
                <span className="absolute right-4 top-4 font-display text-5xl font-black text-border">
                  {step.num}
                </span>
                <step.icon className="h-5 w-5 text-accent-primary" />
                <h3 className="mt-4 font-display text-lg font-bold text-primary">{step.title}</h3>
                <p className="mt-2 text-sm leading-relaxed text-muted">{step.desc}</p>
                {step.preview}
              </article>
            </ScrollReveal>
          ))}
        </div>
      </div>
    </section>
  )
}
