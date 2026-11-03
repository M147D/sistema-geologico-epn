# Desarrollo del backend: SDK completo, el código se monta (no se copia).
FROM mcr.microsoft.com/dotnet/sdk:10.0.403

# Fuente para la marca de agua (ImageProcessingService).
RUN apt-get update \
 && apt-get install -y --no-install-recommends fonts-dejavu-core \
 && rm -rf /var/lib/apt/lists/*

# dotnet-ef para crear migraciones, instalado fuera de HOME para cualquier UID_HOST.
ENV DOTNET_TOOLS=/opt/dotnet-tools
RUN dotnet tool install dotnet-ef --version 10.0.14 --tool-path "$DOTNET_TOOLS"
ENV PATH="${DOTNET_TOOLS}:${PATH}"

# Caché de NuGet en un volumen; 1777 para que sirva a cualquier UID_HOST.
ENV NUGET_PACKAGES=/nuget
RUN mkdir -p /nuget && chmod 1777 /nuget

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    DOTNET_USE_POLLING_FILE_WATCHER=1
