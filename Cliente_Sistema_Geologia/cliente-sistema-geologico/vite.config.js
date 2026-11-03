import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

// https://vite.dev/config/
// VITE_PROXY_TARGET: a dónde reenvía /api el servidor de desarrollo.
// En Docker es http://api:5095 (compose.yaml); sin Docker, el backend local.
export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
    port: 5174,
    strictPort: true,
    proxy: {
      '/api': {
        target: process.env.VITE_PROXY_TARGET ?? 'http://localhost:5095',
        changeOrigin: true,
      }
    }
  }
})
