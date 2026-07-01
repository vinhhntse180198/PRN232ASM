import { useMemo, useState } from 'react'
import AppNavbar from '../components/app/AppNavbar'
import ScrollReveal from '../components/ui/ScrollReveal'
import { getLang, setLang, t } from '../config/i18n'
import { getTheme, setTheme } from '../config/theme'
import { changePassword } from '../services/authService'

export default function AccountPage() {
  const [theme, setThemeState] = useState(getTheme())
  const [lang, setLangState] = useState(getLang())
  const [saving, setSaving] = useState(false)
  const [pw, setPw] = useState({ current: '', next: '', confirm: '' })
  const [pwError, setPwError] = useState('')
  const [pwOk, setPwOk] = useState(false)

  const firstName = useMemo(() => {
    const userName = localStorage.getItem('userFullName') || 'Researcher'
    return userName.split(' ')[0]
  }, [])

  const applyTheme = (value) => {
    setTheme(value)
    setThemeState(value)
  }

  const applyLang = (value) => {
    setLang(value)
    setLangState(value)
    window.location.reload()
  }

  const submitPassword = async (e) => {
    e.preventDefault()
    setPwError('')
    setPwOk(false)

    if (!pw.current || !pw.next) {
      setPwError('Missing password fields.')
      return
    }
    if (pw.next.length < 6) {
      setPwError('New password must be at least 6 characters.')
      return
    }
    if (pw.next !== pw.confirm) {
      setPwError('Confirm password does not match.')
      return
    }

    setSaving(true)
    try {
      await changePassword({ currentPassword: pw.current, newPassword: pw.next })
      setPw({ current: '', next: '', confirm: '' })
      setPwOk(true)
    } catch (err) {
      setPwError(err.message || 'Failed to update password')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="min-h-screen bg-base">
      <AppNavbar />

      <main className="mx-auto max-w-4xl px-4 pb-20 pt-28 sm:px-6 lg:px-8">
        <ScrollReveal>
          <h1 className="font-display text-3xl font-extrabold text-primary">{t('account')}</h1>
          <p className="mt-2 text-muted">
            {t('hi')},{' '}
            <span className="font-medium text-primary">{firstName}</span>
          </p>
        </ScrollReveal>

        <div className="mt-10 grid gap-6 lg:grid-cols-2">
          <section className="rounded-2xl border border-border bg-surface p-6">
            <h2 className="font-display text-lg font-bold text-primary">{t('appearance')}</h2>
            <div className="mt-4 flex items-center justify-between gap-4">
              <span className="text-sm text-muted">{t('theme')}</span>
              <div className="flex items-center gap-2">
                <button
                  type="button"
                  onClick={() => applyTheme('dark')}
                  className={`rounded-lg border px-3 py-2 text-sm ${
                    theme === 'dark'
                      ? 'border-accent-primary/40 bg-elevated text-primary'
                      : 'border-border bg-surface text-muted hover:text-primary'
                  }`}
                >
                  {t('dark')}
                </button>
                <button
                  type="button"
                  onClick={() => applyTheme('light')}
                  className={`rounded-lg border px-3 py-2 text-sm ${
                    theme === 'light'
                      ? 'border-accent-primary/40 bg-elevated text-primary'
                      : 'border-border bg-surface text-muted hover:text-primary'
                  }`}
                >
                  {t('light')}
                </button>
              </div>
            </div>
          </section>

          <section className="rounded-2xl border border-border bg-surface p-6">
            <h2 className="font-display text-lg font-bold text-primary">{t('language')}</h2>
            <div className="mt-4 flex items-center justify-between gap-4">
              <span className="text-sm text-muted">{t('language')}</span>
              <select
                value={lang}
                onChange={(e) => applyLang(e.target.value)}
                className="rounded-lg border border-border bg-surface px-3 py-2 text-sm text-primary"
              >
                <option value="vi">Tiếng Việt</option>
                <option value="en">English</option>
              </select>
            </div>
          </section>

          <section className="rounded-2xl border border-border bg-surface p-6 lg:col-span-2">
            <h2 className="font-display text-lg font-bold text-primary">{t('changePassword')}</h2>
            <form onSubmit={submitPassword} className="mt-4 grid gap-4 sm:grid-cols-2">
              <label className="grid gap-1">
                <span className="text-sm text-muted">{t('currentPassword')}</span>
                <input
                  type="password"
                  value={pw.current}
                  onChange={(e) => setPw((s) => ({ ...s, current: e.target.value }))}
                  className="rounded-lg border border-border bg-base px-3 py-2 text-sm text-primary"
                />
              </label>
              <div className="hidden sm:block" />
              <label className="grid gap-1">
                <span className="text-sm text-muted">{t('newPassword')}</span>
                <input
                  type="password"
                  value={pw.next}
                  onChange={(e) => setPw((s) => ({ ...s, next: e.target.value }))}
                  className="rounded-lg border border-border bg-base px-3 py-2 text-sm text-primary"
                />
              </label>
              <label className="grid gap-1">
                <span className="text-sm text-muted">{t('confirmPassword')}</span>
                <input
                  type="password"
                  value={pw.confirm}
                  onChange={(e) => setPw((s) => ({ ...s, confirm: e.target.value }))}
                  className="rounded-lg border border-border bg-base px-3 py-2 text-sm text-primary"
                />
              </label>

              {pwError && (
                <p className="sm:col-span-2 rounded-lg border border-rose-500/30 bg-rose-500/10 px-4 py-3 text-sm text-rose-300">
                  {pwError}
                </p>
              )}
              {pwOk && (
                <p className="sm:col-span-2 rounded-lg border border-emerald-500/30 bg-emerald-500/10 px-4 py-3 text-sm text-emerald-300">
                  {t('updated')}
                </p>
              )}

              <div className="sm:col-span-2 flex justify-end">
                <button
                  type="submit"
                  disabled={saving}
                  className="rounded-lg bg-accent-primary px-4 py-2 text-sm font-semibold text-white hover:bg-accent-primary/90 disabled:opacity-50"
                >
                  {t('updatePassword')}
                </button>
              </div>
            </form>
          </section>
        </div>
      </main>
    </div>
  )
}

