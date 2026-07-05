import { MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import MarkerElement from "./MarkerElement.jsx";
import ElementFilterLegend from "./elements/ElementFilterLegend.jsx";
import MapResizer from "./MapResizer.jsx";
import GeoMapShell from "./geological/GeoMapShell.jsx";
import { MAP_CENTER, MAP_ZOOM, MAP_ATTRIBUTION, MAP_TILE_URL } from "../../constants/mapConstants.js";

const ElementsMapView = ({ elementos = [], filteredElementos = [], onFilteredChange, active = true }) => (
  <GeoMapShell active={active}>
    <MapContainer center={MAP_CENTER} zoom={MAP_ZOOM} style={{ height: '100%' }}>
      <MapResizer active={active} />
      <TileLayer url={MAP_TILE_URL} attribution={MAP_ATTRIBUTION} />
      <MarkerElement elementos={filteredElementos} />
      <ElementFilterLegend elementos={elementos} onFilteredChange={onFilteredChange} />
    </MapContainer>
  </GeoMapShell>
);

export default ElementsMapView;
