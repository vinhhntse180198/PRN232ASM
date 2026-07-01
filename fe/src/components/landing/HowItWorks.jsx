import { Bookmark, BookOpen, Search } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

const steps = [
  {
    num: '01',
    icon: Search,
    title: 'Search & Filter',
    desc: 'Query OpenAlex by keyword and year. Filter free or paid papers instantly.',
    preview: (
      <div className="mt-4 space-y-2 rounded-lg border border-border bg-base/60 p-3">
        <div className="rounded-md bg-elevated px-2 py-1.5 text-[10px] text-muted">machine learning · 2024</div>
        {[
          { name: 'Deep Learning Survey', match: 'Free' },
          { name: 'Neural Networks Review', match: 'PDF' },
          { name: 'AI in Medicine', match: 'Paid' },
        ].map((r) => (
          <div key={r.name} className="flex items-center justify-between text-[10px]">
            <span className="text-primary">{r.name}</span>
            <span className="font-mono text-accent-green">{r.match}</span>
          </div>
        ))}
      </div>
    ),
  },
  {
    num: '02',
    icon: BookOpen,
    title: 'Read In-App',
    desc: 'Open abstracts and full-text PDFs in a clean reader with metadata sidebars.',
    preview: (
      <div className="mt-4 rounded-lg border border-border bg-base/60 p-3">
        <p className="text-xs font-semibold text-primary">Abstract · Full Text</p>
        <div className="mt-2 h-16 rounded bg-elevated/80" />
        <p className="mt-2 text-[10px] text-muted">PDF viewer · Citation widgets</p>
      </div>
    ),
  },
  {
    num: '03',
    icon: Bookmark,
    title: 'Save & Track',
    desc: 'Bookmark papers to your library and monitor citations and topics.',
    preview: (
      <div className="mt-4 grid grid-cols-3 gap-1.5">
        {['Saved', 'Reading', 'Done'].map((col, i) => (
          <div key={col} className="rounded-lg border border-border bg-base/60 p-2">
            <p className="text-[9px] font-semibold text-muted">{col}</p>
            {i === 0 && (
              <div className="mt-1 rounded bg-accent-primary/15 px-1 py-0.5 text-[9px] text-accent-glow">
                18 papers
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
    <section className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <p className="text-center text-sm font-medium text-accent-glow">How It Works</p>
          <h2 className="mt-2 text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Three Steps to Smarter Reading
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
