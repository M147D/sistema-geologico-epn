import { useRef, useState, useEffect } from 'react';
import { Box, Paper, Typography } from '@mui/material';
import { FULLSCREEN_SX } from '../../../constants/mapConstants';
import FullscreenToggle from './FullscreenToggle';

const GeoMapShell = ({ active = true, loading = false, error = null, children }) => {
  const mapContainerRef = useRef(null);
  // Inicia montado si active=true (vista por defecto). Para vistas ocultas al inicio,
  // espera la primera activación para que Leaflet no inicialice con display:none.
  const [mounted, setMounted] = useState(active);

  // Solo monta el MapContainer cuando la vista ha sido activa al menos una vez.
  // Garantiza que Leaflet inicializa con el contenedor visible (no display:none).
  useEffect(() => {
    if (active) setMounted(true);
  }, [active]);

  if (!mounted || loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
        <Typography>Cargando capas geológicas...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
        <Paper sx={{ p: 3 }}><Typography color="error">{error}</Typography></Paper>
      </Box>
    );
  }

  return (
    <Box
      ref={mapContainerRef}
      sx={{
        position: 'relative',
        flex: 1,
        height: 'calc(100vh - 9rem)',
        width: '100%',
        marginTop: '0.5rem',
        bgcolor: 'background.default',
        ...FULLSCREEN_SX,
      }}
    >
      <FullscreenToggle containerRef={mapContainerRef} />
      {children}
    </Box>
  );
};

export default GeoMapShell;
