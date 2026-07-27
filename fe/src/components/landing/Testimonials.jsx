import ScrollReveal from '../ui/ScrollReveal'

const testimonials = [
  {
    quote: 'SciTrend helped us identify emerging AI research topics months before they became mainstream. The trend analysis alone saved countless hours of manual literature review.',
    name: 'Dr. Sarah Chen',
    role: 'Associate Professor, Stanford CS',
    initials: 'SC',
  },
  {
    quote: 'As a PhD student, keeping up with publication trends in machine learning was overwhelming. This platform makes it manageable and insightful.',
    name: 'Marcus Webb',
    role: 'PhD Candidate, MIT',
    initials: 'MW',
  },
  {
    quote: 'The keyword tracking feature is a game-changer. We finally have visibility into which research areas are gaining momentum.',
    name: 'Dr. Elena Rossi',
    role: 'Research Director, Forma Labs',
    initials: 'ER',
  },
]

export default function Testimonials() {
  return (
    <section className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Trusted by Researchers Worldwide
          </h2>
          <p className="mt-2 text-center text-sm text-muted">Used by academics at leading universities and research institutions</p>
        </ScrollReveal>

        <div className="mt-14 grid gap-6 md:grid-cols-3">
          {testimonials.map((t, i) => (
            <ScrollReveal key={t.name} delay={i * 100}>
              <article className="flex h-full flex-col rounded-2xl border border-border bg-surface p-6">
                <p className="flex-1 text-sm italic leading-relaxed text-primary">&ldquo;{t.quote}&rdquo;</p>
                <div className="mt-6 flex items-center gap-3 border-t border-border pt-4">
                  <div className="flex h-10 w-10 items-center justify-center rounded-full bg-elevated text-xs font-bold text-accent-glow">
                    {t.initials}
                  </div>
                  <div>
                    <p className="text-sm font-semibold text-primary">{t.name}</p>
                    <p className="text-xs text-muted">{t.role}</p>
                  </div>
                </div>
                <p className="mt-3 text-sm tracking-widest text-accent-primary">★★★★★</p>
              </article>
            </ScrollReveal>
          ))}
        </div>
      </div>
    </section>
  )
}
