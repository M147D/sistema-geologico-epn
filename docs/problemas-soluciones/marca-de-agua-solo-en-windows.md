# PS-01 · La marca de agua y las miniaturas solo funcionaban en Windows

## Síntomas

Detectado al revisar el código (REV-01, H-01) el 2026-11-03, antes de
ejecutarlo en Linux. Fuera de Windows, un usuario Free habría recibido las
fotos originales sin marca de agua, y `?thumb=true` habría devuelto la imagen
completa. No había ningún error ni aviso en los logs.

## Causa raíz

`FotoElementosController.GetImagen` envolvía el procesamiento en
`OperatingSystem.IsWindows()` (`Controllers/FotoElementosController.cs:71-75`
en `d30f189`), porque usaba System.Drawing (GDI+), que .NET solo soporta en
Windows desde la versión 6. En cualquier otro sistema operativo los dos pasos
se saltaban y la caché guardaba la imagen sin marca bajo la clave `wm`.

## Solución

Commit `c0791da`:

- `Services/Implementation/ImageProcessingService.cs`, con SkiaSharp 4.154.0
  (MIT) y sus binarios nativos sin dependencias para Linux. Replica el texto,
  la opacidad, la sombra y el patrón a −30° del original. Lo único que
  cambió: el texto grande se reduce para caber en el 90 % del ancho.
- La fuente se carga desde un archivo (DejaVu Sans Bold en la imagen,
  `fonts-dejavu-core`; Arial en Windows). Si no hay fuente, **la API no
  arranca**.
- **Falla cerrado:** si la marca no se puede aplicar, la petición falla; ya no
  se devuelve la foto original como hacía el `catch` de GDI+.
- Se quitó la condición de sistema operativo.

Descartado: ImageSharp, por su licencia dividida (Six Labors Split License).

## Verificación

2026-11-03, en Linux (contenedores): ART-01 comprueba que el Admin recibe los
bytes originales y el Free una imagen distinta, y que la miniatura pesa menos
(8 063 contra 62 895 bytes en desarrollo). Además se revisó a ojo la imagen
que recibió un Free. Pasó también en producción local por HTTPS.
