import { useState, useEffect } from 'react';
import { Fab, Tooltip } from '@mui/material';
import FullscreenIcon from '@mui/icons-material/Fullscreen';
import FullscreenExitIcon from '@mui/icons-material/FullscreenExit';

/**
 * Botón flotante para activar/desactivar pantalla completa
 * Maximiza el contenedor del mapa para mejor interacción
 * @param {React.RefObject} containerRef - Referencia al contenedor del mapa
 */
const FullscreenToggle = ({ containerRef }) => {
  const [isFullscreen, setIsFullscreen] = useState(false);

  // Detectar cambios de fullscreen (ej: usuario presiona ESC)
  useEffect(() => {
    const handleFullscreenChange = () => {
      setIsFullscreen(!!document.fullscreenElement);
    };

    document.addEventListener('fullscreenchange', handleFullscreenChange);
    document.addEventListener('webkitfullscreenchange', handleFullscreenChange);
    document.addEventListener('mozfullscreenchange', handleFullscreenChange);
    document.addEventListener('MSFullscreenChange', handleFullscreenChange);

    return () => {
      document.removeEventListener('fullscreenchange', handleFullscreenChange);
      document.removeEventListener('webkitfullscreenchange', handleFullscreenChange);
      document.removeEventListener('mozfullscreenchange', handleFullscreenChange);
      document.removeEventListener('MSFullscreenChange', handleFullscreenChange);
    };
  }, []);

  const toggleFullscreen = async () => {
    if (!containerRef.current) return;

    try {
      if (!isFullscreen) {
        // Entrar a pantalla completa
        if (containerRef.current.requestFullscreen) {
          await containerRef.current.requestFullscreen();
        } else if (containerRef.current.webkitRequestFullscreen) {
          await containerRef.current.webkitRequestFullscreen();
        } else if (containerRef.current.mozRequestFullScreen) {
          await containerRef.current.mozRequestFullScreen();
        } else if (containerRef.current.msRequestFullscreen) {
          await containerRef.current.msRequestFullscreen();
        }
      } else {
        // Salir de pantalla completa
        if (document.exitFullscreen) {
          await document.exitFullscreen();
        } else if (document.webkitExitFullscreen) {
          await document.webkitExitFullscreen();
        } else if (document.mozCancelFullScreen) {
          await document.mozCancelFullScreen();
        } else if (document.msExitFullscreen) {
          await document.msExitFullscreen();
        }
      }
    } catch (error) {
      console.error('Error toggling fullscreen:', error);
    }
  };

  return (
    <Tooltip
      title={isFullscreen ? 'Salir de pantalla completa (ESC)' : 'Pantalla completa (F11)'}
      placement="right"
    >
      <Fab
        size="medium"
        onClick={toggleFullscreen}
        sx={{
          position: 'absolute',
          bottom: 20,
          left: 16,
          zIndex: 1000,
          bgcolor: 'white',
          color: 'primary.main',
          boxShadow: 4,
          transition: 'all 0.3s',
          '&:hover': {
            bgcolor: 'primary.main',
            color: 'white',
            transform: 'scale(1.1)',
            boxShadow: 6
          }
        }}
      >
        {isFullscreen ? (
          <FullscreenExitIcon />
        ) : (
          <FullscreenIcon />
        )}
      </Fab>
    </Tooltip>
  );
};

export default FullscreenToggle;
