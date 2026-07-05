import { MapContainer, TileLayer, GeoJSON, Pane } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import GeologicalLegend from './GeologicalLegend';
import CustomLayerControl from './CustomLayerControl';
import GeoMapShell from './GeoMapShell';
import MapResizer from '../MapResizer';
import { MAP_CENTER, MAP_ZOOM, MAP_ATTRIBUTION, MAP_TILE_URL } from '../../../constants/mapConstants';
import { useGeologia } from '../../../context/GeologiaContext';

const GeologicalMapView = ({ active = true, layerVisibility, onToggleLayer }) => {
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
    onEachGeologicalFeature,
  } = useGeologia();

  return (
    <GeoMapShell active={active} loading={loading} error={error}>
      <MapContainer center={MAP_CENTER} zoom={MAP_ZOOM} style={{ height: '100%', width: '100%' }}>
        <MapResizer active={active} />
        {/* Panes: geología(250) < límites(300) < overlayPane(400) < markerPane(600) */}
        <Pane name="geologia-pane" style={{ zIndex: 250 }} />
        <Pane name="boundaries-pane" style={{ zIndex: 300 }} />

        <TileLayer attribution={MAP_ATTRIBUTION} url={MAP_TILE_URL} />

        {filteredGeologiaData && layerVisibility.geologia && (
          <GeoJSON
            key={geoJsonKey}
            data={filteredGeologiaData}
            style={getGeologicalStyle}
            onEachFeature={onEachGeologicalFeature}
            pane="geologia-pane"
          />
        )}

        {layers.provincias && layerVisibility.provincias && (
          <GeoJSON
            data={layers.provincias}
            style={getGeologicalStyle}
            onEachFeature={onEachGeologicalFeature}
            pane="boundaries-pane"
          />
        )}

        {layers.ecuador && layerVisibility.ecuador && (
          <GeoJSON
            data={layers.ecuador}
            style={getGeologicalStyle}
            onEachFeature={onEachGeologicalFeature}
            pane="boundaries-pane"
          />
        )}

        <CustomLayerControl visibility={layerVisibility} onToggle={onToggleLayer} />

        <GeologicalLegend
          geologiaData={layers.geologia}
          visibleFormations={visibleFormations}
          onFormationToggle={handleFormationToggle}
          onBatchUpdate={handleBatchUpdate}
        />
      </MapContainer>
    </GeoMapShell>
  );
};

export default GeologicalMapView;
