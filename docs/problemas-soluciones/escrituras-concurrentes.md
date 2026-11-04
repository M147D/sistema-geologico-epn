# PS-07 · Una edición concurrente deshacía un borrado lógico

## Síntomas

Auditoría de seguridad, corrida 2 (REV-02, H-03), reproducida en un sandbox.
Cuando un PUT de un Invitado se superponía con el DELETE del Admin, el borrado
se deshacía sin aviso y se perdían los datos de quién borró y cuándo. Pasaba
con elementos y con fotos.

## Causa raíz

Los repositorios llamaban a `DbSet.Update` sobre entidades ya rastreadas, lo que
reescribe todas las columnas con el valor que tenían al leerlas. No había token
de concurrencia.

## Solución

Commit `43a23aa`, migración `TokenConcurrenciaXmin`:

- Token `[Timestamp] uint Version` en `ElementoGeologico` y `FotoElemento`.
  Npgsql lo mapea a la columna de sistema `xmin`, así que la migración no crea
  columnas.
- `UpdateAsync` ya no llama a `Update` sobre entidades rastreadas: se escriben
  solo las columnas que cambiaron.
- El repositorio convierte `DbUpdateConcurrencyException` en
  `ConflictoConcurrenciaException`, para que los servicios no dependan de EF, y
  los controladores responden 409.

## Verificación

2026-11-03, en desarrollo. Se forzó la intercalación con un `LOCK TABLE`
mantenido en la base: el PUT respondió 200 y el DELETE, que había leído una
versión vieja, respondió **409**. La fila quedó coherente y no se perdió nada
sin aviso. ART-03 repite 15 rondas concurrentes sin borrados deshechos.
