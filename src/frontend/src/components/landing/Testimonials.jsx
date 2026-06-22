import ScrollReveal from '../ui/ScrollReveal'

const testimonials = [
  {
    quote: 'Collective OS helped us close $180k in partner-sourced deals in our first quarter. The matching is eerily accurate.',
    name: 'Sarah Chen',
    role: 'Founder, Pixel & Co.',
    initials: 'SC',
  },
  {
    quote: 'We expanded into video production without hiring a single editor. Our partners feel like an extension of our team.',
    name: 'Marcus Webb',
    role: 'CEO, Northline Agency',
    initials: 'MW',
  },
  {
    quote: 'The dashboard alone paid for itself. We finally have visibility into which partnerships actually drive revenue.',
    name: 'Elena Rossi',
    role: 'COO, Forma Labs',
    initials: 'ER',
  },
]

export default function Testimonials() {
  return (
    <section className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Loved by Agencies Worldwide
          </h2>
          <p className="mt-2 text-center text-sm text-muted">2,000+ agency founders</p>
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
