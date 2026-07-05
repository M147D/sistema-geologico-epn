import { Box, Typography, Chip, IconButton, Tooltip } from '@mui/material';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import { ROL_LABELS, ROL_COLORS } from '../../constants/roles';

const UserDrawerCard = ({ open, user }) => {
  if (!user) return null;

  const rolLabel = ROL_LABELS[user.rol] ?? 'Desconocido';
  const rolColor = ROL_COLORS[user.rol] ?? 'default';

  if (!open) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 1 }}>
        <Tooltip title={rolLabel} placement="right">
          <IconButton size="small" sx={{ color: 'text.secondary' }}>
            <AccountCircleIcon />
          </IconButton>
        </Tooltip>
      </Box>
    );
  }

  return (
    <Box sx={{ px: 2, py: 1.5 }}>
      <Typography variant="body2" fontWeight="bold" noWrap>
        {user.nombreCompleto || user.email}
      </Typography>
      <Typography variant="caption" color="text.secondary" noWrap display="block">
        {user.email}
      </Typography>
      <Chip
        label={rolLabel}
        color={rolColor}
        size="small"
        sx={{ mt: 0.5 }}
      />
    </Box>
  );
};

export default UserDrawerCard;