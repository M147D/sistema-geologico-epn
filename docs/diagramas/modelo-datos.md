# DIA-03 · Modelo de datos

**Qué muestra:** las tablas del catálogo y la identidad (SQL Server) y las
capas GIS (PostgreSQL + PostGIS), con tipos, claves y cardinalidades.

**Sale de:** `Servidor_Sistema_Geologia/DAL/SistemaGeologicoDbContext.cs`,
`DAL/PostGISDbContext.cs`, `Models/`, las seis migraciones de `Migrations/`
y `SistemaGeologicoDbContextModelSnapshot.cs` (commit `d30f189`).

**Texto que lo explica:** [ARQ-01](../arquitectura/diagrama.md) y
[REV-01](../revisiones/2026-11-03-sistema-completo.md); índice
[DIA-00](indice.md).

## SQL Server (catálogo e identidad)

```mermaid
erDiagram
  Usuarios ||--o{ UsuarioRoles : "tiene"
  Roles ||--o{ UsuarioRoles : "se asigna en"
  tb_Paises ||--o{ tb_Provincias : "contiene"
  tb_Paises ||--o{ tb_Ubicaciones : "ubica (copia, H-05)"
  tb_Provincias ||--o{ tb_Ubicaciones : "ubica"
  tb_Ubicaciones |o--o{ tb_ElementosGeologicos : "localiza"
  tb_ElementosGeologicos ||--o| tb_GaleriaElementosGeologicos : "tiene"
  tb_GaleriaElementosGeologicos ||--o{ tb_FotosElementos : "agrupa"
  Usuarios |o--o{ tb_ElementosGeologicos : "audita (crea, actualiza, elimina)"

  Usuarios {
    int Id PK
    nvarchar(256) Email "UNIQUE IX_Usuarios_Email"
    nvarchar(256) UserName
    nvarchar(max) PasswordHash
    nvarchar(200) NombreCompleto
    int Rol "0 Free, 1 Premium, 2 Admin, 3 Invitado"
    datetime2 FechaCreacion
    bit EstadoActivo
    int AccessFailedCount
    datetimeoffset LockoutEnd
  }
  Roles {
    int Id PK
    nvarchar(256) Name
  }
  UsuarioRoles {
    int UserId PK, FK
    int RoleId PK, FK
  }
  tb_Paises {
    int id PK
    nvarchar(100) nombrePais
    bit estadoActivo
  }
  tb_Provincias {
    int id PK
    int paisId FK
    nvarchar(100) nombreProvincia
    bit estadoActivo
  }
  tb_Ubicaciones {
    int id PK
    int paisId FK
    int provinciaId FK
    nvarchar(60) latitud "texto, H-06"
    nvarchar(60) longitud "texto, H-06"
    nvarchar(500) localidad
    nvarchar(60) leyenda
    bit estadoActivo
  }
  tb_ElementosGeologicos {
    int id PK
    nvarchar(21) tipoElemento "discriminador: Fosil, Mineral, Roca"
    nvarchar(200) nombre
    nvarchar(100) codigo "UNIQUE"
    nvarchar(50) edad
    nvarchar(200) donante
    datetime2 fechaIngreso
    bigint ejemplares
    nvarchar(max) documentosRelacionados
    bit laminaExiste
    int ubicacionId FK
    int tipoFosil "solo Fosil"
    nvarchar(200) especie "solo Fosil"
    nvarchar(200) periodo "solo Fosil"
    int tipoMineral "solo Mineral"
    nvarchar(500) litologia "solo Mineral"
    int tipoRoca "solo Roca"
    nvarchar(500) litologia_roca "solo Roca"
    datetime2 fechaCreacion "DEFAULT GETDATE()"
    int usuarioCreacionId FK
    int usuarioActualizacionId FK
    int usuarioEliminacionId FK
    bit estadoActivo "DEFAULT 1"
  }
  tb_GaleriaElementosGeologicos {
    int id PK
    int elementoGeologicoId FK "1:1"
    nvarchar(200) detalleGrupo
    bit estadoActivo
  }
  tb_FotosElementos {
    int id PK
    int galeriaElementosGeologicoId FK
    varbinary(max) imagen
    int tipoFoto "0 Desconocido, 1 Lamina, 2 Fotografia"
    nvarchar(500) descripcionEspecifica
    bit estadoActivo
  }
```

Las seis tablas `tb_*` heredan de `EntidadAuditable`: además de lo dibujado,
cada una tiene `fechaCreacion`, `fechaActualizacion`, `fechaEliminacion`,
`usuarioCreacionId`, `usuarioActualizacionId` y `usuarioEliminacionId`, con
FK a `Usuarios` (`ON DELETE RESTRICT`). En el diagrama solo se dibuja esa
relación para `tb_ElementosGeologicos`. Identity agrega `UsuarioClaims`,
`UsuarioLogins`, `UsuarioTokens` y `RolClaims`, que el código no usa.

## PostgreSQL + PostGIS (capas, solo lectura)

```mermaid
erDiagram
  tb_geologia {
    int id PK
    geometry geom "índice GiST idx_geologia_geom"
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
un elemento y su formación lo hace el cliente sobre el mapa. Los tipos de
texto salen de los `[MaxLength]` de `Models/GIS/`; el esquema real no está en
el repo (REV-01, H-04).

## Diccionario

| Tabla | Propósito | Restricciones e índices |
|---|---|---|
| `Usuarios` | cuentas (Identity) con rol y estado | `IX_Usuarios_Email` único; bloqueo con `AccessFailedCount`/`LockoutEnd` |
| `Roles`, `UsuarioRoles` | roles de Identity; copia del rol que no se mantiene (H-07) | PK compuesta `(UserId, RoleId)` |
| `tb_Paises`, `tb_Provincias` | catálogo geográfico; las provincias se crean al vuelo al dar de alta un elemento | FK `paisId`, `RESTRICT` |
| `tb_Ubicaciones` | punto y localidad de un elemento | FK a país y provincia, `RESTRICT`; defaults 1 sin filas sembradas (H-03) |
| `tb_ElementosGeologicos` | fósiles, minerales y rocas en una tabla (TPH) | `IX_ElementosGeologicos_Codigo` único; `nombre` y `codigo` `NOT NULL` |
| `tb_GaleriaElementosGeologicos` | una galería por elemento | FK única a elemento, `CASCADE` |
| `tb_FotosElementos` | imágenes en la base | FK a galería, `CASCADE` |
| `tb_geologia`, `tb_provincias`, `tb_ecuador` | capas importadas de QGIS | índices GiST sobre `geom` |

## Estrategia de persistencia

- **Migraciones:** EF Core, solo para SQL Server; dos de las seis contienen
  T-SQL a mano. No hay `Database.Migrate()` al arrancar: se aplican con
  `dotnet ef database update`. PostGIS no tiene migraciones.
- **Borrado:** lógico (`estadoActivo = false`, con fecha y usuario) para
  elementos, fotos y usuarios; los `CASCADE` solo actúan en un borrado físico,
  que el código no hace.
- **Transacciones:** ninguna explícita; cada `SaveChangesAsync` es su propia
  transacción, y el alta de un elemento usa varias (H-08).
- **Concurrencia:** sin control optimista en las tablas del catálogo (no hay
  `rowversion`); Identity usa su `ConcurrencyStamp` en usuarios y roles. Gana
  la última escritura.
