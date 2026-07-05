// src/components/mapa/FotoModal.jsx
import { useState, useRef, useEffect, useCallback } from 'react';
import { 
  Dialog, 
  IconButton, 
  Box, 
  Typography,
  Fade,
  Paper,
  Slider
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import ArrowBackIosIcon from '@mui/icons-material/ArrowBackIos';
import ArrowForwardIosIcon from '@mui/icons-material/ArrowForwardIos';
import InfoIcon from '@mui/icons-material/Info';
import ZoomInIcon from '@mui/icons-material/ZoomIn';
import ZoomOutIcon from '@mui/icons-material/ZoomOut';
import RestartAltIcon from '@mui/icons-material/RestartAlt';
const FotoModal = ({ open, handleClose, fotos, currentIndex, setCurrentIndex, elemento, getImage }) => {
  const foto = fotos[currentIndex];
  
  // Estados para controlar el zoom y la posición
  const [scale, setScale] = useState(1);
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState({ x: 0, y: 0 });
  const [showControls, setShowControls] = useState(true);
  
  // Referencia al contenedor de la imagen
  const imageContainerRef = useRef(null);
  
  // Manejar navegación entre imágenes
  const handlePrevious = useCallback((e) => {
    e.stopPropagation();
    setCurrentIndex((prevIndex) => (prevIndex > 0 ? prevIndex - 1 : fotos.length - 1));
  }, [fotos.length, setCurrentIndex]);

  const handleNext = useCallback((e) => {
    e.stopPropagation();
    setCurrentIndex((prevIndex) => (prevIndex < fotos.length - 1 ? prevIndex + 1 : 0));
  }, [fotos.length, setCurrentIndex]);

  // Funciones para manejar el zoom con incrementos más pequeños
  const handleZoomIn = useCallback(() => {
    setScale(prevScale => Math.min(prevScale + 0.1, 3));
  }, []);

  const handleZoomOut = useCallback(() => {
    setScale(prevScale => Math.max(prevScale - 0.1, 0.5));
  }, []);

  const handleZoomChange = (_, newValue) => {
    setScale(newValue);
  };

  const resetZoomAndPosition = useCallback(() => {
    setScale(1);
    setPosition({ x: 0, y: 0 });
  }, []);

  // Restablecer zoom y posición al cambiar de imagen
  useEffect(() => {
    resetZoomAndPosition();
  }, [currentIndex, resetZoomAndPosition]);
  
  // Funciones para manejar el movimiento (pan)
  const handleMouseDown = (e) => {
    if (scale > 1) {
      setIsDragging(true);
      setDragStart({
        x: e.clientX - position.x,
        y: e.clientY - position.y
      });
      e.preventDefault();
    }
  };
  
  const handleMouseMove = (e) => {
    if (isDragging && scale > 1) {
      setPosition({
        x: e.clientX - dragStart.x,
        y: e.clientY - dragStart.y
      });
      e.preventDefault();
    }
  };
  
  const handleMouseUp = () => {
    setIsDragging(false);
  };
  
  const handleMouseLeave = () => {
    setIsDragging(false);
  };
  
  // Manejar zoom con la rueda del mouse (incrementos más pequeños)
  const handleWheel = (e) => {
    e.preventDefault();
    
    // Factor de zoom más pequeño para un acercamiento suave
    const zoomFactor = 0.05;
    
    // Calcular la posición del cursor relativa a la imagen
    const rect = e.currentTarget.getBoundingClientRect();
    const mouseX = e.clientX - rect.left;
    const mouseY = e.clientY - rect.top;

    // Calcular el nuevo factor de escala
    const newScale = e.deltaY < 0 
      ? Math.min(scale + zoomFactor, 3) 
      : Math.max(scale - zoomFactor, 0.5);
    
    if (newScale !== scale) {
      // Calcular el desplazamiento para zoom centrado en el cursor
      // Solo aplicar este cálculo si el usuario está haciendo zoom in
      if (newScale > 1) {
        const newX = position.x + (mouseX - rect.width / 2) * (newScale - scale) / newScale;
        const newY = position.y + (mouseY - rect.height / 2) * (newScale - scale) / newScale;
        setPosition({ x: newX, y: newY });
      } else if (newScale <= 1) {
        // Si volvemos a escala 1 o menos, reset de la posición
        setPosition({ x: 0, y: 0 });
      }
      
      // Actualizar la escala
      setScale(newScale);
    }
  };
  
  // Togglear la visibilidad de los controles
  const toggleControls = () => {
    setShowControls(prev => !prev);
  };

// Manejar navegación con teclado
useEffect(() => {
    const handleKeyDown = (e) => {
      if (!open) return;
      
      switch (e.key) {
        case 'ArrowLeft':
          handlePrevious(e);
          break;
        case 'ArrowRight':
          handleNext(e);
          break;
        case 'Escape':
          handleClose();
          break;
        case '+':
          handleZoomIn();
          break;
        case '-':
          handleZoomOut();
          break;
        case '0':
          resetZoomAndPosition();
          break;
        default:
          break;
      }
    };
  
    window.addEventListener('keydown', handleKeyDown);
    
    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [open, handlePrevious, handleNext, handleClose, handleZoomIn, handleZoomOut, resetZoomAndPosition]);

  const [imageUrl, setImageUrl] = useState('');
  useEffect(() => {
    if (!foto?.id) { setImageUrl(''); return; }
    let mounted = true;
    getImage(foto.id).then(url => { if (mounted) setImageUrl(url || ''); });
    return () => { mounted = false; };
  }, [foto?.id, getImage]);
  
  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="xl"
      fullWidth
      TransitionComponent={Fade}
      TransitionProps={{ timeout: 500 }}
      PaperProps={{
        sx: {
          bgcolor: 'rgba(0, 0, 0, 0.9)',
          boxShadow: 'none',
          borderRadius: 0,
          m: 0,
          height: '100vh',
          maxHeight: '100vh',
          width: '100vw',
          maxWidth: '100vw',
          overflowY: 'hidden'
        }
      }}
      sx={{
        '& .MuiDialog-container': {
          alignItems: 'center',
          justifyContent: 'center'
        }
      }}
    >
      {/* Botón de cierre en la esquina superior derecha */}
      <IconButton
        aria-label="cerrar"
        onClick={handleClose}
        sx={{
          position: 'absolute',
          top: 16,
          right: 16,
          zIndex: 10,
          color: 'white',
          bgcolor: 'rgba(0, 0, 0, 0.5)',
          '&:hover': {
            bgcolor: 'rgba(0, 0, 0, 0.7)'
          },
          display: showControls ? 'flex' : 'none'
        }}
      >
        <CloseIcon />
      </IconButton>
      
      {/* Contenedor principal (detecta clics para mostrar/ocultar controles) */}
      <Box 
        onClick={toggleControls}
        sx={{ 
          width: '100%',
          height: '100%',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          position: 'relative',
          cursor: isDragging ? 'grabbing' : (scale > 1 ? 'grab' : 'default')
        }}
      >
        {foto && (
          <>
            {/* Imagen con capacidad de zoom y movimiento */}
            <Box
              ref={imageContainerRef}
              onMouseDown={handleMouseDown}
              onMouseMove={handleMouseMove}
              onMouseUp={handleMouseUp}
              onMouseLeave={handleMouseLeave}
              onWheel={handleWheel}
              onClick={e => e.stopPropagation()} // Prevenir que se active toggleControls
              sx={{
                position: 'relative',
                maxWidth: '90%',
                maxHeight: '85vh',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                overflow: 'hidden'
              }}
            >
              <img
                src={imageUrl}
                alt={foto.tipoFoto || `Imagen ${currentIndex + 1}`}
                style={{ 
                  maxHeight: scale === 1 ? '100%' : 'none', 
                  maxWidth: scale === 1 ? '100%' : 'none', 
                  transform: `scale(${scale}) translate(${position.x / scale}px, ${position.y / scale}px)`,
                  transformOrigin: 'center',
                  transition: isDragging ? 'none' : 'transform 0.2s ease-out',
                  boxShadow: '0 4px 20px rgba(0,0,0,0.3)'
                }}
              />
              
              {/* Información flotante en la parte inferior */}
              {showControls && (
                <Paper
                  elevation={0}
                  sx={{
                    position: 'absolute',
                    bottom: 0,
                    left: 0,
                    right: 0,
                    bgcolor: 'rgba(0, 0, 0, 0.6)',
                    color: 'white',
                    p: 1.5,
                    borderRadius: 0,
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    backdropFilter: 'blur(5px)',
                    zIndex: 5
                  }}
                >
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <InfoIcon fontSize="small" sx={{ mr: 1, opacity: 0.7 }} />
                    <Box>
                      <Typography variant="subtitle2" sx={{ fontWeight: 'bold' }}>
                        {foto.tipoFoto || `Foto ${currentIndex + 1}`}
                      </Typography>
                      {foto.descripcionEspecifica && (
                        <Typography variant="caption" sx={{ opacity: 0.9, display: 'block' }}>
                          {foto.descripcionEspecifica}
                        </Typography>
                      )}
                    </Box>
                  </Box>
                  <Typography variant="caption" sx={{ fontWeight: 'medium' }}>
                    {currentIndex + 1} / {fotos.length}
                  </Typography>
                </Paper>
              )}
            </Box>
            
            {/* Título del elemento en la parte superior */}
            {showControls && (
              <Typography 
                variant="h6" 
                sx={{ 
                  position: 'absolute', 
                  top: 16, 
                  left: 16, 
                  color: 'white',
                  textShadow: '0 2px 4px rgba(0,0,0,0.5)',
                  maxWidth: '70%',
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap'
                }}
              >
                {elemento?.nombre || 'Detalle de la imagen'}
              </Typography>
            )}
            
            {/* Controles de zoom */}
            {showControls && (
              <Paper
                elevation={0}
                sx={{
                  position: 'absolute',
                  bottom: 80,
                  right: 16,
                  bgcolor: 'rgba(0, 0, 0, 0.6)',
                  color: 'white',
                  p: 1,
                  borderRadius: 1,
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center',
                  backdropFilter: 'blur(5px)'
                }}
              >
                <IconButton
                  onClick={handleZoomIn}
                  sx={{ color: 'white', mb: 0.5 }}
                  size="small"
                >
                  <ZoomInIcon />
                </IconButton>
                
                <Slider
                  orientation="vertical"
                  value={scale}
                  min={0.5}
                  max={3}
                  step={0.01}
                  onChange={handleZoomChange}
                  sx={{ 
                    height: 100, 
                    mx: 1,
                    '& .MuiSlider-thumb': {
                      width: 12,
                      height: 12
                    },
                    '& .MuiSlider-track': {
                      width: 4
                    },
                    '& .MuiSlider-rail': {
                      width: 4
                    }
                  }}
                />
                
                <IconButton
                  onClick={handleZoomOut}
                  sx={{ color: 'white', mt: 0.5 }}
                  size="small"
                >
                  <ZoomOutIcon />
                </IconButton>
                
                <IconButton
                  onClick={resetZoomAndPosition}
                  sx={{ color: 'white', mt: 1 }}
                  size="small"
                >
                  <RestartAltIcon />
                </IconButton>
              </Paper>
            )}
            
            {/* Botones de navegación */}
            {showControls && fotos.length > 1 && (
              <>
                <IconButton
                  onClick={handlePrevious}
                  sx={{
                    position: 'absolute',
                    left: 16,
                    color: 'white',
                    bgcolor: 'rgba(0, 0, 0, 0.5)',
                    '&:hover': {
                      bgcolor: 'rgba(0, 0, 0, 0.7)'
                    }
                  }}
                  size="large"
                >
                  <ArrowBackIosIcon />
                </IconButton>
                <IconButton
                  onClick={handleNext}
                  sx={{
                    position: 'absolute',
                    right: 16,
                    color: 'white',
                    bgcolor: 'rgba(0, 0, 0, 0.5)',
                    '&:hover': {
                      bgcolor: 'rgba(0, 0, 0, 0.7)'
                    }
                  }}
                  size="large"
                >
                  <ArrowForwardIosIcon />
                </IconButton>
              </>
            )}
          </>
        )}
      </Box>
    </Dialog>
  );
};

export default FotoModal;