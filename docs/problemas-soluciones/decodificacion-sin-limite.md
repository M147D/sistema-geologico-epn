# PS-08 · Las imágenes se decodificaban sin límite de píxeles

## Síntomas

Auditoría de seguridad, corrida 1, que lo dejó por validar. Un PNG de 1 KB que
declara 3000x3000 subía unos 66 MB la memoria de la API en cada vista que no
estaba en caché, y el crecimiento era lineal en píxeles. Además se aceptaban
archivos que no eran imágenes.

## Causa raíz

Al subir no se validaban ni el formato ni las dimensiones. `SKBitmap.Decode`
decodificaba siempre a tamaño completo.

## Solución

Commit `43a23aa`:

- `ImageProcessingService.ValidarImagen` lee solo la cabecera (`SKCodec`).
  Acepta JPEG, PNG y WebP, con 12000 px por lado y 40 MP como máximo.
- Se valida al subir (400) y otra vez antes de cada decodificación, para las
  filas que se guardaron antes de este cambio.
- `mem_limit: 1g` para `api` en `compose.prod.yaml`.

Sigue sin verificar cuánta memoria tiene el servidor de producción, porque ese
servidor todavía no existe.

## Verificación

ART-03: se rechazan con 400 un PNG que declara 20000x20000 y un archivo que no
es una imagen.
