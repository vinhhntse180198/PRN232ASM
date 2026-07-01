import { ChevronDown } from 'lucide-react'
import { useState } from 'react'
import ScrollReveal from '../ui/ScrollReveal'
import { APP_NAME } from '../../config/app'

const faqs = [
  {
    q: `What is ${APP_NAME}?`,
    a: 'A research platform that lets you search scholarly papers via OpenAlex, read open-access PDFs in the browser, save bookmarks, and explore citation metadata.',
  },
  {
    q: 'Is there a free plan available?',
    a: 'Yes. Create a free account to search papers, browse open-access works, read PDFs in-app, and save bookmarks to your personal library.',
  },
  {
    q: 'Where does the paper data come from?',
    a: 'We integrate with OpenAlex and other open scholarly sources. Free papers with full text can be read directly; paywalled works link to the publisher.',
  },
  {
    q: 'Can I read PDFs without leaving the app?',
    a: 'Yes. Open-access papers with a PDF are proxied for in-app reading. You can also download PDFs or open the publisher page when needed.',
  },
  {
    q: 'How do bookmarks work?',
    a: 'Save any paper to your library from search results or the reader view. Access saved papers anytime from the Bookmarks page.',
  },
]

function FaqItem({ item, open, onToggle }) {
  return (
    <div className="border-b border-border">
      <button
        type="button"
        onClick={onToggle}
        className="flex w-full items-center justify-between gap-4 py-5 text-left text-primary"
      >
        <span className="text-sm font-bold sm:text-base">{item.q}</span>
        <ChevronDown
          className={`h-5 w-5 shrink-0 text-muted transition-transform duration-300 ${open ? 'rotate-180' : ''}`}
        />
      </button>
      <div
        className="overflow-hidden transition-all duration-300 ease-in-out"
        style={{ maxHeight: open ? '200px' : '0' }}
      >
        <p className="pb-5 text-sm font-semibold leading-relaxed text-muted">{item.a}</p>
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
