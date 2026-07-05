import { Snackbar, Alert } from '@mui/material';

const LABELS = { fosil: 'Fósil', mineral: 'Mineral', roca: 'Roca' };

const Notifications = ({ success, setSuccess, error, setError, tipo }) => (
  <>
    <Snackbar open={success} autoHideDuration={6000} onClose={() => setSuccess(false)}>
      <Alert severity="success" sx={{ width: '100%' }}>
        {LABELS[tipo] ?? tipo} creado/a exitosamente
      </Alert>
    </Snackbar>

    <Snackbar open={!!error} autoHideDuration={6000} onClose={() => setError(null)}>
      <Alert severity="error" sx={{ width: '100%' }}>
        {error}
      </Alert>
    </Snackbar>
  </>
);

export default Notifications;
