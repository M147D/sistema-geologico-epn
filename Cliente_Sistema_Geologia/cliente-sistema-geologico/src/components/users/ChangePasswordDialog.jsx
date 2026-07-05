// src/components/users/ChangePasswordDialog.jsx
import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, Alert, CircularProgress, Typography,
} from '@mui/material';

const ChangePasswordDialog = ({ open, usuario, onClose, onGuardar }) => {
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);
  const [apiError, setApiError] = useState('');

  useEffect(() => {
    if (open) {
      setNewPassword('');
      setConfirmPassword('');
      setErrors({});
      setApiError('');
    }
  }, [open]);

  const validate = () => {
    const errs = {};
    if (!newPassword.trim()) errs.newPassword = 'La contraseña es obligatoria';
    else if (newPassword.length < 6) errs.newPassword = 'Mínimo 6 caracteres';
    if (newPassword !== confirmPassword) errs.confirmPassword = 'Las contraseñas no coinciden';
    return errs;
  };

  const handleGuardar = async () => {
    const errs = validate();
    if (Object.keys(errs).length > 0) { setErrors(errs); return; }

    setSaving(true);
    setApiError('');
    try {
      const result = await onGuardar(usuario.id, { newPassword });
      if (result?.success) {
        onClose();
      } else {
        setApiError(result?.message || 'Error al cambiar la contraseña');
      }
    } catch (err) {
      setApiError(err.response?.data?.message || 'Error inesperado');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>Cambiar contraseña</DialogTitle>
      <DialogContent dividers>
        {usuario && (
          <Typography variant="body2" color="text.secondary" mb={2}>
            Usuario: <strong>{usuario.nombreCompleto || usuario.email}</strong>
          </Typography>
        )}
        {apiError && <Alert severity="error" sx={{ mb: 2 }}>{apiError}</Alert>}
        <TextField
          fullWidth size="small" label="Nueva contraseña" type="password" required
          value={newPassword} onChange={(e) => { setNewPassword(e.target.value); setErrors({}); }}
          error={Boolean(errors.newPassword)} helperText={errors.newPassword}
          sx={{ mb: 2 }}
        />
        <TextField
          fullWidth size="small" label="Confirmar contraseña" type="password" required
          value={confirmPassword} onChange={(e) => { setConfirmPassword(e.target.value); setErrors({}); }}
          error={Boolean(errors.confirmPassword)} helperText={errors.confirmPassword}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={saving}>Cancelar</Button>
        <Button
          onClick={handleGuardar}
          variant="contained"
          color="warning"
          disabled={saving}
          startIcon={saving ? <CircularProgress size={16} /> : null}
        >
          {saving ? 'Cambiando...' : 'Cambiar contraseña'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default ChangePasswordDialog;
