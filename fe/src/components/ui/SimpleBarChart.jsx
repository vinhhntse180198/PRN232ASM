export default function SimpleBarChart({ data, labelKey = 'label', valueKey = 'count', color = '#4F46E5' }) {
  if (!data?.length) {
    return <p className="text-sm text-muted">No data available.</p>
  }

  const max = Math.max(...data.map((d) => d[valueKey] || 0), 1)

  return (
    <div className="space-y-3">
      {data.map((item) => {
        const label = String(item[labelKey] || '')
        const truncated = label.length > 32 ? label.slice(0, 30) + '…' : label
        return (
          <div key={label}>
            <div className="mb-1 flex justify-between text-sm">
              <span className="truncate max-w-[70%] font-medium text-primary" title={label}>{truncated}</span>
              <span className="ml-2 shrink-0 rounded-full bg-accent-primary/10 px-2 py-0.5 font-mono text-xs font-bold text-accent-primary">
                {item[valueKey]}
              </span>
            </div>
            <div className="h-2.5 overflow-hidden rounded-full bg-elevated">
              <div
                className="h-full rounded-full transition-all"
                style={{
                  width: `${((item[valueKey] || 0) / max) * 100}%`,
                  backgroundColor: color,
                }}
              />
            </div>
          </div>
        )
      })}
    </div>
  )
}
