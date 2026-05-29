import React from 'react';
import {
  Box, Typography, Paper, TextField, Chip, Switch, FormControlLabel,
  Table, TableBody, TableCell, TableContainer, TableRow, Button
} from '@mui/material';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import DescriptionIcon from '@mui/icons-material/Description';
import ScienceIcon from '@mui/icons-material/Science';
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts';
import { InfoRow } from './DetailHelpers.jsx';

const DetailInfoTemporal = ({ elemento, isEditing, editForm, onEditChange, onSolicitarInforme, isAdmin }) => {
  return (
    <Box sx={{ width: '100%' }}>

      {/* Informacion Temporal */}
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

      {/* Documentacion */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <Box sx={{ width: 4, height: 20, bgcolor: 'info.main', borderRadius: 1, flexShrink: 0 }} />
        <DescriptionIcon color="info" fontSize="small" />
        <Typography variant="subtitle1" fontWeight={600}>Documentación</Typography>
      </Box>

      <TableContainer component={Paper} variant="outlined" sx={{ mb: 3 }}>
        <Table size="small">
          <TableBody>
            <InfoRow
              label="Documentos relacionados"
              value={
                elemento.documentosRelacionados
                  ? <Typography variant="body2">{elemento.documentosRelacionados}</Typography>
                  : null
              }
              isEditing={isEditing}
              editField={
                <TextField
                  value={editForm.documentosRelacionados || ''}
                  onChange={e => onEditChange('documentosRelacionados', e.target.value)}
                  size="small"
                  fullWidth
                  multiline
                  rows={3}
                />
              }
            />
            <InfoRow
              label="Lámina existente"
              value={
                <Chip
                  label={elemento.laminaExiste ? 'Sí' : 'No'}
                  size="small"
                  color={elemento.laminaExiste ? 'success' : 'default'}
                />
              }
              isEditing={isEditing}
              editField={
                <FormControlLabel
                  control={
                    <Switch
                      checked={editForm.laminaExiste || false}
                      onChange={e => onEditChange('laminaExiste', e.target.checked)}
                      size="small"
                    />
                  }
                  label={editForm.laminaExiste ? 'Sí' : 'No'}
                />
              }
            />
          </TableBody>
        </Table>
      </TableContainer>

      {/* Auditoría — solo Admin */}
      {isAdmin && (
        <>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
            <Box sx={{ width: 4, height: 20, bgcolor: 'error.main', borderRadius: 1, flexShrink: 0 }} />
            <ManageAccountsIcon color="error" fontSize="small" />
            <Typography variant="subtitle1" fontWeight={600}>Auditoría</Typography>
          </Box>

          <TableContainer component={Paper} variant="outlined" sx={{ mb: 3 }}>
            <Table size="small">
              <TableBody>
                <InfoRow
                  label="Creado por"
                  value={
                    elemento.creadoPor
                      ? `${elemento.creadoPor}${elemento.fechaCreacion ? ` — ${new Date(elemento.fechaCreacion).toLocaleDateString('es-EC')}` : ''}`
                      : null
                  }
                  isEditing={false}
                />
                {elemento.fechaActualizacion && (
                  <InfoRow
                    label="Actualizado por"
                    value={
                      elemento.actualizadoPor
                        ? `${elemento.actualizadoPor} — ${new Date(elemento.fechaActualizacion).toLocaleDateString('es-EC')}`
                        : new Date(elemento.fechaActualizacion).toLocaleDateString('es-EC')
                    }
                    isEditing={false}
                  />
                )}
                {!elemento.estadoActivo && elemento.fechaEliminacion && (
                  <InfoRow
                    label="Eliminado por"
                    value={
                      elemento.eliminadoPor
                        ? `${elemento.eliminadoPor} — ${new Date(elemento.fechaEliminacion).toLocaleDateString('es-EC')}`
                        : new Date(elemento.fechaEliminacion).toLocaleDateString('es-EC')
                    }
                    isEditing={false}
                  />
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </>
      )}

      {/* Botón solicitar informe */}
      {onSolicitarInforme && (
        <Button
          variant="contained"
          fullWidth
          size="large"
          startIcon={<ScienceIcon />}
          onClick={onSolicitarInforme}
          sx={{
            mt: 1,
            py: 1.5,
            fontWeight: 700,
            letterSpacing: 0.5,
            bgcolor: 'secondary.dark',
            '&:hover': { bgcolor: 'secondary.main' },
            boxShadow: 3,
          }}
        >
          Solicitar Informe Petrográfico
        </Button>
      )}

    </Box>
  );
};

export default DetailInfoTemporal;
