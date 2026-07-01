const STORAGE_KEY = 'theme'

export function getTheme() {
  const raw = localStorage.getItem(STORAGE_KEY)
  return raw === 'light' || raw === 'dark' ? raw : 'dark'
}

export function setTheme(theme) {
  localStorage.setItem(STORAGE_KEY, theme)
  applyTheme(theme)
}

export function applyTheme(theme = getTheme()) {
  document.documentElement.setAttribute('data-theme', theme)
}

