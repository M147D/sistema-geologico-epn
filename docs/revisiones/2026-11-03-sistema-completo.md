# REV-01 · Revisión de diseño (sistema completo)

**Commit revisado:** `d30f18991b24d86066813b2a3187fdfeb0719b53`

**Fecha:** 2026-11-03

**Alcance:** backend ASP.NET Core (`Servidor_Sistema_Geologia/`: controladores,
servicios, repositorios, modelos, los dos DbContext y las seis migraciones),
frontend React (`Cliente_Sistema_Geologia/cliente-sistema-geologico/`: rutas,
contextos, hooks de servicio, cliente HTTP) y la configuración versionada
(`.gitignore`, `.csproj`, `vite.config.js`, `.env.example`).
**No se revisó:** el contenido de las bases reales (no hay acceso; el esquema
de PostGIS no está en el repo, ver H-04), los componentes de UI uno por uno,
el archivo QML de colores más allá de cómo se carga, ni la seguridad (queda
para la skill `security-audit`). Es la primera revisión del proyecto, hecha
al traerlo a `~/proyectos/SistemaGeologico` para dockerizarlo.

## Reconocimiento

- **Lenguajes y stack:** C# sobre .NET 8 (`Servidor_Sistema_Geologia.csproj`,
  `net8.0`), API REST con controladores, EF Core 8 e Identity; JavaScript con
  React 18 y Vite 6 (versiones del `pnpm-lock.yaml`), MUI 6, Leaflet.
- **Puntos de entrada:** `Servidor_Sistema_Geologia/Program.cs` (API en el
  puerto 5095 según `Properties/launchSettings.json`) y
  `Cliente_Sistema_Geologia/cliente-sistema-geologico/src/main.jsx` (SPA en el
  5174, con proxy de `/api` al 5095 en `vite.config.js`).
- **Unidades desplegables:** dos, el backend y el frontend; no hay
  contenedores ni scripts de despliegue en el repo.
- **Tipo de datos:** relacional, en **dos motores**:
  - SQL Server (`SistemaGeologicoDbContext`): Identity (tablas `Usuarios`,
    `Roles`, `UsuarioRoles`, …) y el catálogo (`tb_ElementosGeologicos` con
    herencia TPH, `tb_Paises`, `tb_Provincias`, `tb_Ubicaciones`,
    `tb_GaleriaElementosGeologicos`, `tb_FotosElementos`, con las fotos como
    `varbinary`). El esquema efectivo es el `OnModelCreating` más las seis
    migraciones de `Servidor_Sistema_Geologia/Migrations/` (la última,
    `20260224125734_RenombrarTablasColumnasCamelCase`), que coinciden con
    `SistemaGeologicoDbContextModelSnapshot.cs`.
  - PostgreSQL + PostGIS (`PostGISDbContext`): solo lectura de `tb_geologia`,
    `tb_provincias` y `tb_ecuador`. Ninguna migración ni script del repo crea
    esas tablas ni las carga.
- **Configuración:** `appsettings.json` y `package.json` no están en el repo
  (H-02); los nombres de las claves se leyeron del código (`Program.cs`,
  `JwtService.cs`, `EmailService.cs`, `QmlColorService.cs`) y las dependencias
  del frontend, de la sección `importers` del `pnpm-lock.yaml`.

## Arquitectura y patrones

| ID | Tipo | Nombre | Grado | Evidencia | Notas |
|---|---|---|---|---|---|
| P-01 | arquitectura | SPA + API | aplicado | `Servidor_Sistema_Geologia/Controllers/ElementosGeologicosController.cs:42-43`, `Cliente_Sistema_Geologia/cliente-sistema-geologico/src/components/auth/ProtectedRoute.jsx:5-24` | El backend exige rol en cada endpoint; las guardias de ruta del cliente solo ordenan la navegación. |
| P-02 | arquitectura | En capas (Controller → Service → Repository) | parcial | `Servidor_Sistema_Geologia/Program.cs:63-95`, `Servidor_Sistema_Geologia/Services/Implementation/BaseElementoGeologicoService.cs:127-128`, `Servidor_Sistema_Geologia/Controllers/FotoElementosController.cs:357-485` | Se registra y usa en casi todo, pero los servicios de elementos escriben directo en el DbContext (galería, provincia, ubicación) y el controlador de fotos hace el procesamiento de imagen. |
| P-03 | patrón | Repository genérico | aplicado | `Servidor_Sistema_Geologia/Repositories/Implementation/BaseElementoGeologicoRepository.cs:37-50` | Base genérica con `OfType<T>()` para fósil, mineral y roca. |
| P-04 | patrón | Template Method | aplicado | `Servidor_Sistema_Geologia/Services/Implementation/BaseElementoGeologicoService.cs:97-117`, `Servidor_Sistema_Geologia/Services/Implementation/BaseElementoGeologicoService.cs:301` | `CreateAsync` fija el flujo y cada subtipo implementa `ValidateCreateAsync`. |
| P-05 | patrón | Herencia TPH (tabla única con discriminador) | aplicado | `Servidor_Sistema_Geologia/DAL/SistemaGeologicoDbContext.cs:72-76` | Discriminador `tipoElemento`; columnas de subtipo en la misma tabla. |
| P-06 | patrón | Auditoría y borrado lógico | aplicado | `Servidor_Sistema_Geologia/Models/Auditoria/EntidadAuditable.cs:1-45`, `Servidor_Sistema_Geologia/Repositories/Implementation/ElementoGeologicoRepository.cs:49-52` | Fechas y usuario de creación/actualización/eliminación; `EstadoActivo` filtra por defecto. |
| P-07 | patrón | DTO | aplicado | `Servidor_Sistema_Geologia/DTO/ElementosGeologicos/ElementoGeologicoListDto.cs:1` | Carpetas canónicas por área; quedan carpetas heredadas (`DTO/Galeria/`, `DTO/Ubicaciones/`, `DTO/Usuarios/`) con un archivo cada una. |
| P-08 | patrón | Inyección de dependencias | aplicado | `Servidor_Sistema_Geologia/Program.cs:63-101` | Repositorios y servicios `Scoped`; QML y correo `Singleton`. |
| P-09 | patrón | Cache-aside de imágenes | aplicado | `Servidor_Sistema_Geologia/Controllers/FotoElementosController.cs:61-81` | Clave por foto, rol y tamaño; se invalida al editar o borrar. |
| P-10 | patrón | Store con Context + hooks de servicio, Container/Presentational | aplicado | `Cliente_Sistema_Geologia/cliente-sistema-geologico/src/App.jsx:62-66`, `Cliente_Sistema_Geologia/cliente-sistema-geologico/src/lib/apiClient.js:13-16` | Cada hook HTTP es privado de su contexto; las páginas consumen el contexto y los componentes reciben props. Cliente Axios como singleton de módulo. |

## Hallazgos

| ID | Severidad | Dimensión | Criterio | Evidencia | Problema | Corrección | Seguimiento |
|---|---|---|---|---|---|---|---|
| H-01 | alta | Dependencias | Acoplamiento a un proveedor (plataforma) | `Servidor_Sistema_Geologia/Controllers/FotoElementosController.cs:71-75`, `Servidor_Sistema_Geologia/Controllers/FotoElementosController.cs:392-422` | La miniatura y la marca de agua usan System.Drawing (GDI+) y solo se aplican si el sistema es Windows. En Linux (un contenedor) el usuario Free recibe la foto original sin marca, y la miniatura es la imagen completa: la regla "marca de agua por rol" deja de cumplirse sin error. | Procesar la imagen con una biblioteca multiplataforma y quitar la condición de sistema operativo. | PEN-01 |
| H-02 | alta | Consistencia | Contrato divergente (repo vs. lo que el código necesita) | `.gitignore:372-379` | Las reglas `*.json`, `*.md`, `*.sh` y `docs/` dejan fuera del repo `package.json` del frontend y `appsettings.json` del backend (y cualquier documentación). Un clon no se puede construir: `pnpm install` no tiene manifiesto y el backend arranca sin cadenas de conexión. | Reglas explícitas: ignorar solo `appsettings.*.json` con secretos y `.env`; versionar `package.json` y plantillas sin secretos. | PEN-02 |
| H-03 | alta | Datos | Integridad | `Servidor_Sistema_Geologia/Models/Locations/Ubicacion.cs:8-9`, `Servidor_Sistema_Geologia/Services/Implementation/BaseElementoGeologicoService.cs:405-406` | Una ubicación sin país o provincia apunta por defecto a los ids 1 ("Desconocido"), pero ninguna migración ni semilla crea esas filas. En una base nueva, crear un elemento sin país viola la FK. | Sembrar las filas de referencia en una migración o al arrancar, y dejar de depender del id literal. | PEN-03 |
| H-04 | alta | Consistencia | Contrato divergente (esquema no reproducible) | `Servidor_Sistema_Geologia/DAL/PostGISDbContext.cs:51`, `Servidor_Sistema_Geologia/DAL/PostGISDbContext.cs:82`, `Servidor_Sistema_Geologia/DAL/PostGISDbContext.cs:107` | El código lee tres tablas de PostGIS que nada en el repo crea ni carga. Sin ellas el mapa geológico no funciona en un entorno nuevo. | Versionar o documentar la fuente de las capas (volcado o shapefiles) y un paso de carga repetible. | PEN-04 |
| H-05 | media | Datos | 3FN | `Servidor_Sistema_Geologia/Models/Locations/Ubicacion.cs:8-9`, `Servidor_Sistema_Geologia/DAL/SistemaGeologicoDbContext.cs:101-119` | `Ubicacion` guarda `PaisId` y `ProvinciaId`, y la provincia ya tiene su `PaisId`: el país depende de la provincia. Nada impide que los dos países difieran. | Dejar el país solo en la provincia (con una provincia "Desconocida" por país), o validar que coincidan. | PEN-05 |
| H-06 | media | Datos | Integridad (tipo de columna) | `Servidor_Sistema_Geologia/Models/Locations/Ubicacion.cs:13`, `Servidor_Sistema_Geologia/Models/Locations/Ubicacion.cs:17`, `Cliente_Sistema_Geologia/cliente-sistema-geologico/src/components/mapa/MarkerElement.jsx:36-38` | Latitud y longitud se guardan como texto; el rango no se valida y cada cliente hace `parseFloat`. El valor por defecto `"0.0"` pone el punto en el golfo de Guinea. | Columnas numéricas con rango (−90..90, −180..180) y nulo cuando no se conoce. | PEN-05 |
| H-07 | media | Datos | Copias | `Servidor_Sistema_Geologia/Services/Implementation/JwtService.cs:33`, `Servidor_Sistema_Geologia/Services/Implementation/UserService.cs:182`, `Servidor_Sistema_Geologia/Program.cs:271-276`, `Servidor_Sistema_Geologia/Program.cs:307` | El rol vive en dos sitios: la columna `Usuarios.Rol` (lo que va en el JWT) y las tablas de roles de Identity. Al editar un usuario solo se cambia la columna; las tablas de Identity quedan desactualizadas. | Una sola fuente: la columna, o los roles de Identity, y eliminar la otra. | PEN-06 |
| H-08 | media | Consistencia | Transacción incompleta | `Servidor_Sistema_Geologia/Services/Implementation/BaseElementoGeologicoService.cs:117-134`, `Servidor_Sistema_Geologia/Services/Implementation/BaseElementoGeologicoService.cs:411-412` | Crear un elemento guarda la ubicación, el elemento y la galería en `SaveChanges` separados; si falla la galería se registra un aviso y el elemento queda sin galería. | Una sola transacción (o un solo `SaveChanges` con el grafo completo). | PEN-07 |
| H-09 | media | Consistencia | Contrato divergente | `Servidor_Sistema_Geologia/Controllers/FotoElementosController.cs:305-307`, `Servidor_Sistema_Geologia/Repositories/Implementation/FotoElementoRepository.cs:99-104` | El endpoint `por-elemento` dice devolver metadatos "sin bytes de imagen", pero la consulta incluye `Fotos` completas y trae cada `varbinary` de la base. | Proyectar solo los metadatos en la consulta. | PEN-08 |
| H-10 | media | Dependencias | Acoplamiento a un proveedor (motor de base) | `Servidor_Sistema_Geologia/DAL/SistemaGeologicoDbContext.cs:263`, `Servidor_Sistema_Geologia/Migrations/20251124111644_RestaurarRelacionUbicacionPais.cs:36-46` | El catálogo exige SQL Server: `GETDATE()` en el modelo y T-SQL en migraciones. El sistema necesita dos motores (SQL Server y PostgreSQL), y dos servidores de base en cada entorno. | Decidir si se mantiene SQL Server o se consolida en PostgreSQL; esto último exige regenerar las migraciones y migrar datos. | PEN-09 |
| H-11 | media | Responsabilidades | Regla duplicada | `Servidor_Sistema_Geologia/Program.cs:114`, `Servidor_Sistema_Geologia/Services/Implementation/JwtService.cs:116-131` | La clave JWT tiene un valor por defecto al validar (`Program.cs`) pero `JwtService` exige la configuración para emitir. Sin configuración la API arranca, valida con la clave de ejemplo y falla al iniciar sesión. | Una sola lectura de la clave, obligatoria al arrancar. | PEN-10 |
| H-12 | baja | Responsabilidades | Regla duplicada | `Servidor_Sistema_Geologia/Program.cs:260-341`, `Servidor_Sistema_Geologia/Data/SeedData.cs:5-59` | La creación de roles y del superadministrador está en `Program.cs` y en `SeedData`, que nadie llama y no crea el rol `Invitado`. | Borrar `SeedData` o llamarla desde `Program.cs`. | — |
| H-13 | baja | Consistencia | Contrato divergente | `Servidor_Sistema_Geologia/Scripts/InsertSampleLocations.sql:5`, `Servidor_Sistema_Geologia/Servidor_Sistema_Geologia.http:3`, `Servidor_Sistema_Geologia/Models/GIS/GeologiaFeature.cs:11` | Restos que no coinciden con el código: el script usa las tablas `Paises` y la columna `CodigoPais`, que ya no existen; el `.http` llama a `weatherforecast`; el atributo `[Table("geologia")]` contradice el `ToTable("tb_geologia")` que manda. | Borrar o actualizar. | — |
| H-14 | baja | Consistencia | Transacción incompleta (carga masiva) | `Cliente_Sistema_Geologia/cliente-sistema-geologico/src/context/ExcelContext.jsx:126-142` | La carga desde Excel crea los elementos de a uno desde el cliente; si falla a la mitad, queda una carga parcial sin forma de deshacerla. | Endpoint de carga por lotes, o reporte de las filas creadas para reintentar. | — |

## Descartados

- **`Usuario.Rol` como entero (`HasConversion<int>`):** no viola 1FN; es un
  valor único por usuario. El problema es la copia en las tablas de Identity
  (H-07), no el tipo.
- **Columnas de auditoría repetidas en cada tabla (`EntidadAuditable`):** son
  datos propios de cada fila, no una copia de otra tabla.
- **Fotos en la base (`varbinary`) en vez de en disco:** es una decisión de
  diseño con un costo (tamaño de la base y de las copias de seguridad), no
  un error; en Docker simplifica, porque no hace falta un volumen de
  archivos. Lo que sí es un hallazgo es traer los bytes sin necesidad (H-09).
- **El filtrado de elementos en el cliente (`aplicarFiltros`):** es una
  decisión deliberada, documentada en el `CLAUDE.md` que tenía el repo (borrado
  en `294fd37`); con el tamaño del catálogo actual no hay evidencia de que sea
  un problema.
- **Dos DbContext:** separar el catálogo y la parte espacial es coherente con
  usar dos motores; lo discutible es tener dos motores (H-10), no los dos
  contextos.
