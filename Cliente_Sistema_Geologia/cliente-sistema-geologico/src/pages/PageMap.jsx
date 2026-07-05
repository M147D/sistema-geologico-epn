import { useState, useMemo, useCallback, useEffect } from 'react';
import ElementsMapView from "../components/mapa/ElementsMapView.jsx";
import GeologicalMapView from "../components/mapa/geological/GeologicalMapView.jsx";
import CombinedMapView from "../components/mapa/CombinedMapView.jsx";
import { useElementos } from '../context/ElementosContext';
import { GeologiaProvider, useGeologia } from '../context/GeologiaContext';
import {
  Box, CircularProgress, Alert, Paper,
  ToggleButtonGroup, ToggleButton,
  Typography, Tooltip,
} from '@mui/material';
import MapIcon     from '@mui/icons-material/Map';
import TerrainIcon from '@mui/icons-material/Terrain';
import PublicIcon  from '@mui/icons-material/Public';

const VIEW_OPTIONS = [
  { value: 'normal',     icon: <MapIcon fontSize="small" />,     label: 'Normal',     tooltip: 'Elementos geológicos sobre mapa base' },
  { value: 'geological', icon: <TerrainIcon fontSize="small" />, label: 'Geológica',  tooltip: 'Formaciones y polígonos GIS' },
  { value: 'combined',   icon: <PublicIcon fontSize="small" />,  label: 'Combinada',  tooltip: 'Elementos + formaciones' },
];

const PageMapContent = () => {
  const { elementos, loading, error, resultadosBusqueda } = useElementos();
  const { enable } = useGeologia();
  const [mapView, setMapView] = useState('normal');
  const [layerVisibility, setLayerVisibility] = useState({
    geologia: true, provincias: false, ecuador: true,
  });
  const [filteredNormal,   setFilteredNormal]   = useState([]);
  const [filteredCombined, setFilteredCombined] = useState([]);

  const elementosAMostrar = useMemo(
    () => resultadosBusqueda.length > 0 ? resultadosBusqueda : elementos,
    [resultadosBusqueda, elementos]
  );

  const handleMapViewChange = (_, newView) => {
    if (newView !== null) {
      setMapView(newView);
      if (newView !== 'normal') enable();
    }
  };

  const toggleLayer = useCallback((key) =>
    setLayerVisibility(prev => ({ ...prev, [key]: !prev[key] })), []);

  const handleFilteredNormal   = useCallback((f) => setFilteredNormal(f),   []);
  const handleFilteredCombined = useCallback((f) => setFilteredCombined(f), []);

  useEffect(() => {
    setFilteredNormal(elementosAMostrar);
    setFilteredCombined(elementosAMostrar);
  }, [elementosAMostrar]);

  if (loading && elementos.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="calc(100vh - 64px)" sx={{ p: 3 }}>
        <Alert severity="error" sx={{ maxWidth: 200 }}>{error}</Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ height: 'calc(92.5vh - 64px)', position: 'relative' }}>

      {/* Panel flotante — selector de vista */}
      <Paper
        elevation={4}
        sx={{
          position: 'absolute',
          top: 16,
          right: 16,
          zIndex: 1000,
          borderRadius: 2,
          overflow: 'hidden',
          display: 'flex',
          flexDirection: 'column',
          minWidth: 120,
        }}
      >
        <Box sx={{
          px: 1.5, py: 0.75,
          bgcolor: 'grey.100',
          borderBottom: 1, borderColor: 'divider',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <Typography variant="caption" fontWeight={600} color="text.secondary" sx={{ letterSpacing: 0.8 }}>
            VISTA
          </Typography>
        </Box>
        <ToggleButtonGroup
          value={mapView}
          exclusive
          onChange={handleMapViewChange}
          size="small"
          orientation="vertical"
          sx={{
            '& .MuiToggleButton-root': {
              justifyContent: 'flex-start',
              gap: 1,
              px: 1.5,
              py: 0.75,
              border: 'none',
              borderRadius: 0,
              borderBottom: '1px solid',
              borderColor: 'divider',
              '&:last-child': { borderBottom: 'none' },
              '&.Mui-selected': {
                bgcolor: 'primary.main',
                color: 'primary.contrastText',
                '&:hover': { bgcolor: 'primary.dark' },
              },
            },
          }}
        >
          {VIEW_OPTIONS.map(({ value, icon, label, tooltip }) => (
            <Tooltip key={value} title={tooltip} placement="left" arrow>
              <ToggleButton value={value}>
                {icon}
                <Typography variant="caption" fontWeight={500}>
                  {label}
                </Typography>
              </ToggleButton>
            </Tooltip>
          ))}
        </ToggleButtonGroup>
      </Paper>

      <Box sx={{ display: mapView === 'normal' ? 'flex' : 'none', flexDirection: 'column', height: '100%' }}>
        <ElementsMapView
          elementos={elementosAMostrar}
          filteredElementos={filteredNormal}
          onFilteredChange={handleFilteredNormal}
          active={mapView === 'normal'}
        />
      </Box>

      <Box sx={{ display: mapView === 'geological' ? 'flex' : 'none', flexDirection: 'column', height: '100%' }}>
        <GeologicalMapView
          active={mapView === 'geological'}
          layerVisibility={layerVisibility}
          onToggleLayer={toggleLayer}
        />
      </Box>

      <Box sx={{ display: mapView === 'combined' ? 'flex' : 'none', flexDirection: 'column', height: '100%' }}>
        <CombinedMapView
          elementos={elementosAMostrar}
          filteredElementos={filteredCombined}
          onFilteredChange={handleFilteredCombined}
          active={mapView === 'combined'}
          layerVisibility={layerVisibility}
          onToggleLayer={toggleLayer}
        />
      </Box>

    </Box>
  );
};

const PageMap = () => (
  <GeologiaProvider>
    <PageMapContent />
  </GeologiaProvider>
);

export default PageMap;
