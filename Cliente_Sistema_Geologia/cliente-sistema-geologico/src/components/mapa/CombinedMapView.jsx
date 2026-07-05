import { MapContainer, TileLayer, GeoJSON, Pane } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import GeologicalLegend from './geological/GeologicalLegend';
import CustomLayerControl from './geological/CustomLayerControl';
import GeoMapShell from './geological/GeoMapShell';
import MarkerElement from './MarkerElement';
import ElementFilterLegend from './elements/ElementFilterLegend.jsx';
import MapResizer from './MapResizer';
import { MAP_CENTER, MAP_ZOOM, MAP_ATTRIBUTION, MAP_TILE_URL } from '../../constants/mapConstants.js';
import { useGeologia } from '../../context/GeologiaContext';

const CombinedMapView = ({ elementos = [], filteredElementos = [], onFilteredChange, active = true, layerVisibility, onToggleLayer }) => {
  const {
    layers,
    loading,
    error,
    visibleFormations,
    filteredGeologiaData,
    geoJsonKey,
    handleFormationToggle,
    handleBatchUpdate,
    getGeologicalStyle,
    onEachGeologicalFeatureCombined,
  } = useGeologia();

  return (
    <GeoMapShell active={active} loading={loading} error={error}>
      <MapContainer center={MAP_CENTER} zoom={MAP_ZOOM} style={{ height: '100%', width: '100%' }}>
        <MapResizer active={active} />
        {/* Panes ordenados: geología < límites < marcadores(600) < popups(700) */}
        <Pane name="geologia-pane" style={{ zIndex: 250 }} />
        <Pane name="boundaries-pane" style={{ zIndex: 300 }} />

        <TileLayer attribution={MAP_ATTRIBUTION} url={MAP_TILE_URL} />

        {/* onEachGeologicalFeatureCombined: solo tooltip en hover, sin openPopup
            para no interferir con los popups de marcadores */}
        {filteredGeologiaData && layerVisibility.geologia && (
          <GeoJSON
            key={`combined-${geoJsonKey}`}
            data={filteredGeologiaData}
            style={getGeologicalStyle}
            onEachFeature={onEachGeologicalFeatureCombined}
            pane="geologia-pane"
          />
        )}

        {/* interactive=false: bordes provinciales son solo visuales, no capturan clicks */}
        {layers.provincias && layerVisibility.provincias && (
          <GeoJSON
            data={layers.provincias}
            style={getGeologicalStyle}
            pane="boundaries-pane"
            interactive={false}
          />
        )}

        {/* interactive=false: borde nacional es solo referencia visual */}
        {layers.ecuador && layerVisibility.ecuador && (
          <GeoJSON
            data={layers.ecuador}
            style={getGeologicalStyle}
            pane="boundaries-pane"
            interactive={false}
          />
        )}

        <MarkerElement elementos={filteredElementos} />

        <CustomLayerControl visibility={layerVisibility} onToggle={onToggleLayer} />

        <GeologicalLegend
          geologiaData={layers.geologia}
          visibleFormations={visibleFormations}
          onFormationToggle={handleFormationToggle}
          onBatchUpdate={handleBatchUpdate}
        />

        <ElementFilterLegend
          elementos={elementos}
          onFilteredChange={onFilteredChange}
        />
      </MapContainer>
    </GeoMapShell>
  );
};

export default CombinedMapView;
