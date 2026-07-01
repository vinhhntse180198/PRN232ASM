import { BarChart2, BookOpen, Bookmark, Globe, Search, Zap } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

const features = [
  { icon: Search, title: 'Smart Search', desc: 'Find papers by topic, author, or year with OpenAlex-powered search.' },
  { icon: BookOpen, title: 'In-App Reader', desc: 'Read open-access PDFs and abstracts without leaving the platform.' },
  { icon: Bookmark, title: 'Bookmarks', desc: 'Save papers to your library and revisit them anytime.' },
  { icon: Globe, title: 'Open Access', desc: 'Browse free papers or filter paywalled works with clear labels.' },
  { icon: Zap, title: 'Fast Discovery', desc: 'Surface relevant works in seconds with year and access filters.' },
  { icon: BarChart2, title: 'Citation Insights', desc: 'View citation counts, topics, and publication metadata at a glance.' },
]

export default function FeaturesGrid() {
  return (
    <section id="features" className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Built for Research Workflows
          </h2>
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
