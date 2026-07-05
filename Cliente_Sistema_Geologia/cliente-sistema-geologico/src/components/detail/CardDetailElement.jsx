import { useState } from 'react';
import {
  Box, Container, Divider, Paper, Alert, CircularProgress
} from '@mui/material';
import { getSubtipo } from '../../utils/detailUtils.js';
import DetailHeader from './DetailHeader';
import DetailInfoGeneral from './DetailInfoGeneral';
import DetailInfoTemporal from './DetailInfoTemporal';
import DetailInfoDocumentacion from './DetailInfoDocumentacion';
import DetailInfoAuditoria from './DetailInfoAuditoria';
import DetailInfoUbicacion from './DetailInfoUbicacion';
import DetailGallery from './DetailGallery';
import DetailTopBar from './DetailTopBar';
import FotoModal from './FotoModal';
import DialogInformePetrografico from './DialogInformePetrografico';

const CardDetailElement = ({
  elemento, fotos, loadingDetail,
  user, isAdmin, canEdit, canDelete,
  currentIndex, totalElementos, handleNavigate, handleGoBack,
  isEditing, editForm, saving, saveError, saveSuccess,
  handleStartEdit, handleCancelEdit, handleEditChange, handleSave,
  clearSaveError, clearSaveSuccess, handleSetSaveError,
  handleUploadPhoto, handleSavePhoto, handleDeletePhoto, handleRestorePhoto,
  handleSolicitarInforme,
  getImage, getImageThumbnail,
}) => {
  const [modalOpen, setModalOpen] = useState(false);
  const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
  const [informeDialogOpen, setInformeDialogOpen] = useState(false);

  const subtipo = getSubtipo(elemento);

  return (
    <Container maxWidth="xl" sx={{ my: 4 }}>
      {saveSuccess && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={clearSaveSuccess}>
          Cambios guardados correctamente
        </Alert>
      )}

      <DetailTopBar
        onGoBack={handleGoBack}
        currentIndex={currentIndex}
        totalElementos={totalElementos}
        onNavigate={handleNavigate}
      />

      <Paper elevation={2} sx={{ overflow: 'hidden' }}>
        <DetailHeader
          elemento={elemento}
          subtipo={subtipo}
          canEdit={canEdit}
          isEditing={isEditing}
          saving={saving}
          saveError={saveError}
          onStartEdit={handleStartEdit}
          onSave={handleSave}
          onCancelEdit={handleCancelEdit}
          onClearSaveError={clearSaveError}
          onSolicitarInforme={() => setInformeDialogOpen(true)}
        />

        <Box sx={{ p: 3 }}>
          {loadingDetail && (
            <Box display="flex" alignItems="center" justifyContent="center" py={2}>
              <CircularProgress size={20} />
            </Box>
          )}

          <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
            {/* Col 1: General */}
            <Box sx={{ flex: '1 1 260px', minWidth: 220 }}>
              <DetailInfoGeneral
                elemento={elemento}
                isEditing={isEditing}
                editForm={editForm}
                onEditChange={handleEditChange}
              />
            </Box>

            {/* Col 2: Temporal + Documentación apiladas */}
            <Box sx={{ flex: '1 1 240px', minWidth: 200 }}>
              <DetailInfoTemporal
                elemento={elemento}
                isEditing={isEditing}
                editForm={editForm}
                onEditChange={handleEditChange}
              />
              <DetailInfoDocumentacion
                elemento={elemento}
                isEditing={isEditing}
                editForm={editForm}
                onEditChange={handleEditChange}
              />
            </Box>

            {/* Col 3: Ubicación + Auditoría (admin) apiladas */}
            <Box sx={{ flex: '1 1 240px', minWidth: 200 }}>
              <DetailInfoUbicacion
                elemento={elemento}
                isEditing={isEditing}
                editForm={editForm}
                onEditChange={handleEditChange}
              />
              {isAdmin && (
                <Box sx={{ mt: 2 }}>
                  <DetailInfoAuditoria elemento={elemento} />
                </Box>
              )}
            </Box>
          </Box>

          <Divider sx={{ my: 3 }} />

          <DetailGallery
            elemento={elemento}
            fotos={fotos}
            canEdit={canEdit}
            canDelete={canDelete}
            isAdmin={isAdmin}
            onOpenModal={(index) => { setCurrentPhotoIndex(index); setModalOpen(true); }}
            onUploadPhoto={handleUploadPhoto}
            onSavePhoto={handleSavePhoto}
            onDeletePhoto={handleDeletePhoto}
            onRestorePhoto={handleRestorePhoto}
            saveError={saveError}
            onSetSaveError={handleSetSaveError}
            getImage={getImage}
            getImageThumbnail={getImageThumbnail}
          />
        </Box>
      </Paper>

      <DialogInformePetrografico
        open={informeDialogOpen}
        onClose={() => setInformeDialogOpen(false)}
        onSubmit={handleSolicitarInforme}
        elemento={elemento}
        userEmail={user?.email}
      />

      <FotoModal
        open={modalOpen}
        handleClose={() => setModalOpen(false)}
        fotos={fotos}
        currentIndex={currentPhotoIndex}
        setCurrentIndex={setCurrentPhotoIndex}
        elemento={elemento}
        getImage={getImage}
      />
    </Container>
  );
};

export default CardDetailElement;
