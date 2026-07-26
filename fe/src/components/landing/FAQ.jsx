import { ChevronDown } from 'lucide-react'
import { useState } from 'react'
import ScrollReveal from '../ui/ScrollReveal'

const faqs = [
  {
    q: 'What data sources does SciTrend use?',
    a: 'SciTrend aggregates metadata from academic APIs including Semantic Scholar, OpenAlex, and Crossref. We collect paper titles, abstracts, keywords, publication years, authors, and journal information — no full-text content is stored.',
  },
  {
    q: 'How does keyword trend tracking work?',
    a: 'You can follow specific keywords or research topics. The system monitors new publications and sends notifications when papers matching your interests are published. Trends are recalculated every 6 hours from the latest data.',
  },
  {
    q: 'What research domains are covered?',
    a: 'SciTrend currently focuses on curated datasets from Computer Science and AI research. The admin can configure additional data sources and research domains through the system settings.',
  },
  {
    q: 'Can I export or generate reports?',
    a: 'Yes. Both regular users and administrators can generate trend reports showing publication counts, top keywords, and growth analysis for any time period in the dataset.',
  },
  {
    q: 'How often is the paper database updated?',
    a: 'Data synchronization runs daily via the OpenAlex API when enabled by the administrator. Manual sync can also be triggered from the admin panel at any time.',
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
