import { useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { useMap } from 'react-leaflet';
import L from 'leaflet';
import {
  Paper, Typography, Box, IconButton, Grow, Tooltip, Checkbox, Divider, Radio,
} from '@mui/material';
import LayersIcon from '@mui/icons-material/Layers';
import CloseIcon from '@mui/icons-material/Close';

const OVERLAY_LAYERS = [
  { key: 'geologia',   label: 'Geología' },
  { key: 'provincias', label: 'Límites Provinciales' },
  { key: 'ecuador',    label: 'Límite Nacional' },
];

const CustomLayerControl = ({ visibility, onToggle }) => {
  const map = useMap();
  const containerRef = useRef(null);
  const [isOpen, setIsOpen] = useState(false);

  useEffect(() => {
    const control = L.control({ position: 'topleft' });
    control.onAdd = function () {
      const div = L.DomUtil.create('div', 'custom-layer-control-mui');
      div.style.border = 'none';
      div.style.backgroundColor = 'transparent';
      containerRef.current = div;
      L.DomEvent.disableClickPropagation(div);
      L.DomEvent.disableScrollPropagation(div);
      return div;
    };
    control.addTo(map);
    return () => { control.remove(); containerRef.current = null; };
  }, [map]);

  if (!containerRef.current) return null;

  return createPortal(
    <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start', gap: 0.75 }}>
      {/* Toggle button */}
      <Tooltip title={isOpen ? 'Cerrar capas' : 'Capas del mapa'} placement="right">
        <Paper
          elevation={3}
          onClick={() => setIsOpen(o => !o)}
          sx={{
            width: 34, height: 34,
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            cursor: 'pointer', borderRadius: 1,
            bgcolor: isOpen ? 'primary.main' : 'white',
            color: isOpen ? 'white' : 'text.secondary',
            transition: 'all 0.2s',
            '&:hover': { bgcolor: isOpen ? 'primary.dark' : '#f0f0f0' },
          }}
        >
          <LayersIcon sx={{ fontSize: 18 }} />
        </Paper>
      </Tooltip>

      {/* Expanded panel — grows downward */}
      <Grow in={isOpen} mountOnEnter unmountOnExit>
        <Paper
          elevation={4}
          sx={{
            width: 195,
            backgroundColor: 'rgba(255,255,255,0.97)',
            borderRadius: 2,
            overflow: 'hidden',
          }}
        >
          {/* Header */}
          <Box sx={{
            px: 1.5, py: 0.75,
            display: 'flex', justifyContent: 'space-between', alignItems: 'center',
            bgcolor: 'primary.main', color: 'primary.contrastText',
          }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75 }}>
              <LayersIcon sx={{ fontSize: 14 }} />
              <Typography variant="caption" fontWeight={700} sx={{ letterSpacing: 0.8 }}>
                CAPAS
              </Typography>
            </Box>
            <IconButton
              size="small"
              onClick={() => setIsOpen(false)}
              sx={{ color: 'inherit', p: 0.25 }}
            >
              <CloseIcon sx={{ fontSize: 14 }} />
            </IconButton>
          </Box>

          {/* Base layer — informational only */}
          <Box sx={{ px: 1.5, pt: 1, pb: 0.75 }}>
            <Typography variant="caption" color="text.disabled" sx={{ fontSize: '0.63rem', letterSpacing: 0.6 }}>
              MAPA BASE
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mt: 0.25 }}>
              <Radio size="small" checked readOnly sx={{ p: 0.25, color: 'primary.main' }} />
              <Typography variant="body2" sx={{ fontSize: '0.78rem' }}>OpenStreetMap</Typography>
            </Box>
          </Box>

          <Divider />

          {/* Overlay layers */}
          <Box sx={{ px: 1.5, pt: 0.75, pb: 1 }}>
            <Typography variant="caption" color="text.disabled" sx={{ fontSize: '0.63rem', letterSpacing: 0.6 }}>
              CAPAS DE DATOS
            </Typography>
            {OVERLAY_LAYERS.map(({ key, label }) => (
              <Box
                key={key}
                onClick={() => onToggle(key)}
                sx={{
                  display: 'flex', alignItems: 'center', gap: 0.25,
                  py: 0.1, px: 0.25, mx: -0.25,
                  cursor: 'pointer', borderRadius: 1,
                  '&:hover': { bgcolor: 'action.hover' },
                }}
              >
                <Checkbox
                  size="small"
                  checked={visibility[key] ?? true}
                  sx={{ p: 0.5 }}
                  tabIndex={-1}
                />
                <Typography variant="body2" sx={{ fontSize: '0.78rem', userSelect: 'none' }}>
                  {label}
                </Typography>
              </Box>
            ))}
          </Box>
        </Paper>
      </Grow>
    </Box>,
    containerRef.current
  );
};

export default CustomLayerControl;
