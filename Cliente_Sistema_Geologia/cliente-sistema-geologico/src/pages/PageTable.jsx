// src/pages/PageTable.jsx
import { useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Container, CircularProgress, Alert } from '@mui/material';
import TableElement from '../components/crud/TableElement.jsx';
import { useElementos } from '../context/ElementosContext.jsx';

const PageTable = () => {
  const navigate = useNavigate();
  const { elementos, loading, error, sincronizarResultados, limpiarFiltros } = useElementos();

  const handleFiltersChange = useCallback((filtrados, hayFiltros) => {
    if (hayFiltros) {
      sincronizarResultados(filtrados);
    } else {
      limpiarFiltros();
    }
  }, [sincronizarResultados, limpiarFiltros]);

  if (loading && elementos.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="60vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ my: 4 }}>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <TableElement
        elementos={elementos}
        showTypeFilter
        showLocationFilter
        onFiltersChange={handleFiltersChange}
        onVerEnMapa={() => navigate('/mapa')}
      />
    </Container>
  );
};

export default PageTable;