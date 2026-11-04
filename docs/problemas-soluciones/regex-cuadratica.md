# PS-04 · Las regex de validación costaban CPU cuadrática antes del login

## Síntomas

Auditoría de seguridad, corrida 1, confirmada en un sandbox: una petición
anónima de 64 KB a `register` ocupaba unos 10 s de CPU. La corrida 2 mostró lo
mismo en los DTO autenticados, aunque tuvieran `StringLength`.

## Causa raíz

`NoInjectionAttribute` ejecutaba `Regex.IsMatch` sin timeout sobre patrones con
`.*?` y `.+` sin anclar (O(n²)). Los campos de autenticación no tenían longitud
máxima, y ASP.NET ejecuta todos los atributos de una propiedad aunque otro ya
haya fallado: por eso poner `StringLength` no alcanzaba.

## Solución

Commit `43a23aa`:

- Patrones compilados una sola vez con `RegexOptions.NonBacktracking` (tiempo
  lineal) y un timeout de 250 ms.
- Tope de 5000 caracteres dentro del propio atributo, antes de cualquier regex.
- Los patrones usan `[^>]` y `[^=]` en lugar de `.*?` y `.+`.
- `StringLength` en Email (254) y Password (100) de `register` y `login`.

## Verificación

ART-03: el mismo payload de 64 KB se rechaza con 400 en 3-4 ms.
