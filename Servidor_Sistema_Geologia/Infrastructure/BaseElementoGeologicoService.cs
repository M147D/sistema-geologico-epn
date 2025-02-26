using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public abstract class BaseElementoGeologicoService
	{
		protected GestorSistemaGeologia _db;

		protected BaseElementoGeologicoService(GestorSistemaGeologia db)
		{
			_db = db;
		}

		protected async Task<EstadoElemento> ObtenerOcrearEstadoAsync(EstadoElementoDto estadoDto)
		{
			var estado = await _db.EstadosElementos
				.FirstOrDefaultAsync(e => e.DescripcionEstado == estadoDto.DescripcionEstado);

			if (estado == null)
			{
				estado = new EstadoElemento { DescripcionEstado = estadoDto.DescripcionEstado };
				_db.EstadosElementos.Add(estado);
				await _db.SaveChangesAsync();
			}

			return estado;
		}

		protected async Task<Ubicacion> ObtenerOcrearUbicacionAsync(UbicacionDto ubicacionDto)
		{
			var ubicacion = await _db.Ubicaciones
			.Include(u => u.Provincia)
			.Include(u => u.Pais)
			.FirstOrDefaultAsync(u =>
				u.Latitud == ubicacionDto.Latitud &&
				u.Longitud == ubicacionDto.Longitud &&
				u.Localidad == ubicacionDto.Localidad &&
				u.Pais.NombrePais == ubicacionDto.Pais.NombrePais &&
				u.Provincia.NombreProvincia == ubicacionDto.Provincia.NombreProvincia);

			if (ubicacion == null)
			{
				var pais = await _db.Paises.FirstOrDefaultAsync(p => p.NombrePais == ubicacionDto.Pais.NombrePais);
				if (pais == null)
				{
					pais = new Pais { NombrePais = ubicacionDto.Pais.NombrePais };
					_db.Paises.Add(pais);
					await _db.SaveChangesAsync();
				}

				var provincia = await _db.Provincias.FirstOrDefaultAsync(p =>
					p.NombreProvincia == ubicacionDto.Provincia.NombreProvincia && p.PaisId == pais.Id);
				if (provincia == null)
				{
					provincia = new Provincia
					{
						NombreProvincia = ubicacionDto.Provincia.NombreProvincia,
						PaisId = pais.Id
					};
					_db.Provincias.Add(provincia);
					await _db.SaveChangesAsync();
				}

				ubicacion = new Ubicacion
				{
					Latitud = ubicacionDto.Latitud,
					Longitud = ubicacionDto.Longitud,
					Localidad = ubicacionDto.Localidad,
					Leyenda = ubicacionDto.Leyenda,
					ProvinciaId = provincia.Id,
					PaisId = pais.Id
				};

				_db.Ubicaciones.Add(ubicacion);
				await _db.SaveChangesAsync();
			}

			return ubicacion;
		}

		protected async Task GuardarFotosAsync(int elementoId, IEnumerable<FotoElementoDto> fotosDto)
		{
			if (fotosDto == null || !fotosDto.Any()) return;

			// Verificar si ya existe una galería para este elemento
			var galeria = await _db.GaleriaElementosGeologicos
				.FirstOrDefaultAsync(g => g.ElementoGeologicoId == elementoId);

			// Si no existe, crear una nueva
			if (galeria == null)
			{
				galeria = new GaleriaElementoGeologico
				{
					DetalleGrupo = "Galería principal",
					ElementoGeologicoId = elementoId
				};

				_db.GaleriaElementosGeologicos.Add(galeria);
				await _db.SaveChangesAsync();

				// Actualizar el elemento geológico con la referencia a la galería
				var elemento = await _db.ElementosGeologicos.FindAsync(elementoId);
				if (elemento != null)
				{
					elemento.GaleriaElementosGeologicoId = galeria.Id;
					await _db.SaveChangesAsync();
				}
			}

			// Agregar las fotos a la galería existente
			foreach (var fotoDto in fotosDto)
			{
				var foto = new FotoElemento
				{
					TipoFoto = fotoDto.TipoFoto,
					FechaSubida = fotoDto.FechaSubida ?? DateTime.UtcNow,
					CreadoPor = fotoDto.CreadoPor,
					DescripcionEspecifica = fotoDto.DescripcionEspecifica,
					Etiquetas = fotoDto.Etiquetas,
					Imagen = fotoDto.Imagen,
					GaleriaElementosGeologicoId = galeria.Id
				};

				_db.FotosElementos.Add(foto);
			}

			await _db.SaveChangesAsync();
		}
	}
}