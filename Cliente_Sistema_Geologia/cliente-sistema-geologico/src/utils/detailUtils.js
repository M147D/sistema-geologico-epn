import { SUBTIPOS_FOSIL, SUBTIPOS_MINERAL, SUBTIPOS_ROCA } from '../constants/tipoMaps';

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
