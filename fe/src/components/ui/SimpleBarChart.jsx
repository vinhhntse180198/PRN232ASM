export default function SimpleBarChart({ data, labelKey = 'label', valueKey = 'count', color = '#6366F1' }) {
  if (!data?.length) {
    return <p className="text-sm text-muted">No data available.</p>
  }

  const max = Math.max(...data.map((d) => d[valueKey] || 0), 1)

  return (
    <div className="space-y-3">
      {data.map((item) => (
        <div key={item[labelKey]}>
          <div className="mb-1 flex justify-between text-xs">
            <span className="text-muted">{item[labelKey]}</span>
            <span className="font-mono text-primary">{item[valueKey]}</span>
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-elevated">
            <div
              className="h-full rounded-full transition-all"
              style={{
                width: `${((item[valueKey] || 0) / max) * 100}%`,
                backgroundColor: color,
              }}
            />
          </div>
        </div>
      ))}
    </div>
  )
}
