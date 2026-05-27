// src/stubs/elementosStubs.js
// Datos estáticos para VITE_USE_STUBS=true.
// Firmas idénticas a useElementosService — sin peticiones HTTP.

const STUB_OK = { success: true, message: 'Stub: operación simulada', data: null, errors: [] };

const PAIS_ECUADOR = {
  id: 1,
  nombrePais: 'Ecuador',
  fechaCreacion: '2020-01-01T00:00:00',
  estadoActivo: true,
  fechaActualizacion: null,
  totalProvincias: 2,
};

const PROVINCIAS_STUB = [
  { id: 1, nombreProvincia: 'Pichincha',  paisId: 1, nombrePais: 'Ecuador', fechaCreacion: '2020-01-01T00:00:00', estadoActivo: true, fechaActualizacion: null, totalUbicaciones: 1 },
  { id: 2, nombreProvincia: 'Guayas',     paisId: 1, nombrePais: 'Ecuador', fechaCreacion: '2020-01-01T00:00:00', estadoActivo: true, fechaActualizacion: null, totalUbicaciones: 1 },
  { id: 3, nombreProvincia: 'Imbabura',   paisId: 1, nombrePais: 'Ecuador', fechaCreacion: '2020-01-01T00:00:00', estadoActivo: true, fechaActualizacion: null, totalUbicaciones: 1 },
  { id: 4, nombreProvincia: 'Azuay',      paisId: 1, nombrePais: 'Ecuador', fechaCreacion: '2020-01-01T00:00:00', estadoActivo: true, fechaActualizacion: null, totalUbicaciones: 1 },
  { id: 5, nombreProvincia: 'Cotopaxi',   paisId: 1, nombrePais: 'Ecuador', fechaCreacion: '2020-01-01T00:00:00', estadoActivo: true, fechaActualizacion: null, totalUbicaciones: 1 },
  { id: 6, nombreProvincia: 'Sucumbíos',  paisId: 1, nombrePais: 'Ecuador', fechaCreacion: '2020-01-01T00:00:00', estadoActivo: true, fechaActualizacion: null, totalUbicaciones: 1 },
];

const FOSIL_1 = {
  id: 1, nombre: 'Cráneo de Mastodon andinum', codigo: 'FOC-001',
  tipoElemento: 'Fosil', tipo: 'fosil', tipoEspecifico: 'Vertebrado',
  edad: 'Pleistoceno', donante: 'Universidad Central del Ecuador',
  fechaIngreso: '2020-03-15T00:00:00', ejemplares: 1,
  laminaExiste: true, estadoActivo: true,
  fechaCreacion: '2020-03-15T10:00:00', fechaActualizacion: null,
  ubicacionId: 1, localidad: 'Quito',
  nombrePais: 'Ecuador', nombreProvincia: 'Pichincha',
  latitud: '-0.2295', longitud: '-78.5243',
  especie: 'Mastodon andinum', periodo: 'Cuaternario', litologia: null,
  totalFotos: 0, tieneGaleria: false,
};

const FOSIL_2 = {
  id: 2, nombre: 'Amonite jurásico', codigo: 'FOI-001',
  tipoElemento: 'Fosil', tipo: 'fosil', tipoEspecifico: 'Invertebrado',
  edad: 'Jurásico Medio', donante: 'EPN',
  fechaIngreso: '2019-06-10T00:00:00', ejemplares: 3,
  laminaExiste: false, estadoActivo: true,
  fechaCreacion: '2019-06-10T08:00:00', fechaActualizacion: null,
  ubicacionId: 2, localidad: 'Guayaquil',
  nombrePais: 'Ecuador', nombreProvincia: 'Guayas',
  latitud: '-2.1962', longitud: '-79.8862',
  especie: 'Ammonites sp.', periodo: 'Jurásico Medio', litologia: null,
  totalFotos: 0, tieneGaleria: false,
};

const MINERAL_1 = {
  id: 3, nombre: 'Cuarzo rosa Cotacachi', codigo: 'MIS-001',
  tipoElemento: 'Mineral', tipo: 'mineral', tipoEspecifico: 'Silicato',
  edad: 'Paleógeno', donante: 'Museo Nacional',
  fechaIngreso: '2021-01-20T00:00:00', ejemplares: 5,
  laminaExiste: true, estadoActivo: true,
  fechaCreacion: '2021-01-20T09:00:00', fechaActualizacion: null,
  ubicacionId: 3, localidad: 'Cotacachi',
  nombrePais: 'Ecuador', nombreProvincia: 'Imbabura',
  latitud: '0.3036', longitud: '-78.2669',
  especie: null, periodo: null, litologia: 'SiO₂',
  totalFotos: 0, tieneGaleria: false,
};

const MINERAL_2 = {
  id: 4, nombre: 'Calcita cristalizada', codigo: 'MIC-001',
  tipoElemento: 'Mineral', tipo: 'mineral', tipoEspecifico: 'Carbonato',
  edad: 'Cretácico', donante: 'Donante anónimo',
  fechaIngreso: '2022-08-05T00:00:00', ejemplares: 2,
  laminaExiste: false, estadoActivo: true,
  fechaCreacion: '2022-08-05T14:00:00', fechaActualizacion: null,
  ubicacionId: 4, localidad: 'Cuenca',
  nombrePais: 'Ecuador', nombreProvincia: 'Azuay',
  latitud: '-2.8974', longitud: '-79.0045',
  especie: null, periodo: null, litologia: 'CaCO₃',
  totalFotos: 0, tieneGaleria: false,
};

const ROCA_1 = {
  id: 5, nombre: 'Basalto Cotopaxi', codigo: 'ROI-001',
  tipoElemento: 'Roca', tipo: 'roca', tipoEspecifico: 'Ígnea',
  edad: 'Cuaternario', donante: 'IGEPN',
  fechaIngreso: '2018-11-12T00:00:00', ejemplares: 10,
  laminaExiste: true, estadoActivo: true,
  fechaCreacion: '2018-11-12T11:00:00', fechaActualizacion: null,
  ubicacionId: 5, localidad: 'Volcán Cotopaxi',
  nombrePais: 'Ecuador', nombreProvincia: 'Cotopaxi',
  latitud: '-0.6846', longitud: '-78.4356',
  especie: null, periodo: null, litologia: 'Lava basáltica',
  totalFotos: 0, tieneGaleria: false,
};

const ROCA_2 = {
  id: 6, nombre: 'Arenisca del Oriente', codigo: 'ROS-001',
  tipoElemento: 'Roca', tipo: 'roca', tipoEspecifico: 'Sedimentaria',
  edad: 'Cretácico', donante: 'Petroecuador',
  fechaIngreso: '2017-04-22T00:00:00', ejemplares: 7,
  laminaExiste: false, estadoActivo: true,
  fechaCreacion: '2017-04-22T16:00:00', fechaActualizacion: null,
  ubicacionId: 6, localidad: 'Lago Agrio',
  nombrePais: 'Ecuador', nombreProvincia: 'Sucumbíos',
  latitud: '0.0833', longitud: '-76.8833',
  especie: null, periodo: null, litologia: 'Arenisca cuarzosa',
  totalFotos: 0, tieneGaleria: false,
};

const FOSILES   = [FOSIL_1,   FOSIL_2];
const MINERALES = [MINERAL_1, MINERAL_2];
const ROCAS     = [ROCA_1,    ROCA_2];
const TODOS     = [...FOSILES, ...MINERALES, ...ROCAS];

const makeUbicacion = (el) => ({
  id: el.ubicacionId,
  latitud: el.latitud,
  longitud: el.longitud,
  localidad: el.localidad,
  leyenda: '',
  estadoActivo: true,
  fechaCreacion: el.fechaCreacion,
  fechaActualizacion: null,
  nombrePais: el.nombrePais,
  nombreProvincia: el.nombreProvincia,
  paisId: 1,
  provinciaId: el.ubicacionId,
});

const makeDetail = (el) => ({
  ...el,
  documentosRelacionados: null,
  ubicacion: makeUbicacion(el),
  galeria: null,
  tipoFosil:   el.tipoElemento === 'Fosil'   ? (el.tipoEspecifico === 'Vertebrado' ? 1 : 2) : null,
  tipoMineral: el.tipoElemento === 'Mineral' ? (el.tipoEspecifico === 'Silicato'   ? 1 : 2) : null,
  tipoRoca:    el.tipoElemento === 'Roca'    ? (el.tipoEspecifico === 'Ígnea'      ? 1 : 2) : null,
  litologiaMineral: el.tipoElemento === 'Mineral' ? el.litologia : null,
  litologiaRoca:    el.tipoElemento === 'Roca'    ? el.litologia : null,
  creadoPor: 'Admin Stub',
  actualizadoPor: null,
  eliminadoPor: null,
});

export const elementosStubs = {
  getFosiles:           () => Promise.resolve(FOSILES),
  getMinerales:         () => Promise.resolve(MINERALES),
  getRocas:             () => Promise.resolve(ROCAS),
  getTodosElementos:    () => Promise.resolve(TODOS),
  getPaises:            () => Promise.resolve([PAIS_ECUADOR]),
  getProvincias:        (paisId = 1) => Promise.resolve(paisId === 1 ? PROVINCIAS_STUB : []),
  getByCodigo:          () => Promise.resolve({ exists: false }),
  getFotosGaleria:      () => Promise.resolve([]),
  getFotosByElemento:   () => Promise.resolve({ galeriaId: null, fotos: [] }),
  uploadFoto:           () => Promise.resolve(STUB_OK),
  uploadFotoByElemento: () => Promise.resolve(STUB_OK),
  updateFoto:           () => Promise.resolve(STUB_OK),
  deleteFoto:           () => Promise.resolve(),
  restoreFoto:          () => Promise.resolve(),
  createFosil:          () => Promise.resolve(STUB_OK),
  createMineral:        () => Promise.resolve(STUB_OK),
  createRoca:           () => Promise.resolve(STUB_OK),
  updateFosil:          () => Promise.resolve(STUB_OK),
  updateMineral:        () => Promise.resolve(STUB_OK),
  updateRoca:           () => Promise.resolve(STUB_OK),
  deleteElemento:       () => Promise.resolve(STUB_OK),

  getElementById(id) {
    const el = TODOS.find(e => e.id === Number(id));
    if (!el) return Promise.resolve({ success: false, message: 'Stub: no encontrado', data: null, errors: [] });
    return Promise.resolve({ success: true, message: 'OK', data: makeDetail(el), errors: [] });
  },

  filtrarElementos(filtros = {}) {
    let result = TODOS;
    if (filtros.tipo === 'Fosil')   result = FOSILES;
    else if (filtros.tipo === 'Mineral') result = MINERALES;
    else if (filtros.tipo === 'Roca')    result = ROCAS;
    if (filtros.nombre) {
      const q = filtros.nombre.toLowerCase();
      result = result.filter(e => e.nombre.toLowerCase().includes(q));
    }
    return Promise.resolve(result);
  },
};
