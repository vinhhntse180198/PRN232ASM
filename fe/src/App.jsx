import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import ProtectedRoute from './components/auth/ProtectedRoute'
import AdminRoute from './components/auth/AdminRoute'
import AppLayout from './components/layout/AppLayout'
import AdminLayout from './components/layout/AdminLayout'
import LandingPage from './pages/LandingPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import DashboardPage from './pages/app/DashboardPage'
import PapersPage from './pages/app/PapersPage'
import PaperDetailPage from './pages/app/PaperDetailPage'
import BookmarksPage from './pages/app/BookmarksPage'
import TopicsPage from './pages/app/TopicsPage'
import NotificationsPage from './pages/app/NotificationsPage'
import ReportsPage from './pages/app/ReportsPage'
import AdminIndexPage from './pages/admin/AdminIndexPage'
import AdminUsersPage from './pages/admin/AdminUsersPage'
import AdminJournalsPage from './pages/admin/AdminJournalsPage'
import AdminTopicsPage from './pages/admin/AdminTopicsPage'
import AdminDataSourcesPage from './pages/admin/AdminDataSourcesPage'
import AdminSyncPage from './pages/admin/AdminSyncPage'
import AdminMonitoringPage from './pages/admin/AdminMonitoringPage'
import AdminReportsPage from './pages/admin/AdminReportsPage'

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route
            element={
              <ProtectedRoute>
                <AppLayout />
              </ProtectedRoute>
            }
          >
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/papers" element={<PapersPage />} />
            <Route path="/papers/:id" element={<PaperDetailPage />} />
            <Route path="/bookmarks" element={<BookmarksPage />} />
            <Route path="/topics" element={<TopicsPage />} />
            <Route path="/notifications" element={<NotificationsPage />} />
            <Route path="/reports" element={<ReportsPage />} />
          </Route>

          <Route
            path="/admin"
            element={
              <AdminRoute>
                <AdminLayout />
              </AdminRoute>
            }
          >
            <Route index element={<AdminIndexPage />} />
            <Route path="users" element={<AdminUsersPage />} />
            <Route path="journals" element={<AdminJournalsPage />} />
            <Route path="topics" element={<AdminTopicsPage />} />
            <Route path="datasources" element={<AdminDataSourcesPage />} />
            <Route path="sync" element={<AdminSyncPage />} />
            <Route path="monitoring" element={<AdminMonitoringPage />} />
            <Route path="reports" element={<AdminReportsPage />} />
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}
