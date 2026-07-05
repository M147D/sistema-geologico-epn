// src/context/GeologiaContext.jsx
import { createContext, useContext, useState, useEffect, useCallback, useMemo, useRef, startTransition } from 'react';
import L from 'leaflet';
import { useGeologiaService } from '../hooks/useGeologiaService';
import GeologicalPopup from '../components/mapa/geological/GeologicalPopup';

const GeologiaContext = createContext();

// eslint-disable-next-line react-refresh/only-export-components
export const useGeologia = () => {
  const context = useContext(GeologiaContext);
  if (!context) throw new Error('useGeologia must be used within GeologiaProvider');
  return context;
};

const LAYER_CONFIG = {
  ecuador:   { weight: 3,    color: '#000000', fillColor: 'transparent', fillOpacity: 0,   opacity: 1 },
  provincias:{ weight: 1,    color: '#444444', fillColor: '#ffffff',     fillOpacity: 0.1, opacity: 0.8, dashArray: '10, 5' },
  geologia:  { weight: 0.26, color: '#232323', fillOpacity: 1,           opacity: 1 },
};

const processLayer = (data, layerName) => {
  if (!data?.features) return null;
  return {
    ...data,
    features: data.features.map(f => ({
      ...f,
      layer: layerName,
      id: f.id || `${layerName}_${f.properties?.Gid || Math.random()}`,
    })),
  };
};

export const GeologiaProvider = ({ children }) => {
  const geologiaService = useGeologiaService();
  const [enabled, setEnabled] = useState(false);
  const [layers, setLayers] = useState({ ecuador: null, provincias: null, geologia: null });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [visibleFormations, setVisibleFormations] = useState(new Set());
  const loadedRef = useRef(false);

  // PageMap llama enable() la primera vez que el usuario cambia a una vista GIS.
  // Hasta entonces loading=true mantiene las vistas geológicas en estado de espera.
  const enable = useCallback(() => setEnabled(true), []);

  useEffect(() => {
    if (!enabled || loadedRef.current) return;
    loadedRef.current = true;

    const load = async () => {
      try {
        setLoading(true);
        let anyLoaded = false;

        try {
          const layer = processLayer(await geologiaService.getEcuador(), 'ecuador');
          if (layer) { startTransition(() => setLayers(prev => ({ ...prev, ecuador: layer }))); anyLoaded = true; }
        } catch (err) { console.warn('Error cargando Ecuador:', err.message); }

        try {
          const layer = processLayer(await geologiaService.getProvincias(), 'provincias');
          if (layer) { startTransition(() => setLayers(prev => ({ ...prev, provincias: layer }))); anyLoaded = true; }
        } catch (err) { console.warn('Error cargando Provincias:', err.message); }

        try {
          const layer = processLayer(await geologiaService.getSimplifiedGeologia(0.005), 'geologia');
          if (layer) {
            startTransition(() => {
              setLayers(prev => ({ ...prev, geologia: layer }));
              const all = new Set();
              layer.features.forEach(f => { if (f.properties?.CodA) all.add(f.properties.CodA); });
              setVisibleFormations(all);
            });
            anyLoaded = true;
          }
        } catch (err) { console.warn('Error cargando Geología:', err.message); }

        if (!anyLoaded) throw new Error('No se pudieron cargar datos geológicos desde la API');
      } catch (err) {
        setError(`Error cargando datos: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [enabled]); // eslint-disable-line react-hooks/exhaustive-deps

  const handleFormationToggle = useCallback((codA) => {
    setVisibleFormations(prev => {
      const next = new Set(prev);
      if (next.has(codA)) next.delete(codA); else next.add(codA);
      return next;
    });
  }, []);

  const handleBatchUpdate = useCallback((newSet) => setVisibleFormations(newSet), []);

  const getGeologicalStyle = useCallback((feature) => {
    const layerType = feature.layer || 'geologia';
    const config = LAYER_CONFIG[layerType] || LAYER_CONFIG.geologia;
    const color = layerType === 'geologia' ? feature.properties?.ColorRgb : null;
    return { ...config, fillColor: color || config.fillColor };
  }, []);

  const onEachGeologicalFeature = useCallback((feature, layer) => {
    if (!feature.properties) return;
    if (feature.layer === 'geologia') {
      layer.bindPopup(GeologicalPopup(feature.properties));
      layer.on({
        mouseover: (e) => { e.target.setStyle({ weight: 1.5, color: '#3388ff', fillOpacity: 0.9 }); e.target.openPopup(); },
        mouseout:  (e) => { e.target.setStyle(getGeologicalStyle(feature)); e.target.closePopup(); },
      });
    } else {
      const label = feature.properties.DPA_DESPRO || feature.properties.NOMBRE || 'Ecuador';
      layer.bindTooltip(label, { sticky: true });
      layer.on({
        mouseover: (e) => e.target.setStyle({ ...getGeologicalStyle(feature), weight: 3, color: '#3388ff' }),
        mouseout:  (e) => e.target.setStyle(getGeologicalStyle(feature)),
      });
    }
  }, [getGeologicalStyle]);

  const onEachGeologicalFeatureCombined = useCallback((feature, layer) => {
    if (!feature.properties) return;
    if (feature.layer === 'geologia') {
      layer.bindTooltip(feature.properties.LabelQml || 'Formación geológica', { sticky: true, direction: 'auto', opacity: 0.9 });
      layer.on({
        click:     (e) => L.popup({ maxWidth: 300 }).setLatLng(e.latlng).setContent(GeologicalPopup(feature.properties)).openOn(e.target._map),
        mouseover: (e) => e.target.setStyle({ weight: 1.5, color: '#3388ff', fillOpacity: 0.85 }),
        mouseout:  (e) => e.target.setStyle(getGeologicalStyle(feature)),
      });
    } else {
      const label = feature.properties.DPA_DESPRO || feature.properties.NOMBRE || 'Ecuador';
      layer.bindTooltip(label, { sticky: true });
      layer.on({
        mouseover: (e) => e.target.setStyle({ ...getGeologicalStyle(feature), weight: 3, color: '#3388ff' }),
        mouseout:  (e) => e.target.setStyle(getGeologicalStyle(feature)),
      });
    }
  }, [getGeologicalStyle]);

  const filteredGeologiaData = useMemo(() => {
    if (!layers.geologia) return null;
    return {
      ...layers.geologia,
      features: layers.geologia.features.filter(f => {
        const codA = f.properties?.CodA;
        return codA && visibleFormations.has(codA);
      }),
    };
  }, [layers.geologia, visibleFormations]);

  const value = {
    enable,
    layers,
    loading,
    error,
    visibleFormations,
    filteredGeologiaData,
    geoJsonKey: `geologia-${visibleFormations.size}`,
    handleFormationToggle,
    handleBatchUpdate,
    getGeologicalStyle,
    onEachGeologicalFeature,
    onEachGeologicalFeatureCombined,
  };

  return (
    <GeologiaContext.Provider value={value}>
      {children}
    </GeologiaContext.Provider>
  );
};
