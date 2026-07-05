import L from 'leaflet';

// Leaflet default icon fix — runs once when this module is first imported
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

export const MAP_CENTER = [-1.8312, -80.1834];
export const MAP_ZOOM = 6;
export const MAP_ATTRIBUTION =
  '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors | ' +
  'Referencia Mapa Geológico del Ecuador (IIGE, 2017) | ' +
  'Mapa de rocas del Museo Petrográfico Tomas Feininger | ' +
  'Autores: Yesenia Enríquez, Miguel Pastuña, Sandra Procel, Ana Cabero y David Mejía';
export const MAP_TILE_URL = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';

export const FULLSCREEN_SX = {
  '&:fullscreen': { height: '100vh', marginTop: 0 },
  '&:-webkit-full-screen': { height: '100vh', marginTop: 0 },
  '&:-moz-full-screen': { height: '100vh', marginTop: 0 },
  '&:-ms-fullscreen': { height: '100vh', marginTop: 0 },
};
