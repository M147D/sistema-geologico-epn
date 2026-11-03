#!/usr/bin/env bash
# ART-02 · Verifica por la API el cierre de los hallazgos de REV-01:
#   H-06 coordenadas numéricas con rango, H-05 país derivado de la provincia,
#   H-03 sin ids literales, H-08 alta atómica, H-14 carga por lotes todo-o-nada.
# Uso:
#   ADMIN_EMAIL=... ADMIN_PASSWORD=... scripts/artefactos/ART-02-verificacion-rev01.sh [URL_BASE]
# URL_BASE: http://127.0.0.1:5174 (desarrollo) o https://<dominio>. INSECURE=1 para la CA interna de Caddy.
# ESCRIBE en la base: crea minerales de prueba (códigos con sufijo único) y los desactiva al final.
# Requiere: bash, curl, python3. Se puede repetir.
set -euo pipefail

BASE="${1:-http://127.0.0.1:5174}"
: "${ADMIN_EMAIL:?falta ADMIN_EMAIL}"
: "${ADMIN_PASSWORD:?falta ADMIN_PASSWORD}"
TMP="$(mktemp -d)"; trap 'rm -rf "$TMP"' EXIT
S="$(date +%s)"; fallos=0; creados=()

ok()    { printf '  ok    %s\n' "$1"; }
falla() { printf '  FALLA %s\n' "$1"; fallos=$((fallos + 1)); }
j()     { python3 -I -c "import json,sys; d=json.load(open('$TMP/body')); print($1)"; }
req()   { local m="$1" r="$2"; shift 2
          curl -s ${INSECURE:+-k} -o "$TMP/body" -w '%{http_code}' -X "$m" "$BASE$r" \
            -H "Authorization: Bearer $TOKEN" -H 'Content-Type: application/json' "$@"; }
mineral() { # codigo pais provincia latitud longitud
  printf '{"nombre":"Mineral prueba","edad":"10 m","donante":"Prueba","fechaIngreso":"2026-11-03","codigo":"%s","ejemplares":1,"laminaExiste":false,"tipoMineral":1,"litologia":"Cuarzo","ubicacion":{"nombrePais":"%s","nombreProvincia":"%s","localidad":"Prueba","latitud":"%s","longitud":"%s","leyenda":"Prueba"}}' "$@"; }

echo "Verificación de REV-01 contra $BASE"
TOKEN=$(curl -s ${INSECURE:+-k} -X POST "$BASE/api/auth/login" -H 'Content-Type: application/json' \
  -d "{\"email\":\"$ADMIN_EMAIL\",\"password\":\"$ADMIN_PASSWORD\"}" | python3 -I -c 'import json,sys; print(json.load(sys.stdin)["token"])')

# H-06: coordenadas
code=$(req POST /api/elementos-geologicos/minerales -d "$(mineral "R1A$S" Ecuador Pichincha '' '')")
[ "$code" = 201 ] && [ "$(j 'd["ubicacion"]["latitud"]')" = None ] && ok "H-06 coordenadas vacías -> null" || falla "H-06 vacías: HTTP $code"
id1=$(j 'd["id"]'); creados+=("$id1")
code=$(req POST /api/elementos-geologicos/minerales -d "$(mineral "R1B$S" Ecuador Pichincha '-0,5' '-78.5')")
[ "$code" = 201 ] && [ "$(j 'd["ubicacion"]["latitud"]')" = "-0.5" ] && ok "H-06 coma decimal aceptada y guardada como número" || falla "H-06 coma: HTTP $code"
creados+=("$(j 'd["id"]')")
for mal in 'abc' '95'; do
  code=$(req POST /api/elementos-geologicos/minerales -d "$(mineral "R1X$S" Ecuador Pichincha "$mal" '0')")
  [ "$code" = 400 ] && ok "H-06 latitud '$mal' rechazada (400)" || falla "H-06 latitud '$mal': HTTP $code"
done

# H-05 / H-03: país de la provincia; sin provincia -> 'Desconocida' de ese país
code=$(req POST /api/elementos-geologicos/minerales -d "$(mineral "R1C$S" "Peru$S" '' '-12' '-77')")
idc=$(j 'd["id"]'); creados+=("$idc")
if [ "$code" = 201 ] && [ "$(j 'd["ubicacion"]["nombrePais"]')" = "Peru$S" ] && [ "$(j 'd["ubicacion"]["nombreProvincia"]')" = Desconocida ]; then
  ok "H-05 sin provincia: provincia 'Desconocida' del mismo país"
else falla "H-05 provincia por defecto: HTTP $code $(cat "$TMP/body" | head -c 200)"; fi
req GET "/api/elementos-geologicos/minerales?pageSize=10000" >/dev/null
lista=$(j "[e.get('nombrePais') for e in (d if isinstance(d,list) else (d.get('data') or d.get('elementosGeologicos') or [])) if e.get('id')==$idc]")
case "$lista" in *"Peru$S"*) ok "H-05 el listado muestra el mismo país que el detalle";; *) falla "H-05 listado: $lista";; esac

# H-08: alta atómica — ubicación inexistente no deja filas a medias
code=$(req POST /api/elementos-geologicos/minerales -d "$(mineral "R1D$S" Ecuador Pichincha '0' '0' | sed 's/"ubicacion":/"ubicacionId":999999,"ubicacion":/')")
code2=$(req GET "/api/elementos-geologicos/by-codigo/R1D$S")
[ "$code" != 201 ] && [ "$code2" = 404 ] && ok "H-08 alta fallida no deja el elemento creado ($code, by-codigo $code2)" || falla "H-08: alta $code, by-codigo $code2"

# H-14: lote todo-o-nada
lote() { printf '{"filas":[%s]}' "$1"; }
code=$(req POST /api/elementos-geologicos/lote/minerales -d "$(lote "$(mineral "R1E$S" Ecuador Azuay '-2.9' '-79'),$(mineral "R1F$S" Ecuador Azuay '' ''),$(mineral "R1A$S" Ecuador Pichincha '' '')")")
if [ "$code" = 200 ] && [ "$(j 'd["creados"]')" = 2 ] && [ "$(j 'd["actualizados"]')" = 1 ]; then
  ok "H-14 lote válido: 2 creados, 1 actualizado (código existente)"
else falla "H-14 lote válido: HTTP $code $(head -c 300 "$TMP/body")"; fi
for c in "R1E$S" "R1F$S"; do req GET "/api/elementos-geologicos/by-codigo/$c" >/dev/null; creados+=("$(j 'd.get("id") or d.get("data",{}).get("id")')"); done
# borrar uno y mandar un lote que lo incluya: debe fallar completo
req DELETE "/api/elementos-geologicos/$id1" >/dev/null
code=$(req POST /api/elementos-geologicos/lote/minerales -d "$(lote "$(mineral "R1G$S" Ecuador Azuay '' ''),$(mineral "R1A$S" Ecuador Pichincha '' '')")")
code2=$(req GET "/api/elementos-geologicos/by-codigo/R1G$S")
[ "$code" = 400 ] && [ "$code2" = 404 ] && ok "H-14 lote con una fila inválida: 400 y no se guardó ninguna fila" || falla "H-14 todo-o-nada: lote $code, by-codigo R1G $code2"
code=$(req POST /api/elementos-geologicos/lote/minerales -d "$(lote "$(mineral "R1H$S" Ecuador Azuay '' ''),$(mineral "R1I$S" Ecuador Azuay '200' '')")")
[ "$code" = 400 ] && [ "$(req GET "/api/elementos-geologicos/by-codigo/R1H$S")" = 404 ] && ok "H-14 validación de modelo por fila: 400 y nada guardado" || falla "H-14 validación de modelo: HTTP $code"

# Limpieza
for id in "${creados[@]}"; do req DELETE "/api/elementos-geologicos/$id" >/dev/null || true; done
echo
[ "$fallos" = 0 ] && echo "Verificación REV-01: OK" || { echo "Verificación REV-01: $fallos falla(s)"; exit 1; }
