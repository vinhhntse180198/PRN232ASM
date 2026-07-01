import { useMemo } from 'react'
import { buildPdfEmbedSrc } from '../../utils/paperReader'

export default function PageBookPdfViewer({ proxyUrl, title, className = '' }) {
  const src = useMemo(() => buildPdfEmbedSrc(proxyUrl), [proxyUrl])

  if (!src) return null

  return (
    <iframe
      key={src}
      title={title}
      src={src}
      className={`block w-full border-0 bg-base ${className}`}
      style={{ minHeight: 'calc(100vh - 7rem)' }}
    />
  )
}
