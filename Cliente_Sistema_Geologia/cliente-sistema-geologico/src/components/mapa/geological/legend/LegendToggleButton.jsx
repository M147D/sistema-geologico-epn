import { Fab, Tooltip, Badge } from '@mui/material';
import LayersIcon from '@mui/icons-material/Layers';

/**
 * Botón flotante (FAB) para abrir/cerrar la leyenda
 * @param {boolean} isOpen - Si la leyenda está abierta
 * @param {Function} onToggle - Callback para abrir/cerrar
 * @param {number} selectedCount - Número de formaciones seleccionadas (opcional)
 * @param {number} totalCount - Total de formaciones (opcional)
 */
const LegendToggleButton = ({ isOpen, onToggle, selectedCount, totalCount }) => {
  if (isOpen) return null;

  const showBadge = selectedCount !== undefined && totalCount !== undefined;
  const badgeContent = showBadge ? `${selectedCount}/${totalCount}` : null;

  return (
    <Tooltip title="Abrir Leyenda Geológica" placement="left">
      <Badge
        badgeContent={badgeContent}
        color="primary"
        max={999}
        sx={{
          '& .MuiBadge-badge': {
            fontSize: '0.7rem',
            height: 20,
            minWidth: 20,
            padding: '0 6px',
            top: 0,
            right: 55,
            border: '2px solid #fff', 
            boxSizing: 'content-box'
          }
        }}
      >
        <Fab
          color="default"
          size="small"
          onClick={onToggle}
          sx={{
            bgcolor: 'white',
            color: 'text.secondary',
            boxShadow: 3,
            transition: 'all 0.3s',
            '&:hover': {
              bgcolor: '#f5f5f5',
              transform: 'scale(1.1)',
              boxShadow: 4
            }
          }}
        >
          <LayersIcon fontSize="small" />
        </Fab>
      </Badge>
    </Tooltip>
  );
};

export default LegendToggleButton;