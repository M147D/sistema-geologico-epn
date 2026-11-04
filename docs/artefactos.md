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

## ART-03 · Regresión de seguridad (PEN-12)

**Qué hace:** comprueba por la API que siguen corregidos los hallazgos de las
auditorías de seguridad:

- el token deja de valer al degradar al usuario, al cerrar sesión y al cambiar
  la contraseña;
- la regex de 64 KB se rechaza en milisegundos;
- las fotos borradas no son visibles fuera de Admin (imagen, galería y
  detalle);
- se rechazan imágenes de más píxeles que el máximo y archivos que no son
  imágenes;
- el informe sobre un elemento borrado da 404, y el informe tiene límite de 3
  por hora;
- 15 rondas de PUT concurrente con DELETE no deshacen ningún borrado;
- el login tiene límite por IP.

**Cómo se ejecuta:**

```bash
ADMIN_EMAIL=admin@sistemageologico.local ADMIN_PASSWORD=SoloDesarrollo1 \
  python3 -I scripts/artefactos/ART-03-regresion-seguridad.py http://127.0.0.1:5174
```

`INSECURE=1` acepta la CA interna de Caddy.

**Entrada y salida:** la URL base y las credenciales del administrador. Imprime
una línea por comprobación y termina con código 1 si alguna falla. Usa solo la
biblioteca estándar de Python.

**Origen:** PEN-12 y las corridas 1 y 2 de la auditoría (2026-11-03); commit
`43a23aa`.

**Estado:** reutilizable. Pasó el 2026-11-03 en desarrollo. La intercalación
determinista PUT/DELETE se probó aparte con un `LOCK TABLE` (PS-07).

**Riesgos:**

- Escribe en la base: crea usuarios, elementos y fotos de prueba, y los
  desactiva al final.
- La última prueba agota a propósito el límite de login de la IP: después de
  correrla, hay que esperar 1 minuto para volver a iniciar sesión desde esa IP.
- No correrlo en producción salvo a propósito.

Archivo: [`scripts/artefactos/ART-03-regresion-seguridad.py`](../scripts/artefactos/ART-03-regresion-seguridad.py).
