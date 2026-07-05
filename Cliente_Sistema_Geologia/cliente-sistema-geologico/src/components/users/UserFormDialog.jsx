// src/components/users/UserFormDialog.jsx
import { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, Select, MenuItem, FormControl, InputLabel,
  FormControlLabel, Switch, Grid, Alert, CircularProgress,
} from '@mui/material';

const ROL_OPTIONS = [
  { value: 0, label: 'Free' },
  { value: 1, label: 'Premium' },
  { value: 2, label: 'Admin' },
  { value: 3, label: 'Invitado' },
];

const EMPTY_FORM = {
  nombreCompleto: '',
  userName: '',
  email: '',
  password: '',
  rol: 0,
  estadoActivo: true,
  emailConfirmed: false,
};

const UserFormDialog = ({ open, usuario, onClose, onGuardar }) => {
  const esEdicion = Boolean(usuario);
  const [form, setForm] = useState(EMPTY_FORM);
  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);
  const [apiError, setApiError] = useState('');

  useEffect(() => {
    if (open) {
      if (usuario) {
        setForm({
          nombreCompleto: usuario.nombreCompleto || '',
          userName: usuario.userName || '',
          email: usuario.email || '',
          password: '',
          rol: usuario.rol ?? 0,
          estadoActivo: usuario.estadoActivo ?? true,
          emailConfirmed: usuario.emailConfirmed ?? false,
        });
      } else {
        setForm(EMPTY_FORM);
      }
      setErrors({});
      setApiError('');
    }
  }, [open, usuario]);

  const set = (field) => (e) => {
    const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value;
    setForm(prev => ({ ...prev, [field]: value }));
    if (errors[field]) setErrors(prev => ({ ...prev, [field]: '' }));
  };

  const validate = () => {
    const errs = {};
    if (!form.email.trim()) errs.email = 'El email es obligatorio';
    else if (!/\S+@\S+\.\S+/.test(form.email)) errs.email = 'Email inválido';
    if (!form.userName.trim()) errs.userName = 'El nombre de usuario es obligatorio';
    else if (form.userName.length < 3) errs.userName = 'Mínimo 3 caracteres';
    if (!esEdicion) {
      if (!form.password.trim()) errs.password = 'La contraseña es obligatoria';
      else if (form.password.length < 6) errs.password = 'Mínimo 6 caracteres';
      else if (!/[A-Z]/.test(form.password)) errs.password = 'Debe contener al menos una mayúscula';
      else if (!/[a-z]/.test(form.password)) errs.password = 'Debe contener al menos una minúscula';
      else if (!/[0-9]/.test(form.password)) errs.password = 'Debe contener al menos un número';
    }
    return errs;
  };

  const handleGuardar = async () => {
    const errs = validate();
    if (Object.keys(errs).length > 0) { setErrors(errs); return; }

    setSaving(true);
    setApiError('');

    try {
      const dto = esEdicion
        ? {
            nombreCompleto: form.nombreCompleto,
            userName: form.userName,
            email: form.email,
            rol: Number(form.rol),
            estadoActivo: form.estadoActivo,
            emailConfirmed: form.emailConfirmed,
          }
        : {
            nombreCompleto: form.nombreCompleto,
            userName: form.userName,
            email: form.email,
            password: form.password,
            rol: Number(form.rol),
            estadoActivo: form.estadoActivo,
          };

      const result = await onGuardar(usuario?.id, dto);
      if (result?.success) {
        onClose();
      } else {
        setApiError(result?.message || 'Error al guardar el usuario');
      }
    } catch (err) {
      const data = err.response?.data;
      const serverErrors = data?.errors;
      if (serverErrors?.length > 0) {
        setApiError(serverErrors.join(' '));
      } else {
        setApiError(data?.message || 'Error inesperado al guardar');
      }
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{esEdicion ? 'Editar usuario' : 'Nuevo usuario'}</DialogTitle>
      <DialogContent dividers>
        {apiError && <Alert severity="error" sx={{ mb: 2 }}>{apiError}</Alert>}
        <Grid container spacing={2} mt={0}>
          <Grid item xs={12}>
            <TextField
              fullWidth size="small" label="Nombre completo"
              value={form.nombreCompleto} onChange={set('nombreCompleto')}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth size="small" label="Username" required
              value={form.userName} onChange={set('userName')}
              error={Boolean(errors.userName)} helperText={errors.userName}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth size="small" label="Email" required type="email"
              value={form.email} onChange={set('email')}
              error={Boolean(errors.email)} helperText={errors.email}
            />
          </Grid>
          {!esEdicion && (
            <Grid item xs={12}>
              <TextField
                fullWidth size="small" label="Contraseña" required type="password"
                value={form.password} onChange={set('password')}
                error={Boolean(errors.password)}
                helperText={errors.password || 'Mínimo 6 caracteres con mayúscula, minúscula y número'}
              />
            </Grid>
          )}
          <Grid item xs={12} sm={6}>
            <FormControl fullWidth size="small">
              <InputLabel>Rol</InputLabel>
              <Select value={form.rol} label="Rol" onChange={set('rol')}>
                {ROL_OPTIONS.map(o => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={6}>
            <FormControlLabel
              control={<Switch checked={form.estadoActivo} onChange={set('estadoActivo')} />}
              label="Usuario activo"
            />
          </Grid>
          {esEdicion && (
            <Grid item xs={12}>
              <FormControlLabel
                control={<Switch checked={form.emailConfirmed} onChange={set('emailConfirmed')} />}
                label="Email confirmado"
              />
            </Grid>
          )}
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={saving}>Cancelar</Button>
        <Button
          onClick={handleGuardar}
          variant="contained"
          disabled={saving}
          startIcon={saving ? <CircularProgress size={16} /> : null}
        >
          {saving ? 'Guardando...' : esEdicion ? 'Guardar cambios' : 'Crear usuario'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default UserFormDialog;
