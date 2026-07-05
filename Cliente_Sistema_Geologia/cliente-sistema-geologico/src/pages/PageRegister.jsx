import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Box } from '@mui/material';
import { useAuth } from '../context/AuthContext';
import CardRegister from '../components/login/CardRegister';

const PageRegister = () => {
  const { register: registerUser, loading } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState(null);

  const handleRegister = async (data) => {
    setError(null);
    try {
      await registerUser({
        email: data.email,
        password: data.password,
        confirmPassword: data.confirmPassword,
        nombreCompleto: data.nombreCompleto || undefined,
      });
      navigate('/mapa');
    } catch (err) {
      setError(err.message || 'Error al registrar');
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
        <CardRegister onSubmit={handleRegister} loading={loading} error={error} />
      </Box>
    </Container>
  );
};

export default PageRegister;