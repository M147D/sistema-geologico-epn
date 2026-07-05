// src/pages/PageLogin.jsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Box } from '@mui/material';
import { useAuth } from '../context/AuthContext';
import CardLogin from '../components/login/CardLogin';

const PageLogin = () => {
  const { login, loading } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState(null);

  const handleLogin = async (data) => {
    setError(null);
    try {
      await login(data);
      navigate('/mapa');
    } catch (err) {
      setError(err.response?.data?.message || 'Correo o contraseña incorrectos');
    }
  };

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '10vh',
        }}
      >
        <CardLogin onSubmit={handleLogin} loading={loading} error={error} />
      </Box>
    </Container>
  );
};

export default PageLogin;