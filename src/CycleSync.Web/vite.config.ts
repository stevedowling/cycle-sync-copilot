import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const apiTarget =
  process.env.services__api__https__0 ??
  process.env.services__api__http__0 ??
  process.env.SERVER_HTTPS ??
  process.env.SERVER_HTTP;

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: apiTarget
      ? {
          // Proxy API calls to the API service injected by Aspire.
          '/api': {
            target: apiTarget,
            changeOrigin: true
          }
        }
      : undefined
  }
});
