import { TextField, Box, Typography, Stack, FormControl, InputLabel, Select, MenuItem } from '@mui/material';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import ImageIcon from '@mui/icons-material/Image';
import { Controller } from 'react-hook-form';

const SectionHeader = ({ children }) => (
  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
    <Box sx={{ width: 4, height: 20, bgcolor: 'secondary.main', borderRadius: 1, mr: 1.5, flexShrink: 0 }} />
    <Typography variant="subtitle1" fontWeight={600}>{children}</Typography>
  </Box>
);

const PhotoSection = ({ register, handleFileChange, selectedFile, previewImage, control }) => (
  <Box>
    <SectionHeader>Fotografía</SectionHeader>

    <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 3 }}>

      {/* Columna izquierda: tipo + descripción */}
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, flex: '0 0 220px' }}>
        <Controller
          name="tipoFoto"
          control={control}
          defaultValue="Fotografia"
          render={({ field }) => (
            <FormControl fullWidth>
              <InputLabel>Tipo de Foto</InputLabel>
              <Select {...field} label="Tipo de Foto">
                <MenuItem value="Desconocido">Desconocido</MenuItem>
                <MenuItem value="Lamina">Lámina</MenuItem>
                <MenuItem value="Fotografia">Fotografía</MenuItem>
              </Select>
            </FormControl>
          )}
        />
        <TextField
          label="Descripción de la foto"
          fullWidth
          multiline
          rows={3}
          {...register("descripcionFoto")}
        />
      </Box>

      {/* Columna central: zona de carga */}
      <Box sx={{ flex: 1 }}>
        <input
          type="file"
          onChange={handleFileChange}
          accept="image/*"
          id="imagen-upload"
          style={{ display: 'none' }}
        />
        <label htmlFor="imagen-upload" style={{ display: 'block', cursor: 'pointer', height: '100%' }}>
          <Box
            sx={{
              border: '2px dashed',
              borderColor: selectedFile ? 'primary.main' : 'grey.300',
              borderRadius: 2,
              p: 3,
              textAlign: 'center',
              bgcolor: selectedFile ? 'primary.50' : 'grey.50',
              height: '100%',
              minHeight: 130,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              transition: 'all 0.2s',
              '&:hover': { borderColor: 'primary.main', bgcolor: 'action.hover' },
            }}
          >
            {selectedFile ? (
              <Stack alignItems="center" spacing={0.5}>
                <ImageIcon sx={{ fontSize: 36, color: 'primary.main' }} />
                <Typography variant="body2" fontWeight={500} color="primary.main">
                  {selectedFile.name}
                </Typography>
                <Typography variant="caption" color="text.secondary">Clic para cambiar</Typography>
              </Stack>
            ) : (
              <Stack alignItems="center" spacing={0.5}>
                <CloudUploadIcon sx={{ fontSize: 36, color: 'text.disabled' }} />
                <Typography variant="body2" color="text.secondary">Clic para seleccionar imagen</Typography>
                <Typography variant="caption" color="text.disabled">JPG, PNG, WEBP — máx. 10 MB</Typography>
              </Stack>
            )}
          </Box>
        </label>
      </Box>

      {/* Columna derecha: preview */}
      {previewImage && (
        <Box
          sx={{
            flex: '0 0 200px',
            borderRadius: 2,
            overflow: 'hidden',
            border: '1px solid',
            borderColor: 'divider',
            alignSelf: 'stretch',
          }}
        >
          <img
            src={previewImage}
            alt="Vista previa"
            style={{ display: 'block', width: '100%', height: '100%', objectFit: 'cover' }}
          />
        </Box>
      )}

    </Box>
  </Box>
);

export default PhotoSection;
