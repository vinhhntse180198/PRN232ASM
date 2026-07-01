const STORAGE_KEY = 'lang'

const DICT = {
  en: {
    hi: 'Hi',
    account: 'Account',
    signOut: 'Sign out',
    appearance: 'Appearance',
    theme: 'Theme',
    dark: 'Dark',
    light: 'Light',
    language: 'Language',
    changePassword: 'Change password',
    currentPassword: 'Current password',
    newPassword: 'New password',
    confirmPassword: 'Confirm new password',
    updatePassword: 'Update password',
    updated: 'Updated',
    library: 'Library',
    trends: 'Trends',
    bookmarks: 'Bookmarks',
  },
  vi: {
    hi: 'Xin chào',
    account: 'Tài khoản',
    signOut: 'Đăng xuất',
    appearance: 'Giao diện',
    theme: 'Chế độ',
    dark: 'Tối',
    light: 'Sáng',
    language: 'Ngôn ngữ',
    changePassword: 'Đổi mật khẩu',
    currentPassword: 'Mật khẩu hiện tại',
    newPassword: 'Mật khẩu mới',
    confirmPassword: 'Xác nhận mật khẩu mới',
    updatePassword: 'Cập nhật mật khẩu',
    updated: 'Đã cập nhật',
    library: 'Thư viện',
    trends: 'Xu hướng',
    bookmarks: 'Dấu trang',
  },
}

export function getLang() {
  const raw = localStorage.getItem(STORAGE_KEY)
  return raw === 'vi' || raw === 'en' ? raw : 'vi'
}

export function setLang(lang) {
  localStorage.setItem(STORAGE_KEY, lang)
}

export function t(key) {
  const lang = getLang()
  return DICT[lang]?.[key] ?? DICT.en[key] ?? key
}

