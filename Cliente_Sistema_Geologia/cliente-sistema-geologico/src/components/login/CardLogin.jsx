import { useState } from "react";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { Link } from "react-router-dom";
import {
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Box,
  InputAdornment,
  IconButton,
  Stack,
  CircularProgress,
  Alert,
} from "@mui/material";
import {
  Email as EmailIcon,
  Visibility,
  VisibilityOff,
  Login as LoginIcon,
  LockOutlined as LockOutlinedIcon,
} from "@mui/icons-material";

const schema = yup.object({
  email: yup.string().email("Correo inválido").required("El correo es obligatorio"),
  password: yup
    .string()
    .min(6, "Mínimo 6 caracteres")
    .required("La contraseña es obligatoria"),
}).required();

const CardLogin = ({ onSubmit, loading, error }) => {
  const [showPassword, setShowPassword] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: yupResolver(schema),
  });

  const isLocked = error?.toLowerCase().includes('bloqueada');

  return (
    <Card sx={{ maxWidth: 400, width: "100%", mx: "auto", mt: 10, p: 2 }}>
      <CardContent>
        <Typography variant="h5" align="center" gutterBottom>
          Iniciar sesión
        </Typography>

        {error && (
          <Alert
            severity={isLocked ? 'warning' : 'error'}
            icon={isLocked ? <LockOutlinedIcon fontSize="inherit" /> : undefined}
            sx={{ mt: 2 }}
          >
            {error}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mt: 2 }}>
          <TextField
            fullWidth
            margin="normal"
            label="Correo electrónico"
            type="email"
            autoComplete="email"
            disabled={isLocked}
            {...register("email")}
            error={!!errors.email}
            helperText={errors.email?.message}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <EmailIcon />
                </InputAdornment>
              ),
            }}
          />

          <TextField
            fullWidth
            margin="normal"
            label="Contraseña"
            type={showPassword ? "text" : "password"}
            autoComplete="current-password"
            disabled={isLocked}
            {...register("password")}
            error={!!errors.password}
            helperText={errors.password?.message}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPassword((prev) => !prev)}
                    edge="end"
                    disabled={isLocked}
                  >
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />

          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            endIcon={
              loading
                ? <CircularProgress size={20} color="inherit" />
                : isLocked
                ? <LockOutlinedIcon />
                : <LoginIcon />
            }
            disabled={loading || isLocked}
          >
            {loading ? 'Iniciando sesión...' : isLocked ? 'Cuenta bloqueada' : 'Iniciar sesión'}
          </Button>

          <Stack spacing={1} direction="row" justifyContent="center" sx={{ mt: 1 }}>
            <Link to="/register" style={{ textDecoration: 'none' }}>
              <Typography color="primary" variant="body2">
                ¿No tienes cuenta? Registrarse
              </Typography>
            </Link>
          </Stack>
        </Box>
      </CardContent>
    </Card>
  );
};

export default CardLogin;
