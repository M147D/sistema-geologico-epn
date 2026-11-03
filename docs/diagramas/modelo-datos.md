# DIA-03 · Modelo de datos

**Qué muestra:** las tablas de las dos bases del servidor PostgreSQL:
`sistemageologico_datos` (catálogo e identidad) y `sistemageologico_gis`
(capas), con tipos, claves y cardinalidades.

**Sale de:** `Servidor_Sistema_Geologia/DAL/SistemaGeologicoDbContext.cs`,
`DAL/PostGISDbContext.cs`, `Models/`,
`Migrations/` (`InitialPostgres`, `UbicacionPaisDesdeProvinciaYCoordenadasNumericas`,
`RolSoloEnColumnaUsuarios`) y `docker/db/init/01-bases-y-capas-gis.sh` (commit `e3d2880`). Hasta `d30f189`
el catálogo estaba en SQL Server (REV-01).

**Texto que lo explica:** [ARQ-01](../arquitectura/diagrama.md) y
[REV-01](../revisiones/2026-11-03-sistema-completo.md); índice
[DIA-00](indice.md).

## `sistemageologico_datos` (catálogo e identidad)

```mermaid
erDiagram
  Usuarios ||--o{ UsuarioRoles : "tiene"
  Roles ||--o{ UsuarioRoles : "se asigna en"
  tb_Paises ||--o{ tb_Provincias : "contiene"
  tb_Provincias ||--o{ tb_Ubicaciones : "ubica"
  tb_Ubicaciones |o--o{ tb_ElementosGeologicos : "localiza"
  tb_ElementosGeologicos ||--o| tb_GaleriaElementosGeologicos : "tiene"
  tb_GaleriaElementosGeologicos ||--o{ tb_FotosElementos : "agrupa"
  Usuarios |o--o{ tb_ElementosGeologicos : "audita (crea, actualiza, elimina)"

  Usuarios {
    int Id PK
    varchar(256) Email "UNIQUE IX_Usuarios_Email"
    varchar(256) UserName
    text PasswordHash
    varchar(200) NombreCompleto
    int Rol "0 Free, 1 Premium, 2 Admin, 3 Invitado"
    timestamp FechaCreacion
    boolean EstadoActivo
    int AccessFailedCount
    timestamptz LockoutEnd
  }
  Roles {
    int Id PK
    varchar(256) Name
  }
  UsuarioRoles {
    int UserId PK, FK
    int RoleId PK, FK
  }
  tb_Paises {
    int id PK
    varchar(100) nombrePais
    boolean estadoActivo
  }
  tb_Provincias {
    int id PK
    int paisId FK
    varchar(100) nombreProvincia
    boolean estadoActivo
  }
  tb_Ubicaciones {
    int id PK
    int provinciaId FK "el país sale de la provincia"
    float8 latitud "NULL o -90..90 (CHECK)"
    float8 longitud "NULL o -180..180 (CHECK)"
    varchar(500) localidad
    varchar(60) leyenda
    boolean estadoActivo
  }
  tb_ElementosGeologicos {
    int id PK
    varchar(21) tipoElemento "discriminador: Fosil, Mineral, Roca"
    varchar(200) nombre
    varchar(100) codigo "UNIQUE"
    varchar(50) edad
    varchar(200) donante
    timestamp fechaIngreso
    bigint ejemplares
    varchar(5000) documentosRelacionados
    boolean laminaExiste
    int ubicacionId FK
    int tipoFosil "solo Fosil"
    varchar(200) especie "solo Fosil"
    varchar(200) periodo "solo Fosil"
    int tipoMineral "solo Mineral"
    varchar(500) litologia "solo Mineral"
    int tipoRoca "solo Roca"
    varchar(500) litologia_roca "solo Roca"
    timestamp fechaCreacion "DEFAULT LOCALTIMESTAMP"
    int usuarioCreacionId FK
    int usuarioActualizacionId FK
    int usuarioEliminacionId FK
    boolean estadoActivo "DEFAULT 1"
  }
  tb_GaleriaElementosGeologicos {
    int id PK
    int elementoGeologicoId FK "1:1"
    varchar(200) detalleGrupo
    boolean estadoActivo
  }
  tb_FotosElementos {
    int id PK
    int galeriaElementosGeologicoId FK
    bytea imagen
    int tipoFoto "0 Desconocido, 1 Lamina, 2 Fotografia"
    varchar(500) descripcionEspecifica
    boolean estadoActivo
  }
```

Las seis tablas `tb_*` heredan de `EntidadAuditable`: además de lo dibujado,
cada una tiene `fechaCreacion`, `fechaActualizacion`, `fechaEliminacion`,
`usuarioCreacionId`, `usuarioActualizacionId` y `usuarioEliminacionId`, con
FK a `Usuarios` (`ON DELETE RESTRICT`). En el diagrama solo se dibuja esa
relación para `tb_ElementosGeologicos`. Identity agrega `UsuarioClaims`,
`UsuarioLogins`, `UsuarioTokens` y `RolClaims`, que el código no usa.

## `sistemageologico_gis` (capas, solo lectura para la aplicación)

```mermaid
erDiagram
  tb_geologia {
    int id PK
    geometry geom "SRID 4326, índice GiST idx_geologia_geom"
    varchar cod_a "clave del color en el QML"
    varchar Edad
    varchar Litologia
  }
  tb_provincias {
    int id PK
    geometry geom "índice GiST"
    varchar dpa_codpro
    varchar dpa_despro
    float area_km2
    int poblacion
  }
  tb_ecuador {
    int id PK
    geometry geom "índice GiST"
    varchar pais
    varchar iso_code
    float area_km2
  }
```

Las tres tablas no se relacionan entre sí ni con el catálogo: el cruce entre
un elemento y su formación lo hace el cliente sobre el mapa. Las crea vacías
`docker/db/init/01-bases-y-capas-gis.sh` con los tipos de `Models/GIS/` y
SRID 4326; los datos del IIGE no están en el repo (PEN-04).

## Diccionario

| Tabla | Propósito | Restricciones e índices |
|---|---|---|
| `Usuarios` | cuentas (Identity) con rol y estado | `IX_Usuarios_Email` único; bloqueo con `AccessFailedCount`/`LockoutEnd` |
| `Roles`, `UsuarioRoles` | tablas que `AddIdentity` exige; vacías y sin uso: el rol vive solo en `Usuarios.Rol` | PK compuesta `(UserId, RoleId)` |
| `tb_Paises`, `tb_Provincias` | catálogo geográfico; países y provincias se crean al vuelo al dar de alta un elemento; cada país tiene una provincia "Desconocida"; la semilla crea "Desconocido"/"Desconocida" | FK `paisId`, `RESTRICT` |
| `tb_Ubicaciones` | punto y localidad de un elemento | FK a provincia (`RESTRICT`); el país es el de la provincia; sin provincia se usa la "Desconocida" de ese país; coordenadas nulas si se desconocen, con `CHECK` de rango |
| `tb_ElementosGeologicos` | fósiles, minerales y rocas en una tabla (TPH) | `IX_ElementosGeologicos_Codigo` único; `nombre` y `codigo` `NOT NULL` |
| `tb_GaleriaElementosGeologicos` | una galería por elemento | FK única a elemento, `CASCADE` |
| `tb_FotosElementos` | imágenes en la base | FK a galería, `CASCADE` |
| `tb_geologia`, `tb_provincias`, `tb_ecuador` | capas del mapa geológico (vacías hasta cargar los datos) | índices GiST sobre `geom`; dueño `postgres` |

## Estrategia de persistencia

- **Migraciones:** EF Core con Npgsql, solo para `sistemageologico_datos`; tres
  migraciones (2026-11-03). `UbicacionPaisDesdeProvinciaYCoordenadasNumericas`
  usa SQL explícito (`ALTER ... TYPE ... USING`, normaliza vacíos y `0.0/0.0`
  a `NULL`) y su `Down` se probó ida y vuelta. Se aplican con el
  paso explícito `--migrate`, nunca al arrancar. La base GIS no tiene
  migraciones: su esquema está en el script de init.
- **Fechas:** `timestamp without time zone` con hora local
  (`DateTime.Now`), mediante `Npgsql.EnableLegacyTimestampBehavior` en
  `Program.cs`.
- **Permisos:** el rol de la aplicación es dueño de `sistemageologico_datos`
  y solo tiene `SELECT` en `sistemageologico_gis`.
- **Borrado:** lógico (`estadoActivo = false`, con fecha y usuario) para
  elementos, fotos y usuarios; los `CASCADE` solo actúan en un borrado físico,
  que el código no hace.
- **Transacciones:** el alta de un elemento guarda el grafo (ubicación,
  provincia y país nuevos, elemento y galería) en un solo `SaveChangesAsync`.
  La carga masiva usa `IUnitOfWork`: transacción explícita, todo o nada.
- **Concurrencia:** sin control optimista en las tablas del catálogo; Identity
  usa `ConcurrencyStamp` en usuarios. Gana la última escritura, y un `Update`
  de entidad completa puede deshacer un borrado lógico concurrente (REV-02,
  H-03; PEN-12).
