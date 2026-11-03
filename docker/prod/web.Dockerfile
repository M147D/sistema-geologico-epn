# Producción del frontend. Contexto: Cliente_Sistema_Geologia/cliente-sistema-geologico/
# Contexto adicional "config": docker/prod (nginx.conf).
FROM node:26.13.0-trixie-slim AS build
RUN npm install -g pnpm@10.34.9
WORKDIR /app
COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile
COPY . .
# Variables de Vite: se incrustan en el JS al compilar. No son secretos.
ARG VITE_API_URL=/api
ENV VITE_API_URL=$VITE_API_URL \
    VITE_USE_STUBS=false
RUN pnpm build

# Nginx sin privilegios: usuario 101, puerto 8080.
FROM nginxinc/nginx-unprivileged:1.29.8-alpine
COPY --from=config nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=3s --retries=3 --start-period=10s --start-interval=2s \
  CMD wget -q --spider http://127.0.0.1:8080/ || exit 1
