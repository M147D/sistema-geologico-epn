# Control de cambios

Despliegues, cambios de versión, migraciones de datos, cambios de
arquitectura y revisiones de diseño: lo que git no dice (por qué, y si llegó
a producción). El resto de los cambios está en `git log`. Las entradas no se
reescriben: un despliegue posterior es una entrada nueva. Para agregar una:
siguiente número libre (`grep -h "^## CAM-" docs/control-cambios.md`).

## CAM-01 · Primera revisión de diseño y documentación

**Fecha:** 2026-11-03.

**Tipo:** revisión.

**Commit:** `d30f189` (revisado).

**Qué cambió y por qué:** el repo se trajo a `~/proyectos/SistemaGeologico`
para dockerizar la base de datos, el backend y el frontend por separado
(pedido del usuario, 2026-11-03). Antes se escaneó el diseño (REV-01) y se
creó esta documentación. Para poder versionarla se quitaron de
`.gitignore` las reglas `*.md` y `docs/`.

**Producción:** no aplica (no cambia código).

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md),
[ARQ-01](arquitectura/diagrama.md), PEN-01 a PEN-10 en
[pendientes](pendientes.md).

## CAM-02 · .NET 8 → .NET 10 y Node 20 → Node 26

**Fecha:** 2026-11-03.

**Tipo:** versión.

**Commit:** `c0791da`, `edc0544`.

**Qué cambió y por qué:** el backend pasa de `net8.0` a `net10.0` (SDK
10.0.403, runtime 10.0.14) con todos los paquetes de Microsoft y Npgsql en la
línea 10: .NET 8 termina su soporte el 2026-11-10 y 10 es LTS hasta
2028-11-14 (`releases-index.json` de Microsoft, consultado el 2026-11-03).
Como las migraciones se regeneraban por el cambio de motor (CAM-03), el salto
no agregó trabajo de datos. Swashbuckle pasa a 10.3.1 (API de OpenAPI 2 en
`Program.cs`). El frontend se compila con Node 26.13.0, la LTS más nueva
(`nodejs.org/dist/index.json`), y pnpm 10.34.9; las versiones de React, Vite
y el resto del lockfile no cambiaron. Verificado: `dotnet build` sin
advertencias, `pnpm lint` y `pnpm build` sin errores, ART-01 OK.

**Producción:** no (no hay producción; según el README, el sistema se
ejecutaba en el puerto 5095 con .NET 8, pero dónde corría no está registrado).

**Relacionados:** [ARQ-03](arquitectura/stack-y-entornos.md), CAM-03.

## CAM-03 · Catálogo en PostgreSQL y despliegue en Docker

**Fecha:** 2026-11-03.

**Tipo:** arquitectura.

**Commit:** `03a514d`, `c0791da`, `edc0544`, `8dfe171`.

**Qué cambió y por qué:** pedido del usuario (2026-11-03): base de datos,
backend y frontend en contenedores separados, consolidando en PostgreSQL y
con una base nueva y vacía. El catálogo y la identidad salen de SQL Server:
se borraron las seis migraciones de SQL Server y se generó
`InitialPostgres`. Un solo servidor PostGIS 18-3.6 con dos bases y un rol de
aplicación sin privilegios de superusuario. Se agregan `compose.yaml`
(desarrollo), `compose.prod.yaml` y `Caddyfile` (producción en un VPS), las
imágenes de `docker/`, `--migrate` como paso explícito y `/health`. Se
corrigen de paso PEN-01, PEN-02, PEN-03 y PEN-10. **No hubo migración de
datos**: los datos de la base SQL Server anterior, si existen, no se
trasladaron.

**Producción:** no (probado en el equipo de desarrollo con `DOMINIO=localhost`
el 2026-11-03; ningún servidor todavía).

**Relacionados:** [PLAN-2026-11-03](superpowers/plans/2026-11-03-docker-postgresql.md),
[DEP-01](despliegue/vps-docker.md), [DIA-08](diagramas/entornos.md),
[REV-01](revisiones/2026-11-03-sistema-completo.md), PS-01, PS-02.
