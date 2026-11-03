# DIA-08 · Entornos y despliegue

**Qué muestra:** los contenedores de desarrollo, el empaquetado de las
imágenes de producción y el camino de una petición en producción.

**Sale de:** `compose.yaml`, `compose.prod.yaml`, `Caddyfile`,
`docker/dev/*.Dockerfile`, `docker/prod/*`, `docker/db/init/` (commit
`8dfe171`).

**Texto que lo explica:** [ARQ-03](../arquitectura/stack-y-entornos.md) y
[DEP-01](../despliegue/vps-docker.md); índice [DIA-00](indice.md).

## Desarrollo (`compose.yaml`)

```mermaid
flowchart LR
  Dev["Navegador del desarrollador"]
  subgraph Host["Equipo: solo git y Docker"]
    Repo[("Repo montado en /app")]
    subgraph C["Compose: sistemageologico"]
      Web["web: Node 26 + pnpm, Vite 5174"]
      Api["api: SDK .NET 10, dotnet watch 5095"]
      Db[("db: PostGIS 18-3.6")]
    end
  end
  Dev -- "127.0.0.1:5174" --> Web
  Dev -- "127.0.0.1:5095/swagger" --> Api
  Web -- "proxy /api" --> Api
  Api --> Db
  Repo -.-> Web
  Repo -.-> Api
```

## Empaquetado de producción

```mermaid
flowchart LR
  subgraph A["docker/prod/api.Dockerfile"]
    A1["sdk:10.0.403: restore + publish"] --> A2["aspnet:10.0.14 + fonts-dejavu-core, usuario 1654"]
  end
  subgraph W["docker/prod/web.Dockerfile"]
    W1["node:26.13.0 + pnpm: pnpm build"] --> W2["nginx-unprivileged:1.29.8, usuario 101"]
  end
  A2 --> IA["sistemageologico-api:1.0.0"]
  W2 --> IW["sistemageologico-web:1.0.0"]
```

## Una petición en producción (`compose.prod.yaml`)

```mermaid
flowchart LR
  U["Navegador"] -- "HTTPS 443" --> Caddy["caddy: TLS, CSP, HSTS"]
  subgraph RedWeb["red web"]
    Caddy
    Web["web: Nginx 8080 (SPA)"]
    Api["api: ASP.NET 8080"]
  end
  subgraph RedInterna["red interna (sin salida)"]
    Db[("db: PostGIS — datos y gis")]
  end
  Caddy -- "/api/*" --> Api
  Caddy -- "resto" --> Web
  Api --> Db
  Api -- "SMTP" --> Smtp["Servidor de correo"]
  U -- "teselas" --> Osm["OpenStreetMap"]
```

La migración no corre al arrancar: es el paso explícito
`docker compose -f compose.prod.yaml run --rm api --migrate`.
