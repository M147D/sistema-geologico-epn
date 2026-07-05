import {
  Box, Typography, Paper, Table, TableBody, TableContainer, TextField
} from '@mui/material';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import { InfoRow } from './DetailHelpers.jsx';

const DetailInfoUbicacion = ({ elemento, isEditing, editForm, onEditChange }) => {
  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <Box sx={{ width: 4, height: 20, bgcolor: 'success.main', borderRadius: 1, flexShrink: 0 }} />
        <LocationOnIcon color="success" fontSize="small" />
        <Typography variant="subtitle1" fontWeight={600}>Ubicación</Typography>
      </Box>

      <TableContainer component={Paper} variant="outlined">
        <Table size="small">
          <TableBody>
            <InfoRow
              labelWidth="18%"
              label="Localidad"
              value={elemento.ubicacion?.localidad || elemento.localidad}
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.localidad || ''}
                  onChange={e => onEditChange('localidad', e.target.value)}
                  size="small"
                  fullWidth
                />
              }
            />
            <InfoRow
              labelWidth="18%"
              label="Provincia"
              value={elemento.ubicacion?.nombreProvincia || elemento.nombreProvincia}
              isEditing={false}
            />
            <InfoRow
              labelWidth="18%"
              label="País"
              value={elemento.ubicacion?.nombrePais || elemento.nombrePais}
              isEditing={false}
            />
            <InfoRow
              labelWidth="18%"
              label="Latitud"
              value={elemento.ubicacion?.latitud || elemento.latitud}
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.latitud || ''}
                  onChange={e => onEditChange('latitud', e.target.value)}
                  size="small"
                  fullWidth
                />
              }
            />
            <InfoRow
              labelWidth="18%"
              label="Longitud"
              value={elemento.ubicacion?.longitud || elemento.longitud}
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.longitud || ''}
                  onChange={e => onEditChange('longitud', e.target.value)}
                  size="small"
                  fullWidth
                />
              }
            />
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};

export default DetailInfoUbicacion;
