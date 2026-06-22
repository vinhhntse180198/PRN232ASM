import { ChevronDown } from 'lucide-react'
import { useState } from 'react'
import ScrollReveal from '../ui/ScrollReveal'

const faqs = [
  {
    q: 'How does Collective OS match agencies?',
    a: 'Our AI analyzes your agency profile, skills, industry focus, and deal history to surface partners with the highest compatibility score. Matches are ranked by fit, availability, and past collaboration success.',
  },
  {
    q: 'Is there a free plan available?',
    a: 'Yes. Start free with up to 5 partner matches per month, basic analytics, and access to our vetted network. Upgrade anytime for unlimited matching and advanced pipeline tools.',
  },
  {
    q: 'How long does it take to find a partner?',
    a: 'Most agencies receive their first qualified matches within 24 hours of completing their profile. High-priority matches often surface in under 4 hours.',
  },
  {
    q: 'What types of agencies use Collective OS?',
    a: 'Branding, web development, SEO, content, video, and full-service digital agencies across B2B and B2C. We support boutiques to 200+ person teams.',
  },
  {
    q: 'Can I manage deals directly on the platform?',
    a: 'Absolutely. Track opportunities from discovery through proposal to won — with shared workspaces, messaging, and revenue reporting built in.',
  },
]

function FaqItem({ item, open, onToggle }) {
  return (
    <div className="border-b border-border">
      <button
        type="button"
        onClick={onToggle}
        className="flex w-full items-center justify-between gap-4 py-5 text-left"
      >
        <span className="text-sm font-medium text-primary sm:text-base">{item.q}</span>
        <ChevronDown
          className={`h-5 w-5 shrink-0 text-muted transition-transform duration-300 ${open ? 'rotate-180' : ''}`}
        />
      </button>
      <div
        className="overflow-hidden transition-all duration-300 ease-in-out"
        style={{ maxHeight: open ? '200px' : '0' }}
      >
        <p className="pb-5 text-sm leading-relaxed text-muted">{item.a}</p>
      </div>
    </div>
  )
}

export default function FAQ() {
  const [openIndex, setOpenIndex] = useState(0)

  return (
    <section className="px-4 py-20 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-3xl">
        <ScrollReveal>
          <h2 className="text-center font-display text-3xl font-bold tracking-tight text-primary sm:text-4xl">
            Frequently Asked Questions
          </h2>
        </ScrollReveal>

        <ScrollReveal delay={100}>
          <div className="mt-10 rounded-2xl border border-border bg-surface px-6">
            {faqs.map((item, i) => (
              <FaqItem
                key={item.q}
                item={item}
                open={openIndex === i}
                onToggle={() => setOpenIndex(openIndex === i ? -1 : i)}
              />
            ))}
          </div>
        </ScrollReveal>
      </div>
    </section>
  )
}
