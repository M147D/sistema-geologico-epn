import React, { useCallback } from 'react';
import {
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Checkbox,
  Box
} from '@mui/material';

const FormationItem = React.memo(({ formation, isChecked, onToggle }) => {
  const handleClick = useCallback(() => {
    onToggle(formation.codA);
  }, [onToggle, formation.codA]);

  return (
    <ListItemButton
      dense
      onClick={handleClick}
      sx={{
        py: 0,
        px: 1.5,
        borderLeft: isChecked
          ? `4px solid ${formation.color}`
          : '4px solid transparent',
        transition: 'all 0.2s',
        '&:hover': {
          bgcolor: 'action.hover',
          borderLeftColor: formation.color
        }
      }}
    >
      <ListItemIcon sx={{ minWidth: 32 }}>
        <Checkbox
          edge="start"
          checked={isChecked}
          tabIndex={-1}
          disableRipple
          size="small"
          sx={{ p: 0.5 }}
        />
      </ListItemIcon>

      <Box
        sx={{
          width: 14,
          height: 14,
          bgcolor: formation.color,
          border: '1px solid #ccc',
          mr: 1.5,
          flexShrink: 0,
          opacity: isChecked ? 1 : 0.5,
          transition: 'opacity 0.2s'
        }}
      />

      <ListItemText
        primary={formation.label}
        slotProps={{
          primary: {
            variant: 'caption',
            sx: {
              fontSize: '0.75rem',
              fontWeight: isChecked ? 600 : 400,
              lineHeight: 1.2,
              color: isChecked ? 'text.primary' : 'text.secondary',
              transition: 'all 0.2s'
            }
          }
        }}
        sx={{ my: 0.5 }}
      />
    </ListItemButton>
  );
});

FormationItem.displayName = 'FormationItem';

export default FormationItem;