# Pendientes

Mejoras decididas o detectadas que **todavía no están en el código**. Cada una
se diseña antes de implementarse. Un pendiente hecho **no se borra**: cambia su
**Estado** (con la fecha) y enlaza el trabajo que lo cerró. Para agregar uno:
siguiente número libre (`grep -h "^## PEN-" docs/pendientes.md`), nunca
reutilizar, y una línea en el índice de `CLAUDE.md`.

## PEN-01 · Miniaturas y marca de agua que funcionen fuera de Windows

**Estado:** pendiente (2026-11-03).

**Situación actual:** `FotoElementosController` usa System.Drawing y aplica la
miniatura y la marca de agua solo si `OperatingSystem.IsWindows()` (REV-01,
H-01).

**Problema que resolvería:** en un contenedor Linux los usuarios Free reciben
las fotos sin marca de agua y las miniaturas pesan lo mismo que la foto
completa, sin ningún error visible.

**Propuesta inicial:** sacar el procesamiento a un servicio propio
(`IImageProcessingService`) con una biblioteca multiplataforma, quitar la
condición de sistema operativo e incluir en la imagen de Docker la fuente que
usa la marca de agua.

**Restricciones:** el texto, la opacidad y el patrón de la marca deben seguir
como están. Al cerrarlo: ARQ-01 (componentes), ARQ-03 (stack) y el diagrama de clases (aún no
existe).

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-02 · Versionar los archivos que el repo necesita para construirse

**Estado:** pendiente (2026-11-03).

**Situación actual:** `.gitignore` ignoraba `*.json`, `*.md`, `*.sh` y
`docs/`. `*.md` y `docs/` se quitaron el 2026-11-03 para poder versionar
esta documentación; siguen ignorados `*.json` (con eso, `package.json` y
`appsettings.json`) y `*.sh` (REV-01, H-02).

**Problema que resolvería:** un clon no se puede construir (sin manifiesto del
frontend ni configuración del backend).

**Propuesta inicial:** reconstruir `package.json` desde la sección `importers`
del `pnpm-lock.yaml`; versionar `appsettings.json` sin secretos (los secretos
entran por variables de entorno); reglas de `.gitignore` explícitas.

**Restricciones:** ningún secreto real en git. Al cerrarlo: el runbook de reconstrucción y ARQ-03.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-03 · Sembrar los datos de referencia que el código da por existentes

**Estado:** pendiente (2026-11-03).

**Situación actual:** una ubicación sin país o provincia toma los ids 1
("Desconocido"), que nada crea (REV-01, H-03).

**Problema que resolvería:** en una base nueva, crear un elemento sin país
falla por la clave foránea.

**Propuesta inicial:** una semilla idempotente al arrancar (o una migración)
que cree el país y la provincia "Desconocido", y que el servicio busque esas
filas por nombre en lugar de usar el id literal.

**Restricciones:** no debe duplicar filas en una base que ya las tenga. Al
cerrarlo: DIA-03 y el runbook de reconstrucción.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md),
PEN-05.

## PEN-04 · Capas GIS reproducibles

**Estado:** pendiente (2026-11-03).

**Situación actual:** `tb_geologia`, `tb_provincias` y `tb_ecuador` se leen
desde PostGIS, pero no hay en el repo ni el esquema ni los datos ni el
procedimiento de carga (REV-01, H-04). Según el README, salen del mapa
geológico del Ecuador (IIGE, 2017) importado con QGIS.

**Problema que resolvería:** el mapa geológico no funciona en un entorno nuevo.

**Propuesta inicial:** un volcado (`pg_dump`) o los shapefiles originales, y un
script de carga que se ejecute al crear el contenedor de PostGIS.

**Restricciones:** el volcado puede pesar mucho; revisar la licencia de los
datos del IIGE antes de publicarlo en un repo público. Al cerrarlo: DIA-03, el runbook de
reconstrucción y el documento de despliegue.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-05 · Ubicación en tercera forma normal y con coordenadas numéricas

**Estado:** pendiente (2026-11-03).

**Situación actual:** `Ubicacion` repite el país de su provincia y guarda
latitud y longitud como texto (REV-01, H-05 y H-06).

**Problema que resolvería:** países contradictorios entre la ubicación y su
provincia; coordenadas fuera de rango o mal escritas.

**Propuesta inicial:** columnas decimales con validación de rango; el país
derivado de la provincia (o una restricción que los haga coincidir).

**Restricciones:** cambia el esquema y migra datos reales: se hace como
trabajo aparte, con copia de seguridad. Cambia el contrato de la API y el
cliente (`parseFloat`). Al cerrarlo: DIA-03 y ARQ-01.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md),
PEN-03.

## PEN-06 · Una sola fuente para el rol del usuario

**Estado:** pendiente (2026-11-03).

**Situación actual:** el rol está en la columna `Usuarios.Rol` (va en el JWT)
y en las tablas de roles de Identity, que solo se tocan al crear el
superadministrador (REV-01, H-07).

**Problema que resolvería:** dos fuentes que ya divergen al editar usuarios.

**Propuesta inicial:** quedarse con la columna (es la que usa la
autorización) y dejar de escribir en `UsuarioRoles`, o al revés, de forma
consistente.

**Restricciones:** no cambiar los valores del enum (`Admin = 2` está en datos
reales). Al cerrarlo: DIA-03 y ARQ-01.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-07 · Crear un elemento en una sola transacción

**Estado:** pendiente (2026-11-03).

**Situación actual:** ubicación, elemento y galería se guardan por separado
(REV-01, H-08).

**Problema que resolvería:** elementos sin galería o ubicaciones huérfanas
cuando falla un paso.

**Propuesta inicial:** `BeginTransactionAsync` en `CreateAsync`, o armar el
grafo completo y hacer un solo `SaveChangesAsync`.

**Restricciones:** al cerrarlo: el diagrama de secuencias (aún no existe).

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-08 · Listar fotos sin leer sus bytes

**Estado:** pendiente (2026-11-03).

**Situación actual:** `GetGaleriaConFotosAsync` incluye las fotos completas
(REV-01, H-09).

**Problema que resolvería:** cada vista de detalle lee de la base todas las
imágenes del elemento aunque solo necesite los metadatos.

**Propuesta inicial:** proyectar los campos de metadatos en la consulta
(`Select`), sin `Imagen`.

**Restricciones:** al cerrarlo: ARQ-01.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-09 · Decidir el motor de la base del catálogo

**Estado:** pendiente (2026-11-03).

**Situación actual:** el catálogo y la identidad exigen SQL Server; las capas
GIS, PostgreSQL con PostGIS (REV-01, H-10).

**Problema que resolvería:** operar, respaldar y monitorear dos motores en
cada entorno.

**Propuesta inicial:** mantener SQL Server por ahora (no hace falta migrar
datos para dockerizar) y evaluar consolidar en PostgreSQL como trabajo
aparte: regenerar las migraciones y migrar los datos.

**Restricciones:** la consolidación toca todas las migraciones y los datos
reales. Al cerrarlo: ARQ-03, DIA-03 y el diagrama de entornos (aún no existe).

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-10 · Configuración obligatoria y explícita al arrancar

**Estado:** pendiente (2026-11-03).

**Situación actual:** la clave JWT tiene un valor por defecto al validar y es
obligatoria al emitir (REV-01, H-11). Los orígenes CORS están fijos en el
código (`localhost`), y el superadministrador tiene una contraseña por defecto
en el código.

**Problema que resolvería:** un despliegue sin configuración arranca con
valores de ejemplo en lugar de fallar.

**Propuesta inicial:** leer y validar la configuración una vez al arrancar
(clave JWT ≥ 32 caracteres, cadenas de conexión, orígenes CORS desde la
configuración) y fallar si falta algo en producción.

**Restricciones:** en desarrollo puede haber valores locales, pero fuera del
código. Al cerrarlo: ARQ-01 (variables de entorno) y el runbook
de reconstrucción.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).
