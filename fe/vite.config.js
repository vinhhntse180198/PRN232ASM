import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api/openalex': {
        target: 'http://localhost:5005',
        changeOrigin: true,
      },
      '/api/sync': {
        target: 'http://localhost:5005',
        changeOrigin: true,
      },
      '/api/trends': {
        target: 'http://localhost:5003',
        changeOrigin: true,
      },
    },
  },
})
