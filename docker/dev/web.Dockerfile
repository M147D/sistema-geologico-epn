# Desarrollo del frontend: Node + pnpm con Vite; el código se monta.
FROM node:26.13.0-trixie-slim

RUN npm install -g pnpm@10.34.9

# 1777: el volumen de dependencias hereda este modo y sirve para cualquier UID_HOST.
RUN mkdir -p /app/Cliente_Sistema_Geologia/cliente-sistema-geologico/node_modules \
 && chmod 1777 /app/Cliente_Sistema_Geologia/cliente-sistema-geologico/node_modules

USER node
