import React, { useState, useEffect, useCallback } from 'react';
import { TextField, InputAdornment, IconButton, Box } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';

/**
 * Barra de búsqueda para filtrar formaciones geológicas
 * Usa debouncing interno para evitar re-renders excesivos
 * @param {string} searchQuery - Texto de búsqueda actual (controlado por padre)
 * @param {Function} onSearchChange - Callback cuando cambia el texto
 * @param {number} resultCount - Número de resultados encontrados (opcional)
 * @param {number} debounceMs - Tiempo de debounce en ms (default: 300)
 */
const FormationSearchBar = React.memo(({
  searchQuery,
  onSearchChange,
  resultCount,
  debounceMs = 300
}) => {
  // Estado local para el input (respuesta inmediata)
  const [localValue, setLocalValue] = useState(searchQuery);

  // Sincronizar con prop externa cuando cambia (ej: al limpiar desde fuera)
  useEffect(() => {
    setLocalValue(searchQuery);
  }, [searchQuery]);

  // Debounce: notificar al padre después del delay
  useEffect(() => {
    const timer = setTimeout(() => {
      if (localValue !== searchQuery) {
        onSearchChange(localValue);
      }
    }, debounceMs);

    return () => clearTimeout(timer);
  }, [localValue, debounceMs, onSearchChange, searchQuery]);

  const handleClear = useCallback(() => {
    setLocalValue('');
    onSearchChange('');
  }, [onSearchChange]);

  const handleChange = useCallback((e) => {
    setLocalValue(e.target.value);
  }, []);

  return (
    <Box sx={{ p: 1, bgcolor: '#fafafa', borderBottom: '1px solid rgba(0,0,0,0.08)' }}>
      <TextField
        fullWidth
        size="small"
        placeholder="Buscar formación..."
        value={localValue}
        onChange={handleChange}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchIcon fontSize="small" color="action" />
            </InputAdornment>
          ),
          endAdornment: searchQuery && (
            <InputAdornment position="end">
              <IconButton
                size="small"
                onClick={handleClear}
                edge="end"
                aria-label="Limpiar búsqueda"
                sx={{ p: 0.5 }}
              >
                <ClearIcon fontSize="small" />
              </IconButton>
            </InputAdornment>
          )
        }}
        sx={{
          '& .MuiOutlinedInput-root': {
            fontSize: '0.8rem',
            bgcolor: 'white',
            '& fieldset': {
              borderColor: 'rgba(0,0,0,0.12)'
            },
            '&:hover fieldset': {
              borderColor: 'primary.main'
            },
            '&.Mui-focused fieldset': {
              borderColor: 'primary.main',
              borderWidth: 1
            }
          },
          '& .MuiInputBase-input': {
            py: 0.75
          }
        }}
      />

      {/* Contador de resultados */}
      {searchQuery && resultCount !== undefined && (
        <Box
          sx={{
            mt: 0.5,
            fontSize: '0.65rem',
            color: 'text.secondary',
            textAlign: 'right'
          }}
        >
          {resultCount} {resultCount === 1 ? 'resultado' : 'resultados'}
        </Box>
      )}
    </Box>
  );
});

FormationSearchBar.displayName = 'FormationSearchBar';

export default FormationSearchBar;
