// src/stubs/gisStubs.js
// GeoJSON placeholder centrado en Ecuador (~lon -78, lat -1).
// Geometrías inventadas — suficientes para probar rendering, leyenda y filtros.
// processLayer (useGeoLayers) añade el campo `layer` automáticamente.

const ECUADOR_FC = {
  type: 'FeatureCollection',
  features: [{
    type: 'Feature',
    properties: { NOMBRE: 'Ecuador' },
    geometry: {
      type: 'Polygon',
      coordinates: [[
        [-80.5,  1.5], [-75.2,  1.4], [-75.3, -0.5],
        [-76.2, -2.3], [-80.3, -3.5], [-81.0, -2.2],
        [-80.5,  1.5],
      ]],
    },
  }],
};

const PROVINCIAS_FC = {
  type: 'FeatureCollection',
  features: [
    {
      type: 'Feature',
      properties: { DPA_DESPRO: 'Pichincha', Gid: 1 },
      geometry: { type: 'Polygon', coordinates: [[[-79.2, 0.3], [-77.8, 0.3], [-77.8, -0.8], [-79.2, -0.8], [-79.2, 0.3]]] },
    },
    {
      type: 'Feature',
      properties: { DPA_DESPRO: 'Guayas', Gid: 2 },
      geometry: { type: 'Polygon', coordinates: [[[-80.4, -1.6], [-79.0, -1.6], [-79.0, -2.5], [-80.4, -2.5], [-80.4, -1.6]]] },
    },
    {
      type: 'Feature',
      properties: { DPA_DESPRO: 'Azuay', Gid: 3 },
      geometry: { type: 'Polygon', coordinates: [[[-79.5, -2.4], [-78.5, -2.4], [-78.5, -3.3], [-79.5, -3.3], [-79.5, -2.4]]] },
    },
    {
      type: 'Feature',
      properties: { DPA_DESPRO: 'Manabí', Gid: 4 },
      geometry: { type: 'Polygon', coordinates: [[[-80.7, 0.6], [-79.5, 0.6], [-79.5, -1.4], [-80.7, -1.4], [-80.7, 0.6]]] },
    },
    {
      type: 'Feature',
      properties: { DPA_DESPRO: 'Tungurahua', Gid: 5 },
      geometry: { type: 'Polygon', coordinates: [[[-78.9, -1.0], [-78.2, -1.0], [-78.2, -1.6], [-78.9, -1.6], [-78.9, -1.0]]] },
    },
  ],
};

const GEOLOGIA_FC = {
  type: 'FeatureCollection',
  features: [
    {
      type: 'Feature',
      properties: { Gid: 101, CodA: 'PJLV', ColorRgb: 'rgb(180,120,60)',  LabelQml: 'Volcánicos del Pastaza',   Litologia: 'Lavas andesíticas y piroclásticos' },
      geometry: { type: 'Polygon', coordinates: [[[-78.6, -1.0], [-78.2, -1.0], [-78.2, -1.4], [-78.6, -1.4], [-78.6, -1.0]]] },
    },
    {
      type: 'Feature',
      properties: { Gid: 102, CodA: 'KGCH', ColorRgb: 'rgb(90,160,210)',   LabelQml: 'Grupo Chota',              Litologia: 'Conglomerados y areniscas rojas' },
      geometry: { type: 'Polygon', coordinates: [[[-78.8, 0.0], [-78.3, 0.0], [-78.3, -0.5], [-78.8, -0.5], [-78.8, 0.0]]] },
    },
    {
      type: 'Feature',
      properties: { Gid: 103, CodA: 'JTSC', ColorRgb: 'rgb(210,190,80)',   LabelQml: 'Terciario Santa Clara',    Litologia: 'Lutitas calcáreas y margas' },
      geometry: { type: 'Polygon', coordinates: [[[-79.8, -2.0], [-79.3, -2.0], [-79.3, -2.4], [-79.8, -2.4], [-79.8, -2.0]]] },
    },
    {
      type: 'Feature',
      properties: { Gid: 104, CodA: 'PQAM', ColorRgb: 'rgb(140,200,140)',  LabelQml: 'Formación Ambuquí',        Litologia: 'Terrazas fluviales cuaternarias' },
      geometry: { type: 'Polygon', coordinates: [[[-78.1, 0.5], [-77.7, 0.5], [-77.7, 0.1], [-78.1, 0.1], [-78.1, 0.5]]] },
    },
  ],
};

export const gisStubs = {
  getEcuador:            () => Promise.resolve(ECUADOR_FC),
  getProvincias:         () => Promise.resolve(PROVINCIAS_FC),
  getSimplifiedGeologia: () => Promise.resolve(GEOLOGIA_FC),
};
