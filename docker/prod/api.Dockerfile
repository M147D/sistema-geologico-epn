# Producción del backend. Contexto: Servidor_Sistema_Geologia/
FROM mcr.microsoft.com/dotnet/sdk:10.0.403 AS build
WORKDIR /src
COPY Servidor_Sistema_Geologia.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /out --no-restore /p:UseAppHost=false

# Runtime: sin SDK ni fuentes del código.
FROM mcr.microsoft.com/dotnet/aspnet:10.0.14
# Fuente para la marca de agua (ImageProcessingService).
RUN apt-get update \
 && apt-get install -y --no-install-recommends fonts-dejavu-core \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
# Usuario sin privilegios que trae la imagen (uid 1654); escucha en 8080.
USER $APP_UID
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --retries=3 --start-period=30s --start-interval=2s \
  CMD ["dotnet", "Servidor_Sistema_Geologia.dll", "--healthcheck"]
# Migraciones (paso explícito): docker compose -f compose.prod.yaml run --rm api --migrate
ENTRYPOINT ["dotnet", "Servidor_Sistema_Geologia.dll"]
