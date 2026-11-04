# PS-03 · El token seguía valiendo después de revocar el acceso

## Síntomas

Lo encontró la auditoría de seguridad (corrida 1, 2026-11-03) y se reprodujo en
un sandbox. Un Admin degradado, desactivado y con la contraseña cambiada
mantenía 12 h de acceso de Admin con su token anterior. El logout no tenía
efecto en el servidor.

## Causa raíz

La autorización leía solo el rol que traía el token firmado, y la validación del
JWT no consultaba el estado actual del usuario (`Program.cs`, antes de
`43a23aa`). Las acciones de revocación cambiaban la fila en la base, pero no el
token, y `LogoutAsync` solo escribía en el registro.

## Solución

Commit `43a23aa`:

- El JWT lleva el `SecurityStamp` de Identity en el claim `sstamp`.
- `OnTokenValidated` carga al usuario en cada petición y rechaza el token si
  falta, está inactivo, cambió de rol o tiene otro sello.
- Renuevan el sello: editar o desactivar un usuario (`UserRepository`), el
  logout (`AuthService.LogoutAsync`) y el cambio de contraseña (Identity ya lo
  hacía).
- `OnAuthenticationFailed` ya no escribe la respuesta: la escribía y la acción
  seguía con la respuesta ya iniciada.

Costo asumido: una consulta por petición autenticada. El logout cierra todas las
sesiones del usuario, no solo la actual.

## Verificación

ART-03 (2026-11-03): después de degradar a un Admin, de cerrar sesión y de
cambiar la contraseña, el token anterior recibe 401.
