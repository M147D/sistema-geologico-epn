import {
  Typography, Box, Chip, Button, Stack, Alert, CircularProgress
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import SaveIcon from '@mui/icons-material/Save';
import CancelIcon from '@mui/icons-material/Cancel';
import ScienceIcon from '@mui/icons-material/Science';
import BiotechIcon from '@mui/icons-material/Biotech';
import DiamondIcon from '@mui/icons-material/Diamond';
import LandscapeIcon from '@mui/icons-material/Landscape';
import { getTypeColor, getTypeLabel } from '../../utils/detailUtils.js';
import { getSubtipoColor, getSubtipoDisplayLabel } from '../../constants/subtipoColors';

const TYPE_CONFIG = {
  Fosil:   { Icon: BiotechIcon,   accentColor: 'secondary.main', color: 'secondary' },
  Mineral: { Icon: DiamondIcon,   accentColor: 'success.main',   color: 'success'   },
  Roca:    { Icon: LandscapeIcon, accentColor: 'info.main',      color: 'info'      },
};

const DetailHeader = ({
  elemento,
  subtipo,
  canEdit,
  isEditing,
  saving,
  saveError,
  onStartEdit,
  onSave,
  onCancelEdit,
  onClearSaveError,
  onSolicitarInforme,
}) => {
  const tipoElemento = elemento.tipoElemento;
  const { Icon, accentColor, color } = TYPE_CONFIG[tipoElemento] || { Icon: null, accentColor: 'primary.main', color: 'primary' };
  const subtipoHex = getSubtipoColor(tipoElemento, subtipo);

  return (
    <Box sx={{ px: 3, py: 2.5, borderBottom: 1, borderColor: 'divider' }}>
      {/* Title row */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 1 }}>
        <Box sx={{ width: 4, height: 28, bgcolor: accentColor, borderRadius: 1, flexShrink: 0 }} />
        {Icon && <Icon color={color} />}
        <Typography variant="h5" fontWeight={600} sx={{ flex: 1 }}>
          {elemento.nombre}
        </Typography>
        {onSolicitarInforme && !isEditing && (
          <Button
            variant="contained"
            size="small"
            startIcon={<ScienceIcon />}
            onClick={onSolicitarInforme}
            sx={{ bgcolor: 'secondary.dark', '&:hover': { bgcolor: 'secondary.main' }, fontWeight: 600 }}
          >
            Solicitar Informe
          </Button>
        )}
        {canEdit && !isEditing && (
          <Button variant="outlined" size="small" startIcon={<EditIcon />} onClick={onStartEdit}>
            Editar
          </Button>
        )}
      </Box>

      {/* Chips row */}
      <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', pl: 3.5 }}>
        <Chip label={`Código: ${elemento.codigo}`} size="small" color="primary" />
        <Chip label={getTypeLabel(tipoElemento)} size="small" color={getTypeColor(tipoElemento)} />
        {subtipo && (
          <Chip
            label={getSubtipoDisplayLabel(subtipo)}
            size="small"
            variant="outlined"
            sx={{ borderColor: subtipoHex, color: subtipoHex, fontWeight: 500 }}
          />
        )}
        {elemento.estadoActivo !== undefined && (
          <Chip
            label={elemento.estadoActivo ? 'Activo' : 'Inactivo'}
            size="small"
            color={elemento.estadoActivo ? 'success' : 'default'}
          />
        )}
      </Box>

      {/* Edit mode controls */}
      {isEditing && (
        <Box sx={{ mt: 2 }}>
          <Alert severity="info" sx={{ mb: 1.5 }}>
            Modo edición activo. Modifique los campos y guarde los cambios.
          </Alert>
          {saveError && (
            <Alert severity="error" sx={{ mb: 1.5 }} onClose={onClearSaveError}>
              {saveError}
            </Alert>
          )}
          <Stack direction="row" spacing={2}>
            <Button
              variant="contained"
              startIcon={saving ? <CircularProgress size={16} color="inherit" /> : <SaveIcon />}
              onClick={onSave}
              disabled={saving}
            >
              {saving ? 'Guardando...' : 'Guardar cambios'}
            </Button>
            <Button
              variant="outlined"
              startIcon={<CancelIcon />}
              onClick={onCancelEdit}
              disabled={saving}
            >
              Cancelar
            </Button>
          </Stack>
        </Box>
      )}
    </Box>
  );
};

export default DetailHeader;
