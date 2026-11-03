# CLAUDE.md — Sistema Geológico

Catálogo y mapa georreferenciado de fósiles, minerales y rocas del Museo
Petrográfico "Tomás Feininger" (EPN). Backend ASP.NET Core 10 en
`Servidor_Sistema_Geologia/`, frontend React + Vite en
`Cliente_Sistema_Geologia/cliente-sistema-geologico/`, PostgreSQL 18 +
PostGIS con dos bases (catálogo e identidad; capas GIS). Todo corre en
Docker. Detalle en ARQ-01.

- Levantar: `docker compose up -d --build`; migraciones:
  `docker compose run --rm api dotnet run --no-launch-profile -- --migrate`.
  Frontend en `http://localhost:5174`, API en `http://localhost:5095`. Ver ARQ-02.
- Nada corre en el equipo salvo git y Docker: `dotnet`, `pnpm` y `psql` se
  ejecutan en sus contenedores (`docker compose exec api|web|db …`).
- Frontend: **pnpm únicamente**, nunca npm.
- Secretos solo por variables de entorno (`.env`, plantilla en `.env.example`);
  nunca en `appsettings.json` ni en git.
- Verificación: `pnpm lint`, `pnpm build`, `dotnet build` y la prueba de humo
  ART-01 (no hay tests).

## Documentation Index

All project documentation lives under `docs/`, and every document carries a stable **code** in its title (`# ARQ-NN · …`; lessons and pending items are `## LA-NN · …` / `## PEN-NN · …` sections of one file each). Cross-references cite code + relative link, so a broken link can be recovered by grepping the code. Codes are never reused. `node scripts/check-docs.mjs` verifies links and codes.

**Start here:** rebuild from scratch → ARQ-02; deploy → DEP-01; architecture → ARQ-01; every diagram → DIA-00; stack, why each piece, and environments → ARQ-03; design review → REV-01.

| Code | Document | What it covers |
|---|---|---|
| ARQ-01 | [docs/arquitectura/diagrama.md](docs/arquitectura/diagrama.md) | Current architecture: directory trees, full API contract, components, security state, environment variables; links to the diagrams. |
| ARQ-02 | [docs/arquitectura/reconstruir-desde-cero.md](docs/arquitectura/reconstruir-desde-cero.md) | Runbook: run it on a new machine with Docker, baseline checks, migrations, everyday commands, symptom → where to look. |
| ARQ-03 | [docs/arquitectura/stack-y-entornos.md](docs/arquitectura/stack-y-entornos.md) | Stack by layer with versions and sourced reasons; development vs. production. |
| DIA-00 | [docs/diagramas/indice.md](docs/diagramas/indice.md) | Diagram index, and which models are still missing. |
| DIA-01 | [docs/diagramas/vista-general.md](docs/diagramas/vista-general.md) | Overview: Caddy, SPA, API, both PostgreSQL databases, SMTP and map tiles. |
| DIA-03 | [docs/diagramas/modelo-datos.md](docs/diagramas/modelo-datos.md) | Data model (two PostgreSQL databases): erDiagram, dictionary, persistence strategy. |
| DIA-08 | [docs/diagramas/entornos.md](docs/diagramas/entornos.md) | Development containers, production image build, and a request in production. |
| DEP-01 | [docs/despliegue/vps-docker.md](docs/despliegue/vps-docker.md) | Deploy to a VPS with Docker Compose and Caddy; update; tested backup and restore. Tested locally only. |
| PS-01 | [docs/problemas-soluciones/marca-de-agua-solo-en-windows.md](docs/problemas-soluciones/marca-de-agua-solo-en-windows.md) | Watermark and thumbnails only worked on Windows; moved to SkiaSharp, fail-closed. Fixed. |
| PS-02 | [docs/problemas-soluciones/dotnet-watch-hot-reload.md](docs/problemas-soluciones/dotnet-watch-hot-reload.md) | `dotnet watch` died in the dev container on a Hot Reload error; now `--no-hot-reload` + restart. Fixed. |
| PEN-01 … | [docs/pendientes.md](docs/pendientes.md) | Pending improvements, listed below. |
| REV-01 | [docs/revisiones/2026-11-03-sistema-completo.md](docs/revisiones/2026-11-03-sistema-completo.md) | Design review at `d30f189` (before Docker): 10 patterns, 14 findings (4 high, 7 medium, 3 low). |
| CAM-01 … | [docs/control-cambios.md](docs/control-cambios.md) | Change control: deployments, version changes, migrations, architecture changes and reviews, with their commit and whether they reached production. |
| ART-01 … | [docs/artefactos.md](docs/artefactos.md) | Code kept from executing plans, in `scripts/artefactos/`; listed below. |

Plans: [PLAN-2026-11-03](docs/superpowers/plans/2026-11-03-docker-postgresql.md) — Docker with separate db, backend and frontend, on PostgreSQL.

Pending improvements (`docs/pendientes.md`; an item is never deleted when done — its status changes and it links the work that closed it):

- PEN-01 — Thumbnails and watermark that work outside Windows. Done 2026-11-03.
- PEN-02 — Version the files the repo needs to build (`package.json`, `appsettings.json`). Done 2026-11-03.
- PEN-03 — Seed the "Desconocido" country/province rows the code relies on. Done 2026-11-03.
- PEN-04 — Reproducible GIS layers. In progress: empty schema created at db init; IIGE data and load step missing.
- PEN-05 — `Ubicacion` in 3NF and with numeric coordinates. Pending.
- PEN-06 — Single source for the user role (column vs. Identity roles). Pending.
- PEN-07 — Create an element in one transaction. Pending.
- PEN-08 — List photos without reading their bytes. Pending.
- PEN-09 — Catalog database engine: consolidated in PostgreSQL. Done 2026-11-03.
- PEN-10 — Mandatory, explicit configuration at startup. Done 2026-11-03.
- PEN-11 — Scheduled off-server backups. Pending.
- PEN-12 — Fix the 2026-11-03 security audit findings (2 medium, 4 low confirmed, 1 needs validation; report outside the repo). Pending.

Artifacts (`docs/artefactos.md`, code in `scripts/artefactos/`):

- ART-01 — End-to-end smoke test against a running deployment (auth, roles, element + photo, watermark, GIS); reusable, writes test data.

Changes (`docs/control-cambios.md`):

- CAM-01 — First design review and documentation; production: not applicable.
- CAM-02 — .NET 8 → 10 and Node 20 → 26; production: no.
- CAM-03 — Catalog in PostgreSQL and Docker deployment; production: no (tested locally).

**Adding a document:** take the next free code in its category, put it in the title (`# CODE · Title`, `## LA-NN · Title`, `## PEN-NN · Title`, `## ART-NN · Title`, `# REV-NN · Title`, `## CAM-NN · Title`), add it to this index, and run `node scripts/check-docs.mjs`.
