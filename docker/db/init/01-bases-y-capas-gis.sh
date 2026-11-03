#!/bin/sh
# Se ejecuta UNA vez, al crear el volumen de datos (docker-entrypoint-initdb.d).
# Crea el usuario de la aplicación, las dos bases y las capas GIS vacías con el
# esquema que espera Models/GIS/ (PEN-04: los datos del IIGE se cargan aparte).
set -eu

: "${APP_DB_USER:?falta APP_DB_USER}"
: "${APP_DB_PASSWORD:?falta APP_DB_PASSWORD}"
: "${DATOS_DB:?falta DATOS_DB}"
: "${GIS_DB:?falta GIS_DB}"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname postgres \
  -v app_user="$APP_DB_USER" -v app_password="$APP_DB_PASSWORD" \
  -v datos_db="$DATOS_DB" -v gis_db="$GIS_DB" <<'SQL'
CREATE ROLE :"app_user" LOGIN PASSWORD :'app_password';
-- Catálogo e identidad: la aplicación es dueña (aplica las migraciones).
CREATE DATABASE :"datos_db" OWNER :"app_user";
-- Capas GIS: las crea el administrador; la aplicación solo lee.
CREATE DATABASE :"gis_db";
REVOKE ALL ON DATABASE :"datos_db" FROM PUBLIC;
REVOKE ALL ON DATABASE :"gis_db" FROM PUBLIC;
GRANT CONNECT ON DATABASE :"datos_db" TO :"app_user";
GRANT CONNECT ON DATABASE :"gis_db" TO :"app_user";
SQL

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$GIS_DB" \
  -v app_user="$APP_DB_USER" <<'SQL'
CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE IF NOT EXISTS public.tb_geologia (
  id          serial PRIMARY KEY,
  geom        geometry(Geometry, 4326),
  cod_a       varchar(50),
  "Edad"      varchar(200),
  "Litologia" varchar(500)
);
CREATE INDEX IF NOT EXISTS idx_geologia_geom ON public.tb_geologia USING gist (geom);

CREATE TABLE IF NOT EXISTS public.tb_provincias (
  id          serial PRIMARY KEY,
  geom        geometry(Geometry, 4326),
  dpa_codpro  varchar(10),
  dpa_despro  varchar(255),
  area_km2    double precision,
  poblacion   integer
);
CREATE INDEX IF NOT EXISTS idx_provincias_geom ON public.tb_provincias USING gist (geom);

CREATE TABLE IF NOT EXISTS public.tb_ecuador (
  id          serial PRIMARY KEY,
  geom        geometry(Geometry, 4326),
  pais        varchar(255),
  iso_code    varchar(3),
  area_km2    double precision
);
CREATE INDEX IF NOT EXISTS idx_ecuador_geom ON public.tb_ecuador USING gist (geom);

REVOKE CREATE ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO :"app_user";
GRANT SELECT ON ALL TABLES IN SCHEMA public TO :"app_user";
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO :"app_user";
SQL
