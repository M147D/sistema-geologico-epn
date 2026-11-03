# ARQ-01 · Arquitectura actual

Estado descrito: rama `main`, commit `d30f189` (2026-07-11), tal como se
clonó de `github.com/M147D/sistema-geologico-epn` el 2026-11-03. El runbook
de reconstrucción y el documento de despliegue en Docker todavía no existen.
Stack y entornos: [ARQ-03](stack-y-entornos.md). Diagramas:
[DIA-00](../diagramas/indice.md). Índice del proyecto: [CLAUDE.md](../../CLAUDE.md).

## Stack (resumen)

- **Frontend:** SPA en React 18 + Vite 6, MUI 6, Leaflet / react-leaflet, Axios.
- **Backend:** API REST en ASP.NET Core 8 con EF Core 8, Identity y JWT.
- **Persistencia:** SQL Server (identidad y catálogo, fotos incluidas) y
  PostgreSQL + PostGIS (capas del mapa geológico, solo lectura).
- **Servicios externos:** SMTP (solicitud de informe petrográfico) y teselas
  de OpenStreetMap. Detalle en [ARQ-03](stack-y-entornos.md).

## Estilo y patrones

SPA + API, con el backend en capas Controller → Service → Repository
(**parcial**: los servicios de elementos escriben directo en el DbContext y el
controlador de fotos procesa imágenes). Repository genérico, Template Method,
herencia TPH, auditoría con borrado lógico, DTO e inyección de dependencias,
aplicados. En el frontend, contextos como store con hooks de servicio privados
y páginas contenedoras con componentes presentacionales. Evidencia y
hallazgos: [REV-01](../revisiones/2026-11-03-sistema-completo.md).

## Vista general

[DIA-01](../diagramas/vista-general.md).

## Arquitectura de carpetas

### Raíz

```text
SistemaGeologico/
├── Servidor_Sistema_Geologia/     # backend ASP.NET Core (API REST)
├── Cliente_Sistema_Geologia/
│   └── cliente-sistema-geologico/ # frontend React + Vite
├── R_Sistema_Geologico.sln        # solución de Visual Studio (solo el backend)
├── DiagramasTIC.png               # diagrama de construcción del README
├── docs/                          # esta documentación (índice en CLAUDE.md)
├── scripts/check-docs.mjs         # verificador de la documentación
└── CLAUDE.md                      # índice de la documentación
```

### Backend (`Servidor_Sistema_Geologia/`)

```text
Servidor_Sistema_Geologia/
├── Program.cs              # composición: DbContexts, Identity, JWT, CORS, DI, semilla del superadmin
├── Controllers/            # endpoints HTTP; FotoElementosController también procesa imágenes
├── Services/
│   ├── Interfaces/         # contratos de la lógica de negocio
│   └── Implementation/     # lógica: base genérica de elementos + fósil/mineral/roca, usuarios, auth, JWT, correo, GIS, QML
├── Repositories/
│   ├── Interfaces/
│   └── Implementation/     # acceso a datos; base genérica con OfType<T>
├── Models/                 # entidades por área: ElementosGeologicos, Locations, Users, Auditoria, Galeria, GIS
├── DTO/                    # contratos de la API por área (Auth, ElementosGeologicos, Users, Gallery, Locations, GIS)
├── DAL/                    # SistemaGeologicoDbContext (SQL Server) y PostGISDbContext (PostgreSQL)
├── Migrations/             # migraciones de EF Core del catálogo (solo SQL Server)
├── Constants/              # enums: roles, subtipos, tipo de foto
├── Data/SeedData.cs        # semilla duplicada que nadie llama (REV-01, H-12)
├── GIS/                    # estilo QML de QGIS: colores y etiquetas de las formaciones
└── Scripts/                # SQL de ejemplo desactualizado (REV-01, H-13)
```

### Frontend (`Cliente_Sistema_Geologia/cliente-sistema-geologico/`)

```text
cliente-sistema-geologico/
├── index.html, index.css   # documento y estilos globales
├── vite.config.js          # servidor de desarrollo en 5174, proxy /api → 5095
├── .env.example            # VITE_API_URL, VITE_USE_STUBS
├── pnpm-lock.yaml          # dependencias (package.json no está en el repo: REV-01, H-02)
└── src/
    ├── main.jsx, App.jsx   # arranque, proveedores globales y rutas
    ├── pages/              # pantallas: consumen el contexto y manejan carga y errores
    ├── components/         # UI pura por área: auth, crud, detail, excel, layout, login, mapa, users
    ├── context/            # estado: Auth, Elementos (globales); Users, Geologia, Excel, Detail (por página)
    ├── hooks/              # servicios HTTP, privados de su contexto
    ├── lib/apiClient.js    # instancia única de Axios + token JWT
    ├── constants/          # roles, colores de subtipos, mapas, parámetros del mapa
    ├── types/              # typedefs JSDoc que reflejan los DTO del backend
    ├── utils/              # caché de imágenes, utilidades del detalle
    └── stubs/              # datos falsos para VITE_USE_STUBS=true
```

## Modelo de datos

[DIA-03](../diagramas/modelo-datos.md).

## Contrato de API

Todas las rutas cuelgan de `/api`. "Autenticado" = cualquier JWT válido.
Las respuestas de error de autenticación son JSON
(`{ success:false, message, ... }`) con 401 o 403 (`Program.cs`).

| Método | Ruta | Acceso | Qué hace |
|---|---|---|---|
| POST | `/auth/register` | público | Registra un usuario; el rol se fuerza a Free; devuelve JWT. |
| POST | `/auth/login` | público | Devuelve JWT; bloquea 5 min tras 3 fallos. |
| POST | `/auth/logout` | autenticado | Sin estado en el servidor: el cliente descarta el token. |
| GET | `/auth/me` | autenticado | Usuario del token. |
| GET | `/users` | Admin | Lista paginada y filtrada. |
| GET | `/users/{id}` | Admin | Detalle. |
| POST | `/users` | Admin | Crea usuario con rol. |
| PUT | `/users/{id}` | Admin | Edita (incluye rol y estado). |
| DELETE | `/users/{id}` | Admin | Desactiva. |
| POST | `/users/{id}/change-password`, `/toggle-status`, `/reactivate` | Admin | Contraseña y estado. |
| GET | `/users/stats`, `/check-email/{email}`, `/check-username/{userName}` | Admin | Estadísticas y unicidad. |
| GET | `/elementos-geologicos` | Admin, Premium, Free, Invitado | Lista (filtro `tipo`). |
| GET | `/elementos-geologicos/{id}` | Admin, Premium, Free, Invitado | Detalle. |
| GET | `/elementos-geologicos/fosiles`, `/minerales`, `/rocas` | Admin, Premium, Free, Invitado | Lista por tipo (`page`, `pageSize`). |
| GET | `/elementos-geologicos/by-codigo/{codigo}` | Admin, Invitado | Busca por código. |
| POST | `/elementos-geologicos/fosiles`, `/minerales`, `/rocas` | Admin, Invitado | Crea (con ubicación y galería). |
| PUT | `/elementos-geologicos/{fosiles\|minerales\|rocas}/{id}` | Admin, Invitado | Edita. |
| DELETE | `/elementos-geologicos/{id}` | Admin | Borrado lógico. |
| POST | `/elementos-geologicos/{id}/solicitar-informe` | autenticado | Envía por correo la solicitud de informe petrográfico. |
| GET | `/foto-elementos/galeria/{galeriaId}` | autenticado | Fotos de una galería. |
| GET | `/foto-elementos/imagen/{id}` | autenticado | Bytes de la imagen (`?thumb=true`); marca de agua si no es Premium/Admin/Invitado (solo en Windows: REV-01, H-01). |
| GET | `/foto-elementos/por-elemento/{elementoId}` | autenticado | Galería y metadatos de fotos. |
| POST | `/foto-elementos/galeria/{galeriaId}`, `/elemento/{elementoId}` | Admin, Invitado | Sube una foto (multipart). |
| PUT | `/foto-elementos/{id}` | Admin, Invitado | Edita una foto. |
| DELETE | `/foto-elementos/{id}` | Admin | Borrado lógico. |
| PATCH | `/foto-elementos/{id}/restaurar` | Admin | Restaura. |
| GET | `/paises/active` | autenticado | Países activos. |
| GET | `/provincias/by-pais/{paisId}` | autenticado | Provincias de un país. |
| GET | `/GeologiaGIS/ecuador` | autenticado | Contorno del Ecuador (GeoJSON). |
| GET | `/GeologiaGIS/provincias` | autenticado | Polígonos de provincias (GeoJSON). |
| GET | `/GeologiaGIS/geologia/simplified?tolerance=` | autenticado | Formaciones simplificadas con `ST_Simplify` (GeoJSON con color y etiqueta del QML). |

`/swagger` solo existe con `ASPNETCORE_ENVIRONMENT=Development`. La lista de
endpoints que `Program.cs` imprime al arrancar está desactualizada (nombra
`/geologia/point`, `/estadisticas` e `/info`, que no existen).

## Componentes y servicios principales

| Módulo | Responsabilidad | Depende de |
|---|---|---|
| `Program.cs` | Composición, autenticación, CORS, cabeceras de seguridad, roles y superadmin al arrancar | configuración |
| `BaseElementoGeologicoService` + `Fosil/Mineral/RocaService` | Alta, edición y borrado de elementos; crea ubicación, provincia y galería | repositorio y DbContext |
| `ElementoGeologicoService` | Consultas unificadas de los tres tipos | `ElementoGeologicoRepository` |
| `FotoElementoService` + `FotoElementosController` | Fotos en la base; miniatura y marca de agua (GDI+, solo Windows) con caché en memoria de 500 MB | `IMemoryCache` |
| `AuthService`, `JwtService` | Registro, login con bloqueo, emisión de JWT (12 h por defecto) | Identity |
| `UserService` | Administración de usuarios | Identity |
| `GeologiaGISService` + `QmlColorService` | GeoJSON de PostGIS con colores y etiquetas leídos del QML | `PostGISDbContext`, `GIS/*.qml` |
| `EmailService` | Envío SMTP con MailKit | configuración `EmailSettings` |
| `src/lib/apiClient.js` | Axios con `baseURL = VITE_API_URL` y el token en `localStorage` | — |
| `src/context/*` | Estado y operaciones por área; cada uno usa su hook de servicio | hooks, `apiClient` |

## Estado de seguridad

Resumen de lo que hace el código; la auditoría de seguridad es un trabajo
aparte.

- **Autenticación:** JWT HS256, emisor y audiencia validados, sin tolerancia de
  reloj; 12 h de vida; el rol va en el token, así que un cambio de rol o una
  desactivación no rigen hasta que el token expira.
- **Contraseñas:** Identity (mínimo 6, dígito, minúscula y mayúscula);
  bloqueo de 5 min tras 3 fallos.
- **Autorización:** atributos `[Authorize(Roles=…)]` por endpoint; el cliente
  repite las guardias solo para la navegación.
- **Secretos:** fuera del repo (por la regla `*.json`), pero con valores por
  defecto en el código para la clave JWT y la contraseña del superadmin, y la
  contraseña se escribe en el log al crearlo.
- **Cabeceras:** `nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy` y una CSP
  restrictiva en las respuestas de la API. HTTPS y HSTS solo fuera de
  Development.
- **CORS:** orígenes `localhost` fijos en el código.
- **Validación:** DataAnnotations y expresiones regulares en modelos y DTO
  (`NoInjectionAttribute`); la regex de nombres (`^[a-zA-Z\s]+$`) rechaza
  tildes y eñes.

## Variables de entorno

Backend (`appsettings.json` o variables de entorno con `__` como separador,
p. ej. `ConnectionStrings__DefaultConnection`):

| Clave | Uso | Por defecto en el código | Si falta |
|---|---|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server del catálogo e identidad | — | falla al primer acceso a datos |
| `ConnectionStrings:PostGISConnection` | PostgreSQL + PostGIS | — | falla el mapa |
| `JwtSettings:SecretKey` | firma del JWT (≥ 32 caracteres) | clave de ejemplo solo al validar | no se puede iniciar sesión |
| `JwtSettings:Issuer` / `Audience` | emisor y audiencia | `SistemaGeologico` / `SistemaGeologico-API` | usa el valor por defecto |
| `JwtSettings:ExpirationMinutes` | vida del token | 720 | usa el valor por defecto |
| `DefaultSuperAdmin:Email`, `UserName`, `Password`, `NombreCompleto` | superadmin creado al arrancar | `admin@sistemageologico.com`, `superadmin`, `Admin123!`, `Super Administrador` | usa el valor por defecto |
| `EmailSettings:SmtpHost`, `SmtpPort`, `UseSsl`, `UseStartTls`, `SenderEmail`, `SenderName`, `SenderPassword`, `DestinationEmails[]`, `DestinationName` | correo de informes | puerto 587, STARTTLS | la solicitud de informe responde error sin 500 |
| `QmlColorFilePath` | ruta del estilo QML | `GIS/Estilo colores geología.qml` | formaciones sin color |
| `ASPNETCORE_ENVIRONMENT` | Development habilita Swagger y la página de errores | Production | — |

Frontend (en tiempo de **build**, Vite las incrusta en el JS):

| Variable | Uso | Por defecto |
|---|---|---|
| `VITE_API_URL` | base de la API | `/api` en `.env.example` |
| `VITE_USE_STUBS` | datos falsos sin backend | `false` |
