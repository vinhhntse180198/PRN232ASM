import ScrollReveal from '../ui/ScrollReveal'

const testimonials = [
  {
    quote: 'I can search OpenAlex and read PDFs without jumping between tabs. Bookmarks keep my literature review organized.',
    name: 'Sarah Chen',
    role: 'PhD Researcher, Public Health',
    initials: 'SC',
  },
  {
    quote: 'The in-app reader and citation widgets save hours every week when screening papers for my thesis.',
    name: 'Marcus Webb',
    role: 'Graduate Student, Computer Science',
    initials: 'MW',
  },
  {
    quote: 'Filtering free vs paid papers and browsing by year makes it easy to build a focused reading list.',
    name: 'Elena Rossi',
    role: 'Research Assistant, Biology',
    initials: 'ER',
  },
]

export default function Testimonials() {
  return (
    <section className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-7xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Trusted by Researchers
          </h2>
          <p className="mt-2 text-center text-sm text-muted">Students & academics worldwide</p>
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
