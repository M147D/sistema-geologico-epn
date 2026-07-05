// Colores por tipo (fallback cuando no hay subtipo)
export const TIPO_COLORS = {
  Fosil:   '#2196F3',
  Mineral: '#9C27B0',
  Roca:    '#4CAF50',
};

// Keys coinciden con C# enum.ToString() (sin tildes) que devuelve el backend en tipoEspecifico
export const SUBTIPO_COLORS = {
  Fosil: {
    Desconocido:   '#90A4AE',
    Vertebrado:    '#1565C0',
    Invertebrado:  '#42A5F5',
    Paleobotanica: '#00897B',
    Icnofosil:     '#00838F',
    Microfosil:    '#00695C',
  },
  Mineral: {
    Desconocido: '#90A4AE',
    Silicato:    '#7B1FA2',
    Carbonato:   '#AB47BC',
    Metalico:    '#F9A825',
    Oxido:       '#E64A19',
    Sulfuro:     '#9E9D24',
    Sulfato:     '#D81B60',
    Fosfato:     '#E91E63',
    Vanadato:    '#B71C1C',
  },
  Roca: {
    Desconocido:          '#90A4AE',
    'Ígnea':              '#C62828',
    Sedimentaria:         '#81C784',
    'Metamórfica':        '#4DD0E1',
    Meteorito:            '#FF8F00',
    'PiroVolcanoclástica':'#F9A825',
  },
};

const normStr = (s) =>
  String(s ?? '').toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '').replace(/[-\s]/g, '');

export const getSubtipoDisplayLabel = (key) => key ?? null;

export const getSubtipoEntries = (tipo) => {
  const map = SUBTIPO_COLORS[tipo];
  if (!map) return [];
  return Object.keys(map).map(key => ({ value: key, label: getSubtipoDisplayLabel(key) }));
};

// Resuelve el color para un subtipo dado. Tolera tildes y mayúsculas/minúsculas.
export const getSubtipoColor = (tipo, tipoEspecifico) => {
  if (!tipo) return '#757575';
  const tipoMap = SUBTIPO_COLORS[tipo];
  if (!tipoMap || !tipoEspecifico) return TIPO_COLORS[tipo] ?? '#757575';
  if (tipoEspecifico in tipoMap) return tipoMap[tipoEspecifico];
  const normKey = normStr(tipoEspecifico);
  const entry = Object.entries(tipoMap).find(([k]) => normStr(k) === normKey);
  return entry ? entry[1] : (TIPO_COLORS[tipo] ?? '#757575');
};
