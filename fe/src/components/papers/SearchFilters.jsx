import { Search } from 'lucide-react'

export default function SearchFilters({ filters, topics, onChange, onSubmit, loading }) {
  return (
    <form
      onSubmit={(e) => {
        e.preventDefault()
        onSubmit()
      }}
      className="rounded-2xl border border-border bg-surface p-6"
    >
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <div className="lg:col-span-2">
          <label className="block text-sm font-medium text-primary">Keyword</label>
          <input
            type="text"
            value={filters.keyword}
            onChange={(e) => onChange({ ...filters, keyword: e.target.value })}
            placeholder="Title, abstract, keyword..."
            className="mt-2 w-full rounded-xl border border-border bg-elevated px-4 py-3 text-sm outline-none focus:border-accent-primary/50"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-primary">Author</label>
          <input
            type="text"
            value={filters.author}
            onChange={(e) => onChange({ ...filters, author: e.target.value })}
            placeholder="Author name"
            className="mt-2 w-full rounded-xl border border-border bg-elevated px-4 py-3 text-sm outline-none focus:border-accent-primary/50"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-primary">Journal</label>
          <input
            type="text"
            value={filters.journal}
            onChange={(e) => onChange({ ...filters, journal: e.target.value })}
            placeholder="Journal name"
            className="mt-2 w-full rounded-xl border border-border bg-elevated px-4 py-3 text-sm outline-none focus:border-accent-primary/50"
          />
        </div>
        <div className="md:col-span-2 lg:col-span-1">
          <label className="block text-sm font-medium text-primary">Topic</label>
          <select
            value={filters.topicId}
            onChange={(e) => onChange({ ...filters, topicId: e.target.value })}
            className="mt-2 w-full rounded-xl border border-border bg-elevated px-4 py-3 text-sm outline-none focus:border-accent-primary/50"
          >
            <option value="">All topics</option>
            {topics.map((t) => (
              <option key={t.id} value={t.id}>
                {t.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="mt-4 flex justify-end">
        <button
          type="submit"
          disabled={loading}
          className="inline-flex items-center gap-2 rounded-xl bg-accent-primary px-5 py-3 text-sm font-semibold text-white shadow-glow-sm hover:bg-accent-glow disabled:opacity-60"
        >
          <Search className="h-4 w-4" />
          Search papers
        </button>
      </div>
    </form>
  )
}
