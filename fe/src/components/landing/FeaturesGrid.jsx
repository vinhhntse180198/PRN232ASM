import { BarChart2, Globe, BookOpen, Shield, TrendingUp, Target } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

const features = [
  { icon: TrendingUp, title: 'Publication Trends', desc: 'Visualize research output over time with interactive charts and keyword-based trend analysis.' },
  { icon: BookOpen, title: 'Paper Discovery', desc: 'Search and filter thousands of scientific papers by keyword, author, journal, and publication year.' },
  { icon: Globe, title: 'Multi-Source Data', desc: 'Aggregate metadata from academic APIs including Semantic Scholar, OpenAlex, and Crossref.' },
  { icon: Target, title: 'Topic Tracking', desc: 'Follow research topics and receive alerts when new papers matching your interests are published.' },
  { icon: Shield, title: 'Curated Datasets', desc: 'High-quality metadata including titles, abstracts, keywords, authors, and journal information.' },
  { icon: BarChart2, title: 'Deep Analytics', desc: 'Insights on journal impact, keyword frequency, and emerging research areas over time.' },
]

export default function FeaturesGrid() {
  return (
    <section className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Built for Research Analysis
          </h2>
          <p className="mt-2 text-center text-sm text-muted">From keyword trends to journal impact — all in one platform</p>
        </ScrollReveal>

        <div className="mt-14 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {features.map((f, i) => (
            <ScrollReveal key={f.title} delay={i * 80}>
              <article className="h-full rounded-2xl border border-border bg-surface p-6 transition-all duration-300 hover:-translate-y-1 hover:border-accent-primary/25 hover:bg-elevated hover:shadow-card">
                <f.icon className="h-5 w-5 stroke-[1.5] text-accent-primary" />
                <h3 className="mt-4 font-display text-base font-bold text-primary">{f.title}</h3>
                <p className="mt-2 text-sm leading-relaxed text-muted">{f.desc}</p>
              </article>
            </ScrollReveal>
          ))}
        </div>
      </div>
    </section>
  )
}
