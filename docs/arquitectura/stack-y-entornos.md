# ARQ-03 · Stack y entornos

Versiones leídas de `Servidor_Sistema_Geologia/Servidor_Sistema_Geologia.csproj`,
de `Cliente_Sistema_Geologia/cliente-sistema-geologico/pnpm-lock.yaml` (versión
resuelta) y de las imágenes en `docker/` y `compose*.yaml`, commit `8dfe171`.
Arquitectura: [ARQ-01](diagrama.md). Diagramas: [DIA-00](../diagramas/indice.md).
Cambios de versión: [CAM-02](../control-cambios.md).

## Frontend

| Tecnología | Versión | Función | Por qué | Fuente |
|---|---|---|---|---|
| React | 18.3.1 | UI | Motivo no registrado | `pnpm-lock.yaml` |
| Vite + `@vitejs/plugin-react-swc` | 6.4.2 / 3.11.0 | servidor de desarrollo con HMR y build estático | Motivo no registrado | `pnpm-lock.yaml` |
| MUI (`@mui/material`, `@mui/icons-material`) + Emotion | 6.5.0 / 11.14 | componentes | Motivo no registrado | `pnpm-lock.yaml` |
| Leaflet / react-leaflet | 1.9.4 / 4.2.1 | mapas y capas GeoJSON | Motivo no registrado | `pnpm-lock.yaml` |
| Axios | 1.15.0 | cliente HTTP | Motivo no registrado | `pnpm-lock.yaml` |
| react-router-dom | 7.14.0 | rutas | Motivo no registrado | `pnpm-lock.yaml` |
| react-hook-form + yup + `@hookform/resolvers` | 7.72.1 / 1.7.1 / 4.1.3 | formularios y validación | Motivo no registrado | `pnpm-lock.yaml` |
| xlsx (SheetJS) | 0.18.5 | lectura de Excel para la carga masiva | Motivo no registrado | `pnpm-lock.yaml` |
| `@faker-js/faker` | 9.9.0 | datos falsos de los stubs | Motivo no registrado | `pnpm-lock.yaml` |
| `@react-oauth/google` | 0.12.2 | instalada; el login con Google no está implementado | Motivo no registrado | `pnpm-lock.yaml` |
| pnpm | 10.34.9 (`packageManager`) | gestor de paquetes; npm prohibido | Decisión del equipo, registrada en el `CLAUDE.md` borrado en `294fd37`; la línea 10 lee el lockfile v9 sin reescribirlo | `package.json` |
| Node.js | 26.13.0 (LTS) | compila el frontend; solo en contenedores | La LTS más nueva el 2026-11-03 (`nodejs.org/dist/index.json`); el README decía 20 | `docker/dev/web.Dockerfile`, `docker/prod/web.Dockerfile` |
| Nginx sin privilegios | 1.29.8-alpine | sirve `dist/` en producción (usuario 101, 8080) | Plantilla de la skill `docker-project-setup` | `docker/prod/web.Dockerfile` |

## Backend

| Tecnología | Versión | Función | Por qué | Fuente |
|---|---|---|---|---|
| .NET / ASP.NET Core | SDK 10.0.403, runtime 10.0.14 (`net10.0`) | API REST | .NET 8 termina su soporte el 2026-11-10; 10 es LTS hasta 2028-11-14 (`releases-index.json` de Microsoft). Decisión del 2026-11-03 (CAM-02) | `.csproj`, `docker/*/api.Dockerfile` |
| EF Core (Npgsql) | Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3 | ORM y migraciones de las dos bases | Consolidar en un motor (pedido del usuario, 2026-11-03; REV-01 H-10) | `.csproj` |
| NetTopologySuite + IO.GeoJSON | 2.6.0 / 4.0.0 | geometrías y GeoJSON | Motivo no registrado | `.csproj` |
| ASP.NET Core Identity (EF) | 10.0.14 | usuarios, contraseñas, bloqueo | Motivo no registrado | `.csproj` |
| JwtBearer / System.IdentityModel.Tokens.Jwt | 10.0.14 / 8.23.0 | autenticación por token | Motivo no registrado | `.csproj` |
| SkiaSharp + NativeAssets.Linux.NoDependencies | 4.154.0 | miniaturas y marca de agua | Multiplataforma (System.Drawing solo funciona en Windows, REV-01 H-01) y licencia MIT; se descartó ImageSharp por su licencia dividida | `.csproj` |
| MailKit | 4.18.2 | correo SMTP | Motivo no registrado | `.csproj` |
| Swashbuckle | 10.3.1 | Swagger en desarrollo | Motivo no registrado | `.csproj` |
| HealthChecks.EntityFrameworkCore | 10.0.14 | `/health` con las dos bases | Para el `HEALTHCHECK` de Docker | `.csproj` |

Se quitaron el 2026-11-03 los paquetes que ningún código usaba:
`Google.Apis.Auth`, `Authentication.Google`, `Authentication.Cookies`,
`Identity.UI`, `CodeGeneration.Design`, y los que dejaron de usarse:
`EntityFrameworkCore.SqlServer`, `System.Drawing.Common`.

## Datos y servicios

| Tecnología | Versión | Función | Por qué | Fuente |
|---|---|---|---|---|
| PostgreSQL + PostGIS | `postgis/postgis:18-3.6` | dos bases: catálogo e identidad; capas GIS | Un solo motor (pedido del usuario, 2026-11-03); antes SQL Server + PostgreSQL 16 | `compose*.yaml` |
| Caddy | 2.10.2-alpine | HTTPS automático y proxy inverso en producción | Plantilla de la skill `docker-project-setup` | `compose.prod.yaml`, `Caddyfile` |
| SMTP | — | solicitud de informe petrográfico | — | `EmailService.cs` |
| Teselas de OpenStreetMap | — | mapa base | — | `src/constants/mapConstants.js` |

## Arquitectura de desarrollo

Todo corre en contenedores (`compose.yaml`); en el equipo solo hacen falta
git y Docker. Diagrama: [DIA-08](../diagramas/entornos.md).

- `db` (PostGIS) sin puerto publicado; `api` en `127.0.0.1:5095` (Swagger en
  `/swagger`); `web` en `127.0.0.1:5174`, con proxy de `/api` a `api:5095`.
- El repo se monta en `/app`; los contenedores corren con el usuario del
  equipo (`UID_HOST`/`GID_HOST`); NuGet y `node_modules` viven en volúmenes.
- **Qué recarga solo:** el frontend (HMR de Vite) y el backend (`dotnet watch`
  recompila y reinicia; sin Hot Reload, ver PS-02).
- **Qué exige un paso:** una migración nueva (`--migrate`) y un cambio en un
  Dockerfile (`docker compose build`).
- **Sin tests:** el repo no tiene proyecto de tests; la verificación es la
  prueba de humo ART-01, el lint y el build.
- **Capas GIS vacías** hasta cargar los datos del IIGE (PEN-04).

## Arquitectura de despliegue

Un VPS con Docker Compose (`compose.prod.yaml`), detrás de Caddy. Diagrama:
[DIA-08](../diagramas/entornos.md); procedimiento: [DEP-01](../despliegue/vps-docker.md).
Producción todavía no existe: se probó en el equipo con `DOMINIO=localhost`
(2026-11-03).

| | Desarrollo | Producción |
|---|---|---|
| URL | `http://localhost:5174` (web), `http://localhost:5095` (api) | `https://<DOMINIO>`; la API en `https://<DOMINIO>/api` |
| Imágenes | SDK de .NET y Node con el código montado | multi-etapa: runtime de ASP.NET y Nginx, sin código fuente |
| Configuración | valores de prueba en `compose.yaml`, sobreescribibles con `.env` | `.env` en el servidor (`chmod 600`); sin valores por defecto |
| Credenciales | falsas a propósito (`solo-desarrollo`) | generadas al instalar (DEP-01) |
| Datos | volumen `sistemageologico_db-datos` | volumen `sistemageologico-prod_db-datos`, con respaldo (DEP-01) |
| Migraciones | `docker compose run --rm api dotnet run --no-launch-profile -- --migrate` | `docker compose -f compose.prod.yaml run --rm api --migrate` |
| Versiones | las mismas imágenes base y paquetes | idem |
