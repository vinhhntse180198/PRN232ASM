import { Link } from 'react-router-dom'
import ScrollReveal from '../ui/ScrollReveal'

export default function CTASection() {
  return (
    <section className="relative mx-4 my-20 overflow-hidden rounded-3xl bg-cta-gradient sm:mx-6 lg:mx-8">
      <div className="pointer-events-none absolute inset-0 bg-halo opacity-60" />
      <div className="relative px-6 py-16 text-center sm:px-12 sm:py-20">
        <ScrollReveal>
          <h2 className="font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Ready to Explore Research Trends?
          </h2>
          <p className="mx-auto mt-4 max-w-xl text-sm text-muted sm:text-base">
            Join researchers and students tracking the latest scientific publication trends.
          </p>
          <div className="mt-8 flex flex-col items-center justify-center gap-4 sm:flex-row">
            <Link
              to="/register"
              className="inline-flex w-full items-center justify-center rounded-xl bg-primary px-8 py-3.5 text-sm font-semibold text-base transition-opacity hover:opacity-90 sm:w-auto"
            >
              Start for Free
            </Link>
            <Link
              to="/dashboard"
              className="inline-flex w-full items-center justify-center rounded-xl border border-primary/30 px-8 py-3.5 text-sm font-semibold text-primary transition-colors hover:bg-primary/10 sm:w-auto"
            >
              View Live Dashboard
            </Link>
          </div>
        </ScrollReveal>
      </div>
    </section>
  )
}
