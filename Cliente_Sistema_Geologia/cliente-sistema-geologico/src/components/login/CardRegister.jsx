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
  Person as PersonIcon,
  Visibility,
  VisibilityOff,
  PersonAdd as PersonAddIcon,
  PersonAddOutlined as PersonAddOutlinedIcon,
} from "@mui/icons-material";

const schema = yup.object({
  nombreCompleto: yup.string().max(200, "Máximo 200 caracteres"),
  email: yup.string().email("Correo inválido").required("El correo es obligatorio"),
  password: yup
    .string()
    .min(6, "Mínimo 6 caracteres")
    .matches(/[0-9]/, "Debe contener al menos un número")
    .matches(/[a-z]/, "Debe contener al menos una minúscula")
    .matches(/[A-Z]/, "Debe contener al menos una mayúscula")
    .required("La contraseña es obligatoria"),
  confirmPassword: yup
    .string()
    .oneOf([yup.ref("password")], "Las contraseñas no coinciden")
    .required("Confirme la contraseña"),
}).required();

const CardRegister = ({ onSubmit, loading, error }) => {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: yupResolver(schema),
  });

  return (
    <Card sx={{ maxWidth: 400, width: "100%", mx: "auto", mt: 4 }}>
      <CardContent>
        <Stack direction="row" alignItems="center" justifyContent="center" spacing={1} sx={{ mb: 1 }}>
          <PersonAddOutlinedIcon color="primary" />
          <Typography variant="h5" align="center">
            Crear cuenta
          </Typography>
        </Stack>

        {error && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {error}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mt: 2 }}>
          <TextField
            fullWidth
            margin="normal"
            label="Nombre completo"
            autoComplete="name"
            {...register("nombreCompleto")}
            error={!!errors.nombreCompleto}
            helperText={errors.nombreCompleto?.message}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <PersonIcon />
                </InputAdornment>
              ),
            }}
          />

          <TextField
            fullWidth
            margin="normal"
            label="Correo electrónico"
            type="email"
            autoComplete="email"
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
            autoComplete="new-password"
            {...register("password")}
            error={!!errors.password}
            helperText={errors.password?.message}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPassword((prev) => !prev)}
                    edge="end"
                  >
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />

          <TextField
            fullWidth
            margin="normal"
            label="Confirmar contraseña"
            type={showConfirm ? "text" : "password"}
            autoComplete="new-password"
            {...register("confirmPassword")}
            error={!!errors.confirmPassword}
            helperText={errors.confirmPassword?.message}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowConfirm((prev) => !prev)}
                    edge="end"
                  >
                    {showConfirm ? <VisibilityOff /> : <Visibility />}
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
                : <PersonAddIcon />
            }
            disabled={loading}
          >
            {loading ? "Registrando..." : "Crear cuenta"}
          </Button>

          <Stack spacing={1} direction="row" justifyContent="center" sx={{ mt: 1 }}>
            <Link to="/" style={{ textDecoration: "none" }}>
              <Typography color="primary" variant="body2">
                ¿Ya tienes cuenta? Iniciar sesión
              </Typography>
            </Link>
          </Stack>
        </Box>
      </CardContent>
    </Card>
  );
};

export default CardRegister;