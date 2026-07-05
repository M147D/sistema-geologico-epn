// src/components/excel/ExcelDataPreview.jsx
import { useMemo } from 'react';
import { Box, Typography, Button, Alert, CircularProgress, Chip } from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import TableElement from '../crud/TableElement.jsx';
import { SUBTIPOS_FOSIL, SUBTIPOS_MINERAL, SUBTIPOS_ROCA } from '../../constants/tipoMaps';

const TIPO_NORMALIZED = { fosil: 'Fosil', mineral: 'Mineral', roca: 'Roca' };

const SUBTIPOS_BY_TIPO = { Fosil: SUBTIPOS_FOSIL, Mineral: SUBTIPOS_MINERAL, Roca: SUBTIPOS_ROCA };

const normalizeStr = (s) =>
  String(s).toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '');

const resolveSubtipoLabel = (rawValue, subtipos) => {
  if (rawValue === null || rawValue === undefined || rawValue === '') return null;
  const num = typeof rawValue === 'number' ? rawValue : parseInt(rawValue, 10);
  if (!isNaN(num)) return subtipos.find(s => s.value === num)?.label ?? null;
  const exact = subtipos.find(s => s.label === rawValue);
  if (exact) return exact.label;
  const normRaw = normalizeStr(rawValue);
  return subtipos.find(s => normalizeStr(s.label) === normRaw)?.label ?? String(rawValue);
};

const ExcelDataPreview = ({ data, tipoElemento, loading, processingStatus, onSaveToDatabase }) => {
  const tipoCapitalized = TIPO_NORMALIZED[tipoElemento] ?? 'Roca';

  const mappedData = useMemo(() => {
    const subtipos = SUBTIPOS_BY_TIPO[tipoCapitalized] ?? [];
    return data.map(row => ({
      ...row,
      tipoElemento: tipoCapitalized,
      tipoEspecifico: resolveSubtipoLabel(row[`tipo${tipoCapitalized}`], subtipos),
    }));
  }, [data, tipoCapitalized]);

  if (data.length === 0) return null;

  return (
    <Box sx={{ mt: 3 }}>
      {/* Header de sección */}
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Box sx={{ width: 4, height: 20, bgcolor: 'warning.main', borderRadius: 1, flexShrink: 0 }} />
          <Typography variant="subtitle1" fontWeight={600}>Vista previa</Typography>
          <Chip label={`${data.length} registros`} size="small" color="warning" variant="outlined" />
        </Box>
        <Button
          variant="contained"
          color="primary"
          startIcon={loading ? <CircularProgress size={18} color="inherit" /> : <SaveIcon />}
          onClick={onSaveToDatabase}
          disabled={loading}
          size="small"
        >
          {loading ? 'Guardando...' : 'Guardar en Base de Datos'}
        </Button>
      </Box>

      {processingStatus && (
        <Alert severity="info" sx={{ mb: 2 }}>{processingStatus}</Alert>
      )}

      <TableElement elementos={mappedData} showTypeFilter={false} />
    </Box>
  );
};

export default ExcelDataPreview;
