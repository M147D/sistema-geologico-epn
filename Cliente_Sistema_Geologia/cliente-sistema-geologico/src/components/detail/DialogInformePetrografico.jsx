import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, Typography, Box, Divider,
  Alert, CircularProgress, Chip, Stack
} from '@mui/material';
import ScienceIcon from '@mui/icons-material/Science';
import SendIcon from '@mui/icons-material/Send';
import LocationOnIcon from '@mui/icons-material/LocationOn';

const DialogInformePetrografico = ({ open, onClose, onSubmit, elemento, userEmail }) => {
  const [correoSolicitante, setCorreoSolicitante] = useState(userEmail || '');
  const [observaciones, setObservaciones] = useState('');
  const [enviando, setEnviando] = useState(false);
  const [resultado, setResultado] = useState(null);

  const handleClose = () => {
    if (enviando) return;
    setCorreoSolicitante(userEmail || '');
    setObservaciones('');
    setResultado(null);
    onClose();
  };

  const handleSubmit = async () => {
    if (!correoSolicitante.trim() || !emailValido) return;
    setEnviando(true);
    setResultado(null);
    try {
      const result = await onSubmit({
        correoSolicitante: correoSolicitante.trim(),
        observaciones: observaciones.trim() || null,
      });
      setResultado(result);
      if (result?.success) {
        setTimeout(() => handleClose(), 2500);
      }
    } catch (err) {
      const msg = err.response?.data?.message || err.message || 'Error al enviar la solicitud';
      setResultado({ success: false, message: msg });
    } finally {
      setEnviando(false);
    }
  };

  const ubicacionTexto = (() => {
    const u = elemento?.ubicacion;
    if (!u) return 'No especificada';
    return [u.localidad, u.nombreProvincia, u.nombrePais].filter(Boolean).join(', ') || 'No especificada';
  })();

  const emailValido = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correoSolicitante);
  const yaEnviado = resultado?.success === true;

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      fullWidth
      PaperProps={{ sx: { borderTop: 3, borderColor: 'primary.main' } }}
    >
      <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1, pb: 1 }}>
        <ScienceIcon color="primary" />
        <Typography variant="h6" fontWeight={600}>Solicitar Informe Petrográfico</Typography>
      </DialogTitle>

      <DialogContent dividers>
        {/* Información del elemento (readonly) */}
        <Box sx={{ mb: 2 }}>
          <Typography variant="caption" color="text.secondary" fontWeight={600} sx={{ textTransform: 'uppercase', letterSpacing: 0.5 }}>
            Elemento seleccionado
          </Typography>
          <Stack direction="row" spacing={1} flexWrap="wrap" sx={{ mt: 0.75, mb: 1 }}>
            <Chip label={elemento?.nombre} size="small" color="primary" />
            <Chip label={`Código: ${elemento?.codigo}`} size="small" variant="outlined" />
            <Chip label={elemento?.tipoElemento} size="small" color="secondary" variant="outlined" />
          </Stack>
          {ubicacionTexto !== 'No especificada' && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <LocationOnIcon fontSize="small" color="action" />
              <Typography variant="body2" color="text.secondary">{ubicacionTexto}</Typography>
            </Box>
          )}
        </Box>

        <Divider sx={{ my: 2 }} />

        {/* Correo del solicitante */}
        <TextField
          label="Correo del solicitante"
          value={correoSolicitante}
          onChange={e => setCorreoSolicitante(e.target.value)}
          fullWidth
          required
          type="email"
          size="small"
          sx={{ mb: 2 }}
          error={correoSolicitante.length > 0 && !emailValido}
          helperText={
            correoSolicitante.length > 0 && !emailValido
              ? 'Ingrese un correo electrónico válido'
              : 'Se usará para contactarle con la respuesta del informe'
          }
          disabled={enviando || yaEnviado}
        />

        {/* Observaciones */}
        <TextField
          label="Observaciones / Comentarios"
          value={observaciones}
          onChange={e => setObservaciones(e.target.value)}
          fullWidth
          multiline
          rows={4}
          size="small"
          inputProps={{ maxLength: 2000 }}
          helperText={`${observaciones.length} / 2000 caracteres`}
          placeholder="Indique el propósito del informe, aspectos de interés, urgencia, etc."
          disabled={enviando || yaEnviado}
        />

        {/* Resultado */}
        {resultado && (
          <Alert
            severity={resultado.success ? 'success' : 'warning'}
            sx={{ mt: 2 }}
          >
            {resultado.message}
          </Alert>
        )}
      </DialogContent>

      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button onClick={handleClose} disabled={enviando}>
          {yaEnviado ? 'Cerrar' : 'Cancelar'}
        </Button>
        {!yaEnviado && (
          <Button
            variant="contained"
            onClick={handleSubmit}
            disabled={enviando || !emailValido}
            startIcon={enviando ? <CircularProgress size={16} color="inherit" /> : <SendIcon />}
          >
            {enviando ? 'Enviando...' : 'Enviar solicitud'}
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
};

export default DialogInformePetrografico;
