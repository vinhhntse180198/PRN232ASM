import { ChevronDown } from 'lucide-react'
import { useState } from 'react'
import ScrollReveal from '../ui/ScrollReveal'

const faqs = [
  {
    q: 'Where does the data come from?',
    a: 'We use public metadata from free academic sources such as OpenAlex, Crossref, and Semantic Scholar — including titles, abstracts, keywords, publication year, authors, and journals.',
  },
  {
    q: 'Is Paper Trend Tracker free to use?',
    a: 'Yes. The platform is built on free, open academic APIs, so you can search papers and explore publication trends at no cost.',
  },
  {
    q: 'How are publication trends calculated?',
    a: 'We aggregate the number of papers published per keyword and per topic across years, then visualize the change over time so you can see what is rising or fading.',
  },
  {
    q: 'Which research fields are covered?',
    a: 'To keep results focused and fast, the system initially tracks selected domains such as Computer Science and Artificial Intelligence, with more fields added over time.',
  },
  {
    q: 'How often is the data updated?',
    a: 'Data is synchronized on a periodic schedule (for example daily or weekly). It is not real-time, which keeps the dataset stable and consistent for analysis.',
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
    <section id="faq" className="px-4 py-20 sm:px-6 lg:px-8">
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
