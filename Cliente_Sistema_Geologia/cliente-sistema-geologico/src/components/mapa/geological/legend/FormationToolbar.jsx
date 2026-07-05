import { Box, Button } from '@mui/material';
import CheckBoxIcon from '@mui/icons-material/CheckBox';
import CheckBoxOutlineBlankIcon from '@mui/icons-material/CheckBoxOutlineBlank';

/**
 * Barra de herramientas para selección masiva de formaciones
 * @param {boolean} isAllSelected - Si todas las formaciones están seleccionadas
 * @param {boolean} isNoneSelected - Si ninguna formación está seleccionada
 * @param {Function} onSelectAll - Callback para seleccionar todas
 * @param {Function} onDeselectAll - Callback para deseleccionar todas
 */
const FormationToolbar = ({
  isAllSelected,
  isNoneSelected,
  onSelectAll,
  onDeselectAll
}) => {
  return (
    <Box
      sx={{
        display: 'flex',
        gap: 1,
        p: 1,
        bgcolor: '#f8f9fa',
        borderBottom: '1px solid rgba(0,0,0,0.08)',
        justifyContent: 'space-evenly'
      }}
    >
      {/* Botón Seleccionar Todas */}
      <Button
        size="small"
        startIcon={
          isAllSelected
            ? <CheckBoxIcon fontSize="small" />
            : <CheckBoxOutlineBlankIcon fontSize="small" />
        }
        onClick={onSelectAll}
        sx={{
          fontSize: '0.7rem',
          textTransform: 'none',
          py: 0.25,
          px: 1.5,
          color: isAllSelected ? 'primary.main' : 'text.secondary',
          fontWeight: isAllSelected ? 'bold' : 'normal',
          transition: 'all 0.2s',
          '&:hover': {
            bgcolor: 'action.hover',
            color: 'primary.main'
          }
        }}
      >
        Todas
      </Button>

      {/* Botón Deseleccionar Todas */}
      <Button
        size="small"
        startIcon={
          isNoneSelected
            ? <CheckBoxIcon fontSize="small" />
            : <CheckBoxOutlineBlankIcon fontSize="small" />
        }
        onClick={onDeselectAll}
        sx={{
          fontSize: '0.7rem',
          textTransform: 'none',
          py: 0.25,
          px: 1.5,
          color: isNoneSelected ? 'primary.main' : 'text.secondary',
          fontWeight: isNoneSelected ? 'bold' : 'normal',
          transition: 'all 0.2s',
          '&:hover': {
            bgcolor: 'action.hover',
            color: 'primary.main'
          }
        }}
      >
        Ninguna
      </Button>
    </Box>
  );
};

export default FormationToolbar;
