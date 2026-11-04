# PS-06 · El borrado lógico no se aplicaba en todas las lecturas

## Síntomas

Auditorías de seguridad, corridas 1 y 2 (REV-02, H-02). Para un usuario Free:

- `imagen/{id}` y `galeria/{id}` seguían entregando fotos borradas, y un
  Premium recibía la original;
- el detalle de un elemento listaba fotos borradas;
- `solicitar-informe` aceptaba elementos borrados, lo que revelaba que
  existían.

## Causa raíz

Cada consulta decidía por su cuenta si filtraba `EstadoActivo`. Solo
`por-elemento` y las rutas de elementos aplicaban la política.

## Solución

Commit `43a23aa`. Una sola regla: fuera de Admin, solo se ve lo que está activo
en toda la cadena (foto, galería y elemento).

- `imagen/{id}` comprueba la visibilidad **antes** de la caché.
- `galeria/{id}` y el detalle reciben el rol del usuario.
- `solicitar-informe` busca solo elementos activos.
- No se pueden editar fotos borradas ni subir fotos a la galería de un elemento
  borrado.
- Las fotos registran quién las creó, editó, borró y restauró.

## Verificación

ART-03: para un Free, una foto borrada da 404 en `imagen/{id}` y no aparece en
la galería ni en el detalle, mientras el Admin la sigue viendo. El informe sobre
un elemento borrado da 404.
