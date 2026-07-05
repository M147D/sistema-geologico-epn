// src/context/AuthContext.jsx
import { createContext, useState, useContext, useEffect, useMemo } from 'react';
import { setApiToken, onUnauthorized, getStoredToken } from '../lib/apiClient';
import { useAuthService } from '../hooks/useAuthService';
import { ROLES } from '../constants/roles';
import { clearAllCache } from '../utils/imageCache';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(getStoredToken);
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const authService = useAuthService();
  const isAuthenticated = !!token;

  const setAuthToken = (newToken) => {
    setToken(newToken);
    setApiToken(newToken);
  };

  const clearToken = () => {
    setToken(null);
    setApiToken(null);
  };

  // El interceptor de apiClient avisa aquí cuando el servidor responde 401,
  // así token y user se limpian juntos sin que apiClient conozca a AuthContext.
  useEffect(() => {
    onUnauthorized(() => {
      clearToken();
      setUser(null);
    });
  }, []);

  useEffect(() => {
    const checkAuthentication = async () => {
      // No token → nothing to restore
      if (!isAuthenticated) {
        setLoading(false);
        return;
      }
      // User already hydrated (login/register already set it) → skip /auth/me round-trip
      if (user) {
        setLoading(false);
        return;
      }
      // Token en localStorage pero sin user en memoria → recarga de página, restaurar sesión
      try {
        const data = await authService.me();
        if (data?.success && data.user) {
          setUser(data.user);
        }
      } catch (error) {
        console.error('Error en verificación de autenticación:', {
          status: error.response?.status,
          data: error.response?.data,
          message: error.message
        });
        setUser(null);
        clearToken();
      } finally {
        setLoading(false);
      }
    };

    checkAuthentication();
  }, [isAuthenticated, user, authService]);

  const login = async (credentials) => {
    try {
      const data = await authService.login(credentials);

      if (data.success && data.token) {
        setAuthToken(data.token);
        setUser(data.user);
        return data;
      } else {
        throw new Error(data.message || 'Login failed');
      }
    } catch (error) {
      console.error('Error en login:', error);
      throw error;
    }
  };

  const register = async (userData) => {
    try {
      const data = await authService.register(userData);
      if (data.success && data.token) {
        setAuthToken(data.token);
        setUser(data.user);
        return data;
      } else {
        throw new Error(data.message || 'Error al registrar');
      }
    } catch (error) {
      console.error('Error en registro:', error);
      throw error;
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.error('Error al hacer logout en el servidor:', error);
    } finally {
      clearAllCache();
      clearToken();
      setUser(null);
    }
  };

  const permissions = useMemo(() => {
    const rol = user?.rol;
    return {
      isAdmin: rol === ROLES.ADMIN,
      canCreate: rol === ROLES.ADMIN || rol === ROLES.INVITADO,
      canEdit: rol === ROLES.ADMIN || rol === ROLES.INVITADO,
      canDelete: rol === ROLES.ADMIN,
      canManageUsers: rol === ROLES.ADMIN,
    };
  }, [user]);

  return (
    <AuthContext.Provider value={{
      user,
      loading,
      login,
      register,
      logout,
      isAuthenticated,
      ...permissions
    }}>
      {children}
    </AuthContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === null) {
    throw new Error('useAuth debe ser usado dentro de un AuthProvider');
  }
  return context;
};