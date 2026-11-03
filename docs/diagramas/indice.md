# DIA-00 · Índice de diagramas

Todos los modelos salen del código, no de la intención: ARQ explica y DIA
dibuja. Arquitectura: [ARQ-01](../arquitectura/diagrama.md); stack:
[ARQ-03](../arquitectura/stack-y-entornos.md); reconstruir:
[ARQ-02](../arquitectura/reconstruir-desde-cero.md). La especificación
funcional todavía no existe.

| Código | Documento | Modelo | Tipo | Sale de | Revisado |
|---|---|---|---|---|---|
| DIA-01 | [vista-general.md](vista-general.md) | Vista general | `flowchart` | `Program.cs`, `Caddyfile`, `compose.prod.yaml`, `apiClient.js`, `EmailService.cs`, `ImageProcessingService.cs` | 2026-11-03, `8dfe171` |
| DIA-03 | [modelo-datos.md](modelo-datos.md) | Modelo de datos (dos bases PostgreSQL) | `erDiagram` | `SistemaGeologicoDbContext.cs`, `PostGISDbContext.cs`, `Models/`, `Migrations/`, `docker/db/init/` | 2026-11-03, `e3d2880` |
| DIA-08 | [entornos.md](entornos.md) | Entornos y despliegue | `flowchart` | `compose.yaml`, `compose.prod.yaml`, `Caddyfile`, `docker/` | 2026-11-03, `8dfe171` |

Todavía sin dibujar, aunque el código tiene la pieza: casos de uso, clases del
backend, secuencias (alta de elemento, login), estados (borrado lógico y
bloqueo de usuario) y frontend.
