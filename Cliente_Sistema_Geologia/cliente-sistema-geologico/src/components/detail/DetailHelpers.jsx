import { useState, useEffect } from 'react';
import { Box, TableRow, TableCell, Typography } from '@mui/material';

export const FotoComponente = ({ fotoId, alt, height = "200px", onClick, thumbnail = false, getImage, getImageThumbnail }) => {
  const [blobUrl, setBlobUrl] = useState(null);

  useEffect(() => {
    if (!fotoId) { setBlobUrl(null); return; }
    let mounted = true;
    const fetchFn = thumbnail ? getImageThumbnail : getImage;
    fetchFn(fotoId).then(url => { if (mounted) setBlobUrl(url); });
    return () => { mounted = false; };
  }, [fotoId, thumbnail, getImage, getImageThumbnail]);

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

export const InfoRow = ({ label, value, isEditing, editField, labelWidth = '38%' }) => (
  <TableRow sx={{ '&:nth-of-type(odd)': { bgcolor: 'grey.50' } }}>
    <TableCell
      component="th"
      sx={{ fontWeight: 600, width: labelWidth, py: 1, color: 'text.secondary', fontSize: '0.8125rem' }}
    >
      {label}
    </TableCell>
    <TableCell sx={{ py: 1 }}>
      {isEditing && editField
        ? editField
        : (value ?? <Typography variant="body2" color="text.disabled">No disponible</Typography>)}
    </TableCell>
  </TableRow>
);
