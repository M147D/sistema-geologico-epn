// src/context/UsersContext.jsx
import { createContext, useContext, useState, useCallback, useRef } from 'react';
import { useUsersService } from '../hooks/useUsersService';

const UsersContext = createContext();

// eslint-disable-next-line react-refresh/only-export-components
export const useUsers = () => {
  const context = useContext(UsersContext);
  if (!context) throw new Error('useUsers must be used within UsersProvider');
  return context;
};

export const UsersProvider = ({ children }) => {
  const usersService = useUsersService();

  const [usuarios, setUsuarios] = useState([]);
  const [estadisticas, setEstadisticas] = useState(null);
  const [loading, setLoading] = useState(false);
  const [loadingStats, setLoadingStats] = useState(false);
  const [error, setError] = useState(null);

  // Guard: evita double-fetch en StrictMode
  const loadingRef = useRef(false);

  const cargarUsuarios = useCallback(async (includeInactive = false) => {
    if (loadingRef.current) return;
    loadingRef.current = true;
    setLoading(true);
    setError(null);
    try {
      const response = await usersService.getUsuarios({
        pageSize: 500,
        includeInactive: includeInactive ? true : undefined,
      });
      if (response?.success && response?.data?.users) {
        setUsuarios(response.data.users);
      } else {
        setUsuarios([]);
      }
    } catch (err) {
      console.error('Error al cargar usuarios:', err);
      setError('Error al cargar la lista de usuarios');
    } finally {
      setLoading(false);
      loadingRef.current = false;
    }
  }, [usersService]);

  const cargarEstadisticas = useCallback(async () => {
    setLoadingStats(true);
    try {
      const response = await usersService.getEstadisticas();
      if (response?.success && response?.data) {
        setEstadisticas(response.data);
      }
    } catch (err) {
      console.error('Error al cargar estadísticas:', err);
    } finally {
      setLoadingStats(false);
    }
  }, [usersService]);

  const crearUsuario = useCallback(async (dto) => {
    const response = await usersService.crearUsuario(dto);
    if (response?.success) {
      await cargarUsuarios(true);
      await cargarEstadisticas();
    }
    return response;
  }, [usersService, cargarUsuarios, cargarEstadisticas]);

  const actualizarUsuario = useCallback(async (id, dto) => {
    const response = await usersService.actualizarUsuario(id, dto);
    if (response?.success && response?.user) {
      setUsuarios(prev => prev.map(u => u.id === id ? { ...u, ...response.user } : u));
    }
    return response;
  }, [usersService]);

  const eliminarUsuario = useCallback(async (id) => {
    const response = await usersService.eliminarUsuario(id);
    if (response?.success) {
      setUsuarios(prev => prev.map(u => u.id === id ? { ...u, estadoActivo: false } : u));
      await cargarEstadisticas();
    }
    return response;
  }, [usersService, cargarEstadisticas]);

  const reactivarUsuario = useCallback(async (id) => {
    const response = await usersService.reactivarUsuario(id);
    if (response?.success && response?.user) {
      setUsuarios(prev => prev.map(u => u.id === id ? { ...u, ...response.user } : u));
      await cargarEstadisticas();
    }
    return response;
  }, [usersService, cargarEstadisticas]);

  const cambiarPassword = useCallback(async (id, dto) => {
    return usersService.cambiarPassword(id, dto);
  }, [usersService]);

  const toggleEstado = useCallback(async (id) => {
    const response = await usersService.toggleEstado(id);
    if (response?.success && response?.user) {
      setUsuarios(prev => prev.map(u => u.id === id ? { ...u, ...response.user } : u));
      await cargarEstadisticas();
    }
    return response;
  }, [usersService, cargarEstadisticas]);

  const value = {
    usuarios,
    estadisticas,
    loading,
    loadingStats,
    error,
    cargarUsuarios,
    cargarEstadisticas,
    crearUsuario,
    actualizarUsuario,
    eliminarUsuario,
    reactivarUsuario,
    cambiarPassword,
    toggleEstado,
    clearError: () => setError(null),
  };

  return (
    <UsersContext.Provider value={value}>
      {children}
    </UsersContext.Provider>
  );
};

export default UsersContext;