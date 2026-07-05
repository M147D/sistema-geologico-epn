import {
  Box, Typography, Paper, TextField,
  Table, TableBody, TableContainer
} from '@mui/material';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import { InfoRow } from './DetailHelpers.jsx';

const DetailInfoTemporal = ({ elemento, isEditing, editForm, onEditChange }) => {
  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <Box sx={{ width: 4, height: 20, bgcolor: 'warning.main', borderRadius: 1, flexShrink: 0 }} />
        <CalendarTodayIcon color="warning" fontSize="small" />
        <Typography variant="subtitle1" fontWeight={600}>Información Temporal</Typography>
      </Box>

      <TableContainer component={Paper} variant="outlined" sx={{ mb: 3 }}>
        <Table size="small">
          <TableBody>
            <InfoRow
              label="Fecha de ingreso"
              value={elemento.fechaIngreso
                ? new Date(elemento.fechaIngreso).toLocaleDateString('es-EC')
                : null}
              isEditing={isEditing}
              editField={
                <TextField
                  type="date"
                  value={editForm.fechaIngreso || ''}
                  onChange={e => onEditChange('fechaIngreso', e.target.value)}
                  size="small"
                  InputLabelProps={{ shrink: true }}
                />
              }
            />
            {elemento.fechaCreacion && (
              <InfoRow
                label="Fecha de creación"
                value={new Date(elemento.fechaCreacion).toLocaleDateString('es-EC')}
                isEditing={false}
              />
            )}
            {elemento.fechaActualizacion && (
              <InfoRow
                label="Última actualización"
                value={new Date(elemento.fechaActualizacion).toLocaleDateString('es-EC')}
                isEditing={false}
              />
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};

export default DetailInfoTemporal;
