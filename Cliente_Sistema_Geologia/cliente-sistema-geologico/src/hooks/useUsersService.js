// src/hooks/useUsersService.js
import { useMemo } from 'react';
import { api } from '../lib/apiClient';

/**
 * Hook privado para operaciones de usuarios.
 * Solo debe ser consumido por UsersContext.
 */
export const useUsersService = () => {
  return useMemo(() => ({
    async getUsuarios(filtros = {}) {
      const params = new URLSearchParams();
      Object.entries(filtros).forEach(([k, v]) => {
        if (v !== null && v !== undefined && v !== '') params.append(k, v);
      });
      const response = await api.get(`/users?${params.toString()}`);
      return response.data;
    },

    async getUsuarioPorId(id) {
      const response = await api.get(`/users/${id}`);
      return response.data;
    },

    async crearUsuario(dto) {
      const response = await api.post('/users', dto);
      return response.data;
    },

    async actualizarUsuario(id, dto) {
      const response = await api.put(`/users/${id}`, dto);
      return response.data;
    },

    async eliminarUsuario(id) {
      const response = await api.delete(`/users/${id}`);
      return response.data;
    },

    async reactivarUsuario(id) {
      const response = await api.post(`/users/${id}/reactivate`);
      return response.data;
    },

    async cambiarPassword(id, dto) {
      const response = await api.post(`/users/${id}/change-password`, dto);
      return response.data;
    },

    async toggleEstado(id) {
      const response = await api.post(`/users/${id}/toggle-status`);
      return response.data;
    },

    async getEstadisticas() {
      const response = await api.get('/users/stats');
      return response.data;
    },

    async verificarEmail(email, excludeId = null) {
      const params = excludeId ? `?excludeUserId=${excludeId}` : '';
      const response = await api.get(`/users/check-email/${encodeURIComponent(email)}${params}`);
      return response.data;
    },

    async verificarUsername(username, excludeId = null) {
      const params = excludeId ? `?excludeUserId=${excludeId}` : '';
      const response = await api.get(`/users/check-username/${encodeURIComponent(username)}${params}`);
      return response.data;
    },
  }), []);
};
