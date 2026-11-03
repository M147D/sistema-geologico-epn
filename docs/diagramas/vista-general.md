# DIA-01 · Vista general

**Qué muestra:** las piezas del sistema y cómo se comunican hoy, en desarrollo
local.

**Sale de:** `Servidor_Sistema_Geologia/Program.cs`,
`Cliente_Sistema_Geologia/cliente-sistema-geologico/vite.config.js`,
`src/lib/apiClient.js`, `Services/Implementation/EmailService.cs`,
`src/constants/mapConstants.js`.

**Texto que lo explica:** [ARQ-01](../arquitectura/diagrama.md); índice
[DIA-00](indice.md).

```mermaid
flowchart LR
  U["Navegador (Admin, Invitado, Premium, Free)"]
  subgraph Front["Frontend: SPA React + Vite (5174)"]
    SPA["Páginas, contextos y hooks"]
    AX["apiClient (Axios + JWT en localStorage)"]
  end
  subgraph Back["Backend: ASP.NET Core 8 (5095)"]
    CTRL["Controllers"]
    SRV["Services"]
    REPO["Repositories"]
    IMG["Miniatura y marca de agua (GDI+, solo Windows)"]
    QML["QmlColorService (GIS/*.qml)"]
  end
  SQL[("SQL Server: identidad, catálogo y fotos")]
  PG[("PostgreSQL + PostGIS: capas geológicas")]
  SMTP["Servidor SMTP"]
  OSM["Teselas de OpenStreetMap"]

  U --> SPA --> AX
  AX -- "/api (proxy de Vite)" --> CTRL
  CTRL --> SRV --> REPO
  CTRL --> IMG
  SRV --> QML
  REPO --> SQL
  REPO --> PG
  SRV -- "solicitud de informe" --> SMTP
  U -- "mapa base" --> OSM
```
