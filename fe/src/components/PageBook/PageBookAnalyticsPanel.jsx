import { Building2, Calendar, Layers, Lock, LockOpen, MoreHorizontal } from 'lucide-react'

function WidgetCard({ icon: Icon, title, children, light }) {
  return (
    <div className={`rounded-lg shadow-sm ${light ? 'bg-white' : 'border border-border bg-surface shadow-card'}`}>
      <div className={`flex items-center justify-between px-3 py-2 ${light ? 'border-b border-stone-100' : 'border-b border-border/60'}`}>
        <div className={`flex items-center gap-2 text-xs font-semibold ${light ? 'text-stone-700' : 'text-primary'}`}>
          <Icon className={`h-3.5 w-3.5 ${light ? 'text-stone-400' : 'text-accent-primary'}`} />
          {title}
        </div>
        <MoreHorizontal className={`h-4 w-4 ${light ? 'text-stone-300' : 'text-muted'}`} />
      </div>
      <div className="p-3">{children}</div>
    </div>
  )
}

function TopicRow({ label, checked = true, light }) {
  return (
    <label className={`flex cursor-default items-center gap-2 py-1.5 text-xs last:border-0 ${
      light ? 'border-b border-stone-50' : 'border-b border-border/40'
    }`}>
      <input
        type="checkbox"
        checked={checked}
        readOnly
        className={`rounded ${light ? 'border-stone-300 accent-stone-700' : 'border-border accent-accent-primary'}`}
      />
      <span className={`flex-1 truncate ${light ? 'text-stone-700' : 'text-primary'}`}>{label}</span>
      <span className={`font-mono ${light ? 'text-stone-400' : 'text-muted'}`}>1</span>
    </label>
  )
}

export default function PageBookAnalyticsPanel({ paper, isFree, variant = 'dark' }) {
  const light = variant === 'light'
  const year = paper?.publishedYear ?? new Date().getFullYear()
  const citations = paper?.citationCount ?? 0
  const keywords = paper?.keywords ?? []
  const bars = [0.2, 0.35, 0.5, 0.65, 0.8, 1].map((m) => Math.max(14, Math.round(citations * m * 0.12 + 10)))

  const barActive = light ? 'bg-stone-800' : 'bg-accent-primary'
  const barMuted = light ? 'bg-stone-200' : 'bg-elevated'
  const cardShell = light ? 'rounded-lg bg-white shadow-sm' : 'rounded-lg border border-border bg-surface shadow-card'
  const cardHeader = light ? 'border-b border-stone-100' : 'border-b border-border/60'
  const titleMuted = light ? 'text-stone-500' : 'text-muted'
  const titlePrimary = light ? 'text-stone-900' : 'text-primary'
  const donutBg = light ? '#e7e5e4' : '#1A2235'
  const donutInner = light ? 'bg-white' : 'bg-surface'

  return (
    <div className="space-y-3">
      <div className={cardShell}>
        <div className={`flex items-center justify-between px-3 py-2 ${cardHeader}`}>
          <div className={`text-[10px] font-bold uppercase tracking-widest ${titleMuted}`}>Citation Count</div>
          <MoreHorizontal className={`h-4 w-4 ${light ? 'text-stone-300' : 'text-muted'}`} />
        </div>
        <div className="p-4">
          <p className={`text-3xl font-bold ${titlePrimary}`}>{citations.toLocaleString()}</p>
          <div className="mt-3 flex h-12 items-end gap-1">
            {bars.map((h, i) => (
              <div
                key={i}
                className={`flex-1 rounded-t ${i === bars.length - 1 ? barActive : barMuted}`}
                style={{ height: `${h}%` }}
              />
            ))}
          </div>
        </div>
      </div>

      <WidgetCard icon={Calendar} title="Năm" light={light}>
        <div className="flex h-20 items-end gap-1">
          {bars.map((h, i) => (
            <div key={i} className="group relative flex-1">
              <div
                className={`w-full rounded-t transition-colors ${i === bars.length - 1 ? barActive : barMuted}`}
                style={{ height: `${h}%` }}
              />
            </div>
          ))}
        </div>
        <p className={`mt-2 text-center text-[10px] ${titleMuted}`}>
          {year}: {citations.toLocaleString()} trích dẫn
        </p>
      </WidgetCard>

      <WidgetCard icon={isFree ? LockOpen : Lock} title="Truy cập mở" light={light}>
        <div className="flex items-center gap-4">
          <div
            className="relative h-16 w-16 shrink-0 rounded-full"
            style={{
              background: isFree
                ? `conic-gradient(#059669 0% 100%, ${donutBg} 100%)`
                : `conic-gradient(#78716c 0% 100%, ${donutBg} 100%)`,
            }}
          >
            <div className={`absolute inset-2 flex items-center justify-center rounded-full text-[10px] font-bold ${donutInner} ${titlePrimary}`}>
              {isFree ? '100%' : '0%'}
            </div>
          </div>
          <div>
            <p className={`text-lg font-bold ${titlePrimary}`}>{isFree ? 'Miễn phí' : 'Trả phí'}</p>
            <p className={`text-xs ${titleMuted}`}>1 tác phẩm</p>
          </div>
        </div>
      </WidgetCard>

      {keywords.length > 0 && (
        <WidgetCard icon={Layers} title="Đề tài" light={light}>
          <div className="max-h-32 overflow-y-auto">
            {keywords.slice(0, 6).map((kw) => (
              <TopicRow key={kw} label={kw} light={light} />
            ))}
          </div>
        </WidgetCard>
      )}

      {paper?.journalName && (
        <WidgetCard icon={Building2} title="Tổ chức" light={light}>
          <TopicRow label={paper.journalName} light={light} />
        </WidgetCard>
      )}

      <WidgetCard icon={Layers} title="Kiểu" light={light}>
        <TopicRow label="bài báo" light={light} />
        <TopicRow label="bản in trước" checked={false} light={light} />
        <TopicRow label="luận văn" checked={false} light={light} />
      </WidgetCard>
    </div>
  )
}
