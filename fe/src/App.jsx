import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'

import LandingPage from './pages/LandingPage'

import LoginPage from './pages/LoginPage'

import RegisterPage from './pages/RegisterPage'

import BookmarksPage from './pages/BookmarksPage'

import TrendsPage from './pages/TrendsPage'
import AccountPage from './pages/AccountPage'

import PageBookList from './pages/PageBookList'
import PageBookDetail from './pages/PageBookDetail'
import PageBookReader from './pages/PageBookReader'
import PageBookPaywall from './pages/PageBookPaywall'

import ProtectedRoute from './components/auth/ProtectedRoute'



export default function App() {

  return (

    <BrowserRouter>

      <Routes>

        <Route path="/" element={<LandingPage />} />

        <Route path="/login" element={<LoginPage />} />

        <Route path="/register" element={<RegisterPage />} />

        <Route path="/papers" element={<ProtectedRoute><PageBookList /></ProtectedRoute>} />

        <Route path="/papers/:id" element={<ProtectedRoute><PageBookDetail /></ProtectedRoute>} />

        <Route path="/read/:openAlexId" element={<ProtectedRoute><PageBookReader /></ProtectedRoute>} />

        <Route path="/link/:openAlexId" element={<ProtectedRoute><PageBookPaywall /></ProtectedRoute>} />

        <Route path="/bookmarks" element={<ProtectedRoute><BookmarksPage /></ProtectedRoute>} />

        <Route path="/trends" element={<ProtectedRoute><TrendsPage /></ProtectedRoute>} />

        <Route path="/account" element={<ProtectedRoute><AccountPage /></ProtectedRoute>} />

        <Route path="*" element={<Navigate to="/papers" replace />} />

      </Routes>

    </BrowserRouter>

  )

}

