import { ExternalLink } from 'lucide-react'

export default function PageBookExternalAccess({ publisherUrl, hasPdf }) {
  const articleUrl = publisherUrl?.trim()
  if (!articleUrl) return null

  return (
    <div className="mx-6 mb-6 mt-2 rounded-xl border border-border bg-elevated/40 p-6 sm:mx-8">
      <p className="text-sm text-muted">
        {hasPdf ? 'Xem thêm trên trang nhà xuất bản:' : 'Xem bài trên trang gốc:'}
      </p>
      <div className="mt-4">
        <a
          href={articleUrl}
          target="_blank"
          rel="noreferrer"
          className="inline-flex items-center gap-2 rounded-xl border border-border bg-surface px-4 py-2.5 text-sm font-medium text-primary transition-colors hover:border-accent-primary/40 hover:text-accent-glow"
        >
          <ExternalLink className="h-4 w-4" />
          Mở trang bài viết
        </a>
      </div>
    </div>
  )
}
