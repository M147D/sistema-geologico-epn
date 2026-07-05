// src/components/crud/BasicInfoSection.jsx
import { TextField, Stack, Typography, Box, FormControlLabel, Checkbox, FormControl, InputLabel, Select, MenuItem } from '@mui/material';
import { Controller } from 'react-hook-form';

const SectionHeader = ({ children }) => (
  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
    <Box sx={{ width: 4, height: 20, bgcolor: 'primary.main', borderRadius: 1, mr: 1.5, flexShrink: 0 }} />
    <Typography variant="subtitle1" fontWeight={600}>{children}</Typography>
  </Box>
);

const BasicInfoSection = ({ tipo, register, errors, control }) => (
  <Box>
    <SectionHeader>Información Básica</SectionHeader>

    {/* Fila tipo-específica */}
    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }}>
      {tipo === "fosil" ? (
        <>
          <TextField
            label="Especie"
            fullWidth
            {...register("especie")}
            error={!!errors.especie}
            helperText={errors.especie?.message}
          />
          <TextField
            label="Periodo"
            fullWidth
            {...register("periodo")}
            error={!!errors.periodo}
            helperText={errors.periodo?.message}
          />
          <Controller
            name="tipoFosil"
            control={control}
            defaultValue="Desconocido"
            render={({ field }) => (
              <FormControl fullWidth error={!!errors.tipoFosil}>
                <InputLabel>Subtipo</InputLabel>
                <Select {...field} label="Subtipo">
                  <MenuItem value="Desconocido">Desconocido</MenuItem>
                  <MenuItem value="Vertebrado">Vertebrado</MenuItem>
                  <MenuItem value="Invertebrado">Invertebrado</MenuItem>
                  <MenuItem value="Paleobotánica">Paleobotánica</MenuItem>
                  <MenuItem value="Icnofósil">Icnofósil</MenuItem>
                  <MenuItem value="Microfósil">Microfósil</MenuItem>
                </Select>
              </FormControl>
            )}
          />
        </>
      ) : tipo === "mineral" ? (
        <>
          <Controller
            name="tipoMineral"
            control={control}
            defaultValue="Desconocido"
            render={({ field }) => (
              <FormControl fullWidth error={!!errors.tipoMineral}>
                <InputLabel>Subtipo</InputLabel>
                <Select {...field} label="Subtipo">
                  <MenuItem value="Desconocido">Desconocido</MenuItem>
                  <MenuItem value="Silicato">Silicato</MenuItem>
                  <MenuItem value="Carbonato">Carbonato</MenuItem>
                  <MenuItem value="Metálico">Metálico</MenuItem>
                  <MenuItem value="Oxido">Óxido</MenuItem>
                  <MenuItem value="Sulfuro">Sulfuro</MenuItem>
                  <MenuItem value="Sulfato">Sulfato</MenuItem>
                  <MenuItem value="Fosfato">Fosfato</MenuItem>
                  <MenuItem value="Vanadato">Vanadato</MenuItem>
                </Select>
              </FormControl>
            )}
          />
          <TextField
            label="Litología"
            fullWidth
            {...register("litologia")}
            error={!!errors.litologia}
            helperText={errors.litologia?.message}
          />
        </>
      ) : (
        <>
          <Controller
            name="tipoRoca"
            control={control}
            defaultValue="Desconocido"
            render={({ field }) => (
              <FormControl fullWidth error={!!errors.tipoRoca}>
                <InputLabel>Subtipo</InputLabel>
                <Select {...field} label="Subtipo">
                  <MenuItem value="Desconocido">Desconocido</MenuItem>
                  <MenuItem value="Ígnea">Ígnea</MenuItem>
                  <MenuItem value="Sedimentaria">Sedimentaria</MenuItem>
                  <MenuItem value="Metamórfica">Metamórfica</MenuItem>
                  <MenuItem value="Meteorito">Meteorito</MenuItem>
                  <MenuItem value="PiroVolcanoclástica">Piro-Volcanoclástica</MenuItem>
                </Select>
              </FormControl>
            )}
          />
          <TextField
            label="Litología"
            fullWidth
            {...register("litologia")}
            error={!!errors.litologia}
            helperText={errors.litologia?.message}
          />
        </>
      )}
    </Stack>

    {/* Nombre | Edad */}
    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }}>
      <TextField
        label="Nombre *"
        fullWidth
        {...register("nombre")}
        error={!!errors.nombre}
        helperText={errors.nombre?.message}
      />
      <TextField
        label="Edad"
        fullWidth
        placeholder="Ej: Cretácico Superior"
        {...register("edad")}
        error={!!errors.edad}
        helperText={errors.edad?.message || "Texto libre: números, periodos geológicos, etc."}
      />
    </Stack>

    {/* Donante | Código | Ejemplares — 3 en fila */}
    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }}>
      <TextField
        label="Donante"
        fullWidth
        {...register("donante")}
      />
      <TextField
        label="Código"
        fullWidth
        {...register("codigo")}
      />
      <TextField
        label="Ejemplares"
        type="number"
        fullWidth
        {...register("ejemplares", { valueAsNumber: true })}
      />
    </Stack>

    {/* Documentos relacionados */}
    <TextField
      label="Documentos Relacionados"
      fullWidth
      {...register("documentosRelacionados")}
      sx={{ mb: 2 }}
    />

    {/* URL Lámina | ¿Lámina existe? */}
    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 1 }}>
      <TextField
        label="URL Lámina"
        fullWidth
        {...register("laminaURL")}
      />
      <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
        <Controller
          name="laminaExiste"
          control={control}
          render={({ field }) => (
            <FormControlLabel
              control={<Checkbox {...field} checked={field.value} />}
              label="¿Lámina Existe?"
            />
          )}
        />
      </Box>
    </Stack>
  </Box>
);

export default BasicInfoSection;
