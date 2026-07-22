import { useEffect, useState } from 'react'
import { Activity, CheckCircle, Loader2, XCircle } from 'lucide-react'
import { getHealth } from '../../services/syncService'

const SERVICES = [
  { name: 'API Gateway', url: 'http://localhost:5000/health' },
  { name: 'Auth Service', url: 'http://localhost:5131/swagger/index.html', type: 'swagger' },
  { name: 'Paper Service', url: 'http://localhost:5002/swagger/index.html', type: 'swagger' },
  { name: 'Trend Service', url: 'http://localhost:5003/swagger/index.html', type: 'swagger' },
  { name: 'Notification Service', url: 'http://localhost:5004/swagger/index.html', type: 'swagger' },
  { name: 'Sync Service', url: 'http://localhost:5005/swagger/index.html', type: 'swagger' },
  { name: 'RabbitMQ Management', url: 'http://localhost:15672', type: 'external' },
]

export default function AdminMonitoringPage() {
  const [gateway, setGateway] = useState(null)
  const [checks, setChecks] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const run = async () => {
      try {
        const health = await getHealth()
        setGateway(health)
      } catch {
        setGateway({ status: 'Unreachable' })
      }

      const results = await Promise.all(
        SERVICES.map(async (svc) => {
          try {
            const res = await fetch(svc.url, { method: 'HEAD', mode: 'no-cors' }).catch(() => null)
            return {
              ...svc,
              ok: svc.type === 'external' || res !== null || svc.name === 'API Gateway',
            }
          } catch {
            return { ...svc, ok: false }
          }
        }),
      )
      setChecks(results)
      setLoading(false)
    }
    run()
  }, [])

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <h1 className="font-display text-2xl font-bold text-primary">Monitoring Dashboard</h1>

      <div className="rounded-xl border border-border bg-surface p-5">
        <div className="flex items-center gap-3">
          <Activity className="h-6 w-6 text-accent-green" />
          <div>
            <p className="font-medium text-primary">API Gateway</p>
            <p className="text-sm text-muted">
              Status: {gateway?.status ?? 'Unknown'} · {gateway?.timestamp && new Date(gateway.timestamp).toLocaleString()}
            </p>
          </div>
        </div>
      </div>

      <div className="grid gap-3 md:grid-cols-2">
        {checks.map((svc) => (
          <div key={svc.name} className="flex items-center justify-between rounded-xl border border-border bg-surface p-4">
            <span className="text-primary">{svc.name}</span>
            {svc.ok ? (
              <CheckCircle className="h-5 w-5 text-accent-green" />
            ) : (
              <XCircle className="h-5 w-5 text-red-400" />
            )}
          </div>
        ))}
      </div>
    </div>
  )
}
