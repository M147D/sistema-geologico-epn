// src/components/users/UserFilters.jsx
import {
  Box, TextField, Select, MenuItem, FormControl, InputLabel,
  Button, Stack, InputAdornment, IconButton,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import PersonAddIcon from '@mui/icons-material/PersonAdd';

const ROL_OPTIONS = [
  { value: '', label: 'Todos los roles' },
  { value: '0', label: 'Free' },
  { value: '1', label: 'Premium' },
  { value: '2', label: 'Admin' },
  { value: '3', label: 'Invitado' },
];

const ESTADO_OPTIONS = [
  { value: '', label: 'Todos' },
  { value: 'activos', label: 'Activos' },
  { value: 'inactivos', label: 'Inactivos' },
];

const UserFilters = ({ filtros, onChange, onClear, onNuevoUsuario }) => {
  const handleChange = (field) => (e) => {
    onChange({ ...filtros, [field]: e.target.value });
  };

  const tieneFiltros = filtros.busqueda || filtros.rol || filtros.estado;

  return (
    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} alignItems="center" mb={2} flexWrap="wrap">
      <TextField
        size="small"
        placeholder="Buscar por nombre o email"
        value={filtros.busqueda || ''}
        onChange={handleChange('busqueda')}
        sx={{ minWidth: 240 }}
        InputProps={{
          startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment>,
          endAdornment: filtros.busqueda
            ? <InputAdornment position="end">
                <IconButton size="small" onClick={() => onChange({ ...filtros, busqueda: '' })}>
                  <ClearIcon fontSize="small" />
                </IconButton>
              </InputAdornment>
            : null,
        }}
      />

      <FormControl size="small" sx={{ minWidth: 140 }}>
        <InputLabel>Rol</InputLabel>
        <Select value={filtros.rol || ''} label="Rol" onChange={handleChange('rol')}>
          {ROL_OPTIONS.map(o => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
        </Select>
      </FormControl>

      <FormControl size="small" sx={{ minWidth: 130 }}>
        <InputLabel>Estado</InputLabel>
        <Select value={filtros.estado || ''} label="Estado" onChange={handleChange('estado')}>
          {ESTADO_OPTIONS.map(o => <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>)}
        </Select>
      </FormControl>

      {tieneFiltros && (
        <Button size="small" variant="text" startIcon={<ClearIcon />} onClick={onClear}>
          Limpiar
        </Button>
      )}

      <Box sx={{ flexGrow: 1 }} />

      <Button
        variant="contained"
        startIcon={<PersonAddIcon />}
        onClick={onNuevoUsuario}
        size="small"
      >
        Nuevo usuario
      </Button>
    </Stack>
  );
};

export default UserFilters;
