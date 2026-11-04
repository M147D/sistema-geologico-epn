#!/usr/bin/env python3
"""ART-03 · Regresión de los hallazgos de seguridad corregidos (PEN-12).

Uso:
  ADMIN_EMAIL=... ADMIN_PASSWORD=... python3 -I scripts/artefactos/ART-03-regresion-seguridad.py [URL_BASE]
URL_BASE: http://127.0.0.1:5174 (desarrollo) o https://<dominio>. INSECURE=1 para la CA interna de Caddy.

ESCRIBE en la base: crea usuarios y elementos de prueba (sufijo único) y los desactiva.
La última prueba agota a propósito el límite de login de la IP de quien la ejecuta:
espere 1 minuto antes de volver a iniciar sesión desde esa IP.
Solo biblioteca estándar. Se puede repetir.
"""
import json, os, ssl, struct, sys, threading, time, urllib.request, urllib.error, zlib

BASE = (sys.argv[1] if len(sys.argv) > 1 else "http://127.0.0.1:5174").rstrip("/")
ADMIN = (os.environ["ADMIN_EMAIL"], os.environ["ADMIN_PASSWORD"])
CTX = ssl._create_unverified_context() if os.environ.get("INSECURE") else None
S = str(int(time.time()))
fallos = 0


def req(metodo, ruta, token=None, body=None, multipart=None):
    headers = {}
    data = None
    if token:
        headers["Authorization"] = f"Bearer {token}"
    if body is not None:
        data = json.dumps(body).encode()
        headers["Content-Type"] = "application/json"
    if multipart is not None:
        b = "----art03" + S
        partes = []
        for k, v in multipart.items():
            if isinstance(v, bytes):
                partes.append(f'--{b}\r\nContent-Disposition: form-data; name="{k}"; filename="f.png"\r\nContent-Type: image/png\r\n\r\n'.encode() + v + b"\r\n")
            else:
                partes.append(f'--{b}\r\nContent-Disposition: form-data; name="{k}"\r\n\r\n{v}\r\n'.encode())
        data = b"".join(partes) + f"--{b}--\r\n".encode()
        headers["Content-Type"] = f"multipart/form-data; boundary={b}"
    r = urllib.request.Request(BASE + ruta, data=data, method=metodo, headers=headers)
    t0 = time.time()
    try:
        with urllib.request.urlopen(r, context=CTX, timeout=60) as resp:
            cuerpo = resp.read()
            code = resp.status
    except urllib.error.HTTPError as e:
        cuerpo, code = e.read(), e.code
    try:
        js = json.loads(cuerpo)
    except Exception:
        js = None
    return code, js, time.time() - t0


def check(cond, texto):
    global fallos
    print(("  ok    " if cond else "  FALLA ") + texto)
    if not cond:
        fallos += 1


def login(email, pw):
    code, js, _ = req("POST", "/api/auth/login", body={"email": email, "password": pw})
    return js.get("token") if code == 200 and js else None


def png(w, h, payload=True):
    def chunk(t, d):
        return struct.pack(">I", len(d)) + t + d + struct.pack(">I", zlib.crc32(t + d) & 0xFFFFFFFF)
    ihdr = chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 2, 0, 0, 0))
    idat = chunk(b"IDAT", zlib.compress(b"".join(b"\x00" + b"\x80\x40\x20" * w for _ in range(h)))) if payload else b""
    return b"\x89PNG\r\n\x1a\n" + ihdr + idat + chunk(b"IEND", b"")


def mineral(codigo):
    return {"nombre": "Mineral seguridad", "edad": "1 m", "donante": "Prueba", "fechaIngreso": "2026-11-03",
            "codigo": codigo, "ejemplares": 1, "tipoMineral": 1, "litologia": "Cuarzo",
            "ubicacion": {"nombrePais": "Ecuador", "localidad": "Quito", "latitud": "-0.2", "longitud": "-78.5"}}


print(f"Regresión de seguridad contra {BASE}")
# Si una corrida anterior agotó el límite de login de esta IP, esperar a que se libere.
for _ in range(15):
    adm = login(*ADMIN)
    if adm:
        break
    time.sleep(5)
if not adm:
    sys.exit("no se pudo iniciar sesión como administrador")

# 2. Regex de validación acotada (medio) — primero, con cuota de IP disponible
code, _, dur = req("POST", "/api/auth/register", body={"email": "x@x.co", "password": "<a" * 32000, "confirmPassword": "x"})
check(code == 400 and dur < 1.0, f"contraseña de 64 KB rechazada con 400 en {dur*1000:.0f} ms (antes ~10 s) [HTTP {code}]")


def crear_usuario(nombre, rol):
    email = f"{nombre}{S}@example.com"
    code, js, _ = req("POST", "/api/users", adm, {"email": email, "userName": f"{nombre}{S}", "password": "Prueba123a",
                                                  "confirmPassword": "Prueba123a", "nombreCompleto": "Prueba", "rol": rol,
                                                  "estadoActivo": True, "emailConfirmed": True})
    uid = ((js or {}).get("user") or {}).get("id")
    return email, uid


# 1. Revocación de sesiones (medio)
e2, id2 = crear_usuario("adm", 2)
t2 = login(e2, "Prueba123a")
check(req("GET", "/api/users/stats", t2)[0] == 200, "token de Admin válido antes de revocar")
req("PUT", f"/api/users/{id2}", adm, {"email": e2, "userName": f"adm{S}", "nombreCompleto": "Prueba", "rol": 0,
                                       "estadoActivo": True, "emailConfirmed": True})
check(req("GET", "/api/users/stats", t2)[0] == 401, "degradar a un Admin invalida su token al instante (401)")
ef, idf = crear_usuario("free", 0)
tf = login(ef, "Prueba123a")
req("POST", "/api/auth/logout", tf)
check(req("GET", "/api/auth/me", tf)[0] == 401, "tras logout el token deja de valer (401)")
tf = login(ef, "Prueba123a")
req("POST", f"/api/users/{idf}/change-password", adm, {"newPassword": "Prueba456b", "confirmPassword": "Prueba456b"})
check(req("GET", "/api/elementos-geologicos/minerales?pageSize=1", tf)[0] == 401, "cambiar la contraseña invalida el token (401)")
tf = login(ef, "Prueba456b")


# 3. Fotos borradas no visibles fuera de Admin (bajo)
code, el, _ = req("POST", "/api/elementos-geologicos/minerales", adm, mineral(f"SEG1{S}"))
eid = el["id"]
code, foto, _ = req("POST", f"/api/foto-elementos/elemento/{eid}", adm, multipart={"ImagenFile": png(40, 30), "TipoFoto": "2", "DescripcionEspecifica": "seguridad"})
fid = foto["id"]
gid = foto["galeriaElementosGeologicoId"]
req("DELETE", f"/api/foto-elementos/{fid}", adm)
check(req("GET", f"/api/foto-elementos/imagen/{fid}", tf)[0] == 404, "foto borrada: imagen/{id} da 404 a un Free")
check(fid not in [f["id"] for f in (req("GET", f"/api/foto-elementos/galeria/{gid}", tf)[1] or [])], "foto borrada: no aparece en galeria/{id}")
det = req("GET", f"/api/elementos-geologicos/{eid}", tf)[1] or {}
check(fid not in [f["id"] for f in ((det.get("galeria") or {}).get("fotos") or [])], "foto borrada: no aparece en el detalle del elemento")
check(req("GET", f"/api/foto-elementos/imagen/{fid}", adm)[0] == 200, "el Admin sigue viendo la foto borrada")

# 4. Validación de imágenes (por validar -> corregido)
code, _, _ = req("POST", f"/api/foto-elementos/elemento/{eid}", adm, multipart={"ImagenFile": png(20000, 20000, payload=False), "TipoFoto": "2", "DescripcionEspecifica": "grande"})
check(code == 400, f"PNG que declara 20000x20000 rechazado al subir ({code})")
code, _, _ = req("POST", f"/api/foto-elementos/elemento/{eid}", adm, multipart={"ImagenFile": b"no soy una imagen", "TipoFoto": "2", "DescripcionEspecifica": "texto"})
check(code == 400, f"archivo que no es imagen rechazado al subir ({code})")

# 5. Solicitud de informe (bajo)
code, el2, _ = req("POST", "/api/elementos-geologicos/minerales", adm, mineral(f"SEG2{S}"))
req("DELETE", f"/api/elementos-geologicos/{el2['id']}", adm)
check(req("POST", f"/api/elementos-geologicos/{el2['id']}/solicitar-informe", tf, {"observaciones": "x"})[0] == 404, "informe sobre un elemento borrado: 404")
# El límite es por usuario e incluye la petición anterior (404): quedan 2 permitidas.
codes = [req("POST", f"/api/elementos-geologicos/{eid}/solicitar-informe", tf, {"correoSolicitante": "otro@example.com", "observaciones": "x"})[0] for _ in range(3)]
check(codes == [200, 200, 429], f"informe limitado a 3 por hora por usuario ({codes})")

# 6. Concurrencia (bajo): edición simultánea a un borrado no lo deshace
deshecho = False
for ronda in range(15):
    code, el3, _ = req("POST", "/api/elementos-geologicos/minerales", adm, mineral(f"SEG3{S}R{ronda}"))
    x = el3["id"]
    cuerpo = {**mineral(f"SEG3{S}R{ronda}"), "nombre": "Editado"}
    hilos = [threading.Thread(target=req, args=("PUT", f"/api/elementos-geologicos/minerales/{x}", adm, cuerpo)) for _ in range(5)]
    hilos.append(threading.Thread(target=req, args=("DELETE", f"/api/elementos-geologicos/{x}", adm)))
    for h in hilos: h.start()
    for h in hilos: h.join()
    # si el DELETE perdió por 409 queda activo y correcto; se reintenta el borrado
    if req("GET", f"/api/elementos-geologicos/{x}", adm)[0] == 200:
        borrado = req("DELETE", f"/api/elementos-geologicos/{x}", adm)[0]
        if borrado != 200:
            deshecho = True
check(not deshecho, "15 rondas de PUT concurrente con DELETE: ningún borrado se deshace en silencio")

# limpieza
req("DELETE", f"/api/elementos-geologicos/{eid}", adm)
for uid in (id2, idf):
    req("DELETE", f"/api/users/{uid}", adm)

# 7. Límite por IP en login (bajo): va al final porque bloquea la IP 1 minuto
cuentas = [req("POST", "/api/auth/login", body={"email": f"nadie{S}@example.com", "password": "Equivocada1"})[0] for _ in range(12)]
check(429 in cuentas, f"login limitado por IP ({cuentas.count(429)} de 12 respondieron 429)")

print()
print("Regresión de seguridad: OK" if fallos == 0 else f"Regresión de seguridad: {fallos} falla(s)")
sys.exit(1 if fallos else 0)
