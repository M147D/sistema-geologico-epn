# ARQ-03 · Stack y entornos

Versiones leídas de `Servidor_Sistema_Geologia/Servidor_Sistema_Geologia.csproj`
y de `Cliente_Sistema_Geologia/cliente-sistema-geologico/pnpm-lock.yaml`
(versión resuelta, no el rango), commit `d30f189`. Arquitectura:
[ARQ-01](diagrama.md). Diagramas: [DIA-00](../diagramas/indice.md).

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
| `@react-oauth/google` | 0.12.2 | instalada; el login con Google no está implementado | Motivo no registrado | `pnpm-lock.yaml`, `CLAUDE.md` borrado en `294fd37` |
| pnpm | lockfile v9 | gestor de paquetes; npm prohibido | Decisión del equipo, registrada en el `CLAUDE.md` borrado en `294fd37` | `git show 294fd37^:CLAUDE.md` |
| Node.js | 20 | compila el frontend | Motivo no registrado | `README.md` |

## Backend

| Tecnología | Versión | Función | Por qué | Fuente |
|---|---|---|---|---|
| ASP.NET Core (`net8.0`) | 8.0 | API REST | Motivo no registrado | `.csproj` |
| EF Core SqlServer / Design | 8.0.0 | ORM y migraciones del catálogo | Motivo no registrado | `.csproj` |
| Npgsql EF Core + NetTopologySuite | 8.0.10 / 2.6.0 | lectura de PostGIS y geometrías | Motivo no registrado | `.csproj` |
| NetTopologySuite.IO.GeoJSON | 4.0.0 | serializa geometrías a GeoJSON | Motivo no registrado | `.csproj` |
| ASP.NET Core Identity (EF) | 8.0.0 | usuarios, contraseñas, bloqueo | Motivo no registrado | `.csproj` |
| JwtBearer / System.IdentityModel.Tokens.Jwt | 8.0.0 | autenticación por token | Motivo no registrado | `.csproj` |
| System.Drawing.Common | 8.0.0 | miniaturas y marca de agua; solo funciona en Windows (REV-01, H-01) | Motivo no registrado | `.csproj` |
| MailKit | 4.7.1.1 | correo SMTP | Motivo no registrado | `.csproj` |
| Swashbuckle | 7.2.0 | Swagger en desarrollo | Motivo no registrado | `.csproj` |
| Google.Apis.Auth, Authentication.Google, Authentication.Cookies, Identity.UI, CodeGeneration.Design | 1.69.0 / 8.0.0 / 2.3.0 / 8.0.0 / 8.0.7 | referenciadas; ningún código de `Program.cs` las configura | Motivo no registrado | `.csproj` |

## Datos y servicios

| Tecnología | Versión | Función | Por qué | Fuente |
|---|---|---|---|---|
| SQL Server (SQLEXPRESS) | no registrada | identidad, catálogo y fotos | Motivo no registrado | `README.md` |
| PostgreSQL + PostGIS | 16 (README) | capas del mapa geológico (IIGE, 2017) | consultas espaciales (`ST_Simplify`) | `README.md`, `GeologiaGISRepository.cs` |
| SMTP | — | solicitud de informe petrográfico | — | `EmailService.cs` |
| Teselas de OpenStreetMap | — | mapa base | — | `src/constants/mapConstants.js` |

## Arquitectura de desarrollo

Hoy, sin contenedores: SQL Server y PostgreSQL instalados en el equipo,
`dotnet run` en `Servidor_Sistema_Geologia/` (puerto 5095) y `pnpm dev` en el
frontend (5174, con proxy de `/api` al 5095). Comandos tomados del `CLAUDE.md`
borrado en `294fd37`; el diagrama de entornos se dibuja cuando exista el
despliegue en Docker.

Advertencias reales:

- **El repo no se construye tal cual:** faltan `package.json` y
  `appsettings.json` (REV-01, H-02).
- **Las capas GIS no están en el repo** (REV-01, H-04).
- **Desarrollo en Windows:** la marca de agua solo funciona allí (REV-01,
  H-01), y el `CLAUDE.md` borrado documentaba `taskkill` para reiniciar el
  backend.
- **Recarga:** Vite recarga solo (HMR); el backend necesita `dotnet watch run`
  o reiniciarse.
- **No hay tests** (según el `CLAUDE.md` borrado; no hay proyecto de tests en
  la solución).

## Arquitectura de despliegue

No hay despliegue registrado en el repo. El README describe el esquema
previsto: el frontend como estáticos de `dist/` y el backend como binarios
`.dll` en el puerto 5095.

| | Desarrollo | Producción |
|---|---|---|
| URL frontend | `http://localhost:5174` | desconocido |
| URL API | `http://localhost:5095` (o `/api` por el proxy de Vite) | desconocido |
| Configuración | `appsettings.json` local, fuera de git | desconocido |
| Datos | SQL Server y PostgreSQL locales | desconocido |
