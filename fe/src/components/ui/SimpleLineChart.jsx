export default function SimpleLineChart({ data, labelKey = 'year', valueKey = 'count', color = '#4F46E5' }) {
  if (!data?.length) {
    return <p className="text-sm text-muted">No data available.</p>
  }

  const sorted = [...data].sort((a, b) => Number(a[labelKey]) - Number(b[labelKey]))
  const max = Math.max(...sorted.map((d) => d[valueKey] || 0), 1)

  const padding = { top: 24, right: 32, bottom: 48, left: 64 }
  const width = 720
  const height = 320
  const chartW = width - padding.left - padding.right
  const chartH = height - padding.top - padding.bottom

  const points = sorted.map((d, i) => ({
    x: padding.left + (i / (sorted.length - 1 || 1)) * chartW,
    y: padding.top + chartH - (d[valueKey] / max) * chartH,
    value: d[valueKey],
    label: String(d[labelKey]),
  }))

  const polylinePoints = points.map((p) => `${p.x},${p.y}`).join(' ')

  const yTicks = [0, 0.25, 0.5, 0.75, 1].map((t) => ({
    y: padding.top + chartH - t * chartH,
    value: Math.round(t * max),
  }))

  return (
    <div className="relative w-full overflow-x-auto">
      <svg viewBox={`0 0 ${width} ${height}`} className="w-full min-w-[300px]" style={{ height: 'auto' }}>
        <title>Publications over time</title>
        {/* Grid lines */}
        {yTicks.map((tick) => (
          <line
            key={tick.value}
            x1={padding.left}
            y1={tick.y}
            x2={width - padding.right}
            y2={tick.y}
            stroke="#E2E8F0"
            strokeWidth="1"
            strokeDasharray="4 4"
          />
        ))}

        {/* Y-axis labels */}
        {yTicks.map((tick) => (
          <text
            key={`label-${tick.value}`}
            x={padding.left - 12}
            y={tick.y + 5}
            textAnchor="end"
            fontSize="13"
            fontWeight="500"
            fill="#475569"
            fontFamily="Inter, system-ui, sans-serif"
          >
            {tick.value}
          </text>
        ))}

        {/* X-axis labels */}
        {points.map((p) => (
          <text
            key={`x-${p.label}`}
            x={p.x}
            y={height - padding.bottom + 28}
            textAnchor="middle"
            fontSize="13"
            fontWeight="600"
            fill="#0F172A"
            fontFamily="Inter, system-ui, sans-serif"
          >
            {p.label}
          </text>
        ))}

        {/* Axis line */}
        <line
          x1={padding.left}
          y1={padding.top + chartH}
          x2={width - padding.right}
          y2={padding.top + chartH}
          stroke="#94A3B8"
          strokeWidth="1.5"
        />

        {/* Line */}
        <polyline
          points={polylinePoints}
          fill="none"
          stroke={color}
          strokeWidth="3"
          strokeLinejoin="round"
          strokeLinecap="round"
        />

        {/* Dots */}
        {points.map((p, i) => (
          <g key={i}>
            <circle cx={p.x} cy={p.y} r="12" fill={color} opacity="0.18" />
            <circle cx={p.x} cy={p.y} r="5" fill={color} stroke="white" strokeWidth="2" />
            <text
              x={p.x}
              y={p.y - 14}
              textAnchor="middle"
              fontSize="12"
              fontWeight="700"
              fill="#0F172A"
              fontFamily="Inter, system-ui, sans-serif"
            >
              {p.value}
            </text>
          </g>
        ))}
      </svg>
    </div>
  )
}
