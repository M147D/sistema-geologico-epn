import React, { useState, useRef } from 'react';
import { compressForUpload } from '../../utils/imageProcessor';
import {
  Box, Typography, Paper, Stack, Button, IconButton, TextField,
  FormControl, InputLabel, Select, MenuItem, Chip, CircularProgress,
  Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions,
  Divider, Tooltip
} from '@mui/material';
import ImageIcon from '@mui/icons-material/Image';
import EditIcon from '@mui/icons-material/Edit';
import SaveIcon from '@mui/icons-material/Save';
import DeleteIcon from '@mui/icons-material/Delete';
import RestoreIcon from '@mui/icons-material/RestoreFromTrash';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { FotoComponente } from './DetailHelpers.jsx';

const DetailGallery = ({
  elemento,
  fotos,
  canEdit,
  canDelete,
  isAdmin,
  onOpenModal,
  onUploadPhoto,
  onSavePhoto,
  onDeletePhoto,
  onRestorePhoto,
  saveError,
  onSetSaveError
}) => {
  // Upload state
  const fileInputRef = useRef(null);
  const [uploadFile, setUploadFile] = useState(null);
  const [uploadTipoFoto, setUploadTipoFoto] = useState(2);
  const [uploadDescripcion, setUploadDescripcion] = useState('');
  const [uploading, setUploading] = useState(false);

  // Photo edit state
  const [editingPhotoId, setEditingPhotoId] = useState(null);
  const [editPhotoForm, setEditPhotoForm] = useState({ tipoFoto: 2, descripcionEspecifica: '' });
  const [editPhotoFile, setEditPhotoFile] = useState(null);
  const [savingPhoto, setSavingPhoto] = useState(false);
  const editPhotoFileRef = useRef(null);

  // Delete / restore state
  const [deleteConfirmId, setDeleteConfirmId] = useState(null);
  const [deletingPhoto, setDeletingPhoto] = useState(false);
  const [restoringPhotoId, setRestoringPhotoId] = useState(null);

  const hasPhotos = fotos.length > 0;

  const handleUpload = async () => {
    if (!uploadFile) return;
    setUploading(true);
    try {
      const formData = new FormData();
      formData.append('ImagenFile', uploadFile);
      formData.append('TipoFoto', uploadTipoFoto);
      formData.append('DescripcionEspecifica', uploadDescripcion || 'Vacio');
      await onUploadPhoto(elemento.galeria?.id, formData);
      setUploadFile(null);
      setUploadTipoFoto(2);
      setUploadDescripcion('');
      if (fileInputRef.current) fileInputRef.current.value = '';
    } catch (err) {
      onSetSaveError(err.response?.data?.message || err.message || 'Error al subir la foto');
    } finally {
      setUploading(false);
    }
  };

  const handleStartEditPhoto = (foto) => {
    setEditingPhotoId(foto.id);
    const tipoVal = foto.tipoFoto === 'Lamina' || foto.tipoFoto === 1 ? 1
      : foto.tipoFoto === 'Fotografia' || foto.tipoFoto === 2 ? 2 : 0;
    setEditPhotoForm({ tipoFoto: tipoVal, descripcionEspecifica: foto.descripcionEspecifica || '' });
    setEditPhotoFile(null);
  };

  const handleCancelEditPhoto = () => {
    setEditingPhotoId(null);
    setEditPhotoForm({ tipoFoto: 2, descripcionEspecifica: '' });
    setEditPhotoFile(null);
  };

  const handleSaveEditPhoto = async () => {
    if (!editingPhotoId) return;
    setSavingPhoto(true);
    try {
      const formData = new FormData();
      formData.append('TipoFoto', editPhotoForm.tipoFoto);
      formData.append('DescripcionEspecifica', editPhotoForm.descripcionEspecifica || 'Vacio');
      if (editPhotoFile) formData.append('ImagenFile', editPhotoFile);
      await onSavePhoto(editingPhotoId, formData);
      setEditingPhotoId(null);
      setEditPhotoFile(null);
    } catch (err) {
      onSetSaveError(err.response?.data?.message || err.message || 'Error al actualizar la foto');
    } finally {
      setSavingPhoto(false);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deleteConfirmId) return;
    setDeletingPhoto(true);
    try {
      await onDeletePhoto(deleteConfirmId);
      setDeleteConfirmId(null);
    } catch (err) {
      onSetSaveError(err.response?.data?.message || err.message || 'Error al eliminar la foto');
      setDeleteConfirmId(null);
    } finally {
      setDeletingPhoto(false);
    }
  };

  const handleRestore = async (fotoId) => {
    setRestoringPhotoId(fotoId);
    try {
      await onRestorePhoto(fotoId);
    } catch (err) {
      onSetSaveError(err.response?.data?.message || err.message || 'Error al restaurar la foto');
    } finally {
      setRestoringPhotoId(null);
    }
  };

  // Active photos for the modal (only active ones are navigable)
  const fotosActivas = fotos.filter(f => f.estadoActivo !== false);

  return (
    <Box sx={{ mt: 4 }}>
      <Divider sx={{ mb: 3 }} />
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <Box sx={{ width: 4, height: 20, bgcolor: 'secondary.main', borderRadius: 1, flexShrink: 0 }} />
        <ImageIcon color="secondary" fontSize="small" />
        <Typography variant="subtitle1" fontWeight={600}>Galería</Typography>
        <Chip
          label={`${fotosActivas.length} foto${fotosActivas.length !== 1 ? 's' : ''}`}
          size="small"
          color="secondary"
          variant="outlined"
        />
        {isAdmin && fotos.length > fotosActivas.length && (
          <Chip
            label={`+${fotos.length - fotosActivas.length} inactiva${fotos.length - fotosActivas.length !== 1 ? 's' : ''}`}
            size="small"
            variant="outlined"
          />
        )}
      </Box>

      {hasPhotos ? (
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
          {fotos.map((foto, index) => {
            const isEditingThis = editingPhotoId === foto.id;
            const isInactiva = foto.estadoActivo === false;
            const isLamina = foto.tipoFoto === 'Lamina' || foto.tipoFoto === 1;
            // Modal index uses only active photos
            const activeIndex = fotosActivas.findIndex(f => f.id === foto.id);

            return (
              <Box
                key={foto.id || index}
                sx={{ width: { xs: '100%', sm: 'calc(50% - 8px)', md: 'calc(33.333% - 11px)' } }}
              >
                <Paper
                  elevation={2}
                  sx={{
                    overflow: 'hidden',
                    borderRadius: 2,
                    height: '100%',
                    transition: 'transform 0.3s',
                    ...(!isEditingThis && !isInactiva && {
                      '&:hover': { transform: 'scale(1.02)', cursor: 'pointer' }
                    }),
                    ...(isEditingThis && { border: '2px solid', borderColor: 'primary.main' }),
                    ...(isInactiva && { opacity: 0.5, filter: 'grayscale(80%)' })
                  }}
                  onClick={!isEditingThis && !isInactiva && activeIndex >= 0
                    ? () => onOpenModal(activeIndex)
                    : undefined}
                >
                  <Box sx={{ position: 'relative' }}>
                    <FotoComponente
                      fotoId={foto.id}
                      alt={`${elemento.nombre} - Foto ${index + 1}`}
                      thumbnail={true}
                      onClick={!isEditingThis && !isInactiva && activeIndex >= 0
                        ? () => onOpenModal(activeIndex)
                        : undefined}
                    />

                    {/* Action buttons overlay */}
                    {!isEditingThis && (
                      <Box sx={{ position: 'absolute', top: 4, right: 4, display: 'flex', gap: 0.5 }}>
                        {isInactiva && isAdmin && (
                          <Tooltip title="Restaurar foto">
                            <IconButton
                              size="small"
                              onClick={(e) => { e.stopPropagation(); handleRestore(foto.id); }}
                              disabled={restoringPhotoId === foto.id}
                              sx={{ bgcolor: 'rgba(255,255,255,0.9)', '&:hover': { bgcolor: 'success.light', color: 'white' } }}
                            >
                              {restoringPhotoId === foto.id
                                ? <CircularProgress size={16} />
                                : <RestoreIcon fontSize="small" color="success" />}
                            </IconButton>
                          </Tooltip>
                        )}
                        {!isInactiva && canEdit && (
                          <Tooltip title="Editar foto">
                            <IconButton
                              size="small"
                              onClick={(e) => { e.stopPropagation(); handleStartEditPhoto(foto); }}
                              sx={{ bgcolor: 'rgba(255,255,255,0.85)', '&:hover': { bgcolor: 'white' } }}
                            >
                              <EditIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                        {!isInactiva && canDelete && (
                          <Tooltip title="Eliminar foto">
                            <IconButton
                              size="small"
                              onClick={(e) => { e.stopPropagation(); setDeleteConfirmId(foto.id); }}
                              sx={{ bgcolor: 'rgba(255,255,255,0.85)', '&:hover': { bgcolor: 'error.light', color: 'white' } }}
                            >
                              <DeleteIcon fontSize="small" color="error" />
                            </IconButton>
                          </Tooltip>
                        )}
                      </Box>
                    )}

                    {/* Inactive badge */}
                    {isInactiva && (
                      <Chip
                        label="Inactiva"
                        size="small"
                        color="error"
                        sx={{ position: 'absolute', top: 4, left: 4, opacity: 0.9 }}
                      />
                    )}
                  </Box>

                  {/* Edit form or photo info */}
                  {isEditingThis ? (
                    <Box sx={{ p: 1.5 }}>
                      <Stack spacing={1}>
                        <FormControl size="small" fullWidth>
                          <InputLabel>Tipo</InputLabel>
                          <Select
                            value={editPhotoForm.tipoFoto}
                            label="Tipo"
                            onChange={e => setEditPhotoForm(prev => ({ ...prev, tipoFoto: e.target.value }))}
                          >
                            <MenuItem value={1}>Lamina</MenuItem>
                            <MenuItem value={2}>Fotografia</MenuItem>
                          </Select>
                        </FormControl>
                        <TextField
                          label="Descripcion"
                          value={editPhotoForm.descripcionEspecifica}
                          onChange={e => setEditPhotoForm(prev => ({ ...prev, descripcionEspecifica: e.target.value }))}
                          size="small"
                          fullWidth
                        />
                        <Box>
                          <input
                            ref={editPhotoFileRef}
                            type="file"
                            accept="image/*"
                            style={{ display: 'none' }}
                            onChange={async (e) => {
                              const file = e.target.files?.[0];
                              if (!file) { setEditPhotoFile(null); return; }
                              const compressed = await compressForUpload(file);
                              setEditPhotoFile(compressed);
                            }}
                          />
                          <Button
                            variant="outlined"
                            size="small"
                            fullWidth
                            startIcon={<CloudUploadIcon />}
                            onClick={() => editPhotoFileRef.current?.click()}
                          >
                            {editPhotoFile ? editPhotoFile.name : 'Reemplazar imagen'}
                          </Button>
                        </Box>
                        <Stack direction="row" spacing={1}>
                          <Button
                            variant="contained"
                            size="small"
                            startIcon={savingPhoto ? <CircularProgress size={14} /> : <SaveIcon />}
                            onClick={handleSaveEditPhoto}
                            disabled={savingPhoto}
                            sx={{ flex: 1 }}
                          >
                            Guardar
                          </Button>
                          <Button
                            variant="outlined"
                            size="small"
                            onClick={handleCancelEditPhoto}
                            disabled={savingPhoto}
                            sx={{ flex: 1 }}
                          >
                            Cancelar
                          </Button>
                        </Stack>
                      </Stack>
                    </Box>
                  ) : (
                    <Box sx={{ p: 1 }}>
                      <Chip
                        label={isLamina ? 'Lamina' : 'Fotografia'}
                        size="small"
                        color={isLamina ? 'warning' : 'info'}
                        sx={{ mb: 0.5 }}
                      />
                      {foto.descripcionEspecifica && foto.descripcionEspecifica !== 'Vacio' && (
                        <Typography variant="caption" display="block" color="text.secondary">
                          {foto.descripcionEspecifica}
                        </Typography>
                      )}
                    </Box>
                  )}
                </Paper>
              </Box>
            );
          })}
        </Box>
      ) : (
        <Paper variant="outlined" sx={{ p: 3, textAlign: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            No hay fotografias disponibles para este elemento
          </Typography>
        </Paper>
      )}

      {/* Upload section — always visible for Admin/Invitado */}
      {canEdit && (
        <Box sx={{ mt: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1.5 }}>
            <Box sx={{ width: 4, height: 18, bgcolor: 'secondary.main', borderRadius: 1, flexShrink: 0 }} />
            <Typography variant="subtitle2" fontWeight={600}>Subir nueva fotografía</Typography>
          </Box>
          <Box
            sx={{
              border: '2px dashed',
              borderColor: uploadFile ? 'secondary.main' : 'grey.300',
              borderRadius: 2,
              p: 2,
              bgcolor: uploadFile ? 'secondary.50' : 'grey.50',
              transition: 'all 0.2s',
            }}
          >
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems="flex-end">
              <Box sx={{ flex: 1 }}>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  style={{ display: 'none' }}
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (!file) { setUploadFile(null); return; }
                    const compressed = await compressForUpload(file);
                    setUploadFile(compressed);
                  }}
                />
                <Button
                  variant={uploadFile ? 'contained' : 'outlined'}
                  color="secondary"
                  startIcon={uploadFile ? <ImageIcon /> : <CloudUploadIcon />}
                  onClick={() => fileInputRef.current?.click()}
                  fullWidth
                  size="small"
                >
                  {uploadFile ? uploadFile.name : 'Seleccionar imagen'}
                </Button>
              </Box>
              <FormControl size="small" sx={{ minWidth: 160 }}>
                <InputLabel>Tipo de foto</InputLabel>
                <Select
                  value={uploadTipoFoto}
                  label="Tipo de foto"
                  onChange={e => setUploadTipoFoto(e.target.value)}
                >
                  <MenuItem value={1}>Lámina</MenuItem>
                  <MenuItem value={2}>Fotografía</MenuItem>
                </Select>
              </FormControl>
              <TextField
                label="Descripción"
                value={uploadDescripcion}
                onChange={e => setUploadDescripcion(e.target.value)}
                size="small"
                sx={{ minWidth: 200 }}
              />
              <Button
                variant="contained"
                color="secondary"
                onClick={handleUpload}
                disabled={!uploadFile || uploading}
                startIcon={uploading ? <CircularProgress size={16} color="inherit" /> : <CloudUploadIcon />}
                size="small"
              >
                {uploading ? 'Subiendo...' : 'Subir'}
              </Button>
            </Stack>
          </Box>
        </Box>
      )}

      {/* Delete confirmation dialog */}
      <Dialog open={deleteConfirmId !== null} onClose={() => setDeleteConfirmId(null)}>
        <DialogTitle>Eliminar foto</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Esta seguro de que desea eliminar esta foto? La foto quedara inactiva y el administrador podra restaurarla.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteConfirmId(null)} disabled={deletingPhoto}>
            Cancelar
          </Button>
          <Button
            onClick={handleDeleteConfirm}
            color="error"
            variant="contained"
            disabled={deletingPhoto}
            startIcon={deletingPhoto ? <CircularProgress size={16} /> : <DeleteIcon />}
          >
            {deletingPhoto ? 'Eliminando...' : 'Eliminar'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default DetailGallery;
