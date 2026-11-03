# Pendientes

Mejoras decididas o detectadas que **todavía no están en el código**. Cada una
se diseña antes de implementarse. Un pendiente hecho **no se borra**: cambia su
**Estado** (con la fecha) y enlaza el trabajo que lo cerró. Para agregar uno:
siguiente número libre (`grep -h "^## PEN-" docs/pendientes.md`), nunca
reutilizar, y una línea en el índice de `CLAUDE.md`.

## PEN-01 · Miniaturas y marca de agua que funcionen fuera de Windows

**Estado:** hecho (2026-11-03): `ImageProcessingService` con SkiaSharp, falla cerrado; commit `c0791da`. Ver PS-01.

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

**Estado:** hecho (2026-11-03): `package.json` reconstruido del lockfile, `appsettings.json` sin secretos y `.gitignore` explícito; commit `03a514d`.

**Situación actual:** `.gitignore` ignoraba `*.json`, `*.md`, `*.sh` y
`docs/`. `*.md` y `docs/` se quitaron el 2026-11-03 para poder versionar
esta documentación; siguen ignorados `*.json` (con eso, `package.json` y
`appsettings.json`) y `*.sh` (REV-01, H-02).

**Problema que resolvería:** un clon no se puede construir (sin manifiesto del
frontend ni configuración del backend).

**Propuesta inicial:** reconstruir `package.json` desde la sección `importers`
del `pnpm-lock.yaml`; versionar `appsettings.json` sin secretos (los secretos
entran por variables de entorno); reglas de `.gitignore` explícitas.

**Restricciones:** ningún secreto real en git. Al cerrarlo: ARQ-02 y ARQ-03.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-03 · Sembrar los datos de referencia que el código da por existentes

**Estado:** hecho (2026-11-03): `SeedData.EnsureUbicacionDesconocidaAsync` crea y resuelve por nombre el país y la provincia "Desconocido"; commit `c0791da`. Verificado en una base nueva con ART-01.

**Situación actual:** una ubicación sin país o provincia toma los ids 1
("Desconocido"), que nada crea (REV-01, H-03).

**Problema que resolvería:** en una base nueva, crear un elemento sin país
falla por la clave foránea.

**Propuesta inicial:** una semilla idempotente al arrancar (o una migración)
que cree el país y la provincia "Desconocido", y que el servicio busque esas
filas por nombre en lugar de usar el id literal.

**Restricciones:** no debe duplicar filas en una base que ya las tenga. Al
cerrarlo: DIA-03 y ARQ-02.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md),
PEN-05.

## PEN-04 · Capas GIS reproducibles

**Estado:** en curso (2026-11-03): el esquema de las tres tablas se crea vacío al iniciar PostGIS (`docker/db/init/01-bases-y-capas-gis.sh`, commit `edc0544`); faltan los datos del IIGE y su paso de carga (el usuario no los tiene a mano, 2026-11-03).

**Situación actual:** `tb_geologia`, `tb_provincias` y `tb_ecuador` se leen
desde PostGIS, pero no hay en el repo ni el esquema ni los datos ni el
procedimiento de carga (REV-01, H-04). Según el README, salen del mapa
geológico del Ecuador (IIGE, 2017) importado con QGIS.

**Problema que resolvería:** el mapa geológico no funciona en un entorno nuevo.

**Propuesta inicial:** un volcado (`pg_dump`) o los shapefiles originales, y un
script de carga que se ejecute al crear el contenedor de PostGIS.

**Restricciones:** el volcado puede pesar mucho; revisar la licencia de los
datos del IIGE antes de publicarlo en un repo público. Al cerrarlo: DIA-03, ARQ-02 y DEP-01.

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

**Estado:** hecho (2026-11-03): por decisión del usuario, todo en PostgreSQL; migraciones regeneradas sin migrar datos (base nueva). Ver CAM-03.

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

**Estado:** hecho (2026-11-03): cadenas de conexión y clave JWT obligatorias al arrancar, CORS desde `Cors:AllowedOrigins`, superadmin solo con contraseña configurada y sin escribirla en el log; commit `c0791da`.

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
código. Al cerrarlo: ARQ-01 (variables de entorno) y ARQ-02.

**Relacionados:** [REV-01](revisiones/2026-11-03-sistema-completo.md).

## PEN-11 · Respaldos programados fuera del servidor

**Estado:** pendiente (2026-11-03).

**Situación actual:** DEP-01 tiene los comandos de respaldo y restauración,
probados el 2026-11-03, pero nada los ejecuta solo ni define adónde van.

**Problema que resolvería:** las fotos del museo viven en la base; perder el
volumen sin respaldo es perder la colección digital.

**Propuesta inicial:** un cron diario en el servidor que ejecute los dos
`pg_dump` de DEP-01, conserve N copias y las copie a otro sitio (destino a
decidir por el museo).

**Restricciones:** el destino y la retención los decide el responsable del
servidor. Al cerrarlo: DEP-01.

**Relacionados:** [DEP-01](despliegue/vps-docker.md).

## PEN-12 · Corregir los hallazgos de la auditoría de seguridad del 2026-11-03

**Estado:** pendiente (2026-11-03).

**Situación actual:** auditoría con la skill `security-audit`, perfil `quick`
(pasada parcial), sobre el commit `6c91245`. El informe vive fuera del repo,
en `~/security-audit-skill/sistema-geologico-epn/run-1/` (`REPORT.md`,
`findings.json`). Seis hallazgos confirmados en un sandbox aislado, y uno por
validar:

- **Medio:** un JWT sigue dando acceso con su rol hasta 12 h después de
  desactivar, degradar o cambiar la contraseña de la cuenta
  (`Program.cs`: sin `OnTokenValidated`; logout sin efecto).
- **Medio:** las regex de `NoInjectionAttribute` tienen costo cuadrático y se
  aplican a campos sin longitud máxima en `register` y `login` anónimos
  (64 KB → ~10 s de CPU por petición).
- **Bajo:** cualquiera puede bloquear cualquier cuenta, superadmin incluido,
  con 3 logins fallidos (bloqueo por cuenta sin límite por cliente).
- **Bajo:** las fotos borradas lógicamente, o las de elementos borrados, siguen
  saliendo por `imagen/{id}` y `galeria/{id}` (un Premium recibe el original).
- **Bajo:** `solicitar-informe` permite enviar correos sin límite, con el
  correo del solicitante tomado del cuerpo y no del token.
- **Bajo:** `solicitar-informe` resuelve elementos borrados (oráculo de
  existencia y correo sobre registros borrados).
- **Por validar:** la decodificación de imágenes no tiene límite de píxeles
  (un PNG de ~1 KB de 3000×3000 sube ~66 MB la memoria por cada vista sin
  caché); el efecto depende de la memoria del contenedor en producción.

Corrida 2 (2026-11-03): auditoría acotada, perfil `standard`, sobre las 4
unidades que la corrida 1 dejó diferidas. Informe en
`~/security-audit-skill/sistema-geologico-epn/run-2/`. La cobertura de ese
alcance quedó completa (un crítico de cierre independiente lo confirmó).
Agrega tres hallazgos bajos, confirmados y verificados dos veces:

- **Bajo:** una edición concurrente de un elemento (PUT de un Invitado)
  deshace en silencio el borrado lógico del Admin y borra los datos de quién
  y cuándo lo borró. Causa: `BaseElementoGeologicoRepository.UpdateAsync`
  hace `DbSet.Update` de la entidad completa y no hay token de concurrencia.
- **Bajo:** lo mismo con las fotos, en `FotoElementoRepository.UpdateAsync`.
- **Bajo:** el detalle de un elemento les lista a los usuarios Free las fotos
  borradas (descripción y URL). Causa: el `Include` de `Galeria.Fotos` no
  filtra por estado.

Sin hallazgo nuevo, pero con datos útiles para corregir:

- La regex de `NoInjection` sobre los DTO autenticados tiene la misma causa
  raíz que en la corrida 1, y agregar `StringLength` no la corrige: el arreglo
  va dentro del atributo.
- `xlsx` 0.18.5 sí contamina `Object.prototype.c`, pero en la app no hay
  código que aproveche eso; conviene actualizarlo a SheetJS ≥ 0.20.2.

**Problema que resolvería:** las fronteras de sesión, de costo, de
concurrencia y de borrado lógico que documentan los dos informes.

**Propuesta inicial:** la corrección mínima de cada hallazgo está en
`REPORT.md` y `FINDINGS-DETAIL.md`. Después, una corrida `standard` que
empiece por las 4 unidades diferidas.

**Restricciones:** cada corrección necesita su caso de regresión (el informe
propone uno por hallazgo). Al cerrarlo: ARQ-01 (estado de seguridad) y un PS
por cada hallazgo corregido.

**Relacionados:** [ARQ-01](arquitectura/diagrama.md), PEN-08.
