# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Sistema Geológico is a geological information management system for cataloging fossils, minerals, and rocks with GIS capabilities. It consists of an ASP.NET Core 8.0 backend and a React frontend with Vite.

## Build and Run Commands

### Backend (Servidor_Sistema_Geologia/)
```bash
dotnet restore                              # Restore dependencies
dotnet build                                # Build the project
dotnet run                                  # Run on http://localhost:5095
dotnet watch run                            # Run with hot reload
dotnet ef migrations add <MigrationName>    # Create new migration
dotnet ef database update                   # Apply migrations
```

All `dotnet` commands must be run from the `Servidor_Sistema_Geologia/` directory.

### Frontend (Cliente_Sistema_Geologia/cliente-sistema-geologico/)
```bash
pnpm install     # Install dependencies
pnpm dev         # Start dev server on http://localhost:5174
pnpm build       # Production build
pnpm lint        # Run ESLint
pnpm preview     # Preview production build
```

All `pnpm` commands must be run from `Cliente_Sistema_Geologia/cliente-sistema-geologico/`. **Use pnpm exclusively — npm is prohibited.**

### Running Both
Start backend first (port 5095), then frontend (port 5174). The frontend proxies `/api/*` requests to the backend via Vite config.

### Restarting the backend on Windows
```bash
taskkill //F //IM dotnet.exe
cd Servidor_Sistema_Geologia && dotnet run
```

### Database Requirements
- **SQL Server** (SQLEXPRESS): `db_SistemaGeologico_Datos` — Primary database for Identity, CRUD operations. Context: `SistemaGeologicoDbContext`. Connection string: `DefaultConnection` in `appsettings.json`
- **PostgreSQL + PostGIS**: `db_SistemaGeologico_GIS` — Spatial/GIS queries. Context: `PostGISDbContext`. Connection string: `PostGISConnection` in `appsettings.json`
- No tests exist in this project currently

## Architecture

### Backend (Repository + Service + Controller Pattern)

```
Controllers/          → API endpoints (unified ElementosGeologicosController for all element types)
Services/             → Business logic (base class + specific implementations)
Repositories/         → Data access with generic base repository
Models/               → Domain entities organized by feature (ElementosGeologicos/, Locations/, Users/, Auditoria/, GIS/, Galeria/)
DTO/                  → Data transfer objects (see canonical folders below)
DAL/                  → DbContexts (SistemaGeologicoDbContext for SQL Server, PostGISDbContext for PostgreSQL)
Constants/            → Enums (SubtipoFosil, SubtipoMineral, SubtipoRoca, RolUsuario, TipoFoto)
Data/                 → SeedData.cs for initial data seeding
```

**Canonical DTO folders** (use these for new DTOs):
- `DTO/Auth/` — login, register, token, user info
- `DTO/ElementosGeologicos/` — all CRUD + list/detail/paginated for geological elements
- `DTO/Users/` — user CRUD, filter, stats, paginated
- `DTO/Gallery/` — photo/gallery DTOs (English-named; `DTO/Galeria/` is legacy with one old file)
- `DTO/Locations/` — country/province CRUD DTOs (English-named; `DTO/Ubicaciones/` is legacy with one old file)
- `DTO/GIS/` — GIS feature/statistics DTOs
- `DTO/Validation/` — `NoInjectionAttribute.cs` for SQL injection prevention on string inputs

**Legacy DTO folders** (do not add new files): `DTO/Galeria/`, `DTO/Ubicaciones/`, `DTO/Usuarios/`

### API Endpoints
- `/api/auth` — Authentication (login, register). Google OAuth is NOT implemented — `loginWithGoogle` in `AuthContext` throws immediately. Fields: `RegisterDto` has Email, Password, ConfirmPassword, NombreCompleto (optional); `Rol` is ignored server-side, always forced to Free.
- `/api/users` — User management (Admin only): `GET /` (paginated+filtered), `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}`, `POST /{id}/reactivate`, `POST /{id}/change-password`, `POST /{id}/toggle-status`, `GET /stats`, `GET /by-role/{rol}`
- `/api/elementos-geologicos` — All geological elements (unified for Fosil, Mineral, Roca)
- `/api/foto-elementos` — Photo management (upload, update, soft-delete, restore, image serving with role-based watermarks). Key endpoints: `GET /imagen/{id}` (serves image bytes, applies watermark for Free), `GET /por-elemento/{elementoId}` (role-aware photo list), `POST /elemento/{elementoId}` (upload + auto-create gallery), `PATCH /{id}/restaurar` (Admin only)
- `/api/paises` — Countries
- `/api/provincias` — Provinces
- `/api/geologia-gis` — GIS/spatial queries. Key endpoints: `GET /ecuador` (1 polygon, national boundary), `GET /provincias` (24 province polygons), `GET /geologia/simplified?tolerance=0.005` (simplified geological formations — tolerance controls PostGIS simplification; lower = more detail, larger payload), `GET /geologia/at-point?lat=&lng=` (formation at coordinate), `GET /statistics` (formation stats)
- `POST /api/elementos-geologicos/{id}/solicitar-informe` — Send petrographic report request email. Body: `{ correoSolicitante, observaciones? }`. All authenticated roles. Returns `{ success, message }` (never 500 on SMTP failure).

### Frontend (Context API + Page-based routing)

```
src/constants/        → roles.js — ROLES enum { FREE:0, PREMIUM:1, ADMIN:2, INVITADO:3 }, ROL_LABELS (number→label), ROL_COLORS (number→MUI color)
                        subtipoColors.js — TIPO_COLORS, SUBTIPO_COLORS (hex por subtipo), getSubtipoColor(tipo, tipoEspecifico), getSubtipoDisplayLabel(key), getSubtipoEntries(tipo)
                        tipoMaps.js — SUBTIPOS_FOSIL/MINERAL/ROCA (value+label), inverse maps para Excel
                        mapConstants.js — MAP_CENTER, MAP_ZOOM, MAP_TILE_URL, MAP_ATTRIBUTION, FULLSCREEN_SX
src/lib/              → apiClient.js — singleton Axios instance + JWT interceptors (not a Context: no component-level reactivity needed, the token lives in a module-level variable)
src/context/          → Provider in App.jsx (accessible across all routes): AuthContext, ElementosContext
                        Provider inside its page (mounted/unmounted with the route): UsersContext (PageUsers), GeologiaContext (PageMap), ExcelContext (PageExcel), DetailContext (PageDetails)
src/pages/            → PageLogin, PageRegister, HomePage, PageMap, PageForm, PageTable, PageDetails, PageExcel, PageUsers, PageNotFound
src/components/       → Organized by feature: auth/, crud/, detail/, filters/, login/, mapa/, mapa/elements/, mapa/geological/, results/, users/
src/hooks/            → HTTP-only service hooks, each private to its context: useAuthService (→ AuthContext), useElementosService (→ ElementosContext), useGeologiaService (→ GeologiaContext), useUsersService (→ UsersContext). No hook contains UI state or is consumed directly by a component.
src/layout/           → RootLayout.jsx (main app layout)
src/utils/            → imageCache.js — module-level blob URL cache for photo loading
                        detailUtils.js — getTypeColor, getTypeLabel, getSubtipos, getSubtipoLabel(tipo, value), getSubtipo(el), getStatusColor
src/types/            → JSDoc typedefs mirroring backend DTOs (auth.js, elementosGeologicos.js, locations.js, gallery.js, gis.js)
```

### Frontend Architecture Patterns

- **Page-Component pattern**: Pages own context consumption, loading guards, and error state. Components receive all data as props and contain only UI-local state (`useState` for modals, tooltips, etc.). No component calls a context hook or a service hook directly.
- **Context as single access point**: Every service hook is private to its context — `useAuthService` → `AuthContext`, `useElementosService` → `ElementosContext`, `useGeologiaService` → `GeologiaContext`, `useUsersService` → `UsersContext`. No component or page imports a service hook.
- **Provider + inner component pattern**: When a page both provides AND consumes a context, it must use an inner component to avoid the circular dependency. The page exports the `Provider` wrapping an inner `*Content` component that calls `useX()`. Applied to: `PageUsers` (`UsersProvider` + `UsersPageContent`), `PageMap` (`GeologiaProvider` + `PageMapContent`), `PageExcel` (`ExcelProvider` + `PageExcelContent`), `PageDetails` (`DetailProvider` + `PageDetailsContent`).
- **Context scope by Provider placement**: All contexts use `React.createContext()` identically. The distinction is where the `Provider` is declared: `AuthProvider`/`ElementosProvider` are in `App.jsx` (available across all routes); `UsersProvider`/`GeologiaProvider`/`ExcelProvider`/`DetailProvider` are inside their page (mounted and unmounted with the route).
- **Auto-loading**: `ElementosContext` auto-loads elements and locations when `isAuthenticated` becomes true. Pages don't need to trigger data loading.
- **Context hierarchy**: `AuthProvider` → `ElementosProvider` → `RouterProvider`. There is no `ApiProvider` — `src/lib/apiClient.js` exports a plain singleton, not a Context, since the Axios instance never needs to change identity and nothing besides `AuthContext` needs to mutate the token.
- **Element filtering is client-side**: `aplicarFiltros` in `ElementosContext` filters the already-loaded `elementos` array locally. The backend endpoints for fosiles/minerales/rocas only accept `pageSize` and `page` — they do not support filtering by país/provincia/localidad/subtipo server-side.
- **`AuthContext` owns the token, not `apiClient`**: `apiClient.js` keeps the current token in a module-level variable (read synchronously by the request interceptor — same effect as a `useRef`, without needing a Provider). `AuthContext.setAuthToken`/`clearToken` update both its own `token` state and `apiClient`'s module variable via `setApiToken`, so there is exactly one source of truth for "is there a session" (previously `token` lived in a separate `ApiContext` from `user`, and a 401 could clear the token without clearing `user`, leaving `ProtectedRoute` thinking the session was still valid — fixed by merging both into `AuthContext`). On 401, `apiClient` calls a callback registered via `onUnauthorized(fn)` (set once by `AuthContext` on mount) instead of importing `AuthContext` directly, avoiding a circular dependency.
- **Image loading via cache**: `FotoComponente` and `FotoModal` load images through `getImage(api, fotoId)` from `src/utils/imageCache.js`. The cache is module-level (survives component unmounts). Blob URLs are never revoked individually — use `invalidateImage(fotoId)` only after a photo is updated/deleted to force a fresh fetch. Never call `URL.revokeObjectURL` in component cleanup for these images.
- **ElementosContext `useRef` guard pattern**: `cargarTodosLosElementos` and `cargarUbicaciones` use `elementosLoadedRef`/`ubicacionesLoadedRef` (mirror loaded state) and `loadingElementosRef`/`loadingUbicacionesRef` (in-flight guards) so the loaded flags do NOT appear in `useCallback` deps. Without this, setting `elementosLoaded=true` would create a new callback reference → `useEffect` would re-fire → double API call cascade. The in-flight refs also prevent React 18 `<StrictMode>` double-effect from issuing duplicate fetches (refs persist across the simulated unmount/remount, unlike state). When adding a new auto-loading function to this context, follow this same pattern.

### Auth Flow

**Login** (`PageLogin` → `AuthContext.login` → `useAuthService.login` → `POST /api/auth/login`):
- `PageLogin` owns `useAuth()`, `useNavigate()`, and `error` state. It passes `onSubmit`, `loading`, and `error` as props to `CardLogin`. `CardLogin` is pure UI — it keeps only `showPassword` state and derives `isLocked` from the `error` prop (checks for "bloqueada" in the string).
- Identity lockout activates after 3 failed attempts (5-minute lockout). The backend checks `IsLockedOutAsync` before `CheckPasswordAsync`, calls `AccessFailedAsync` on failure, `ResetAccessFailedCountAsync` on success.
- When locked, `CardLogin` shows `Alert severity="warning"` with `LockOutlinedIcon` and disables all form fields and the submit button.

**Registration** (`PageRegister` → `AuthContext.register` → `useAuthService.register` → `POST /api/auth/register`):
- `PageRegister` owns `useAuth()`, `useNavigate()`, and `error` state. It builds the DTO (sets `nombreCompleto` only if non-empty) and passes `onSubmit`, `loading`, `error` as props to `CardRegister`. `CardRegister` is pure UI — it keeps only `showPassword` and `showConfirm` state.
- `register(userData)` in `AuthContext` posts to `/auth/register`, stores the returned JWT token via `setAuthToken`, sets user, and returns. On error it re-throws so `PageRegister` can set the error state.
- The backend always forces `Rol = RolUsuario.Free` regardless of what the client sends — the `Rol` field in `RegisterDto` is intentionally ignored in `AuthService.RegisterAsync`.
- No email confirmation required (`RequireConfirmedEmail = false`). On success the user is immediately authenticated and redirected to `/inicio`.
- Password rules (enforced client-side via Yup and server-side via Identity): min 6 chars, at least one digit, one lowercase, one uppercase. Non-alphanumeric not required.
- Route `/register` is inside `RootLayout` with no `ProtectedRoute`.

### Detail View Architecture

`PageDetails.jsx` applies the Provider + inner component pattern: exports `<DetailProvider><PageDetailsContent /></DetailProvider>`. `PageDetailsContent` calls `useDetail()`, handles two loading guards (spinner while fetching, `DetailErrorState` when element not found), then renders `<CardDetailElement {...detail} />`.

`CardDetailElement.jsx` is the layout orchestrator — receives all data as props, keeps only 3 `useState` for UI-only state (`modalOpen`, `currentPhotoIndex`, `informeDialogOpen`). Does NOT import `useElementos`, `useAuth`, `useParams`, or `useNavigate`.

**`DetailContext`** (`src/context/DetailContext.jsx`): Encapsulates fetch, edit state (flat `useState`), photo handlers, and element navigation. Consumed via `useDetail()`. Uses `useElementos()` — NOT `useElementosService` directly — because `actualizarElemento` in the context does HTTP AND keeps `elementos[]` in sync so the table/map stay current after an edit.

**Layout inside `CardDetailElement`:**

1. `DetailTopBar` — full width: Volver/Lista/Mapa buttons + `DetailNavigation` (prev/next)
2. `Paper`: `DetailHeader` (full width) → flex-wrap row of 5 info columns → `Divider` → `DetailGallery`
3. `DialogInformePetrografico` + `FotoModal` (outside the Paper, rendered as portals)

**5-column flex-wrap info row:**

| Column | Component | Flex basis | Content |
|--------|-----------|-----------|---------|
| General | `DetailInfoGeneral` | `1 1 160px` | subtipo, especie/litología, edad, donante, ejemplares |
| Temporal | `DetailInfoTemporal` | `1 1 140px` | fecha ingreso (editable), creación, actualización |
| Documentación | `DetailInfoDocumentacion` | `1 1 140px` | docs, lámina, "Solicitar Informe" button |
| Auditoría | `DetailInfoAuditoria` | `1 1 130px` | creado/actualizado/eliminado por — **Admin only** |
| Ubicación | `DetailInfoUbicacion` | `1 1 150px` | localidad, provincia, país, lat, lng |

In narrow viewports each column becomes full-width automatically via `flexWrap: 'wrap'`.

**Shared `InfoRow`**: exported from `DetailHelpers.jsx` with `labelWidth` prop (default `'38%'`). `DetailInfoUbicacion` passes `labelWidth="18%"` for its compact layout. No info component defines its own `InfoRow`.

**Audit section**: `DetailInfoAuditoria` is conditionally rendered — `{isAdmin && <DetailInfoAuditoria elemento={elemento} />}`. Receives only `elemento`. Shows `creadoPor`, `actualizadoPor`, `eliminadoPor` with dates. Backend: `GetByIdWithDetailsAsync` includes `UsuarioCreacion/Actualizacion/Eliminacion` navigation properties; mapped to `ElementoGeologicoDetailDto.CreadoPor/ActualizadoPor/EliminadoPor` via `?.NombreCompleto`.

**Gallery independence**: Photo edit/delete/restore buttons are always visible based on role permissions — the user does NOT need to enter element edit mode to manage photos. `DetailContext` keeps `fotos` as separate state, loaded via `cargarFotosDeElemento()` (role-aware endpoint), not from `elemento.galeria.fotos`.

**Unchanged components**: `DetailHeader`, `DetailNavigation`, `DetailGallery`, `FotoModal`, `DialogInformePetrografico` — no API or implementation changes.

- `src/utils/detailUtils.js` — Utility functions: `getTypeColor`, `getTypeLabel`, `getSubtipos`, `getSubtipoLabel(tipo, value)`, `getSubtipo(el)`, `getStatusColor`
- `DetailHelpers.jsx` — UI components only: `FotoComponente`, `InfoRow`
- `DialogInformePetrografico` — MUI Dialog (correo + observaciones) that POSTs to `POST /api/elementos-geologicos/{id}/solicitar-informe`. Pure UI component: receives `onSubmit` prop from `CardDetailElement` (which gets it from `useDetail()` → `DetailContext` → `ElementosContext` → `useElementosService`). Does NOT import any context. Accessible to all authenticated roles.

**Subtipo display**: Always use `getSubtipoLabel(tipo, value)` from `detailUtils.js` to render subtipo fields — the backend returns numeric enum values (0, 1, 2…), the helper maps them to Spanish labels via `getSubtipos()`. Use `getSubtipoDisplayLabel(key)` from `subtipoColors.js` when the source is the raw `tipoEspecifico` string (enum name from backend) — currently a pass-through with no overrides. Use `getSubtipoEntries(tipo)` from `subtipoColors.js` when you need `{ value: stringKey, label }` pairs keyed by `enum.ToString()` (e.g. for filtering by `tipoEspecifico`). Do NOT use `detailUtils.getSubtipos` for this — it returns numeric enum values for form/API use.

**Subtipo color system**: `src/constants/subtipoColors.js` is the single source of truth for colors. `getSubtipoColor(tipo, tipoEspecifico)` resolves accent-insensitive and hyphen-insensitive. Keys in `SUBTIPO_COLORS` must match `enum.ToString()` output exactly (e.g. `'PiroVolcanoclástica'`, not `'Piro-Volcanoclástica'`). `MarkerElement` icons and `DetailHeader` subtipo chips both use these hex colors. `ElementFilterLegend` derives its subtipo dropdown from `getSubtipoEntries(tipo)` (exported from `subtipoColors.js`), so adding a new subtipo only requires updating `subtipoColors.js` and the backend enum — all UI updates automatically.

### User Management Module (PageUsers.jsx)

Admin-only page at `/usuarios`. Architecture: `PageUsers` = `UsersProvider` + `UsersPageContent` (inner component). `UsersPageContent` calls `useUsers()` and `useAuth()`, handles the loading guard, and passes all data and handlers as props to `CardUserManager`. `CardUserManager` is pure UI — it keeps only local dialog/filter state and receives everything else as props.

```
components/users/
  CardUserManager.jsx        → Orchestrator: stats + filters + table + dialogs
  UserTable.jsx              → MUI DataGrid with columns: nombre, email, rol, estado, fecha
  UserFilters.jsx            → Search by name/email, rol select, estado toggle
  UserFormDialog.jsx         → Create/edit dialog (React Hook Form + Yup)
  ChangePasswordDialog.jsx   → Admin change-password dialog
  UserStatsCards.jsx         → 4 metric cards (total, activos, por rol)
```

`UsersContext` exposes: `usuarios`, `paginacion`, `estadisticas`, `cargarUsuarios(filtros?)`, `crearUsuario`, `actualizarUsuario`, `eliminarUsuario`, `reactivarUsuario`, `cambiarPassword`, `toggleEstado`, `cargarEstadisticas`. Uses the same `useRef` guard pattern as `ElementosContext` to prevent double-fetch in StrictMode.

### Map Views (PageMap.jsx)

Three map views toggled in PageMap via ToggleButtonGroup (Vista selector at `position: absolute, top:16, right:16` outside the Leaflet map container):
- **Normal** (`ElementsMapView`) — Element markers + ElementFilterLegend
- **Geological** (`GeologicalMapView`) — GIS polygons only + GeologicalLegend + CustomLayerControl. No element markers.
- **Combined** (`CombinedMapView`) — GIS polygons + element markers + both legends + CustomLayerControl

**`GeoMapShell`** (`components/mapa/geological/GeoMapShell.jsx`): Shared wrapper used by all three map views. Owns the `mounted` state (delays `MapContainer` initialization until the view has been active at least once — Leaflet must never initialize with `display:none`), loading/error guards, outer `Box` sx, and `FullscreenToggle`. Initialized with `useState(active)` so the default active view (`ElementsMapView`) starts mounted immediately with no loading flash; inactive views defer until first selection. `loading`/`error` props are optional (default `false`/`null`) — geological views pass them from `useGeologia()`, `ElementsMapView` omits them since `PageMap` handles those guards before rendering any view.

**Shared state in `PageMapContent`**: `layerVisibility` + `toggleLayer` are defined once in `PageMapContent` and passed as props to `GeologicalMapView` and `CombinedMapView` (both need them; previously duplicated). `filteredNormal` and `filteredCombined` are kept as **separate** states — one per view — because `ElementFilterLegend` exists in both `ElementsMapView` and `CombinedMapView` simultaneously, and a single shared state would cause both instances to overwrite each other's filter via `onFilteredChange`. Both are synced to `elementosAMostrar` via a single `useEffect`.

**CustomLayerControl** (`components/mapa/geological/CustomLayerControl.jsx`): MUI-based layer panel using `L.control({ position: 'topleft' })` + `createPortal`. Toggle button (LayersIcon) expands a Paper panel with Radio for base layer and Checkboxes for overlay visibility. Positioned `topleft` inside the map — separate from the Vista selector which is outside the map at `topright`.

**Layer interaction rules** (critical for pointer-event correctness):
- In `GeologicalMapView`: geology layers use `onEachGeologicalFeature` (hover opens popup — OK since no markers exist).
- In `CombinedMapView`: geology layers use `onEachGeologicalFeatureCombined` (click opens popup via `L.popup().setLatLng(e.latlng).setContent(html).openOn(map)`, hover only changes style + shows sticky tooltip). Click-based avoids geology popups stealing hover events from marker popups. Province and Ecuador layers have `interactive={false}` (SVG `pointer-events: none`) so they never intercept clicks at all.
- Panes: `geologia-pane: z-index 250`, `boundaries-pane: z-index 300`, markers at default 600, popups at default 700.

Map controls use Leaflet `L.control` + `createPortal` to render MUI components inside the map:
- `GeologicalLegend` (bottomright) — Formation visibility toggles with search
- `ElementFilterLegend` (bottomright) — Element filtering by type, provincia, subtipo, name search

**`onEachGeologicalFeatureCombined`** is defined in `GeologiaContext.jsx` alongside `onEachGeologicalFeature` and exposed via `useGeologia()`. It binds a sticky tooltip and hover style change. On click it opens a popup via `L.popup({ maxWidth: 300 }).setLatLng(e.latlng).setContent(GeologicalPopup(feature.properties)).openOn(e.target._map)` — manual `L.popup` (not `bindPopup`) ensures the click event isn't blocked by pane z-index differences.

## Data Model Hierarchy

### Geological Elements (TPH Inheritance)
```
EntidadAuditable (audit base: creation/update/deletion tracking)
└── ElementoGeologico (common fields: Nombre, Edad, Codigo, Ubicacion, Galeria)
    ├── Fosil (TipoFosil, Especie, Periodo)
    ├── Mineral (TipoMineral, Litologia)
    └── Roca (TipoRoca, Litologia)
```

### Location Hierarchy
```
Pais → Provincia → Ubicacion → ElementoGeologico
```

### Gallery System
```
ElementoGeologico ←→ GaleriaElementoGeologico (1:1) → FotoElemento (1:N)
```

Gallery is created during element creation (if photo is provided via `CreateElementoCompletoDto`), or lazily on first photo upload via `POST /api/foto-elementos/elemento/{elementoId}`. Location update fields (Localidad, Latitud, Longitud) are supported in `UpdateElementoGeologicoBaseDto` and processed by `UpdateUbicacionIfNeededAsync`.

`FotoElemento.EstadoActivo` uses soft-delete: `DELETE` sets `EstadoActivo = false`, `PATCH /{id}/restaurar` (Admin only) sets it back to `true`. `GET /api/foto-elementos/por-elemento/{elementoId}` returns role-aware results — Admin sees all photos (active + inactive), all other roles see only active ones.

## Role System

Four roles defined in `Constants/RolUsuario.cs`. **Admin is value 2 — do not change this** (existing users in DB).

| Rol | Valor | Ver elementos | Fotos | Crear | Editar | Eliminar | Gestionar usuarios |
|-----|-------|---------------|-------|-------|--------|----------|--------------------|
| Free | 0 | Sí | CON watermark | No | No | No | No |
| Premium | 1 | Sí | SIN watermark | No | No | No | No |
| Admin | 2 | Sí | SIN watermark | Sí | Sí | Sí | Sí |
| Invitado | 3 | Sí | SIN watermark | Sí | Sí | No | No |

All new registrations are forced to Free server-side. The only way to change a user's role is via the Admin user management panel (`/usuarios`).

### Backend authorization per action (`ElementosGeologicosController`)
- GET (list/view): `[Authorize(Roles = "Admin,Premium,Free,Invitado")]`
- POST (create) / PUT (update): `[Authorize(Roles = "Admin,Invitado")]`
- DELETE: `[Authorize(Roles = "Admin")]`
- `IsUserPremium()` in `FotoElementosController` includes Invitado (no watermark)

### Frontend permissions
`AuthContext` derives and exposes permission flags from `user.rol`:
```js
isAdmin, canCreate, canEdit, canDelete, canManageUsers
```
Use these flags in components — never compare `user.rol === 2` directly.

`ProtectedRoute` accepts `allowedRoles` prop (array of `ROLES.*` values). If not provided, only checks authentication:
- `/crear-elementos` → `[ROLES.ADMIN, ROLES.INVITADO]`
- `/carga-excel` → `[ROLES.ADMIN]`
- `/usuarios` → `[ROLES.ADMIN]`

Sidebar menu items "Crear", "Excel" and "Usuarios" are conditionally rendered via `canCreate` / `isAdmin`.

### Test users (development)
| Rol | Email | Password |
|-----|-------|----------|
| Admin | `admin@sistemageologico.com` | `Admin123!` |
| Free | `free@test.com` | `Test123!` |
| Premium | `premium@test.com` | `Test123!` |
| Invitado | `invitado@test.com` | `Test123!` |

## Key Patterns

- **Unified Controller**: All geological element types (Fosil, Mineral, Roca) use single `/api/elementos-geologicos` endpoint with type discriminator
- **Soft Delete**: `EstadoActivo` flag applies to elements, galleries, and photos. `EntidadAuditable` tracks creation/update/deletion metadata.
- **Dual Database**: SQL Server for main data/identity, PostgreSQL+PostGIS for GIS queries
- **JWT Authentication**: 600-minute token expiration (10 hours), zero clock skew. Configured via `appsettings.json` → `JwtSettings:ExpirationMinutes`.
- **Identity Lockout**: Configured in `Program.cs` — `MaxFailedAccessAttempts = 3`, `DefaultLockoutTimeSpan = 5 minutes`. Lockout is checked via `IAuthRepository.IsLockedOutAsync` before password verification; failures recorded via `AccessFailedAsync`; reset via `ResetAccessFailedCountAsync` on successful login. Do NOT use `SignInManager.PasswordSignInAsync` for JWT APIs — it relies on cookies. Use `UserManager` methods directly.
- **Base Class Pattern**: Services and repositories use base classes (BaseElementoGeologicoService, BaseElementoGeologicoRepository) with specific implementations inheriting from them
- **CORS**: Configured for `http://localhost:5173`, `http://localhost:5174`, `http://localhost:5175`, and `http://localhost:3000` — the extra ports tolerate Vite occupying an adjacent port when 5174 is busy
- **DI Registration**: All repositories and services registered as Scoped; `QmlColorService` and `EmailService` as Singleton
- **QmlColorService**: Parses a QGIS `.qml` XML file to extract formation color (RGB) and label mappings used by the GIS layer. Configured via `QmlColorFilePath` in `appsettings.json`. Registered as Singleton — loads the QML once on first use and caches it.
- **Email Service**: `IEmailService` / `EmailService` (MailKit 4.7.1.1) sends HTML email via SMTP. Config in `appsettings.json` → `EmailSettings` (SmtpHost, SmtpPort, UseSsl, UseStartTls, SenderEmail, SenderName, SenderPassword, DestinationEmail, DestinationName). Returns `bool`, never throws — SMTP errors are caught and logged. HTML template uses `$"""` raw string with `{{variable}}` interpolation syntax (avoids CSS brace conflict with `$"""`).
- **Map Float Panel**: `PageMap` uses `position: relative` container (`height: calc(100vh - 64px)`). Vista-toggle panel is `position: absolute, top:16, right:16` (Paper elevation=4) — outside the Leaflet container. Element count chip is `bottom:32, left:16` — avoids Leaflet zoom controls (top-left) and legends (bottom-right).
- **Map Filter as Leaflet Control**: `ElementFilterLegend` uses `L.control` + `createPortal` pattern (same as `GeologicalLegend` and `CustomLayerControl`) for element filtering on the map. Positioned at `bottomright`. Present in Normal and Combined views (not Geological-only view). All `Select` components inside map controls use `MenuProps={{ disablePortal: true }}` — this is required for dropdowns to be visible in browser fullscreen mode (MUI portals render in `document.body` which is outside the fullscreen layer).
- **`processLayer` GeoJSON postprocessing**: `processLayer(featureCollection, layerName)` is a module-level pure function in `GeologiaContext.jsx`. It injects a `layer` field into every `Feature` after each HTTP response. `getGeologicalStyle` reads `feature.layer` to apply different styles per layer type (`'geologia'`, `'provincias'`, `'ecuador'`). This avoids prop drilling and keeps the style function pure.
- **`elementosAMostrar` priority in PageMap**: `PageMapContent` computes `resultadosBusqueda.length > 0 ? resultadosBusqueda : elementos` and passes it as `elementos` to all map views. When there is an active search, search results take priority over the full element list. Map-internal element filtering uses `filteredNormal`/`filteredCombined` — separate per-view states — to avoid a two-instances-one-state conflict between the `ElementFilterLegend` components in `ElementsMapView` and `CombinedMapView`.
- **`VITE_USE_STUBS` env var**: When `VITE_USE_STUBS=true`, service hooks return local static data instead of making HTTP requests. Controlled entirely in the hook layer — no UI component changes needed. Useful for frontend development or demos without a running backend.
- **`TableElement` `showTypeFilter` prop**: Pass `showTypeFilter={false}` when the parent already controls type filtering (e.g., `HomePage` uses the `Filtros` component). Defaults to `true` (`PageTable`).
- **Watermark**: Applied in `FotoElementosController.ApplyWatermark()` using `new Bitmap(w, h, Format32bppArgb)` + `graphics.DrawImage(original, ...)`. Do NOT use `new Bitmap(image)` directly — it fails silently for palette/indexed images (GDI+ exception caught, original returned). Watermark text: "Museo Petrográfico Tomas Feininger".
- **Do NOT add `ThenInclude(g => g.Fotos)` to `BaseElementoGeologicoRepository.GetAllAsync`** — it loads all photo `byte[]` data for every element in the list, causing extreme slowness. Photos are loaded separately per element via the dedicated endpoint.
- **GIS SQL scripts at repo root** (`check_lengths.sql`, `copy_gis_data.sql`, `fix_and_copy_geologia.sql`, `migrate_gis.sql`) — one-time scripts used during the initial migration of geological formation data into the PostGIS database. They are not part of the EF migration pipeline and should not be run again.
- **SubtipoRoca enum values**: `Desconocido=0, Ígnea=1, Sedimentaria=2, Metamórfica=3, Meteorito=4, PiroVolcanoclástica=5`. Values 0-3 are stable. Do NOT revert values 4-5 to the old Piroclástica/Volcánoclástica — existing DB records use these integer values. Display name is `PiroVolcanoclástica` (no hyphen) — `getSubtipoDisplayLabel` is a pass-through with no overrides.
- **Excel import subtipo resolution**: `ExcelContext.jsx` uses module-level pure functions `lookupTipo(rawValue, map)` (tolerates accents, mixed case, and hyphens — e.g. "piro-volcanoclastica" → 5) and `mapRow` (reads column "Subtipo" as fallback before "Tipo"). Unrecognized values default to 0 (Desconocido). `PageExcel` applies the Provider + inner component pattern (`ExcelProvider` + `PageExcelContent`); `PageExcelContent` handles the DOM file event and calls `readExcelFile(file)` (accepts a `File` object, not a DOM event). `ExcelDataPreview` adds `tipoElemento` and `tipoEspecifico` fields to each row so `TableElement` can show type/subtipo chips in the preview table.
- **Frontend is untracked**: The entire `Cliente_Sistema_Geologia/` directory is excluded from git (`.gitignore` covers it). Run `pnpm install` and `pnpm dev` from `Cliente_Sistema_Geologia/cliente-sistema-geologico/` as normal — the exclusion is intentional for this repo.

## API Documentation

Swagger UI available at http://localhost:5095/swagger when backend is running.

## Naming Conventions

- **Database names**: `db_` prefix (e.g., `db_SistemaGeologico_Datos`, `db_SistemaGeologico_GIS`)
- **Domain entities**: Spanish (Usuario, Ubicacion, Pais, Provincia, ElementoGeologico)
- **Infrastructure**: English (Controllers, Services, Repositories)
- **DTOs**: Suffixed with purpose (CreateFosilDto, UpdateMineralDto, ElementoGeologicoResponseDto)
- **Interfaces**: Prefixed with I (IElementoGeologicoService, IPaisRepository)
- **Frontend context methods**: Spanish (crearElemento, actualizarElemento, obtenerElementoPorId, subirFotoAElemento)

## Key Technologies

**Backend**: ASP.NET Core 8.0, Entity Framework Core, Identity, JWT, NetTopologySuite, Npgsql, MailKit 4.7.1.1
**Frontend**: React 18, Vite, Material-UI v6, Leaflet/react-leaflet, React Hook Form, Axios, Yup, react-router-dom v7, xlsx

## Documentation

- `Servidor_Sistema_Geologia/docs/Arquitectura_Proyecto.md` — Full project architecture (index, links to detailed docs)
- `Servidor_Sistema_Geologia/docs/Arquitectura_Backend.md` — Backend layers, TPH, DTOs, auth, GIS, soft delete, DI
- `Servidor_Sistema_Geologia/docs/Arquitectura_Frontend.md` — Context API, hooks, JSDoc types, routes, component tree
- `Servidor_Sistema_Geologia/docs/db_RelacionesBaseDatos.md` — Database relationships and ER diagrams for both databases
- `Servidor_Sistema_Geologia/docs/Abstract_Frontend_TIC.md` — Project abstract (TIC/EPN academic context)
- `Servidor_Sistema_Geologia/docs/Arquitectura_Frontend_Build.md` — Vite vs Axios roles, Node.js scope, CORS dev/prod, deployment (dist/ only), Opción A/B production diagrams
