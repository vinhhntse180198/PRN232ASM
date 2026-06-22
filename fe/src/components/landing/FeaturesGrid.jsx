import { BarChart2, Globe, Network, Shield, Zap, Target } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'

const features = [
  { icon: Shield, title: 'Vetted Network', desc: 'Every partner is verified for quality, reliability, and track record.' },
  { icon: BarChart2, title: 'Revenue Tracking', desc: 'Monitor pipeline value, win rates, and partner ROI in real time.' },
  { icon: Globe, title: 'Global Reach', desc: 'Access agencies across 41 countries without opening new offices.' },
  { icon: Zap, title: 'Lightning Fast', desc: 'Match with qualified partners in hours, not months of outreach.' },
  { icon: Target, title: 'Smart Matching', desc: 'AI scores fit based on skills, industry, and deal history.' },
  { icon: Network, title: 'Deep Analytics', desc: 'Insights on partner performance, trends, and growth opportunities.' },
]

export default function FeaturesGrid() {
  return (
    <section className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Why Agencies Choose Us
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
