# DIA-00 · Índice de diagramas

Todos los modelos salen del código, no de la intención: ARQ explica y DIA
dibuja. Arquitectura: [ARQ-01](../arquitectura/diagrama.md); stack:
[ARQ-03](../arquitectura/stack-y-entornos.md). La especificación funcional y
el runbook de reconstrucción todavía no existen.

| Código | Documento | Modelo | Tipo | Sale de | Revisado |
|---|---|---|---|---|---|
| DIA-01 | [vista-general.md](vista-general.md) | Vista general | `flowchart` | `Program.cs`, `vite.config.js`, `src/lib/apiClient.js`, `EmailService.cs`, `mapConstants.js` | 2026-11-03, `d30f189` |
| DIA-03 | [modelo-datos.md](modelo-datos.md) | Modelo de datos (SQL Server y PostGIS) | `erDiagram` | `SistemaGeologicoDbContext.cs`, `PostGISDbContext.cs`, `Models/`, `Migrations/` | 2026-11-03, `d30f189` |

Todavía sin dibujar, aunque el código tiene la pieza: casos de uso, clases del
backend, secuencias (alta de elemento, login), estados (borrado lógico y
bloqueo de usuario) y frontend. El diagrama de entornos y despliegue se
dibuja con el despliegue en Docker.
