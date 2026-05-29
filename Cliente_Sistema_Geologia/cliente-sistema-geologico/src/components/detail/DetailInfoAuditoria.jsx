import React from 'react';
import { Box, Typography, Paper, Table, TableBody, TableContainer } from '@mui/material';
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts';
import { InfoRow } from './DetailHelpers.jsx';

const DetailInfoAuditoria = ({ elemento }) => {
  return (
    <Box sx={{ width: '100%' }}>
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
    </Box>
  );
};

export default DetailInfoAuditoria;
