import { useMemo } from "react";
import { Marker, Popup } from "react-leaflet";
import L from "leaflet";
import { Link } from "react-router-dom";
import { Box, Typography, Chip, Divider } from "@mui/material";
import RoomIcon from "@mui/icons-material/Room";
import PhotoCameraIcon from "@mui/icons-material/PhotoCamera";
import { TIPO_COLORS, getSubtipoColor, getSubtipoDisplayLabel } from "../../constants/subtipoColors";

const TIPO_LABELS = { Fosil: 'Fósil', Mineral: 'Mineral', Roca: 'Roca' };

const makeIcon = (color) => L.divIcon({
  className: 'custom-point-icon',
  html: `<div style="
    background-color:${color};
    width:7px;height:7px;
    border-radius:50%;
    border:0.25px solid white;
  "></div>`,
  iconSize: [10, 10],
  iconAnchor: [5, 5],
  popupAnchor: [0, -8],
});

// Cache de íconos por color — se construyen una sola vez
const iconCache = new Map();
const getIconByColor = (color) => {
  if (!iconCache.has(color)) iconCache.set(color, makeIcon(color));
  return iconCache.get(color);
};

const MarkerElement = ({ elementos = [] }) => {
  const elementosConCoordenadas = useMemo(() =>
    elementos
      .filter(el => {
        if (!el?.latitud || !el?.longitud) return false;
        const lat = parseFloat(el.latitud);
        const lng = parseFloat(el.longitud);
        return !isNaN(lat) && !isNaN(lng) && lat !== 0 && lng !== 0;
      })
      .map(el => ({ ...el, _lat: parseFloat(el.latitud), _lng: parseFloat(el.longitud) })),
    [elementos]
  );

  return (
    <>
      {elementosConCoordenadas.map(el => {
        const tipoColor = TIPO_COLORS[el.tipoElemento] ?? '#757575';
        const subtipoColor = getSubtipoColor(el.tipoElemento, el.tipoEspecifico);
        const tipoLabel = TIPO_LABELS[el.tipoElemento] ?? el.tipoElemento ?? 'Desconocido';

        const infoExtra = el.especie
          ? { label: 'Especie', value: el.especie }
          : null;

        return (
          <Marker
            key={el.id}
            position={[el._lat, el._lng]}
            icon={getIconByColor(subtipoColor)}
          >
            <Popup keepInView minWidth={210} maxWidth={230}>
              <Box sx={{ width: 210 }}>
                {/* Encabezado: nombre + chips de tipo y subtipo */}
                <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', gap: 1, mb: 0.5 }}>
                  <Typography variant="subtitle2" fontWeight={700} sx={{ lineHeight: 1.3, flex: 1 }}>
                    {el.nombre || 'Sin nombre'}
                  </Typography>
                  <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 0.4, flexShrink: 0 }}>
                    <Chip
                      label={tipoLabel}
                      size="small"
                      sx={{ bgcolor: tipoColor, color: '#fff', fontSize: '0.6rem', height: 18 }}
                    />
                    {el.tipoEspecifico && el.tipoEspecifico !== 'Desconocido' && (
                      <Chip
                        label={getSubtipoDisplayLabel(el.tipoEspecifico)}
                        size="small"
                        variant="outlined"
                        sx={{
                          borderColor: subtipoColor,
                          color: subtipoColor,
                          fontSize: '0.6rem',
                          height: 16,
                        }}
                      />
                    )}
                  </Box>
                </Box>

                {el.codigo && (
                  <Typography variant="caption" color="text.disabled" sx={{ display: 'block', mb: 0.75 }}>
                    {el.codigo}
                  </Typography>
                )}

                <Divider sx={{ mb: 0.75 }} />

                {(el.localidad || el.nombreProvincia) && (
                  <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 0.5, mb: 0.5 }}>
                    <RoomIcon sx={{ fontSize: 13, color: 'text.secondary', mt: '2px', flexShrink: 0 }} />
                    <Typography variant="caption" color="text.secondary">
                      {[el.localidad, el.nombreProvincia].filter(Boolean).join(', ')}
                    </Typography>
                  </Box>
                )}

                {infoExtra && (
                  <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 0.5 }}>
                    <Box component="span" fontWeight={600}>{infoExtra.label}:</Box>{' '}
                    {infoExtra.value}
                  </Typography>
                )}

                {el.edad && (
                  <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 0.5 }}>
                    <Box component="span" fontWeight={600}>Edad:</Box>{' '}{el.edad}
                  </Typography>
                )}

                <Divider sx={{ mt: 0.25, mb: 0.75 }} />

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Link
                    to={`/detalle/${el.id}`}
                    state={{ elemento: el }}
                    style={{ fontSize: '0.75rem', color: subtipoColor, fontWeight: 700, textDecoration: 'none' }}
                  >
                    Ver detalles →
                  </Link>
                  {el.totalFotos > 0 && (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.25 }}>
                      <PhotoCameraIcon sx={{ fontSize: 12, color: 'text.disabled' }} />
                      <Typography variant="caption" color="text.disabled">{el.totalFotos}</Typography>
                    </Box>
                  )}
                </Box>
              </Box>
            </Popup>
          </Marker>
        );
      })}
    </>
  );
};

export default MarkerElement;
