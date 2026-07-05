// src/components/users/UserTable.jsx
import { useState, useMemo } from 'react';
import {
  Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  TablePagination, TableSortLabel, Chip, IconButton, Tooltip, Typography,
  Stack, Box,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import LockResetIcon from '@mui/icons-material/LockReset';
import PersonOffIcon from '@mui/icons-material/PersonOff';
import RestoreIcon from '@mui/icons-material/Restore';

import { ROL_LABELS, ROL_COLORS } from '../../constants/roles';

const formatFecha = (dateStr) => {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleDateString('es-EC', { day: '2-digit', month: '2-digit', year: 'numeric' });
};

const UserTable = ({ usuarios, filtros, onEditar, onCambiarPassword, onToggleEstado, onReactivar, currentUserId }) => {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(15);
  const [orderBy, setOrderBy] = useState('fechaCreacion');
  const [order, setOrder] = useState('desc');

  const filtrados = useMemo(() => {
    let result = [...usuarios];

    if (filtros.busqueda) {
      const q = filtros.busqueda.toLowerCase();
      result = result.filter(u =>
        u.nombreCompleto?.toLowerCase().includes(q) ||
        u.email?.toLowerCase().includes(q) ||
        u.userName?.toLowerCase().includes(q)
      );
    }

    if (filtros.rol !== '' && filtros.rol !== undefined && filtros.rol !== null) {
      result = result.filter(u => u.rol === Number(filtros.rol));
    }

    if (filtros.estado === 'activos') {
      result = result.filter(u => u.estadoActivo);
    } else if (filtros.estado === 'inactivos') {
      result = result.filter(u => !u.estadoActivo);
    }

    result.sort((a, b) => {
      let aVal = a[orderBy] ?? '';
      let bVal = b[orderBy] ?? '';
      if (typeof aVal === 'string') aVal = aVal.toLowerCase();
      if (typeof bVal === 'string') bVal = bVal.toLowerCase();
      if (aVal < bVal) return order === 'asc' ? -1 : 1;
      if (aVal > bVal) return order === 'asc' ? 1 : -1;
      return 0;
    });

    return result;
  }, [usuarios, filtros, orderBy, order]);

  const handleSort = (col) => {
    if (orderBy === col) setOrder(o => o === 'asc' ? 'desc' : 'asc');
    else { setOrderBy(col); setOrder('asc'); }
    setPage(0);
  };

  const paginados = filtrados.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

  const SortLabel = ({ col, label }) => (
    <TableSortLabel active={orderBy === col} direction={orderBy === col ? order : 'asc'} onClick={() => handleSort(col)}>
      {label}
    </TableSortLabel>
  );

  return (
    <Paper elevation={1}>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
              <TableCell><SortLabel col="nombreCompleto" label="Nombre" /></TableCell>
              <TableCell><SortLabel col="email" label="Email" /></TableCell>
              <TableCell><SortLabel col="userName" label="Username" /></TableCell>
              <TableCell><SortLabel col="rol" label="Rol" /></TableCell>
              <TableCell><SortLabel col="estadoActivo" label="Estado" /></TableCell>
              <TableCell><SortLabel col="fechaCreacion" label="Creado" /></TableCell>
              <TableCell align="center">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginados.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} align="center" sx={{ py: 4 }}>
                  <Typography color="text.secondary">No se encontraron usuarios</Typography>
                </TableCell>
              </TableRow>
            ) : (
              paginados.map((u) => {
                const esSelf = String(u.id) === String(currentUserId);
                return (
                  <TableRow
                    key={u.id}
                    hover
                    sx={{ opacity: u.estadoActivo ? 1 : 0.6 }}
                  >
                    <TableCell>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <Typography variant="body2" fontWeight={esSelf ? 'bold' : 'normal'} component="span">
                          {u.nombreCompleto || '-'}
                        </Typography>
                        {esSelf && <Chip label="Tú" size="small" />}
                      </Box>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{u.email}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" color="text.secondary">{u.userName || '-'}</Typography>
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={ROL_LABELS[u.rol] ?? u.rol}
                        color={ROL_COLORS[u.rol] ?? 'default'}
                        size="small"
                      />
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={u.estadoActivo ? 'Activo' : 'Inactivo'}
                        color={u.estadoActivo ? 'success' : 'default'}
                        size="small"
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" color="text.secondary">
                        {formatFecha(u.fechaCreacion)}
                      </Typography>
                    </TableCell>
                    <TableCell align="center">
                      <Stack direction="row" justifyContent="center" spacing={0.5}>
                        <Tooltip title="Editar">
                          <IconButton size="small" onClick={() => onEditar(u)} color="primary">
                            <EditIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Cambiar contraseña">
                          <IconButton size="small" onClick={() => onCambiarPassword(u)} color="warning">
                            <LockResetIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        {u.estadoActivo ? (
                          <Tooltip title={esSelf ? 'No puedes desactivarte' : 'Desactivar'}>
                            <span>
                              <IconButton
                                size="small"
                                onClick={() => onToggleEstado(u)}
                                color="error"
                                disabled={esSelf}
                              >
                                <PersonOffIcon fontSize="small" />
                              </IconButton>
                            </span>
                          </Tooltip>
                        ) : (
                          <Tooltip title="Reactivar">
                            <IconButton size="small" onClick={() => onReactivar(u)} color="success">
                              <RestoreIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        )}
                      </Stack>
                    </TableCell>
                  </TableRow>
                );
              })
            )}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={filtrados.length}
        page={page}
        onPageChange={(_, p) => setPage(p)}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={(e) => { setRowsPerPage(Number(e.target.value)); setPage(0); }}
        rowsPerPageOptions={[10, 15, 25, 50]}
        labelRowsPerPage="Filas:"
        labelDisplayedRows={({ from, to, count }) => `${from}-${to} de ${count}`}
      />
    </Paper>
  );
};

export default UserTable;
