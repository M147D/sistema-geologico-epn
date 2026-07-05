// src/lib/apiClient.js
// Instancia única de Axios para toda la app. No es un Context porque no hay
// nada "reactivo" que React necesite propagar: el token vive en una variable
// de módulo (mismo efecto que un useRef, sin necesidad de Provider) y AuthContext
// es el único que la actualiza, a través de setApiToken/onUnauthorized.
import axios from 'axios';

const TOKEN_KEY = 'jwt_token';

let currentToken = localStorage.getItem(TOKEN_KEY);
let unauthorizedHandler = null;

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use(
  (config) => {
    if (currentToken) {
      config.headers.Authorization = `Bearer ${currentToken}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', {
      status: error.response?.status,
      data: error.response?.data,
      message: error.message
    });
    if (error.response?.status === 401) {
      unauthorizedHandler?.();
    }
    return Promise.reject(error);
  }
);

export const getStoredToken = () => currentToken;

export const setApiToken = (token) => {
  currentToken = token;
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY);
  }
};

/** AuthContext registra aquí qué hacer cuando el servidor responde 401. */
export const onUnauthorized = (handler) => {
  unauthorizedHandler = handler;
};