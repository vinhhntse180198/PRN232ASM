import { Search, BarChart2, Database, TrendingUp, Bookmark, LayoutDashboard } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

const features = [
  { icon: Search, title: 'Powerful Search', desc: 'Find papers by keyword, author, or journal across millions of records.' },
  { icon: TrendingUp, title: 'Publication Trends', desc: 'See how research topics rise and fall in volume year over year.' },
  { icon: Database, title: 'Open Data Sources', desc: 'Metadata synced from OpenAlex, Crossref, and Semantic Scholar.' },
  { icon: BarChart2, title: 'Emerging Topics', desc: 'Spot fast-growing research areas before they become mainstream.' },
  { icon: Bookmark, title: 'Save & Follow', desc: 'Bookmark papers and follow the journals or keywords you care about.' },
  { icon: LayoutDashboard, title: 'Smart Dashboards', desc: 'Charts and statistics that turn raw metadata into real insight.' },
]

export default function FeaturesGrid() {
  return (
    <section id="features" className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Everything You Need to Track Research
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
