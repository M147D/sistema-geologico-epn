# PS-05 · Bloqueo de cuentas y envío de correos sin límite de frecuencia

**Estado en el repo:** el envío de correos queda corregido. El bloqueo de
cuentas queda **mitigado, no eliminado**: un atacante con muchas IPs todavía
puede bloquear una cuenta concreta. Eliminarlo del todo exige un CAPTCHA o un
segundo factor, que el proyecto no tiene.

## Síntomas

Auditoría de seguridad, corrida 1, confirmada en un sandbox:

- con 3 logins anónimos fallidos se bloqueaba cualquier cuenta, el superadmin
  incluido;
- cualquier cuenta Free podía mandar correos de informe sin límite, con un
  remitente inventado en el cuerpo de la petición.

## Causa raíz

- No había ningún límite de frecuencia, ni en la API ni en Caddy.
- El bloqueo de Identity saltaba con 3 fallos.
- `solicitar-informe` tomaba el correo del solicitante del cuerpo y no del
  token.

## Solución

Commit `43a23aa`:

- `AddRateLimiter`:
  - login y registro: 10 por minuto por IP;
  - `solicitar-informe`: 3 por hora por usuario y 50 por hora en total.
- `ForwardedHeaders` solo confía en redes privadas (Caddy en la red de Docker),
  con `ForwardLimit = 1`: la IP del cliente no se puede falsificar.
- El bloqueo de cuenta pasa a 10 intentos.
- El informe usa el correo de la sesión, y el diálogo lo muestra en un campo de
  solo lectura.

## Verificación

ART-03:

- el login devuelve 429 después de 10 intentos desde una IP;
- el informe devuelve 429 al cuarto pedido del mismo usuario en una hora.
