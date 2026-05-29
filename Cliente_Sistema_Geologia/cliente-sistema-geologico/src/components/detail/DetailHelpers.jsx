import { useState, useEffect } from 'react';
import { Box } from '@mui/material';
import { useApi } from '../../context/ApiContext';
import { getImage, getImageThumbnail } from '../../utils/imageCache';
import { SUBTIPOS_FOSIL, SUBTIPOS_MINERAL, SUBTIPOS_ROCA } from '../../constants/tipoMaps';

export const getStatusColor = (estado) => {
  if (!estado) return 'default';
  const s = estado.toLowerCase();
  if (s.includes('bueno') || s.includes('activo')) return 'success';
  if (s.includes('deteriorado') || s.includes('pendiente')) return 'warning';
  if (s.includes('malo') || s.includes('dañado')) return 'error';
  return 'default';
};

export const getTypeColor = (tipo) => {
  switch (tipo) {
    case 'Fosil': return 'primary';
    case 'Mineral': return 'secondary';
    case 'Roca': return 'success';
    default: return 'default';
  }
};

export const getTypeLabel = (tipo) => {
  switch (tipo) {
    case 'Fosil': return 'Fosil';
    case 'Mineral': return 'Mineral';
    case 'Roca': return 'Roca';
    default: return tipo || 'Desconocido';
  }
};

export const getSubtipo = (el) => {
  if (!el) return null;
  if (el.tipoElemento === 'Fosil') return el.tipoFosil;
  if (el.tipoElemento === 'Mineral') return el.tipoMineral;
  if (el.tipoElemento === 'Roca') return el.tipoRoca;
  return null;
};

export const getSubtipos = (tipo) => {
  switch (tipo) {
    case 'Fosil': return SUBTIPOS_FOSIL;
    case 'Mineral': return SUBTIPOS_MINERAL;
    case 'Roca': return SUBTIPOS_ROCA;
    default: return [];
  }
};

export const getSubtipoLabel = (tipo, value) => {
  if (value === null || value === undefined) return null;
  return getSubtipos(tipo).find(s => s.value === value)?.label ?? String(value);
};

export const FotoComponente = ({ fotoId, alt, height = "200px", onClick, thumbnail = false }) => {
  const { api } = useApi();
  const [blobUrl, setBlobUrl] = useState(null);

  useEffect(() => {
    if (!fotoId) { setBlobUrl(null); return; }
    let mounted = true;
    const fetchFn = thumbnail ? getImageThumbnail : getImage;
    fetchFn(api, fotoId).then(url => { if (mounted) setBlobUrl(url); });
    return () => { mounted = false; };
  }, [fotoId, api, thumbnail]);

  if (!fotoId) return null;

  if (!blobUrl) {
    return thumbnail
      ? <Box sx={{ width: '100%', height, bgcolor: 'grey.100' }} />
      : null;
  }

  return (
    <img
      src={blobUrl}
      alt={alt || 'Imagen'}
      loading="lazy"
      style={{
        width: '100%',
        height: height,
        objectFit: 'cover',
        display: 'block',
        cursor: onClick ? 'pointer' : 'default'
      }}
      onClick={onClick}
    />
  );
};
