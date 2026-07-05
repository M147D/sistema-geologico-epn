export const SUBTIPOS_FOSIL = [
  { value: 0, label: 'Desconocido' },
  { value: 1, label: 'Vertebrado' },
  { value: 2, label: 'Invertebrado' },
  { value: 3, label: 'Paleobotánica' },
  { value: 4, label: 'Icnofósil' },
  { value: 5, label: 'Microfósil' },
];

export const SUBTIPOS_MINERAL = [
  { value: 0, label: 'Desconocido' },
  { value: 1, label: 'Silicato' },
  { value: 2, label: 'Carbonato' },
  { value: 3, label: 'Metálico' },
  { value: 4, label: 'Oxido' },
  { value: 5, label: 'Sulfuro' },
  { value: 6, label: 'Sulfato' },
  { value: 7, label: 'Fosfato' },
  { value: 8, label: 'Vanadato' },
];

export const SUBTIPOS_ROCA = [
  { value: 0, label: 'Desconocido' },
  { value: 1, label: 'Ígnea' },
  { value: 2, label: 'Sedimentaria' },
  { value: 3, label: 'Metamórfica' },
  { value: 4, label: 'Meteorito' },
  { value: 5, label: 'PiroVolcanoclástica' },
];

// Inverse maps (label → value) used for Excel import
export const TIPO_FOSIL_MAP = Object.fromEntries(SUBTIPOS_FOSIL.map(s => [s.label, s.value]));
export const TIPO_MINERAL_MAP = Object.fromEntries(SUBTIPOS_MINERAL.map(s => [s.label, s.value]));
export const TIPO_ROCA_MAP = Object.fromEntries(SUBTIPOS_ROCA.map(s => [s.label, s.value]));
