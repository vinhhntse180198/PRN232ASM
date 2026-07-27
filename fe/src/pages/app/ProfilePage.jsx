import { useState } from 'react'
import { Loader2, User } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { changePassword } from '../../services/authService'

export default function ProfilePage() {
  const { user } = useAuth()
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  const handleChangePassword = async (e) => {
    e.preventDefault()
    setError('')
    setMessage('')

    if (newPassword.length < 8) {
      setError('New password must be at least 8 characters.')
      return
    }
    if (newPassword !== confirmPassword) {
      setError('New password and confirmation do not match.')
      return
    }

    setLoading(true)
    try {
      await changePassword(currentPassword, newPassword)
      setMessage('Password changed successfully.')
      setCurrentPassword('')
      setNewPassword('')
      setConfirmPassword('')
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl font-bold text-primary">Profile</h1>
        <p className="mt-1 text-sm text-muted">Manage your account settings</p>
      </div>

      {/* User Info */}
      <div className="rounded-xl border border-border bg-surface p-6">
        <div className="flex items-center gap-4">
          <div className="flex h-16 w-16 items-center justify-center rounded-full bg-accent-primary/15 text-xl font-bold text-accent-glow">
            <User className="h-8 w-8" />
          </div>
          <div>
            <p className="font-display text-lg font-bold text-primary">{user?.fullName}</p>
            <p className="text-sm text-muted">{user?.email}</p>
            <span className="mt-1 inline-block rounded-full bg-accent-primary/10 px-3 py-0.5 text-xs font-medium text-accent-glow">
              {user?.role}
            </span>
          </div>
        </div>
      </div>

      {/* Change Password */}
      <div className="rounded-xl border border-border bg-surface p-6">
        <h2 className="mb-4 font-display text-lg font-semibold text-primary">Change Password</h2>

        {message && (
          <div className="mb-4 rounded-lg border border-accent-green/30 bg-accent-green/10 px-4 py-3 text-sm text-accent-green">
            {message}
          </div>
        )}
        {error && (
          <div className="mb-4 rounded-lg border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-300">
            {error}
          </div>
        )}

        <form onSubmit={handleChangePassword} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium text-primary">Current Password</label>
            <input
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              className="w-full rounded-lg border border-border bg-elevated px-4 py-2.5 text-sm text-primary placeholder-muted/50 focus:border-accent-primary/50 focus:outline-none"
              placeholder="Enter current password"
              required
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-primary">New Password</label>
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              className="w-full rounded-lg border border-border bg-elevated px-4 py-2.5 text-sm text-primary placeholder-muted/50 focus:border-accent-primary/50 focus:outline-none"
              placeholder="Min. 8 characters"
              required
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-primary">Confirm New Password</label>
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              className="w-full rounded-lg border border-border bg-elevated px-4 py-2.5 text-sm text-primary placeholder-muted/50 focus:border-accent-primary/50 focus:outline-none"
              placeholder="Re-enter new password"
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="btn-primary flex items-center gap-2"
          >
            {loading && <Loader2 className="h-4 w-4 animate-spin" />}
            Change Password
          </button>
        </form>
      </div>
    </div>
  )
}
