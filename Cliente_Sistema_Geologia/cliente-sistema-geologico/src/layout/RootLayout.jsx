// scr/layout/RootLayout.jsx
import { useState } from 'react';
import { Outlet, useLocation, Link, useNavigate } from 'react-router-dom';
import {
  Box,
  CssBaseline,
  Toolbar,
  IconButton,
  Typography,
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Button
} from '@mui/material';
import { styled, useTheme } from '@mui/material/styles';
import MuiAppBar from '@mui/material/AppBar';
import MuiDrawer from '@mui/material/Drawer';
import MenuIcon from '@mui/icons-material/Menu';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import MapIcon from '@mui/icons-material/Map';
import DynamicFormIcon from '@mui/icons-material/DynamicForm';
import TableChartIcon from '@mui/icons-material/TableChart';
import LogoutIcon from '@mui/icons-material/Logout';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import PeopleIcon from '@mui/icons-material/People';
import { useAuth } from '../context/AuthContext';
import ErrorBoundary from '../components/common/ErrorBoundary';
import UserDrawerCard from '../components/layout/UserDrawerCard';

const drawerWidth = 200;

// AppBar estilizado
const AppBar = styled(MuiAppBar, {
  shouldForwardProp: (prop) => prop !== 'open'
})(({ theme, open }) => ({
  display: 'flex',
  justifyContent: 'center',
  zIndex: theme.zIndex.drawer + 1,
  backgroundColor: '#2274ac',
  height: theme.breakpoints.down('sm') ? '4rem' : '3rem',
  boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
  transition: theme.transitions.create(['width', 'margin'], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  ...(open && {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  }),
}));

// Estilizados para el Drawer (abierto y cerrado)
const openedMixin = (theme) => ({
  width: drawerWidth,
  transition: theme.transitions.create('width', {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.enteringScreen,
  }),
  overflowX: 'hidden',
});
const closedMixin = (theme) => ({
  transition: theme.transitions.create('width', {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  overflowX: 'hidden',
  width: `calc(${theme.spacing(7)} + 1px)`,
  [theme.breakpoints.up('sm')]: {
    width: `calc(${theme.spacing(8)} + 1px)`,
  },
});

const Drawer = styled(MuiDrawer, { shouldForwardProp: (prop) => prop !== 'open' })(
  ({ theme, open }) => ({
    width: 0,
    flexShrink: 0,
    whiteSpace: 'nowrap',
    boxSizing: 'border-box',
    ...(open && {
      ...openedMixin(theme),
      '& .MuiDrawer-paper': openedMixin(theme),
    }),
    ...(!open && {
      ...closedMixin(theme),
      '& .MuiDrawer-paper': closedMixin(theme),
    }),
  })
);

const RootLayout = () => {
  const theme = useTheme();
  const location = useLocation();
  const navigate = useNavigate();
  const { logout, canCreate, isAdmin, user } = useAuth();
  const showSidebar = location.pathname !== '/';

  const [open, setOpen] = useState(false);

  const handleDrawerOpen = () => setOpen(true);
  const handleDrawerClose = () => setOpen(false);

  const handleLogout = async () => {
    try {
      await logout();
      navigate('/');
    } catch (error) {
      console.error('Error al cerrar sesión:', error);
    }
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      {showSidebar && (
        <AppBar position="fixed" open={open}>
          <Toolbar sx={{ minHeight: 'inherit', px: 2 }}>
            <IconButton
              color="inherit"
              aria-label="abrir menú"
              onClick={handleDrawerOpen}
              edge="start"
              sx={{ mr: 2, ...(open && { display: 'none' }) }}
            >
              <MenuIcon />
            </IconButton>
            <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
              MAPA DE ROCAS DEL MUSEO PETROGRÁFICO TOMAS FEININGER
            </Typography>
            <Button
              color="inherit"
              onClick={handleLogout}
              startIcon={<LogoutIcon />}
              size="small"
            >
              Salir
            </Button>
          </Toolbar>
        </AppBar>
      )}

      {showSidebar && (
        <Drawer variant="permanent" open={open}>
          <Toolbar
            sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'flex-end',
              px: 1
            }}
          >
            <IconButton onClick={handleDrawerClose}>
              {theme.direction === 'rtl' ? <ChevronRightIcon /> : <ChevronLeftIcon />}
            </IconButton>
          </Toolbar>
          <Divider />
          <List>
            {[
              { label: 'Mapa', to: '/mapa', icon: <MapIcon />, visible: true },
              { label: 'Crear', to: '/crear-elementos', icon: <DynamicFormIcon />, visible: canCreate },
              { label: 'Listar', to: '/listar-elementos', icon: <TableChartIcon />, visible: true },
              { label: 'Excel', to: '/carga-excel', icon: <CloudUploadIcon />, visible: isAdmin },
              { label: 'Usuarios', to: '/usuarios', icon: <PeopleIcon />, visible: isAdmin }
            ].filter(item => item.visible).map((item) => (
              <ListItem key={item.label} disablePadding sx={{ display: 'block' }}>
                <ListItemButton
                  component={Link}
                  to={item.to}
                  sx={{
                    minHeight: 48,
                    justifyContent: open ? 'initial' : 'center',
                    px: 2.5,
                  }}
                >
                  <ListItemIcon
                    sx={{
                      minWidth: 0,
                      mr: open ? 3 : 'auto',
                      justifyContent: 'center',
                    }}
                  >
                    {item.icon}
                  </ListItemIcon>
                  <ListItemText primary={item.label} sx={{ opacity: open ? 1 : 0 }} />
                </ListItemButton>
              </ListItem>
            ))}
          </List>
          <Divider />
          <Box sx={{ mt: 'auto' }}>
            <UserDrawerCard open={open} user={user} />
          </Box>
        </Drawer>
      )}

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 0,
          mt: showSidebar ? 0 : 0,
          transition: theme.transitions.create('margin', {
            easing: theme.transitions.easing.sharp,
            duration: theme.transitions.duration.enteringScreen,
          }),
        }}
      >
        <ErrorBoundary>
          <Outlet />
        </ErrorBoundary>
      </Box>
    </Box>
  );
};

export default RootLayout;