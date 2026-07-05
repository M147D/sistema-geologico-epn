import {
  Box, Typography, Paper, Table, TableBody, TableContainer,
  TextField, FormControl, Select, MenuItem, Chip
} from '@mui/material';
import InfoIcon from '@mui/icons-material/Info';
import { getTypeColor, getTypeLabel, getSubtipos, getSubtipoLabel } from '../../utils/detailUtils.js';
import { InfoRow } from './DetailHelpers.jsx';

const DetailInfoGeneral = ({ elemento, isEditing, editForm, onEditChange }) => {
  const tipoElemento = elemento.tipoElemento;

  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <Box sx={{ width: 4, height: 20, bgcolor: 'primary.main', borderRadius: 1, flexShrink: 0 }} />
        <InfoIcon color="primary" fontSize="small" />
        <Typography variant="subtitle1" fontWeight={600}>Información General</Typography>
      </Box>

      <TableContainer component={Paper} variant="outlined" sx={{ mb: 3 }}>
        <Table size="small">
          <TableBody>
            <InfoRow
              label="Nombre"
              value={elemento.nombre}
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.nombre || ''}
                  onChange={e => onEditChange('nombre', e.target.value)}
                  size="small"
                  fullWidth
                />
              }
            />
            <InfoRow
              label="Codigo"
              value={elemento.codigo}
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.codigo || ''}
                  onChange={e => onEditChange('codigo', e.target.value)}
                  size="small"
                  fullWidth
                />
              }
            />
            <InfoRow
              label="Tipo"
              value={
                <Chip label={getTypeLabel(tipoElemento)} size="small" color={getTypeColor(tipoElemento)} />
              }
              isEditing={false}
            />

            {/* Subtipo por tipo */}
            {tipoElemento === 'Fosil' && (
              <InfoRow
                label="Subtipo"
                value={getSubtipoLabel('Fosil', elemento.tipoFosil)}
                isEditing={isEditing}
                editField={
                  <FormControl size="small" fullWidth>
                    <Select
                      value={editForm.tipoFosil ?? 0}
                      onChange={e => onEditChange('tipoFosil', e.target.value)}
                    >
                      {getSubtipos('Fosil').map(s => (
                        <MenuItem key={s.value} value={s.value}>{s.label}</MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                }
              />
            )}
            {tipoElemento === 'Mineral' && (
              <InfoRow
                label="Subtipo"
                value={getSubtipoLabel('Mineral', elemento.tipoMineral)}
                isEditing={isEditing}
                editField={
                  <FormControl size="small" fullWidth>
                    <Select
                      value={editForm.tipoMineral ?? 0}
                      onChange={e => onEditChange('tipoMineral', e.target.value)}
                    >
                      {getSubtipos('Mineral').map(s => (
                        <MenuItem key={s.value} value={s.value}>{s.label}</MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                }
              />
            )}
            {tipoElemento === 'Roca' && (
              <InfoRow
                label="Subtipo"
                value={getSubtipoLabel('Roca', elemento.tipoRoca)}
                isEditing={isEditing}
                editField={
                  <FormControl size="small" fullWidth>
                    <Select
                      value={editForm.tipoRoca ?? 0}
                      onChange={e => onEditChange('tipoRoca', e.target.value)}
                    >
                      {getSubtipos('Roca').map(s => (
                        <MenuItem key={s.value} value={s.value}>{s.label}</MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                }
              />
            )}

            {/* Campos especificos por tipo */}
            {tipoElemento === 'Fosil' && (
              <>
                <InfoRow
                  label="Especie"
                  value={elemento.especie}
                  isEditing={isEditing}
                  editField={
                    <TextField
                      value={editForm.especie || ''}
                      onChange={e => onEditChange('especie', e.target.value)}
                      size="small"
                      fullWidth
                    />
                  }
                />
                <InfoRow
                  label="Periodo"
                  value={elemento.periodo}
                  isEditing={isEditing}
                  editField={
                    <TextField
                      value={editForm.periodo || ''}
                      onChange={e => onEditChange('periodo', e.target.value)}
                      size="small"
                      fullWidth
                    />
                  }
                />
              </>
            )}
            {tipoElemento === 'Mineral' && (
              <InfoRow
                label="Litologia"
                value={elemento.litologiaMineral}
                isEditing={isEditing}
                editField={
                  <TextField
                    value={editForm.litologiaMineral || ''}
                    onChange={e => onEditChange('litologiaMineral', e.target.value)}
                    size="small"
                    fullWidth
                  />
                }
              />
            )}
            {tipoElemento === 'Roca' && (
              <InfoRow
                label="Litologia"
                value={elemento.litologiaRoca}
                isEditing={isEditing}
                editField={
                  <TextField
                    value={editForm.litologiaRoca || ''}
                    onChange={e => onEditChange('litologiaRoca', e.target.value)}
                    size="small"
                    fullWidth
                  />
                }
              />
            )}

            {/* Campos comunes */}
            <InfoRow
              label="Edad"
              value={elemento.edad}
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.edad || ''}
                  onChange={e => onEditChange('edad', e.target.value)}
                  size="small"
                  fullWidth
                />
              }
            />
            <InfoRow
              label="Donante"
              value={elemento.donante}
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.donante || ''}
                  onChange={e => onEditChange('donante', e.target.value)}
                  size="small"
                  fullWidth
                />
              }
            />
            <InfoRow
              label="Ejemplares"
              value={elemento.ejemplares}
              isEditing={isEditing}
              editField={
                <TextField
                  type="number"
                  value={editForm.ejemplares || 1}
                  onChange={e => onEditChange('ejemplares', e.target.value)}
                  size="small"
                  fullWidth
                  inputProps={{ min: 1, max: 10000 }}
                />
              }
            />
          </TableBody>
        </Table>
      </TableContainer>

    </Box>
  );
};

export default DetailInfoGeneral;
