# ARQ-02 · Reconstruir desde cero

Llevar el proyecto a funcionar en un equipo nuevo. Comandos probados el
2026-11-03 en Linux (WSL2) con Docker 29.5.3 y Compose 5.1.4. Arquitectura:
[ARQ-01](diagrama.md); stack: [ARQ-03](stack-y-entornos.md); producción:
[DEP-01](../despliegue/vps-docker.md).

## 1. Requisitos

- git y Docker con Compose v2 (`docker compose version`). Nada más: .NET,
  Node y PostgreSQL corren en contenedores.
- Arquitectura x86_64 o arm64 (las imágenes base tienen las dos).

## 2. Clonar

```bash
git clone git@github.com:M147D/sistema-geologico-epn.git SistemaGeologico
cd SistemaGeologico
```

## 3. Variables de entorno

En desarrollo no hace falta nada: `compose.yaml` trae valores de prueba. Si tu
usuario no es 1000 (`id -u`), crea `.env` con `UID_HOST` y `GID_HOST` (ver
`.env.example`).

| Variable | Desarrollo | Producción | Si falta |
|---|---|---|---|
| `UID_HOST`, `GID_HOST` | tu usuario (1000 por defecto) | — | archivos generados con otro dueño |
| `POSTGRES_PASSWORD`, `APP_DB_PASSWORD` | `solo-desarrollo` | aleatorias (DEP-01) | prod no arranca |
| `JWT_SECRET` | clave falsa de prueba | `openssl rand -base64 48` | prod no arranca |
| `ADMIN_EMAIL`, `ADMIN_PASSWORD` | `admin@sistemageologico.local` / `SoloDesarrollo1` | las del responsable | prod no arranca |
| `SMTP_*`, `INFORME_DESTINO` | vacías | las del correo del museo | el informe petrográfico no se envía |

Tabla completa de claves: ARQ-01, "Variables de entorno".

## 4. Credenciales

No hay credenciales en el repo ni en las imágenes. En producción viven solo
en `.env` del servidor. Las del SMTP las provee la cuenta de correo del museo
(quién la administra: desconocido). Nunca se commitea `.env`.

## 5. Levantar en desarrollo

```bash
docker compose up -d --build                        # db, api y web
docker compose run --rm api dotnet run --no-launch-profile -- --migrate   # primera vez y tras cada migración
curl http://127.0.0.1:5095/health                   # → Healthy
```

Abrir `http://localhost:5174` e ingresar con `admin@sistemageologico.local` /
`SoloDesarrollo1`. Swagger: `http://localhost:5095/swagger`.

La primera vez, `web` instala las dependencias (unos minutos) antes de
arrancar Vite: `docker compose logs -f web`.

## 6. Línea base antes de cambiar nada

```bash
docker compose exec web pnpm lint
docker compose exec web pnpm build
docker compose run --rm --no-deps api dotnet build
ADMIN_EMAIL=admin@sistemageologico.local ADMIN_PASSWORD=SoloDesarrollo1 \
  scripts/artefactos/ART-01-prueba-humo.sh http://127.0.0.1:5174
node scripts/check-docs.mjs    # o: docker compose exec web node /app/scripts/check-docs.mjs
```

Resultado esperado el 2026-11-03: lint sin errores, build con un aviso de
tamaño de chunk (1,38 MB), `dotnet build` sin errores, ART-01 "Prueba de humo:
OK".

## 7. Crear una migración

```bash
docker compose run --rm api dotnet ef migrations add <Nombre> --context SistemaGeologicoDbContext --output-dir Migrations
docker compose run --rm api dotnet run --no-launch-profile -- --migrate
```

## 8. Desplegar

[DEP-01](../despliegue/vps-docker.md).

## 9. Comandos de todos los días

| Necesidad | Comando |
|---|---|
| Estado y logs | `docker compose ps`, `docker compose logs -f api` |
| Reconstruir tras cambiar un Dockerfile | `docker compose build` y `docker compose up -d` |
| Cambió `pnpm-lock.yaml` | `docker compose restart web` (instala al arrancar) |
| Volumen de dependencias roto | `docker compose down`, `docker volume rm sistemageologico_dependencias`, `docker compose up -d` |
| Consola SQL | `docker compose exec db psql -U postgres -d sistemageologico_datos` |
| Detener | `docker compose down` (sin `-v`: `-v` borra la base) |

## 10. Síntoma → dónde mirar

| Síntoma | Dónde mirar |
|---|---|
| `Falta la configuración obligatoria '…'` al arrancar la API | variables de entorno (sección 3; ARQ-01) |
| `No se encontró una fuente para la marca de agua` | la imagen no tiene `fonts-dejavu-core` o `ImageProcessing__WatermarkFontPath` mal puesta; PS-01 |
| Usuarios Free ven fotos sin marca de agua | PS-01 |
| `api` se detiene sola tras editar código | PS-02 |
| `relation "Usuarios" does not exist` | faltan las migraciones: `--migrate` (sección 5) |
| Mapa geológico sin formaciones | las capas GIS están vacías: PEN-04 |
| `pnpm install` falla con `ERR_PNPM_OUTDATED_LOCKFILE` | se cambió `package.json` sin actualizar el lockfile: `docker compose run --rm web pnpm install` |
| Archivos de `bin/`, `obj/` o `dist/` con dueño root | `UID_HOST`/`GID_HOST` (sección 3) |
