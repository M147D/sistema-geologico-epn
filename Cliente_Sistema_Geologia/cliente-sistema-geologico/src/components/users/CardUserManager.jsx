// src/components/users/CardUserManager.jsx
import { useState } from 'react';
import { Box, Alert, Snackbar } from '@mui/material';
import UserStatsCards from './UserStatsCards';
import UserFilters from './UserFilters';
import UserTable from './UserTable';
import UserFormDialog from './UserFormDialog';
import ChangePasswordDialog from './ChangePasswordDialog';

const FILTROS_INICIALES = { busqueda: '', rol: '', estado: 'activos' };

const CardUserManager = ({
  usuarios, estadisticas, loadingStats, error, clearError,
  crearUsuario, actualizarUsuario, reactivarUsuario, cambiarPassword, toggleEstado,
  currentUserId,
}) => {
  const [filtros, setFiltros] = useState(FILTROS_INICIALES);
  const [dialog, setDialog] = useState(null); // null | 'form' | 'password'
  const [usuarioSeleccionado, setUsuarioSeleccionado] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

  const mostrarSnack = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleNuevoUsuario = () => {
    setUsuarioSeleccionado(null);
    setDialog('form');
  };

  const handleEditar = (usuario) => {
    setUsuarioSeleccionado(usuario);
    setDialog('form');
  };

  const handleCambiarPassword = (usuario) => {
    setUsuarioSeleccionado(usuario);
    setDialog('password');
  };

  const handleToggleEstado = async (usuario) => {
    const result = await toggleEstado(usuario.id);
    if (result?.success) {
      const nuevoEstado = result.user?.estadoActivo;
      mostrarSnack(`Usuario ${nuevoEstado ? 'activado' : 'desactivado'} correctamente`);
    } else {
      mostrarSnack(result?.message || 'Error al cambiar el estado', 'error');
    }
  };

  const handleReactivar = async (usuario) => {
    const result = await reactivarUsuario(usuario.id);
    if (result?.success) {
      mostrarSnack('Usuario reactivado correctamente');
    } else {
      mostrarSnack(result?.message || 'Error al reactivar el usuario', 'error');
    }
  };

  const handleGuardarUsuario = async (id, dto) => {
    if (id) {
      const result = await actualizarUsuario(id, dto);
      if (result?.success) mostrarSnack('Usuario actualizado correctamente');
      return result;
    } else {
      const result = await crearUsuario(dto);
      if (result?.success) mostrarSnack('Usuario creado correctamente');
      return result;
    }
  };

  const handleGuardarPassword = async (id, dto) => {
    const result = await cambiarPassword(id, dto);
    if (result?.success) mostrarSnack('Contraseña cambiada correctamente');
    return result;
  };

  const handleClearFiltros = () => setFiltros(FILTROS_INICIALES);

  return (
    <Box>
      {error && (
        <Alert severity="error" onClose={clearError} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <UserStatsCards estadisticas={estadisticas} loading={loadingStats} />

      <UserFilters
        filtros={filtros}
        onChange={setFiltros}
        onClear={handleClearFiltros}
        onNuevoUsuario={handleNuevoUsuario}
      />

      <UserTable
        usuarios={usuarios}
        filtros={filtros}
        onEditar={handleEditar}
        onCambiarPassword={handleCambiarPassword}
        onToggleEstado={handleToggleEstado}
        onReactivar={handleReactivar}
        currentUserId={currentUserId}
      />

      <UserFormDialog
        open={dialog === 'form'}
        usuario={usuarioSeleccionado}
        onClose={() => setDialog(null)}
        onGuardar={handleGuardarUsuario}
      />

      <ChangePasswordDialog
        open={dialog === 'password'}
        usuario={usuarioSeleccionado}
        onClose={() => setDialog(null)}
        onGuardar={handleGuardarPassword}
      />

      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar(s => ({ ...s, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert severity={snackbar.severity} onClose={() => setSnackbar(s => ({ ...s, open: false }))}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default CardUserManager;
