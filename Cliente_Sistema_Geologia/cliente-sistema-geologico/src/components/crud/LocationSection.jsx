import { useState, useEffect } from 'react';
import {
  TextField, Stack, Typography, Box,
  FormControl, InputLabel, Select, MenuItem, FormHelperText
} from '@mui/material';
import { Controller } from 'react-hook-form';
import { useElementos } from '../../context/ElementosContext';

const SectionHeader = ({ children }) => (
  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
    <Box sx={{ width: 4, height: 20, bgcolor: 'success.main', borderRadius: 1, mr: 1.5, flexShrink: 0 }} />
    <Typography variant="subtitle1" fontWeight={600}>{children}</Typography>
  </Box>
);

const LocationSection = ({ register, control, errors }) => {
  const { paises, provincias, cargarProvinciasPorPais } = useElementos();
  const [paisSeleccionado, setPaisSeleccionado] = useState('');

  useEffect(() => {
    if (paisSeleccionado) {
      cargarProvinciasPorPais(paisSeleccionado);
    }
  }, [paisSeleccionado, cargarProvinciasPorPais]);

  return (
    <Box>
      <SectionHeader>Ubicación</SectionHeader>

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }}>
        <FormControl fullWidth error={!!errors?.nombrePais} required>
          <InputLabel id="pais-label">País *</InputLabel>
          <Controller
            name="nombrePais"
            control={control}
            rules={{ required: 'El país es obligatorio' }}
            render={({ field }) => (
              <Select
                labelId="pais-label"
                {...field}
                label="País *"
                onChange={(e) => {
                  field.onChange(e);
                  setPaisSeleccionado(e.target.value);
                }}
              >
                <MenuItem value=""><em>Seleccione un país</em></MenuItem>
                {paises?.length > 0 ? (
                  paises.map(pais => (
                    <MenuItem key={pais.id} value={pais.nombrePais}>{pais.nombrePais}</MenuItem>
                  ))
                ) : (
                  <MenuItem disabled><em>No hay países disponibles</em></MenuItem>
                )}
              </Select>
            )}
          />
          {errors?.nombrePais && <FormHelperText>{errors.nombrePais.message}</FormHelperText>}
        </FormControl>

        <FormControl fullWidth disabled={!paisSeleccionado}>
          <InputLabel id="provincia-label">Provincia (opcional)</InputLabel>
          <Controller
            name="nombreProvincia"
            control={control}
            render={({ field }) => (
              <Select labelId="provincia-label" {...field} label="Provincia (opcional)">
                <MenuItem value=""><em>No especificar</em></MenuItem>
                {provincias?.length > 0 ? (
                  provincias.map(p => (
                    <MenuItem key={p.id} value={p.nombreProvincia}>{p.nombreProvincia}</MenuItem>
                  ))
                ) : (
                  <MenuItem disabled><em>Seleccione primero un país</em></MenuItem>
                )}
              </Select>
            )}
          />
          <FormHelperText>
            {!paisSeleccionado
              ? 'Seleccione un país para cargar provincias'
              : provincias?.length === 0
              ? 'No hay provincias para este país'
              : 'Opcional'}
          </FormHelperText>
        </FormControl>
      </Stack>

      <TextField
        label="Localidad"
        fullWidth
        placeholder="Ej: Buenos Aires"
        {...register("localidad")}
        sx={{ mb: 2 }}
      />

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }}>
        <TextField
          label="Latitud"
          fullWidth
          placeholder="Ej: -34.6037"
          {...register("latitud")}
          error={!!errors?.latitud}
          helperText={errors?.latitud?.message}
        />
        <TextField
          label="Longitud"
          fullWidth
          placeholder="Ej: -58.3816"
          {...register("longitud")}
          error={!!errors?.longitud}
          helperText={errors?.longitud?.message}
        />
      </Stack>

      <TextField
        label="Leyenda"
        fullWidth
        placeholder="Información adicional de ubicación"
        {...register("leyenda")}
      />
    </Box>
  );
};

export default LocationSection;
