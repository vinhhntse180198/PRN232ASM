import { Bell, Search, TrendingUp } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

const steps = [
  {
    num: '01',
    icon: Search,
    title: 'Search the Literature',
    desc: 'Look up papers by keyword, author, or journal in seconds.',
    preview: (
      <div className="mt-4 rounded-lg border border-border bg-base/60 p-3">
        <p className="text-xs font-semibold text-primary">Attention Is All You Need</p>
        <div className="mt-2 flex flex-wrap gap-1">
          {['Transformers', 'NLP', 'Deep Learning'].map((t) => (
            <span key={t} className="rounded-full bg-accent-primary/10 px-2 py-0.5 text-[10px] text-accent-glow">
              {t}
            </span>
          ))}
        </div>
        <p className="mt-2 text-[10px] text-accent-amber">★ 120,000+ citations · 2017</p>
      </div>
    ),
  },
  {
    num: '02',
    icon: TrendingUp,
    title: 'Track the Trends',
    desc: 'See how a topic grows across years with clear charts.',
    preview: (
      <div className="mt-4 space-y-2 rounded-lg border border-border bg-base/60 p-3">
        <div className="rounded-md bg-elevated px-2 py-1.5 text-[10px] text-muted">Keyword: "deep learning"</div>
        {[
          { year: '2022', count: '8.2k' },
          { year: '2023', count: '11.4k' },
          { year: '2024', count: '14.1k' },
        ].map((r) => (
          <div key={r.year} className="flex items-center justify-between text-[10px]">
            <span className="text-primary">{r.year}</span>
            <span className="font-mono text-accent-green">{r.count} papers</span>
          </div>
        ))}
      </div>
    ),
  },
  {
    num: '03',
    icon: Bell,
    title: 'Follow & Get Notified',
    desc: 'Bookmark topics and get alerts when new papers are published.',
    preview: (
      <div className="mt-4 grid grid-cols-3 gap-1.5">
        {['Keyword', 'Journal', 'Alerts'].map((col, i) => (
          <div key={col} className="rounded-lg border border-border bg-base/60 p-2">
            <p className="text-[9px] font-semibold text-muted">{col}</p>
            {i === 2 && (
              <div className="mt-1 rounded bg-accent-green/15 px-1 py-0.5 text-[9px] text-accent-green">
                3 new
              </div>
            )}
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
            Three Steps to Insight
          </h2>
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
