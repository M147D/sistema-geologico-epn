import { useEffect, useRef, useState, useMemo, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { useMap } from 'react-leaflet';
import L from 'leaflet';
import {
  Paper, Typography, Box, IconButton, Grow, Fab, Tooltip, Badge,
  TextField, InputAdornment, FormControl, InputLabel, Select, MenuItem,
  List, ListItem, ListItemButton, ListItemIcon, ListItemText, Checkbox, Chip
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import ClearIcon from '@mui/icons-material/Clear';
import SearchIcon from '@mui/icons-material/Search';
import FilterListIcon from '@mui/icons-material/FilterList';
import FilterListOffIcon from '@mui/icons-material/FilterListOff';
import { TIPO_COLORS, getSubtipoColor, getSubtipoEntries } from '../../../constants/subtipoColors';

const TIPOS = [
  { value: 'Fosil',   label: 'Fosil',   color: TIPO_COLORS.Fosil   },
  { value: 'Mineral', label: 'Mineral', color: TIPO_COLORS.Mineral },
  { value: 'Roca',    label: 'Roca',    color: TIPO_COLORS.Roca    },
];


/**
 * Filtro de elementos geologicos como control de Leaflet (estilo GeologicalLegend).
 * Se posiciona en bottomleft junto a la leyenda geologica.
 *
 * @param {Array} elementos - Array completo de elementos
 * @param {Function} onFilteredChange - Callback con los elementos filtrados
 */
const ElementFilterLegend = ({ elementos = [], onFilteredChange }) => {
  const map = useMap();
  const containerRef = useRef(null);
  const [isOpen, setIsOpen] = useState(false);

  // Filter state
  const [searchQuery, setSearchQuery] = useState('');
  const [localSearch, setLocalSearch] = useState('');
  const [selectedTipos, setSelectedTipos] = useState(new Set(['Fosil', 'Mineral', 'Roca']));
  const [selectedProvincia, setSelectedProvincia] = useState('');
  const [selectedSubtipo, setSelectedSubtipo] = useState('');

  // Extract unique provincias from elements
  const provincias = useMemo(() => {
    const set = new Set();
    elementos.forEach(el => {
      const prov = el.ubicacion?.nombreProvincia || el.nombreProvincia;
      if (prov) set.add(prov);
    });
    return Array.from(set).sort();
  }, [elementos]);

  // Debounce search
  useEffect(() => {
    const timer = setTimeout(() => setSearchQuery(localSearch), 300);
    return () => clearTimeout(timer);
  }, [localSearch]);

  // Apply filters
  const filteredElements = useMemo(() => {
    return elementos.filter(el => {
      // Type filter
      if (!selectedTipos.has(el.tipoElemento)) return false;

      // Provincia filter
      if (selectedProvincia) {
        const prov = el.ubicacion?.nombreProvincia || el.nombreProvincia || '';
        if (prov !== selectedProvincia) return false;
      }

      // Subtipo filter — tipoEspecifico viene del backend como string (enum.ToString())
      if (selectedSubtipo && el.tipoEspecifico !== selectedSubtipo) return false;

      // Name search
      if (searchQuery) {
        const q = searchQuery.toLowerCase();
        const nombre = (el.nombre || '').toLowerCase();
        const codigo = (el.codigo || '').toLowerCase();
        if (!nombre.includes(q) && !codigo.includes(q)) return false;
      }

      return true;
    });
  }, [elementos, selectedTipos, selectedProvincia, selectedSubtipo, searchQuery]);

  // Notify parent of filtered results
  useEffect(() => {
    onFilteredChange(filteredElements);
  }, [filteredElements, onFilteredChange]);

  // Type toggle handler
  const handleTipoToggle = useCallback((tipo) => {
    setSelectedTipos(prev => {
      const next = new Set(prev);
      if (next.has(tipo)) next.delete(tipo);
      else next.add(tipo);
      return next;
    });
    setSelectedSubtipo('');
  }, []);

  const handleClearSearch = useCallback(() => {
    setLocalSearch('');
    setSearchQuery('');
  }, []);

  const handleClearAllFilters = useCallback(() => {
    setLocalSearch('');
    setSearchQuery('');
    setSelectedTipos(new Set(['Fosil', 'Mineral', 'Roca']));
    setSelectedProvincia('');
    setSelectedSubtipo('');
  }, []);

  // Determine active subtipo type (only show subtipo if exactly 1 tipo selected)
  const singleTipo = selectedTipos.size === 1 ? Array.from(selectedTipos)[0] : null;

  const activeFilterCount = useMemo(() => {
    let count = 0;
    if (selectedTipos.size < 3) count++;
    if (selectedProvincia) count++;
    if (selectedSubtipo) count++;
    if (searchQuery) count++;
    return count;
  }, [selectedTipos, selectedProvincia, selectedSubtipo, searchQuery]);

  // Create Leaflet control
  useEffect(() => {
    const control = L.control({ position: 'bottomright' });

    control.onAdd = function () {
      const div = L.DomUtil.create('div', 'element-filter-legend-mui');
      div.style.border = 'none';
      div.style.backgroundColor = 'transparent';
      containerRef.current = div;

      L.DomEvent.disableClickPropagation(div);
      L.DomEvent.disableScrollPropagation(div);

      return div;
    };

    control.addTo(map);

    return () => {
      control.remove();
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
      {/* Panel */}
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
          {/* Header */}
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
              FILTRO DE ELEMENTOS
            </Typography>
            <IconButton
              size="small"
              onClick={() => setIsOpen(false)}
              sx={{ color: 'inherit' }}
              aria-label="Cerrar filtro"
            >
              <CloseIcon fontSize="small" />
            </IconButton>
          </Box>

          {/* Scrollable content */}
          <Box sx={{ overflowY: 'auto', flex: 1 }}>
            {/* Search bar */}
            <Box sx={{ p: 1, bgcolor: '#fafafa', borderBottom: '1px solid rgba(0,0,0,0.08)' }}>
              <TextField
                fullWidth
                size="small"
                placeholder="Buscar por nombre..."
                value={localSearch}
                onChange={(e) => setLocalSearch(e.target.value)}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon fontSize="small" color="action" />
                    </InputAdornment>
                  ),
                  endAdornment: localSearch && (
                    <InputAdornment position="end">
                      <IconButton size="small" onClick={handleClearSearch} edge="end" sx={{ p: 0.5 }}>
                        <ClearIcon fontSize="small" />
                      </IconButton>
                    </InputAdornment>
                  )
                }}
                sx={{
                  '& .MuiOutlinedInput-root': {
                    fontSize: '0.8rem',
                    bgcolor: 'white',
                    '& fieldset': { borderColor: 'rgba(0,0,0,0.12)' },
                    '&:hover fieldset': { borderColor: 'primary.main' },
                    '&.Mui-focused fieldset': { borderColor: 'primary.main', borderWidth: 1 }
                  },
                  '& .MuiInputBase-input': { py: 0.75 }
                }}
              />
              {searchQuery && (
                <Box sx={{ mt: 0.5, fontSize: '0.65rem', color: 'text.secondary', textAlign: 'right' }}>
                  {filteredElements.length} {filteredElements.length === 1 ? 'resultado' : 'resultados'}
                </Box>
              )}
            </Box>

            {/* Type checkboxes */}
            <Box sx={{ px: 1, pt: 1, pb: 0.5 }}>
              <Typography variant="caption" fontWeight="600" color="text.secondary" sx={{ px: 0.5 }}>
                Tipo de elemento
              </Typography>
              <List dense sx={{ py: 0 }}>
                {TIPOS.map((tipo) => (
                  <ListItem key={tipo.value} disablePadding sx={{ py: 0 }}>
                    <ListItemButton
                      dense
                      onClick={() => handleTipoToggle(tipo.value)}
                      sx={{ py: 0.25, px: 0.5, borderRadius: 1 }}
                    >
                      <ListItemIcon sx={{ minWidth: 32 }}>
                        <Checkbox
                          edge="start"
                          checked={selectedTipos.has(tipo.value)}
                          size="small"
                          sx={{ p: 0.25 }}
                        />
                      </ListItemIcon>
                      <Box
                        sx={{
                          width: 10,
                          height: 10,
                          bgcolor: tipo.color,
                          borderRadius: '50%',
                          mr: 1,
                          boxShadow: 1
                        }}
                      />
                      <ListItemText
                        primary={tipo.label}
                        primaryTypographyProps={{ variant: 'caption', fontSize: '0.75rem' }}
                      />
                    </ListItemButton>
                  </ListItem>
                ))}
              </List>
            </Box>

            {/* Provincia filter */}
            <Box sx={{ px: 1.5, pb: 1 }}>
              <FormControl fullWidth size="small">
                <InputLabel id="el-filter-provincia" sx={{ fontSize: '0.8rem' }}>
                  Provincia
                </InputLabel>
                <Select
                  labelId="el-filter-provincia"
                  value={selectedProvincia}
                  label="Provincia"
                  onChange={(e) => setSelectedProvincia(e.target.value)}
                  sx={{ fontSize: '0.8rem' }}
                  MenuProps={{ disablePortal: true }}
                >
                  <MenuItem value="">
                    <em>Todas</em>
                  </MenuItem>
                  {provincias.map(p => (
                    <MenuItem key={p} value={p} sx={{ fontSize: '0.8rem' }}>{p}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Box>

            {/* Subtipo filter (only when 1 tipo selected) */}
            {singleTipo && (
              <Box sx={{ px: 1.5, pb: 1 }}>
                <FormControl fullWidth size="small">
                  <InputLabel id="el-filter-subtipo" sx={{ fontSize: '0.8rem' }}>
                    Subtipo
                  </InputLabel>
                  <Select
                    labelId="el-filter-subtipo"
                    value={selectedSubtipo}
                    label="Subtipo"
                    onChange={(e) => setSelectedSubtipo(e.target.value)}
                    sx={{ fontSize: '0.8rem' }}
                    MenuProps={{ disablePortal: true }}
                  >
                    <MenuItem value="">
                      <em>Todos</em>
                    </MenuItem>
                    {getSubtipoEntries(singleTipo).map(s => (
                      <MenuItem key={s.value} value={s.value} sx={{ fontSize: '0.8rem', display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Box
                          component="span"
                          sx={{
                            width: 9, height: 9, borderRadius: '50%', flexShrink: 0,
                            bgcolor: getSubtipoColor(singleTipo, s.value),
                          }}
                        />
                        {s.label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Box>
            )}

            {/* Footer: clear button + results count */}
            <Box
              sx={{
                px: 1.5,
                py: 0.75,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                bgcolor: '#f8f9fa',
                borderTop: '1px solid rgba(0,0,0,0.08)'
              }}
            >
              {activeFilterCount > 0 ? (
                <Chip
                  label="Limpiar filtros"
                  size="small"
                  icon={<FilterListOffIcon sx={{ fontSize: '0.85rem !important' }} />}
                  onClick={handleClearAllFilters}
                  sx={{ fontSize: '0.7rem', height: 22, cursor: 'pointer' }}
                />
              ) : (
                <Box />
              )}
              <Typography variant="caption" color="text.secondary">
                {filteredElements.length} de {elementos.length}
              </Typography>
            </Box>
          </Box>
        </Paper>
      </Grow>

      {/* FAB toggle button */}
      {!isOpen && (
        <Tooltip title="Filtrar Elementos" placement="left">
          {/* Badge exterior: conteo N/total a la izquierda del FAB (mismo patrón que LegendToggleButton) */}
          <Badge
            badgeContent={`${filteredElements.length}/${elementos.length}`}
            color="primary"
            max={9999}
            sx={{
              '& .MuiBadge-badge': {
                fontSize: '0.7rem',
                height: 20,
                minWidth: 20,
                padding: '0 6px',
                top: 0,
                right: 55,
                border: '2px solid #fff',
                boxSizing: 'content-box',
              }
            }}
          >
            {/* Badge interior: indicador de filtros activos en esquina superior derecha */}
            <Badge
              badgeContent={activeFilterCount > 0 ? activeFilterCount : null}
              color="secondary"
              sx={{
                '& .MuiBadge-badge': {
                  fontSize: '0.7rem',
                  height: 18,
                  minWidth: 18,
                  top: -2,
                  right: -2
                }
              }}
            >
              <Fab
                color="default"
                size="small"
                onClick={() => setIsOpen(true)}
                sx={{
                  bgcolor: 'white',
                  color: 'text.secondary',
                  boxShadow: 3,
                  transition: 'all 0.3s',
                  '&:hover': {
                    bgcolor: '#f5f5f5',
                    transform: 'scale(1.1)',
                    boxShadow: 4
                  }
                }}
              >
                <FilterListIcon fontSize="small" />
              </Fab>
            </Badge>
          </Badge>
        </Tooltip>
      )}
    </Box>,
    containerRef.current
  );
};

export default ElementFilterLegend;