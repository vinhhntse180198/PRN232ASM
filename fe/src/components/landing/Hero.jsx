import { Link } from 'react-router-dom'
import { BookOpen } from 'lucide-react'
import ScrollReveal from '../ui/ScrollReveal'
import { APP_NAME } from '../../config/app'

const avatars = ['RB', 'SK', 'AM']

export default function Hero() {
  return (
    <section className="relative overflow-hidden pt-32 pb-20 sm:pt-40 sm:pb-28">
      <div className="dot-grid pointer-events-none absolute inset-0 opacity-[0.05]" />
      <div className="pointer-events-none absolute left-1/2 top-20 h-[500px] w-[500px] -translate-x-1/2 rounded-full bg-halo" />

      <div className="relative mx-auto max-w-7xl px-4 text-center sm:px-6 lg:px-8">
        <ScrollReveal>
          <span className="inline-flex items-center gap-1.5 rounded-full border border-accent-primary/20 bg-accent-primary/10 px-4 py-1.5 text-xs font-medium text-accent-glow">
            <BookOpen className="h-3.5 w-3.5" />
            Research Paper Platform
          </span>
        </ScrollReveal>

        <ScrollReveal delay={100}>
          <h1 className="mx-auto mt-8 max-w-4xl font-display text-4xl font-extrabold leading-[1.1] tracking-tight sm:text-6xl lg:text-7xl">
            <span className="block text-primary">Discover Research</span>
            <span className="block text-gradient">Track Trends</span>
          </h1>
        </ScrollReveal>

        <ScrollReveal delay={200}>
          <p className="mx-auto mt-6 max-w-2xl text-base leading-relaxed text-muted sm:text-lg">
            {APP_NAME} helps you search open-access papers, read PDFs in-app, bookmark findings,
            and follow citation trends — all in one place.
          </p>
        </ScrollReveal>

        <ScrollReveal delay={300}>
          <div className="mt-10 flex flex-col items-center justify-center gap-4 sm:flex-row">
            <Link
              to="/register"
              className="inline-flex w-full items-center justify-center rounded-xl bg-accent-primary px-8 py-3.5 text-sm font-semibold text-white shadow-glow transition-all hover:bg-accent-glow sm:w-auto"
            >
              Get Started Free
            </Link>
            <Link
              to="/papers"
              className="inline-flex w-full items-center justify-center gap-2 rounded-xl border border-border bg-surface/50 px-8 py-3.5 text-sm font-semibold text-primary transition-all hover:border-accent-primary/40 hover:bg-elevated sm:w-auto"
            >
              <BookOpen className="h-4 w-4 text-accent-primary" />
              Browse Papers
            </Link>
          </div>
        </ScrollReveal>

        <ScrollReveal delay={400}>
          <div className="mt-12 flex items-center justify-center gap-4">
            <div className="flex -space-x-3">
              {avatars.map((initials, i) => (
                <div
                  key={initials}
                  className="flex h-9 w-9 items-center justify-center rounded-full border-2 border-base bg-elevated text-[10px] font-semibold text-accent-glow"
                  style={{ zIndex: avatars.length - i }}
                >
                  {initials}
                </div>
              ))}
            </div>
            <p className="text-left text-sm text-muted">
              <span className="font-medium text-primary">OpenAlex-powered library</span>
              <br />
              millions of scholarly works
            </p>
          </div>
        </ScrollReveal>
      </div>
    </section>
  )
}
