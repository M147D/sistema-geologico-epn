import { useEffect } from 'react';
import { useMap } from 'react-leaflet';

// Llama map.invalidateSize() cuando el contenedor pasa de display:none a display:block.
// Necesario para mapas que se montan ocultos (size=0) y luego se muestran.
const MapResizer = ({ active }) => {
  const map = useMap();
  useEffect(() => {
    if (!active) return;
    const t = setTimeout(() => map.invalidateSize(), 50);
    return () => clearTimeout(t);
  }, [active, map]);
  return null;
};

export default MapResizer;
