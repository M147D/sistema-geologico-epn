# CLAUDE.md — Sistema Geológico

Catálogo y mapa georreferenciado de fósiles, minerales y rocas del Museo
Petrográfico "Tomás Feininger" (EPN). Backend ASP.NET Core 8 en
`Servidor_Sistema_Geologia/` (API en el puerto 5095), frontend React + Vite en
`Cliente_Sistema_Geologia/cliente-sistema-geologico/` (5174), SQL Server para
el catálogo e identidad y PostgreSQL + PostGIS para las capas geológicas.
Detalle en ARQ-01.

- Frontend: **pnpm únicamente**, nunca npm.
- `dotnet` se ejecuta desde `Servidor_Sistema_Geologia/`.
- El repo hoy no se construye tal cual: faltan `package.json` y
  `appsettings.json` (PEN-02) y las capas GIS (PEN-04).

## Documentation Index

All project documentation lives under `docs/`, and every document carries a stable **code** in its title (`# ARQ-NN · …`; lessons and pending items are `## LA-NN · …` / `## PEN-NN · …` sections of one file each). Cross-references cite code + relative link, so a broken link can be recovered by grepping the code. Codes are never reused. `node scripts/check-docs.mjs` verifies links and codes.

**Start here:** architecture → ARQ-01; every diagram → DIA-00; stack, why each piece, and environments → ARQ-03; design review → REV-01.

| Code | Document | What it covers |
|---|---|---|
| ARQ-01 | [docs/arquitectura/diagrama.md](docs/arquitectura/diagrama.md) | Current architecture: directory trees, full API contract, components, security state, environment variables; links to the diagrams. |
| ARQ-03 | [docs/arquitectura/stack-y-entornos.md](docs/arquitectura/stack-y-entornos.md) | Stack by layer with versions from the lockfile and csproj; development setup and its real warnings. |
| DIA-00 | [docs/diagramas/indice.md](docs/diagramas/indice.md) | Diagram index, and which models are still missing. |
| DIA-01 | [docs/diagramas/vista-general.md](docs/diagramas/vista-general.md) | Overview: SPA, API, both databases, SMTP and map tiles. |
| DIA-03 | [docs/diagramas/modelo-datos.md](docs/diagramas/modelo-datos.md) | Data model (SQL Server and PostGIS): erDiagram, dictionary, persistence strategy. |
| PEN-01 … | [docs/pendientes.md](docs/pendientes.md) | Pending improvements, listed below. |
| REV-01 | [docs/revisiones/2026-11-03-sistema-completo.md](docs/revisiones/2026-11-03-sistema-completo.md) | Design review at `d30f189`: 10 patterns, 14 findings (4 high, 7 medium, 3 low). |
| CAM-01 … | [docs/control-cambios.md](docs/control-cambios.md) | Change control: deployments, version changes, migrations, architecture changes and reviews, with their commit and whether they reached production. |

Pending improvements (`docs/pendientes.md`; an item is never deleted when done — its status changes and it links the work that closed it):

- PEN-01 — Thumbnails and watermark that work outside Windows (today Free users get unwatermarked photos on Linux). Pending.
- PEN-02 — Version the files the repo needs to build (`package.json`, `appsettings.json`); `.gitignore` still ignores `*.json`. Pending.
- PEN-03 — Seed the "Desconocido" country/province rows the code assumes as id 1. Pending.
- PEN-04 — Reproducible GIS layers (schema, data and load step for PostGIS). Pending.
- PEN-05 — `Ubicacion` in 3NF and with numeric coordinates. Pending.
- PEN-06 — Single source for the user role (column vs. Identity roles). Pending.
- PEN-07 — Create an element in one transaction. Pending.
- PEN-08 — List photos without reading their bytes. Pending.
- PEN-09 — Decide the catalog database engine (SQL Server vs. consolidating in PostgreSQL). Pending.
- PEN-10 — Mandatory, explicit configuration at startup (JWT key, CORS origins, superadmin password). Pending.

Changes (`docs/control-cambios.md`):

- CAM-01 — First design review and documentation; production: not applicable.

**Adding a document:** take the next free code in its category, put it in the title (`# CODE · Title`, `## LA-NN · Title`, `## PEN-NN · Title`, `## ART-NN · Title`, `# REV-NN · Title`, `## CAM-NN · Title`), add it to this index, and run `node scripts/check-docs.mjs`.
