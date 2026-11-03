# ARQ-01 · Arquitectura actual

Estado descrito: rama `docker-postgresql`, commit `8dfe171` (2026-11-03): el
sistema en .NET 10 y PostgreSQL, con base, backend y frontend en contenedores
separados. El estado anterior (SQL Server, .NET 8, sin contenedores) está en
[REV-01](../revisiones/2026-11-03-sistema-completo.md), commit `d30f189`.
Reconstruir: [ARQ-02](reconstruir-desde-cero.md). Desplegar:
[DEP-01](../despliegue/vps-docker.md). Stack: [ARQ-03](stack-y-entornos.md).
Diagramas: [DIA-00](../diagramas/indice.md). Índice: [CLAUDE.md](../../CLAUDE.md).

## Stack (resumen)

- **Frontend:** SPA en React 18 + Vite 6, MUI 6, Leaflet; en producción,
  estáticos en Nginx sin privilegios.
- **Backend:** API REST en ASP.NET Core 10 con EF Core 10, Identity, JWT y
  SkiaSharp para las imágenes.
- **Persistencia:** un servidor PostgreSQL 18 + PostGIS 3.6 con dos bases:
  `sistemageologico_datos` (identidad, catálogo y fotos) y
  `sistemageologico_gis` (capas del mapa, solo lectura).
- **Borde:** Caddy (HTTPS automático, encabezados de seguridad, `/api/*` →
  API, el resto → frontend).
- **Externos:** SMTP (informe petrográfico) y teselas de OpenStreetMap.
  Detalle en [ARQ-03](stack-y-entornos.md).

## Estilo y patrones

SPA + API, con el backend en capas Controller → Service → Repository
(**parcial**: los servicios de elementos escriben directo en el DbContext).
El procesamiento de imágenes salió del controlador a
`ImageProcessingService`. Repository genérico, Template Method, herencia TPH,
auditoría con borrado lógico, DTO e inyección de dependencias, aplicados. En
el frontend, contextos como store con hooks de servicio privados y páginas
contenedoras. Evidencia: [REV-01](../revisiones/2026-11-03-sistema-completo.md).

## Vista general

[DIA-01](../diagramas/vista-general.md); entornos y despliegue en
[DIA-08](../diagramas/entornos.md).

## Arquitectura de carpetas

### Raíz

```text
SistemaGeologico/
├── Servidor_Sistema_Geologia/     # backend ASP.NET Core (API REST)
├── Cliente_Sistema_Geologia/
│   └── cliente-sistema-geologico/ # frontend React + Vite
├── docker/
│   ├── db/init/                   # init de PostgreSQL: usuario, dos bases, capas GIS vacías
│   ├── dev/                       # imágenes de desarrollo (SDK de .NET, Node + pnpm)
│   └── prod/                      # imágenes de producción y nginx.conf
├── compose.yaml                   # desarrollo: db, api, web
├── compose.prod.yaml              # producción: db, api, web, caddy
├── Caddyfile                      # HTTPS, encabezados y rutas del borde
├── .env.example                   # variables (sin valores reales)
├── scripts/
│   ├── check-docs.mjs             # verificador de la documentación
│   └── artefactos/                # ART-NN: prueba de humo
├── docs/                          # esta documentación (índice en CLAUDE.md)
├── R_Sistema_Geologico.sln        # solución de Visual Studio (solo el backend)
└── CLAUDE.md                      # índice de la documentación
```

### Backend (`Servidor_Sistema_Geologia/`)

```text
Servidor_Sistema_Geologia/
├── Program.cs              # composición, config obligatoria, JWT, CORS, /health, --migrate, --healthcheck
├── appsettings.json        # valores no secretos; los secretos llegan por variables de entorno
├── Controllers/            # endpoints HTTP
├── Services/
│   ├── Interfaces/
│   └── Implementation/     # lógica: elementos (base + fósil/mineral/roca), usuarios, auth, JWT, correo, GIS, QML, imágenes
├── Repositories/           # acceso a datos; base genérica con OfType<T>
├── Models/                 # entidades: ElementosGeologicos, Locations, Users, Auditoria, Galeria, GIS
├── DTO/                    # contratos de la API por área
├── DAL/                    # SistemaGeologicoDbContext y PostGISDbContext (ambos Npgsql)
├── Migrations/             # una migración: InitialPostgres (2026-11-03)
├── Constants/              # enums: roles, subtipos, tipo de foto
├── Data/SeedData.cs        # semilla idempotente: roles, superadmin, país/provincia "Desconocido"
└── GIS/                    # estilo QML de QGIS: colores y etiquetas de las formaciones
```

### Frontend (`Cliente_Sistema_Geologia/cliente-sistema-geologico/`)

```text
cliente-sistema-geologico/
├── package.json            # dependencias y scripts (dev, build, lint, preview); pnpm 10
├── pnpm-lock.yaml
├── vite.config.js          # servidor en 5174; proxy /api → VITE_PROXY_TARGET
├── index.html, index.css
└── src/
    ├── main.jsx, App.jsx   # arranque, proveedores globales y rutas
    ├── pages/              # pantallas: consumen el contexto
    ├── components/         # UI pura por área
    ├── context/            # estado: Auth, Elementos (globales); Users, Geologia, Excel, Detail (por página)
    ├── hooks/              # servicios HTTP, privados de su contexto
    ├── lib/apiClient.js    # Axios + token JWT
    ├── constants/, types/, utils/, stubs/
```

## Modelo de datos

[DIA-03](../diagramas/modelo-datos.md).

## Contrato de API

Todas las rutas cuelgan de `/api`. "Autenticado" = cualquier JWT válido.
Errores de autenticación en JSON (`{ success:false, message, ... }`) con 401
o 403; en producción, una excepción no controlada responde 500 con
`{ success:false, message:"Error interno del servidor" }`.

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
| DELETE | `/users/{id}` | Admin | Desactiva (borrado lógico). |
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
| GET | `/foto-elementos/imagen/{id}` | autenticado | Imagen (`?thumb=true` → 300×200 como máximo); marca de agua salvo para Premium, Admin e Invitado. |
| GET | `/foto-elementos/por-elemento/{elementoId}` | autenticado | Galería y metadatos de fotos. |
| POST | `/foto-elementos/galeria/{galeriaId}`, `/elemento/{elementoId}` | Admin, Invitado | Sube una foto (multipart, campo `ImagenFile`). |
| PUT | `/foto-elementos/{id}` | Admin, Invitado | Edita una foto. |
| DELETE | `/foto-elementos/{id}` | Admin | Borrado lógico. |
| PATCH | `/foto-elementos/{id}/restaurar` | Admin | Restaura. |
| GET | `/paises/active` | autenticado | Países activos. |
| GET | `/provincias/by-pais/{paisId}` | autenticado | Provincias de un país. |
| GET | `/GeologiaGIS/ecuador` | autenticado | Contorno del Ecuador (GeoJSON). |
| GET | `/GeologiaGIS/provincias` | autenticado | Polígonos de provincias (GeoJSON). |
| GET | `/GeologiaGIS/geologia/simplified?tolerance=` | autenticado | Formaciones simplificadas con `ST_Simplify` (GeoJSON con color y etiqueta del QML). |

Fuera de `/api`: `GET /health` (anónimo; estado de las dos bases). Caddy
no lo expone: solo reenvía `/api/*` a la API. `/swagger` existe solo con
`ASPNETCORE_ENVIRONMENT=Development`.

Modos de línea de comandos del mismo binario: `--migrate` aplica las
migraciones y la semilla y termina; `--healthcheck` consulta `/health` y
termina con 0 o 1 (lo usa el `HEALTHCHECK` de la imagen).

## Componentes y servicios principales

| Módulo | Responsabilidad | Depende de |
|---|---|---|
| `Program.cs` | Composición; valida la configuración obligatoria al arrancar; JWT; CORS; encabezados; `/health` | configuración |
| `Data/SeedData.cs` | Roles, superadmin (solo si hay email y contraseña configurados) y país/provincia "Desconocido" | Identity, DbContext |
| `BaseElementoGeologicoService` + `Fosil/Mineral/RocaService` | Alta, edición y borrado; crea ubicación (con "Desconocido" si falta), provincia y galería | repositorio y DbContext |
| `FotoElementosController` + `FotoElementoService` | Fotos en la base con caché en memoria (500 MB, 2 h) | `IImageProcessingService`, `IMemoryCache` |
| `ImageProcessingService` | Miniatura y marca de agua con SkiaSharp; falla cerrado si no puede marcar | fuente DejaVu Sans Bold (o `ImageProcessing:WatermarkFontPath`) |
| `AuthService`, `JwtService` | Registro, login con bloqueo, JWT (12 h por defecto) | Identity |
| `UserService` | Administración de usuarios | Identity |
| `GeologiaGISService` + `QmlColorService` | GeoJSON de PostGIS con colores del QML | `PostGISDbContext`, `GIS/*.qml` |
| `EmailService` | SMTP con MailKit | `EmailSettings` |
| `src/lib/apiClient.js` | Axios con `baseURL = VITE_API_URL` y el token en `localStorage` | — |

## Estado de seguridad

Lo que hace el código; los hallazgos de la auditoría de seguridad van aparte.

- **Autenticación:** JWT HS256 con clave obligatoria de 32 caracteres como
  mínimo (la API no arranca sin ella); emisor y audiencia validados; 12 h de
  vida; el rol viaja en el token.
- **Contraseñas:** Identity (mínimo 6, dígito, minúscula y mayúscula); bloqueo
  de 5 min tras 3 fallos. El superadmin solo se crea si hay contraseña
  configurada, y la contraseña no se escribe en el log.
- **Autorización:** `[Authorize(Roles=…)]` por endpoint.
- **Fotos:** la marca de agua para Free se aplica en el servidor en cualquier
  sistema operativo; si falla, no se entrega la imagen.
- **Secretos:** solo por variables de entorno (`.env` fuera de git);
  `appsettings.json` no tiene secretos; las imágenes no contienen secretos
  (verificado con `docker history`).
- **Borde:** Caddy con HTTPS, HSTS, CSP, `X-Frame-Options: DENY`,
  `nosniff`, `Permissions-Policy` y límite de 25 MB por petición. La API agrega
  `default-src 'none'` a sus respuestas.
- **Contenedores:** usuarios sin privilegios (api 1654, web 101), sistema de
  archivos de solo lectura, `no-new-privileges`; la base solo en una red
  interna sin puertos publicados.
- **Base de datos:** la aplicación usa un rol propio, dueño de la base del
  catálogo y con solo `SELECT` en la de GIS.

## Variables de entorno

Backend (variables con `__` como separador de secciones):

| Clave | Uso | Por defecto | Si falta |
|---|---|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL del catálogo e identidad | — | no arranca |
| `ConnectionStrings__PostGISConnection` | PostgreSQL de las capas GIS | — | no arranca |
| `JwtSettings__SecretKey` | firma del JWT (≥ 32 caracteres) | — | no arranca |
| `JwtSettings__Issuer` / `__Audience` / `__ExpirationMinutes` | emisor, audiencia, vida | `SistemaGeologico` / `SistemaGeologico-API` / 720 | valor por defecto |
| `DefaultSuperAdmin__Email`, `__Password`, `__UserName`, `__NombreCompleto` | superadmin de la semilla | — / — / `superadmin` / `Super Administrador` | sin email o contraseña, no se crea |
| `Cors__AllowedOrigins__0…` | orígenes permitidos si el frontend está en otro dominio | ninguno | sin CORS (no hace falta con el proxy) |
| `EmailSettings__SmtpHost`, `__SmtpPort`, `__UseSsl`, `__UseStartTls`, `__SenderEmail`, `__SenderName`, `__SenderPassword`, `__DestinationEmails__0…`, `__DestinationName` | correo de informes | puerto 587, STARTTLS | la solicitud de informe responde 200 con `success:false` |
| `ImageProcessing__WatermarkFontPath` | fuente de la marca de agua | DejaVu Sans Bold, Arial | no arranca si no hay ninguna |
| `QmlColorFilePath` | estilo QML | `GIS/Estilo colores geología.qml` | formaciones sin color |
| `ASPNETCORE_ENVIRONMENT` | `Development` habilita Swagger | `Production` en la imagen | — |

Compose (`.env`, plantilla en `.env.example`): `DOMINIO`, `POSTGRES_PASSWORD`,
`APP_DB_USER`, `APP_DB_PASSWORD`, `JWT_SECRET`, `ADMIN_EMAIL`, `ADMIN_PASSWORD`,
`SMTP_*`, `INFORME_DESTINO`, `PUERTO_HTTP`, `PUERTO_HTTPS`, `UID_HOST`,
`GID_HOST`. Su correspondencia con las claves de arriba está en
`compose.prod.yaml`.

Frontend (en tiempo de **build**; Vite las incrusta en el JS):

| Variable | Uso | Por defecto |
|---|---|---|
| `VITE_API_URL` | base de la API | `/api` |
| `VITE_USE_STUBS` | datos falsos sin backend | `false` |
| `VITE_PROXY_TARGET` | solo servidor de desarrollo: destino de `/api` | `http://localhost:5095` (en Docker, `http://api:5095`) |
