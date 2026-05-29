import React from 'react';
import {
  Box, Typography, Paper, TextField, Chip, Switch, FormControlLabel,
  Table, TableBody, TableContainer, Button
} from '@mui/material';
import DescriptionIcon from '@mui/icons-material/Description';
import ScienceIcon from '@mui/icons-material/Science';
import { InfoRow } from './DetailHelpers.jsx';

const DetailInfoDocumentacion = ({ elemento, isEditing, editForm, onEditChange, onSolicitarInforme }) => {
  return (
    <Box sx={{ width: '100%' }}>
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

export default DetailInfoDocumentacion;
