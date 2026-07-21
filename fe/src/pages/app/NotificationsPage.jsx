import { useEffect, useState } from 'react'
import { Bell, Loader2 } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import {
  getNotifications,
  markAllNotificationsRead,
  markNotificationRead,
} from '../../services/notificationService'

export default function NotificationsPage() {
  const { user } = useAuth()
  const [notifications, setNotifications] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const load = async () => {
    if (!user?.id) return
    try {
      const data = await getNotifications(user.id)
      setNotifications(data || [])
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.id])

  const markRead = async (id) => {
    await markNotificationRead(id)
    await load()
  }

  const markAllRead = async () => {
    await markAllNotificationsRead(user.id)
    await load()
  }

  if (loading) {
    return (
      <div className="flex justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-accent-primary" />
      </div>
    )
  }

  const unread = notifications.filter((n) => !n.isRead).length

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-bold text-primary">Notifications</h1>
          <p className="text-sm text-muted">{unread} unread</p>
        </div>
        {unread > 0 && (
          <button type="button" onClick={markAllRead} className="btn-secondary text-sm">
            Mark all read
          </button>
        )}
      </div>

      {error && <div className="alert-error">{error}</div>}

      {!notifications.length ? (
        <p className="text-muted">No notifications yet.</p>
      ) : (
        <div className="space-y-3">
          {notifications.map((n) => (
            <div
              key={n.id}
              className={`flex gap-4 rounded-xl border p-4 ${
                n.isRead ? 'border-border bg-surface' : 'border-accent-primary/30 bg-accent-primary/5'
              }`}
            >
              <Bell className={`mt-0.5 h-5 w-5 ${n.isRead ? 'text-muted' : 'text-accent-glow'}`} />
              <div className="flex-1">
                <p className="font-medium text-primary">{n.title}</p>
                <p className="mt-1 text-sm text-muted">{n.message}</p>
                <p className="mt-2 text-xs text-muted">
                  {new Date(n.createdAt).toLocaleString()} · {n.type}
                </p>
              </div>
              {!n.isRead && (
                <button type="button" onClick={() => markRead(n.id)} className="text-xs text-accent-glow hover:underline">
                  Mark read
                </button>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
