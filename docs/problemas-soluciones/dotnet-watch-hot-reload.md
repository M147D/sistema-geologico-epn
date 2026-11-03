# PS-02 · `dotnet watch` se detenía en el contenedor de desarrollo

## Síntomas

2026-11-03: después de editar `Program.cs` con el stack de desarrollo en
marcha, el servicio `api` quedó en `Exited (1)`. El log terminaba en una
excepción de Roslyn dentro de Hot Reload
(`HotReloadService.GetUpdatesAsync` → `EmitDifference`).

## Causa raíz

Hot Reload de `dotnet watch` (SDK 10.0.403) falló al calcular la
actualización de una edición en las instrucciones de nivel superior de
`Program.cs`, y el proceso terminó en lugar de reiniciar la aplicación. Por
qué falló Roslyn en esa edición concreta no se investigó.

## Solución

Commit `8dfe171`, en `compose.yaml`:

- `dotnet watch run --no-hot-reload`: ante cualquier cambio recompila y
  reinicia el proceso, sin aplicar parches en caliente.
- `restart: unless-stopped` en `api`, por si el proceso termina por otra
  causa.

## Verificación

2026-11-03: con el servicio levantado, un `touch Program.cs` provocó la
recompilación y el reinicio, y `/health` volvió a responder `Healthy`.
