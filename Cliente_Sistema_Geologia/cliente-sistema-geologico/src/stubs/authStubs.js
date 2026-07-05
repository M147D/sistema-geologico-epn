// src/stubs/authStubs.js
// Respuestas de autenticación para VITE_USE_STUBS=true.
// Acepta cualquier contraseña; el rol se determina únicamente por el correo.
// Variables de módulo: se pierden en recarga de página → me() devuelve Admin por defecto.

const STUB_USERS = {
  'admin@sistemageologico.com': { id: 1, email: 'admin@sistemageologico.com', nombreCompleto: 'Admin Stub',    rol: 2, estadoActivo: true, userName: 'admin'    },
  'premium@test.com':           { id: 2, email: 'premium@test.com',           nombreCompleto: 'Premium Stub',  rol: 1, estadoActivo: true, userName: 'premium'  },
  'free@test.com':              { id: 3, email: 'free@test.com',               nombreCompleto: 'Free Stub',     rol: 0, estadoActivo: true, userName: 'free'     },
  'invitado@test.com':          { id: 4, email: 'invitado@test.com',           nombreCompleto: 'Invitado Stub', rol: 3, estadoActivo: true, userName: 'invitado' },
};

const DEFAULT_STUB_USER = STUB_USERS['admin@sistemageologico.com'];

let activeStubUser = null;

export const authStubs = {
  login({ email } = {}) {
    const user = STUB_USERS[email] ?? DEFAULT_STUB_USER;
    activeStubUser = user;
    return Promise.resolve({ success: true, token: 'stub-jwt-token', user });
  },

  me() {
    return Promise.resolve({ success: true, user: activeStubUser ?? DEFAULT_STUB_USER });
  },

  logout() {
    activeStubUser = null;
    return Promise.resolve({ success: true });
  },

  register({ email, nombreCompleto } = {}) {
    const user = { id: 99, email, nombreCompleto: nombreCompleto || 'Nuevo Usuario', rol: 0, estadoActivo: true, userName: email };
    activeStubUser = user;
    return Promise.resolve({ success: true, token: 'stub-jwt-token', user });
  },
};
