// src/pages/PageUsers.jsx
import { useEffect } from 'react';
import { Container, Typography, Box, CircularProgress } from '@mui/material';
import { UsersProvider, useUsers } from '../context/UsersContext';
import { useAuth } from '../context/AuthContext';
import CardUserManager from '../components/users/CardUserManager';

const UsersPageContent = () => {
  const {
    usuarios, estadisticas, loadingStats, loading, error,
    cargarUsuarios, cargarEstadisticas,
    crearUsuario, actualizarUsuario,
    reactivarUsuario, cambiarPassword, toggleEstado,
    clearError,
  } = useUsers();
  const { user } = useAuth();

  useEffect(() => {
    cargarUsuarios(true);
    cargarEstadisticas();
  }, [cargarUsuarios, cargarEstadisticas]);

  if (loading && usuarios.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="60vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ my: 4 }}>
      <Box mb={3}>
        <Typography variant="h5" fontWeight="bold">
          Gestión de usuarios
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Administra los usuarios del sistema, sus roles y accesos.
        </Typography>
      </Box>
      <CardUserManager
        usuarios={usuarios}
        estadisticas={estadisticas}
        loadingStats={loadingStats}
        error={error}
        clearError={clearError}
        crearUsuario={crearUsuario}
        actualizarUsuario={actualizarUsuario}
        reactivarUsuario={reactivarUsuario}
        cambiarPassword={cambiarPassword}
        toggleEstado={toggleEstado}
        currentUserId={user?.id}
      />
    </Container>
  );
};

const PageUsers = () => (
  <UsersProvider>
    <UsersPageContent />
  </UsersProvider>
);

export default PageUsers;