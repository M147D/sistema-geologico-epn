// src/components/crud/TableElement.jsx
import { useState, useMemo, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  Box, Paper, Typography, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TablePagination, TableSortLabel, Chip, Button,
  TextField, MenuItem, Select, FormControl, InputLabel, Stack,
  InputAdornment, IconButton,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import VisibilityIcon from '@mui/icons-material/Visibility';
import TableChartIcon from '@mui/icons-material/TableChart';
import MapIcon from '@mui/icons-material/Map';
import { SUBTIPOS_FOSIL, SUBTIPOS_MINERAL, SUBTIPOS_ROCA } from '../../constants/tipoMaps';
import { getSubtipoDisplayLabel } from '../../constants/subtipoColors';
import { getTypeColor } from '../../utils/detailUtils.js';

const SUBTIPOS = {
  Fosil:   SUBTIPOS_FOSIL.map(s => s.label),
  Mineral: SUBTIPOS_MINERAL.map(s => s.label),
  Roca:    SUBTIPOS_ROCA.map(s => s.label),
};

const getPais    = (el) => el.nombrePais      || el.ubicacion?.nombrePais      || '';
const getProv    = (el) => el.nombreProvincia  || el.ubicacion?.nombreProvincia  || '';
const getLocal   = (el) => el.localidad        || el.ubicacion?.localidad        || '';
const getUbicacion = (el) => {
  const parts = [getLocal(el), getProv(el), getPais(el)].filter(Boolean);
  return parts.length > 0 ? parts.join(', ') : '-';
};

const TableElement = ({ elementos, showTypeFilter = true, showLocationFilter = false, onFiltersChange, onVerEnMapa }) => {
  const [searchText,     setSearchText]     = useState('');
  const [filterTipo,     setFilterTipo]     = useState('');
  const [filterSubtipo,  setFilterSubtipo]  = useState('');
  const [filterPais,     setFilterPais]     = useState('');
  const [filterProvincia,setFilterProvincia]= useState('');
  const [filterLocalidad,setFilterLocalidad]= useState('');
  const [page,           setPage]           = useState(0);
  const [rowsPerPage,    setRowsPerPage]    = useState(25);
  const [orderBy,        setOrderBy]        = useState('nombre');
  const [order,          setOrder]          = useState('asc');

  useEffect(() => { setFilterSubtipo('');   }, [filterTipo]);
  useEffect(() => { setFilterProvincia(''); }, [filterPais]);

  // Derivar listas de ubicación del propio array — sin llamadas extra
  const paisOptions = useMemo(() => {
    const names = new Set(elementos.map(getPais).filter(Boolean));
    return [...names].sort();
  }, [elementos]);

  const provinciaOptions = useMemo(() => {
    const source = filterPais ? elementos.filter(el => getPais(el) === filterPais) : elementos;
    const names = new Set(source.map(getProv).filter(Boolean));
    return [...names].sort();
  }, [elementos, filterPais]);

  const elementosFiltrados = useMemo(() => {
    let result = [...elementos];
    if (filterTipo)      result = result.filter(el => el.tipoElemento === filterTipo);
    if (filterSubtipo)   result = result.filter(el => el.tipoEspecifico === filterSubtipo);
    if (filterPais)      result = result.filter(el => getPais(el) === filterPais);
    if (filterProvincia) result = result.filter(el => getProv(el) === filterProvincia);
    if (filterLocalidad) {
      const lc = filterLocalidad.toLowerCase();
      result = result.filter(el => getLocal(el).toLowerCase().includes(lc));
    }
    if (searchText) {
      const s = searchText.toLowerCase();
      result = result.filter(el =>
        el.nombre?.toLowerCase().includes(s) || el.codigo?.toLowerCase().includes(s)
      );
    }
    result.sort((a, b) => {
      let valA = a[orderBy] ?? '';
      let valB = b[orderBy] ?? '';
      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();
      if (valA < valB) return order === 'asc' ? -1 : 1;
      if (valA > valB) return order === 'asc' ?  1 : -1;
      return 0;
    });
    return result;
  }, [elementos, filterTipo, filterSubtipo, filterPais, filterProvincia, filterLocalidad, searchText, orderBy, order]);

  useEffect(() => { setPage(0); }, [searchText, filterTipo, filterSubtipo, filterPais, filterProvincia, filterLocalidad]);

  useEffect(() => {
    if (onFiltersChange) {
      const hayFiltros = !!(searchText || filterTipo || filterSubtipo || filterPais || filterProvincia || filterLocalidad);
      onFiltersChange(elementosFiltrados, hayFiltros);
    }
  }, [elementosFiltrados, onFiltersChange, searchText, filterTipo, filterSubtipo, filterPais, filterProvincia, filterLocalidad]);

  const handleSort = (property) => {
    setOrder(orderBy === property && order === 'asc' ? 'desc' : 'asc');
    setOrderBy(property);
  };

  const handleClear = () => {
    setSearchText(''); setFilterTipo(''); setFilterSubtipo('');
    setFilterPais(''); setFilterProvincia(''); setFilterLocalidad('');
    setPage(0);
  };

  const hasActiveFilters = searchText || filterTipo || filterSubtipo || filterPais || filterProvincia || filterLocalidad;

  const paginatedElementos = elementosFiltrados.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  return (
    <Paper elevation={2} sx={{ overflow: 'hidden' }}>
      {/* Header */}
      <Box sx={{ px: 3, pt: 2.5, pb: 0, borderBottom: 1, borderColor: 'divider' }}>
        {/* Título + búsqueda por nombre */}
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 1, pb: showLocationFilter ? 1.5 : 2 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Box sx={{ width: 4, height: 24, bgcolor: 'primary.main', borderRadius: 1, flexShrink: 0 }} />
            <TableChartIcon color="primary" fontSize="small" />
            <Typography variant="h5" fontWeight={600}>Elementos Geológicos</Typography>
            <Chip
              label={elementosFiltrados.length !== elementos.length
                ? `${elementosFiltrados.length} / ${elementos.length}`
                : elementos.length}
              size="small"
              color="primary"
              variant="outlined"
            />
          </Box>

          <Stack direction="row" spacing={1} flexWrap="wrap">
            <TextField
              size="small"
              placeholder="Buscar por nombre o código..."
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              sx={{ minWidth: 240 }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment>
                ),
                endAdornment: searchText && (
                  <InputAdornment position="end">
                    <IconButton size="small" onClick={() => setSearchText('')}>
                      <ClearIcon fontSize="small" />
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            {showTypeFilter && (
              <FormControl size="small" sx={{ minWidth: 130 }}>
                <InputLabel>Tipo</InputLabel>
                <Select value={filterTipo} label="Tipo" onChange={(e) => setFilterTipo(e.target.value)}>
                  <MenuItem value="">Todos</MenuItem>
                  <MenuItem value="Fosil">Fósil</MenuItem>
                  <MenuItem value="Mineral">Mineral</MenuItem>
                  <MenuItem value="Roca">Roca</MenuItem>
                </Select>
              </FormControl>
            )}
            {showTypeFilter && filterTipo && (
              <FormControl size="small" sx={{ minWidth: 150 }}>
                <InputLabel>Subtipo</InputLabel>
                <Select value={filterSubtipo} label="Subtipo" onChange={(e) => setFilterSubtipo(e.target.value)}>
                  <MenuItem value="">Todos</MenuItem>
                  {(SUBTIPOS[filterTipo] ?? []).map(s => (
                    <MenuItem key={s} value={s}>{getSubtipoDisplayLabel(s)}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            )}
            {hasActiveFilters && (
              <Button size="small" variant="outlined" onClick={handleClear} startIcon={<ClearIcon />}>
                Limpiar
              </Button>
            )}
          </Stack>
        </Box>

        {/* Fila de filtros de ubicación + botón mapa */}
        {showLocationFilter && (
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, pb: 2, alignItems: 'center' }}>
            <FormControl size="small" sx={{ minWidth: 150 }}>
              <InputLabel>País</InputLabel>
              <Select value={filterPais} label="País" onChange={(e) => setFilterPais(e.target.value)}>
                <MenuItem value="">Todos</MenuItem>
                {paisOptions.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
              </Select>
            </FormControl>
            <FormControl size="small" sx={{ minWidth: 160 }} disabled={!filterPais}>
              <InputLabel>Provincia</InputLabel>
              <Select value={filterProvincia} label="Provincia" onChange={(e) => setFilterProvincia(e.target.value)}>
                <MenuItem value="">Todas</MenuItem>
                {provinciaOptions.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
              </Select>
            </FormControl>
            <TextField
              label="Localidad" size="small" sx={{ minWidth: 160 }}
              value={filterLocalidad}
              onChange={(e) => setFilterLocalidad(e.target.value)}
            />
            {onVerEnMapa && (
              <Button
                variant="outlined"
                size="small"
                startIcon={<MapIcon />}
                onClick={onVerEnMapa}
                sx={{ ml: 'auto' }}
              >
                Ver en el Mapa
                <Chip label={elementosFiltrados.length} size="small" sx={{ ml: 1 }} />
              </Button>
            )}
          </Box>
        )}
      </Box>

      {/* Tabla */}
      <TableContainer sx={{ maxHeight: showLocationFilter ? 'calc(100vh - 340px)' : 'calc(100vh - 280px)' }}>
        <Table stickyHeader size="small">
          <TableHead>
            <TableRow>
              {[
                { id: 'nombre', label: 'Nombre' },
                { id: 'codigo', label: 'Código' },
              ].map(({ id, label }) => (
                <TableCell key={id}>
                  <TableSortLabel
                    active={orderBy === id}
                    direction={orderBy === id ? order : 'asc'}
                    onClick={() => handleSort(id)}
                  >
                    <strong>{label}</strong>
                  </TableSortLabel>
                </TableCell>
              ))}
              <TableCell><strong>Tipo</strong></TableCell>
              <TableCell><strong>Subtipo</strong></TableCell>
              {(!filterTipo || filterTipo === 'Fosil') && (
                <TableCell><strong>Especie</strong></TableCell>
              )}
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'edad'}
                  direction={orderBy === 'edad' ? order : 'asc'}
                  onClick={() => handleSort('edad')}
                >
                  <strong>Edad</strong>
                </TableSortLabel>
              </TableCell>
              <TableCell><strong>Donante</strong></TableCell>
              <TableCell><strong>Ubicación</strong></TableCell>
              <TableCell><strong>Ejemplares</strong></TableCell>
              <TableCell align="center"><strong>Acciones</strong></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedElementos.length === 0 ? (
              <TableRow>
                <TableCell colSpan={10} align="center" sx={{ py: 6 }}>
                  <Typography variant="body2" color="text.secondary">
                    {elementos.length === 0
                      ? 'No hay elementos cargados'
                      : 'No se encontraron elementos con los filtros aplicados'}
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              paginatedElementos.map((elemento) => (
                <TableRow key={elemento.id} hover sx={{ '&:last-child td': { border: 0 } }}>
                  <TableCell>
                    <Typography variant="body2" noWrap sx={{ maxWidth: 200 }}>
                      {elemento.nombre || 'Sin nombre'}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" fontFamily="monospace" fontSize="0.8rem">
                      {elemento.codigo || '-'}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip label={elemento.tipoElemento} color={getTypeColor(elemento.tipoElemento)} size="small" />
                  </TableCell>
                  <TableCell>
                    <Chip label={getSubtipoDisplayLabel(elemento.tipoEspecifico) || '-'} size="small" variant="outlined" color={getTypeColor(elemento.tipoElemento)} />
                  </TableCell>
                  {(!filterTipo || filterTipo === 'Fosil') && (
                    <TableCell>
                      <Typography variant="body2" noWrap sx={{ maxWidth: 150 }}>
                        {elemento.tipoElemento === 'Fosil' ? (elemento.especie || '-') : '-'}
                      </Typography>
                    </TableCell>
                  )}
                  <TableCell>{elemento.edad || '-'}</TableCell>
                  <TableCell>
                    <Typography variant="body2" noWrap sx={{ maxWidth: 120 }}>{elemento.donante || '-'}</Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" noWrap sx={{ maxWidth: 200 }}>{getUbicacion(elemento)}</Typography>
                  </TableCell>
                  <TableCell align="center">{elemento.ejemplares ?? '-'}</TableCell>
                  <TableCell align="center">
                    <Button
                      variant="contained"
                      size="small"
                      component={Link}
                      to={`/detalle/${elemento.id}`}
                      state={{ elemento }}
                      startIcon={<VisibilityIcon />}
                    >
                      Detalle
                    </Button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={elementosFiltrados.length}
        page={page}
        onPageChange={(_, newPage) => setPage(newPage)}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={(e) => { setRowsPerPage(parseInt(e.target.value, 10)); setPage(0); }}
        rowsPerPageOptions={[10, 25, 50, 100]}
        labelRowsPerPage="Filas por página:"
        labelDisplayedRows={({ from, to, count }) => `${from}-${to} de ${count}`}
      />
    </Paper>
  );
};

export default TableElement;
