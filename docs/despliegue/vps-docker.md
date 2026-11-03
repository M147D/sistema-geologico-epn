# DEP-01 · Despliegue en un VPS con Docker Compose

**Estado:** probado en el equipo de desarrollo con `DOMINIO=localhost` el
2026-11-03 (CAM-03): build, migración, los cuatro servicios sanos, ART-01 OK
por HTTPS, navegador sin violaciones de CSP, y respaldo y restauración
verificados. **Todavía no se desplegó en ningún servidor.**

Servicios: `db` (PostGIS, solo red interna), `api`, `web` y `caddy` (único
con puertos publicados). Diagrama: [DIA-08](../diagramas/entornos.md).
Requisitos y variables: [ARQ-02](../arquitectura/reconstruir-desde-cero.md).

## 1. Servidor

1. Linux x86_64 o arm64 con Docker y Compose v2; al menos 2 GB de RAM.
2. Firewall con solo 22, 80 y 443 abiertos (p. ej. `ufw allow OpenSSH`,
   `ufw allow 80,443/tcp`, `ufw allow 443/udp`, `ufw enable`).
3. Un registro DNS `A`/`AAAA` del dominio apuntando al servidor (Caddy pide el
   certificado al arrancar).

## 2. Instalar

```bash
# Clave de despliegue de solo lectura en GitHub (Settings → Deploy keys).
git clone git@github.com:M147D/sistema-geologico-epn.git /opt/sistemageologico
cd /opt/sistemageologico
cp .env.example .env && chmod 600 .env
```

Completar `.env`:

```bash
openssl rand -hex 24     # POSTGRES_PASSWORD
openssl rand -hex 24     # APP_DB_PASSWORD
openssl rand -base64 48  # JWT_SECRET
```

`DOMINIO`, `APP_DB_USER` (p. ej. `sistemageologico`), `ADMIN_EMAIL`,
`ADMIN_PASSWORD` (mínimo 6, con dígito, minúscula y mayúscula) y, si se usa,
`SMTP_*` e `INFORME_DESTINO`.

```bash
docker compose -f compose.prod.yaml build
docker compose -f compose.prod.yaml up -d --wait db
docker compose -f compose.prod.yaml run --rm api --migrate
docker compose -f compose.prod.yaml up -d --wait
```

El primer arranque de `db` crea las dos bases, el usuario de la aplicación y
las capas GIS vacías (`docker/db/init/`). Solo corre con el volumen vacío.

## 3. Verificar

```bash
docker compose -f compose.prod.yaml ps                       # todo "healthy"
curl -sI https://<DOMINIO>/ | grep -i strict-transport       # HTTPS y HSTS
ADMIN_EMAIL=... ADMIN_PASSWORD=... scripts/artefactos/ART-01-prueba-humo.sh https://<DOMINIO>
```

ART-01 escribe datos de prueba (y los desactiva al final): correrlo en
producción solo en la instalación, o no correrlo. Después, ingresar como
administrador y **cambiar la contraseña** desde Usuarios.

## 4. Actualizar

```bash
cd /opt/sistemageologico
git pull
docker compose -f compose.prod.yaml build
docker compose -f compose.prod.yaml run --rm api --migrate   # si hay migraciones nuevas
docker compose -f compose.prod.yaml up -d --wait
```

Registrar cada despliegue como un `CAM-NN` nuevo, con el commit desplegado.

## 5. Respaldo y restauración

Probado el 2026-11-03 sobre la base de desarrollo, en una base nueva y con
el comando de abajo sobre la misma base: mismos usuarios, elementos, fotos y
bytes de imagen, y las tablas siguen a nombre del usuario de la aplicación.

```bash
# Respaldo (las fotos están en la base: el volcado las incluye)
docker compose -f compose.prod.yaml exec -T db sh -c 'exec pg_dump -U postgres -Fc sistemageologico_datos' > backup-datos-$(date +%F).dump
docker compose -f compose.prod.yaml exec -T db sh -c 'exec pg_dump -U postgres -Fc sistemageologico_gis'   > backup-gis-$(date +%F).dump

# Restauración sobre la base existente
docker compose -f compose.prod.yaml stop api
docker compose -f compose.prod.yaml exec -T db sh -c 'exec pg_restore -U postgres -d sistemageologico_datos --clean --if-exists --no-owner --role="$APP_DB_USER"' < backup-datos-AAAA-MM-DD.dump
docker compose -f compose.prod.yaml start api
```

Los respaldos se copian fuera del servidor. La frecuencia y el destino no
están decididos (PEN-11).

## 6. Cargar las capas GIS

Pendiente de los datos del IIGE (PEN-04). Las tablas existen vacías en
`sistemageologico_gis` con SRID 4326; la carga se hace como `postgres`, que es
su dueño (la aplicación solo tiene `SELECT`).

## 7. Qué no hacer

- `docker compose down -v` o `docker volume rm` sobre el proyecto de
  producción: borran la base.
- Publicar el puerto de `db`: para inspeccionarla, `docker compose exec db
  psql` o un túnel SSH.
- Dejar `ADMIN_PASSWORD` como la contraseña real después de cambiarla en la
  aplicación: se usa solo si el usuario no existe.
