// src/hooks/useAuthService.js
import { useMemo } from 'react';
import { api } from '../lib/apiClient';
import { authStubs } from '../stubs/authStubs';

const USE_STUBS = import.meta.env.VITE_USE_STUBS === 'true';

/**
 * Hook privado para operaciones de autenticación.
 * Solo debe ser consumido por AuthContext.
 */
export const useAuthService = () => {
  return useMemo(() => {
    if (USE_STUBS) return authStubs;

    return {
      async login(credentials) {
        const { data } = await api.post('/auth/login', credentials);
        return data;
      },

      async register(userData) {
        const { data } = await api.post('/auth/register', userData);
        return data;
      },

      async logout() {
        const { data } = await api.post('/auth/logout');
        return data;
      },

      async me() {
        const { data } = await api.get('/auth/me');
        return data;
      },
    };
  }, []);
};