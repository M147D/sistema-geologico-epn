import { Button, CircularProgress, Box } from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';

const LABELS = { fosil: 'Fósil', mineral: 'Mineral', roca: 'Roca' };

const SubmitButton = ({ loading, tipo }) => (
  <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
    <Button
      type="submit"
      variant="contained"
      color="primary"
      disabled={loading}
      size="large"
      startIcon={loading ? <CircularProgress size={20} color="inherit" /> : <SaveIcon />}
      sx={{ minWidth: 200, py: 1.25 }}
    >
      {loading ? 'Guardando...' : `Guardar ${LABELS[tipo] ?? tipo}`}
    </Button>
  </Box>
);

export default SubmitButton;
