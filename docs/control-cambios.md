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
