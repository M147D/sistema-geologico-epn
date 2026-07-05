import React from 'react';
import { Box, Paper, Typography, Button } from '@mui/material';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';

class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error };
  }

  componentDidCatch(error, info) {
    console.error('ErrorBoundary:', error, info.componentStack);
  }

  render() {
    if (this.state.hasError) {
      return (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
          <Paper sx={{ p: 4, textAlign: 'center', maxWidth: 420 }}>
            <ErrorOutlineIcon sx={{ fontSize: 48, color: 'error.main', mb: 2 }} />
            <Typography variant="h6" gutterBottom>Algo salió mal</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              {this.state.error?.message || 'Error inesperado en la aplicación.'}
            </Typography>
            <Button
              variant="contained"
              onClick={() => this.setState({ hasError: false, error: null })}
            >
              Reintentar
            </Button>
          </Paper>
        </Box>
      );
    }
    return this.props.children;
  }
}

export default ErrorBoundary;
