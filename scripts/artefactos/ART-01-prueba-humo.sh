#!/usr/bin/env bash
# ART-01 · Prueba de humo de extremo a extremo contra un despliegue en marcha.
# Uso:
#   ADMIN_EMAIL=... ADMIN_PASSWORD=... scripts/artefactos/ART-01-prueba-humo.sh [URL_BASE]
# URL_BASE: http://127.0.0.1:5174 (desarrollo, proxy de Vite) o https://<dominio> (Caddy).
# ESCRIBE en la base: crea un usuario Free y una roca con una foto, y al final
# los desactiva (borrado lógico). No usar en producción salvo a propósito.
# INSECURE=1 acepta el certificado de la CA interna de Caddy (DOMINIO=localhost).
# Requiere: bash, curl, python3. Se puede repetir: cada corrida usa códigos nuevos.
set -euo pipefail

BASE="${1:-http://127.0.0.1:5174}"
: "${ADMIN_EMAIL:?falta ADMIN_EMAIL}"
: "${ADMIN_PASSWORD:?falta ADMIN_PASSWORD}"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT
SUFIJO="$(date +%s)"
fallos=0

ok()    { printf '  ok    %s\n' "$1"; }
falla() { printf '  FALLA %s\n' "$1"; fallos=$((fallos + 1)); }
json()  { python3 -I -c "import json,sys; d=json.load(sys.stdin); print($1)"; }
# status METODO RUTA TOKEN [args de curl...] → imprime el código HTTP, cuerpo en $TMP/body
status() {
  local metodo="$1" ruta="$2" token="$3"; shift 3
  curl -s ${INSECURE:+-k} -o "$TMP/body" -w '%{http_code}' -X "$metodo" "$BASE$ruta" \
    ${token:+-H "Authorization: Bearer $token"} "$@"
}

echo "Prueba de humo contra $BASE"

# 1. Login del administrador
code=$(status POST /api/auth/login "" -H 'Content-Type: application/json' \
  -d "{\"email\":\"$ADMIN_EMAIL\",\"password\":\"$ADMIN_PASSWORD\"}")
[ "$code" = 200 ] || { echo "login admin: HTTP $code"; cat "$TMP/body"; exit 1; }
ADMIN_TOKEN=$(json 'd["token"]' < "$TMP/body"); ok "login admin"

# 2. Registro de un usuario Free (el rol lo fuerza el servidor)
FREE_EMAIL="humo$SUFIJO@example.com"
code=$(status POST /api/auth/register "" -H 'Content-Type: application/json' \
  -d "{\"email\":\"$FREE_EMAIL\",\"password\":\"Prueba${SUFIJO}a\",\"confirmPassword\":\"Prueba${SUFIJO}a\",\"rol\":2}")
if [ "$code" = 200 ]; then
  FREE_TOKEN=$(json 'd["token"]' < "$TMP/body"); FREE_ID=$(json 'd["user"]["id"]' < "$TMP/body")
  rol=$(json 'd["user"]["rol"]' < "$TMP/body")
  [ "$rol" = 0 ] && ok "registro Free (pidió rol 2, recibió $rol)" || falla "registro: rol $rol en vez de 0"
else
  echo "registro: HTTP $code"; cat "$TMP/body"; exit 1
fi

# 3. Un Free no puede crear elementos
code=$(status POST /api/elementos-geologicos/rocas "$FREE_TOKEN" -H 'Content-Type: application/json' -d '{}')
[ "$code" = 403 ] && ok "Free no crea elementos (403)" || falla "Free al crear: HTTP $code"

# 4. Alta de una roca (crea ubicación y galería)
code=$(status POST /api/elementos-geologicos/rocas "$ADMIN_TOKEN" -H 'Content-Type: application/json' -d "{
  \"nombre\":\"Roca de prueba\",\"edad\":\"10 m\",\"donante\":\"Prueba de humo\",
  \"fechaIngreso\":\"2026-11-03\",\"codigo\":\"HUMO$SUFIJO\",\"ejemplares\":1,
  \"laminaExiste\":false,\"tipoRoca\":1,\"litologia\":\"Andesita\",
  \"ubicacion\":{\"nombrePais\":\"Ecuador\",\"nombreProvincia\":\"Pichincha\",\"localidad\":\"Quito\",
                 \"latitud\":\"-0.2102\",\"longitud\":\"-78.4890\",\"leyenda\":\"Prueba\"}}")
if [ "$code" = 201 ] || [ "$code" = 200 ]; then
  ELEMENTO_ID=$(json 'd.get("id") or d.get("data",{}).get("id")' < "$TMP/body"); ok "alta de roca (id $ELEMENTO_ID)"
else
  echo "alta de roca: HTTP $code"; cat "$TMP/body"; exit 1
fi

# 5. Foto: un PNG de 800×600 generado aquí
python3 -I - "$TMP/foto.png" <<'PY'
import struct, sys, zlib
w, h = 800, 600
filas = b"".join(b"\x00" + b"".join(bytes((x * 255 // w, y * 255 // h, 128)) for x in range(w)) for y in range(h))
def chunk(t, d): return struct.pack(">I", len(d)) + t + d + struct.pack(">I", zlib.crc32(t + d) & 0xffffffff)
png = b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 2, 0, 0, 0)) + chunk(b"IDAT", zlib.compress(filas)) + chunk(b"IEND", b"")
open(sys.argv[1], "wb").write(png)
PY
code=$(status POST "/api/foto-elementos/elemento/$ELEMENTO_ID" "$ADMIN_TOKEN" \
  -F "ImagenFile=@$TMP/foto.png;type=image/png" -F "TipoFoto=2" -F "DescripcionEspecifica=Prueba de humo")
if [ "$code" = 201 ] || [ "$code" = 200 ]; then
  FOTO_ID=$(json 'd["id"]' < "$TMP/body"); ok "subida de foto (id $FOTO_ID)"
else
  echo "subida de foto: HTTP $code"; cat "$TMP/body"; exit 1
fi

# 6. Marca de agua por rol: el Free recibe una imagen distinta de la del Admin
status GET "/api/foto-elementos/imagen/$FOTO_ID" "$ADMIN_TOKEN" >/dev/null; cp "$TMP/body" "$TMP/admin.img"
code=$(status GET "/api/foto-elementos/imagen/$FOTO_ID" "$FREE_TOKEN"); cp "$TMP/body" "$TMP/free.img"
if [ "$code" = 200 ] && ! cmp -s "$TMP/admin.img" "$TMP/free.img" && cmp -s "$TMP/admin.img" "$TMP/foto.png"; then
  ok "Admin recibe la original; Free recibe otra imagen (con marca)"
else
  falla "marca de agua: HTTP $code o imágenes iguales"
fi
code=$(status GET "/api/foto-elementos/imagen/$FOTO_ID?thumb=true" "$FREE_TOKEN")
tam=$(wc -c < "$TMP/body"); libre=$(wc -c < "$TMP/free.img")
[ "$code" = 200 ] && [ "$tam" -lt "$libre" ] && ok "miniatura más liviana ($tam < $libre bytes)" || falla "miniatura: HTTP $code, $tam bytes"

# 7. Capas GIS (vacías hasta cargar los datos del IIGE)
for capa in ecuador provincias "geologia/simplified?tolerance=0.01"; do
  code=$(status GET "/api/GeologiaGIS/$capa" "$FREE_TOKEN")
  [ "$code" = 200 ] && ok "GIS $capa (HTTP 200)" || falla "GIS $capa: HTTP $code"
done

# 8. Sin token: 401
code=$(status GET /api/elementos-geologicos "")
[ "$code" = 401 ] && ok "sin token: 401" || falla "sin token: HTTP $code"

# 9. Limpieza (borrado lógico)
[ "$(status DELETE "/api/elementos-geologicos/$ELEMENTO_ID" "$ADMIN_TOKEN")" = 200 ] && ok "roca desactivada" || falla "no se pudo desactivar la roca"
[ "$(status DELETE "/api/users/$FREE_ID" "$ADMIN_TOKEN")" = 200 ] && ok "usuario Free desactivado" || falla "no se pudo desactivar el usuario"

echo
[ "$fallos" = 0 ] && echo "Prueba de humo: OK" || { echo "Prueba de humo: $fallos falla(s)"; exit 1; }
