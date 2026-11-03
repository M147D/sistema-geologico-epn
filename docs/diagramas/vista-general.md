# DIA-01 · Vista general

**Qué muestra:** las piezas del sistema y cómo se comunican, en producción.

**Sale de:** `Servidor_Sistema_Geologia/Program.cs`, `Caddyfile`,
`compose.prod.yaml`, `src/lib/apiClient.js`,
`Services/Implementation/EmailService.cs`,
`Services/Implementation/ImageProcessingService.cs`,
`src/constants/mapConstants.js` (commit `8dfe171`).

**Texto que lo explica:** [ARQ-01](../arquitectura/diagrama.md); entornos en
[DIA-08](entornos.md); índice [DIA-00](indice.md).

```mermaid
flowchart LR
  U["Navegador (Admin, Invitado, Premium, Free)"]
  Caddy["Caddy: HTTPS y encabezados"]
  subgraph Front["web: SPA React (Nginx)"]
    SPA["Páginas, contextos y hooks"]
    AX["apiClient (Axios + JWT en localStorage)"]
  end
  subgraph Back["api: ASP.NET Core 10"]
    CTRL["Controllers"]
    SRV["Services"]
    REPO["Repositories"]
    IMG["ImageProcessingService (SkiaSharp)"]
    QML["QmlColorService (GIS/*.qml)"]
  end
  subgraph PG["db: PostgreSQL 18 + PostGIS"]
    D1[("sistemageologico_datos: identidad, catálogo, fotos")]
    D2[("sistemageologico_gis: capas")]
  end
  SMTP["Servidor SMTP"]
  OSM["Teselas de OpenStreetMap"]

  U --> Caddy
  Caddy -- "/" --> SPA
  SPA --> AX
  AX -- "/api" --> Caddy
  Caddy -- "/api/*" --> CTRL
  CTRL --> SRV --> REPO
  CTRL --> IMG
  SRV --> QML
  REPO --> D1
  REPO --> D2
  SRV -- "solicitud de informe" --> SMTP
  U -- "mapa base" --> OSM
```

En desarrollo, el proxy de Vite ocupa el lugar de Caddy (DIA-08).
