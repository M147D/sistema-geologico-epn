# PLAN-2026-11-03 · Docker con base, backend y frontend separados, en PostgreSQL

Pedido del usuario (2026-11-03): traer el repo, escanearlo (REV-01), corregir
lo que haga falta mientras se adapta a Docker con la base de datos, el backend
y el frontend separados, y terminar con una auditoría de seguridad.

Decisiones del usuario (2026-11-03): consolidar en **PostgreSQL** (PEN-09),
base del catálogo **nueva y vacía**, capas GIS **sin datos por ahora**
(tablas vacías con el esquema que espera el código), alcance **desarrollo +
producción en un VPS**.

Decisión técnica (2026-11-03): **.NET 10 LTS** (10.0.14) en lugar de .NET 8,
que termina su soporte el 2026-11-10 (`releases-index.json` de Microsoft);
las migraciones se regeneran de todos modos.

## Servicios

| Servicio | Imagen | Rol |
|---|---|---|
| `db` | `postgis/postgis:18-3.6` | PostgreSQL con dos bases: catálogo e identidad, y capas GIS |
| `api` | .NET SDK (dev) / ASP.NET runtime (prod), puerto 8080 | API REST |
| `web` | Node + pnpm con Vite (dev) / Nginx sin privilegios (prod), 8080 | SPA |
| `caddy` | `caddy:2.10.2` (solo prod) | HTTPS; `/api/*` → `api`, el resto → `web` |

## Tareas

1. `.gitignore` explícito; `package.json` reconstruido del lockfile;
   `appsettings.json` sin secretos (PEN-02).
2. .NET 10 y paquetes al día; catálogo en Npgsql; migraciones regeneradas
   (`InitialPostgres`), `GETDATE()` → `now()`; timestamps (PEN-09).
3. Semilla idempotente de "Desconocido" (PEN-03); paso explícito
   `--migrate` para aplicar migraciones.
4. Miniatura y marca de agua con SkiaSharp, en un servicio propio (PEN-01).
5. Configuración obligatoria: clave JWT sin valor por defecto, CORS desde la
   configuración, encabezados reenviados por el proxy, `/health` (PEN-10).
6. Init de PostGIS: segunda base, extensión y tablas GIS vacías (PEN-04,
   parcial).
7. Docker de desarrollo (`compose.yaml`) y de producción
   (`docker/prod/*`, `compose.prod.yaml`, `Caddyfile`); probarlos.
8. Documentación: runbook, despliegue, entornos, CAM; estados de los PEN.
9. Auditoría con la skill `security-audit`.

Fuera de este plan: PEN-05, PEN-06, PEN-07 y PEN-08 (cambian el diseño del
esquema o de la lógica y se piden aparte).
