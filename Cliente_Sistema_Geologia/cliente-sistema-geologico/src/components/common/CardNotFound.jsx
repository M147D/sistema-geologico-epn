// src/components/NotFound.jsx
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Container,
  Paper
} from '@mui/material';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';
import HomeIcon from '@mui/icons-material/Home';

const NotFound = () => {
  const navigate = useNavigate();

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '80vh'
        }}
      >
        <Paper
          elevation={3}
          sx={{
            p: 4,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            gap: 2
          }}
        >
          <ErrorOutlineIcon 
            sx={{ 
              fontSize: 80,
              color: 'error.main'
            }}
          />
          
          <Typography variant="h2" component="h1" gutterBottom>
            404
          </Typography>
          
          <Typography variant="h5" component="h2" gutterBottom>
            Página no encontrada
          </Typography>
          
          <Typography color="text.secondary" align="center" sx={{ mb: 2 }}>
            Lo sentimos, la página que estás buscando no existe o ha sido movida.
          </Typography>

          <Button
            variant="contained"
            startIcon={<HomeIcon />}
            onClick={() => navigate('/mapa')}
            size="large"
          >
            Ir al Mapa
          </Button>
        </Paper>
      </Box>
    </Container>
  );
};

export default NotFound;