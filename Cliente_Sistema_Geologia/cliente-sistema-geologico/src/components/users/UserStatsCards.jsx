// src/components/users/UserStatsCards.jsx
import { Grid, Paper, Typography, Box, Skeleton } from '@mui/material';
import PeopleIcon from '@mui/icons-material/People';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';
import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';

const StatCard = ({ icon, label, value, color, loading }) => (
  <Paper elevation={2} sx={{ p: 2, display: 'flex', alignItems: 'center', gap: 2 }}>
    <Box sx={{ color: `${color}.main`, display: 'flex' }}>{icon}</Box>
    <Box>
      <Typography variant="caption" color="text.secondary">{label}</Typography>
      {loading
        ? <Skeleton width={40} height={28} />
        : <Typography variant="h6" fontWeight="bold">{value ?? '-'}</Typography>
      }
    </Box>
  </Paper>
);

const UserStatsCards = ({ estadisticas, loading }) => {
  const admins = estadisticas?.porRol?.Admin ?? 0;
  const premium = estadisticas?.porRol?.Premium ?? 0;
  const invitados = estadisticas?.porRol?.Invitado ?? 0;
  const free = estadisticas?.porRol?.Free ?? 0;
  const rolSummary = `A:${admins} P:${premium} I:${invitados} F:${free}`;

  return (
    <Grid container spacing={2} mb={2}>
      <Grid item xs={12} sm={6} md={3}>
        <StatCard
          icon={<PeopleIcon />}
          label="Total usuarios"
          value={estadisticas?.totalUsuarios}
          color="primary"
          loading={loading}
        />
      </Grid>
      <Grid item xs={12} sm={6} md={3}>
        <StatCard
          icon={<CheckCircleIcon />}
          label="Activos"
          value={estadisticas?.totalActivos}
          color="success"
          loading={loading}
        />
      </Grid>
      <Grid item xs={12} sm={6} md={3}>
        <StatCard
          icon={<CancelIcon />}
          label="Inactivos"
          value={estadisticas?.totalInactivos}
          color="error"
          loading={loading}
        />
      </Grid>
      <Grid item xs={12} sm={6} md={3}>
        <StatCard
          icon={<AdminPanelSettingsIcon />}
          label="Por rol"
          value={loading ? null : rolSummary}
          color="warning"
          loading={loading}
        />
      </Grid>
    </Grid>
  );
};

export default UserStatsCards;
