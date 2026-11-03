# Artefactos

Código que nació al ejecutar un plan o una tarea y que se conserva: cambios
en masa, migraciones, generadores y comprobaciones repetibles. Vive en
`scripts/artefactos/ART-NN-<slug>.<ext>`. Para agregar uno: siguiente número
libre (`grep -h "^## ART-" docs/artefactos.md`), una ficha aquí y una línea en
el índice de `CLAUDE.md`.

## ART-01 · Prueba de humo de extremo a extremo

**Qué hace:** recorre el sistema en marcha a través del mismo origen que usa
el navegador: login del administrador, registro de un Free (comprueba que el
servidor fuerza el rol), 403 al crear como Free, alta de una roca con
ubicación, subida de una foto PNG generada en el momento, marca de agua por
rol (el Admin recibe la original y el Free otra imagen), miniatura más liviana,
las tres capas GIS y 401 sin token. Existe porque el repo no tiene tests y la
dockerización cambió el motor de base y la librería de imágenes.

**Cómo se ejecuta:**

```bash
ADMIN_EMAIL=admin@sistemageologico.local ADMIN_PASSWORD=SoloDesarrollo1 \
  scripts/artefactos/ART-01-prueba-humo.sh http://127.0.0.1:5174      # desarrollo
INSECURE=1 ADMIN_EMAIL=... ADMIN_PASSWORD=... \
  scripts/artefactos/ART-01-prueba-humo.sh https://localhost:8443     # prod local (CA interna de Caddy)
```

**Entrada y salida:** URL base y credenciales del administrador por variables;
imprime una línea por comprobación y termina con código 1 si alguna falla.

**Origen:** [PLAN-2026-11-03](superpowers/plans/2026-11-03-docker-postgresql.md),
tareas 7 y 8; commits `edc0544` (creación) y `8dfe171` (opción `INSECURE`).

**Estado:** reutilizable. Pasó el 2026-11-03 en desarrollo y en producción
local.

**Riesgos:** escribe en la base: crea un usuario Free y una roca con foto, y
los desactiva al final (borrado lógico: quedan las filas). Se puede repetir:
cada corrida usa un código y un correo nuevos. No correrlo en producción salvo
a propósito.

Archivo: [`scripts/artefactos/ART-01-prueba-humo.sh`](../scripts/artefactos/ART-01-prueba-humo.sh).

## ART-02 · Verificación del cierre de REV-01

**Qué hace:** comprueba por la API los cierres de REV-01:

- **H-06:** coordenada vacía guardada como `null`, coma decimal aceptada, texto
  inválido y valor fuera de rango rechazados con 400.
- **H-05:** un alta sin provincia queda en la "Desconocida" del mismo país, y el
  listado muestra el mismo país que el detalle.
- **H-08:** un alta que falla no deja el elemento creado.
- **H-14:** un lote válido crea y actualiza filas; un lote con una fila inválida,
  por regla de negocio o por validación del modelo, no guarda ninguna.

**Cómo se ejecuta:**

```bash
ADMIN_EMAIL=admin@sistemageologico.local ADMIN_PASSWORD=SoloDesarrollo1 \
  scripts/artefactos/ART-02-verificacion-rev01.sh http://127.0.0.1:5174
```

`INSECURE=1` acepta la CA interna de Caddy.

**Entrada y salida:** la URL base y las credenciales del administrador. Imprime
una línea por comprobación y termina con código 1 si alguna falla.

**Origen:** cierre de REV-01 pedido por el usuario el 2026-11-03; commit
`e3d2880`; [REV-02](revisiones/2026-11-03-cierre-rev01.md).

**Estado:** reutilizable. Pasó el 2026-11-03 en desarrollo. Las comprobaciones
que la API no deja ver (filas huérfanas, tablas de roles vacías, SQL sin
`imagen`) se hicieron aparte y están en REV-02.

**Riesgos:** escribe en la base. Crea minerales con códigos únicos por corrida
y los desactiva al final; quedan como filas inactivas y, si son nuevos, el país
y las provincias. Se puede repetir.

Archivo: [`scripts/artefactos/ART-02-verificacion-rev01.sh`](../scripts/artefactos/ART-02-verificacion-rev01.sh).
