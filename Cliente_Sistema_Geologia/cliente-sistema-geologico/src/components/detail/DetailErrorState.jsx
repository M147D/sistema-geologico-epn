import { Link } from 'react-router-dom';
import { Paper, Alert, Stack, Button } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

const DetailErrorState = ({ id, onGoBack }) => {
  return (
    <Paper
      elevation={2}
      sx={{
        p: 4, maxWidth: 600, mx: 'auto', mt: 4,
        textAlign: 'center', overflow: 'hidden',
        borderTop: 3, borderColor: 'error.main',
      }}
    >
      <Alert severity="error" sx={{ mb: 2 }}>
        {id ? `No se encontró el elemento con ID: ${id}` : 'No se proporcionó ID del elemento'}
      </Alert>
      <Stack direction="row" spacing={2} justifyContent="center">
        <Button variant="contained" startIcon={<ArrowBackIcon />} onClick={onGoBack}>Volver</Button>
        <Button component={Link} to="/listar-elementos" variant="outlined">Lista</Button>
        <Button component={Link} to="/mapa" variant="outlined">Mapa</Button>
      </Stack>
    </Paper>
  );
};

export default DetailErrorState;
