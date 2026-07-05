import { useMemo, useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { useMap } from 'react-leaflet';
import L from 'leaflet';
import { Paper, Typography, Box, Divider, IconButton, Grow } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

// Importar componentes modulares
import FormationList from './legend/FormationList';
import LegendToggleButton from './legend/LegendToggleButton';

/**
 * Componente principal de la leyenda geológica
 * Orquesta los componentes modulares y gestiona el estado
 */
const GeologicalLegend = ({
  geologiaData,
  visibleFormations,
  onFormationToggle,
  onBatchUpdate
}) => {
  const map = useMap();
  const containerRef = useRef(null);
  const [isOpen, setIsOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  // Extraer y procesar formaciones únicas
  const formations = useMemo(() => {
    if (!geologiaData || !geologiaData.features) return [];

    const formationMap = new Map();

    geologiaData.features.forEach(feature => {
      const { CodA, ColorRgb, LabelQml } = feature.properties;
      if (CodA && ColorRgb && LabelQml) {
        if (!formationMap.has(CodA)) {
          formationMap.set(CodA, {
            codA: CodA,
            color: ColorRgb,
            label: LabelQml
          });
        }
      }
    });

    return Array.from(formationMap.values()).sort((a, b) =>
      a.label.localeCompare(b.label)
    );
  }, [geologiaData]);

  // Filtrar formaciones según búsqueda
  const filteredFormations = useMemo(() => {
    if (!searchQuery.trim()) return formations;

    const query = searchQuery.toLowerCase().trim();
    return formations.filter(formation =>
      formation.label.toLowerCase().includes(query) ||
      formation.codA.toLowerCase().includes(query)
    );
  }, [formations, searchQuery]);

  // Handlers para selección masiva
  const handleSelectAll = () => {
    if (onBatchUpdate) {
      const allIds = new Set(formations.map(f => f.codA));
      onBatchUpdate(allIds);
    }
  };

  const handleDeselectAll = () => {
    if (onBatchUpdate) {
      onBatchUpdate(new Set());
    }
  };

  // Crear control de Leaflet
  useEffect(() => {
    const legend = L.control({ position: 'bottomright' });

    legend.onAdd = function () {
      const div = L.DomUtil.create('div', 'geological-legend-mui');
      div.style.border = 'none';
      div.style.backgroundColor = 'transparent';
      containerRef.current = div;

      L.DomEvent.disableClickPropagation(div);
      L.DomEvent.disableScrollPropagation(div);

      return div;
    };

    legend.addTo(map);

    return () => {
      legend.remove();
      containerRef.current = null;
    };
  }, [map]);

  if (!containerRef.current) return null;

  return createPortal(
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'flex-end',
        gap: 1
      }}
    >
      {/* Panel principal de la leyenda */}
      <Grow in={isOpen} mountOnEnter unmountOnExit>
        <Paper
          elevation={4}
          sx={{
            width: 250,
            maxHeight: '45vh',
            backgroundColor: 'rgba(255, 255, 255, 0.98)',
            overflow: 'hidden',
            display: 'flex',
            flexDirection: 'column',
            mb: 1,
            borderRadius: 2
          }}
        >
          {/* Header con título y botón cerrar */}
          <Box
            sx={{
              p: 1,
              pl: 2,
              borderBottom: 1,
              borderColor: 'divider',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              bgcolor: 'primary.main',
              color: 'primary.contrastText'
            }}
          >
            <Typography variant="subtitle2" fontWeight="bold">
              LEYENDA GEOLÓGICA
            </Typography>
            <IconButton
              size="small"
              onClick={() => setIsOpen(false)}
              sx={{ color: 'inherit' }}
              aria-label="Cerrar leyenda"
            >
              <CloseIcon fontSize="small" />
            </IconButton>
          </Box>

          {/* Contenido scrollable */}
          <Box sx={{ overflowY: 'auto', flex: 1 }}>
            {/* Lista de formaciones geológicas */}
            <FormationList
              formations={filteredFormations}
              visibleFormations={visibleFormations}
              onFormationToggle={onFormationToggle}
              onSelectAll={handleSelectAll}
              onDeselectAll={handleDeselectAll}
              searchQuery={searchQuery}
              onSearchChange={setSearchQuery}
            />

            <Divider />
          </Box>
        </Paper>
      </Grow>

      {/* Botón flotante para abrir la leyenda */}
      <LegendToggleButton
        isOpen={isOpen}
        onToggle={() => setIsOpen(true)}
        selectedCount={visibleFormations.size}
        totalCount={formations.length}
      />
    </Box>,
    containerRef.current
  );
};

export default GeologicalLegend;