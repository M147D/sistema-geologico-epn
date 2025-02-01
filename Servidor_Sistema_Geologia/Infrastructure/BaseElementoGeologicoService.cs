using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public abstract class BaseElementoGeologicoService
	{
		protected GestorGeologia _db;

		protected BaseElementoGeologicoService(GestorGeologia db)
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

			foreach (var fotoDto in fotosDto)
			{
				var galeria = new GaleriaElementoGeologico
				{
					DetalleGrupo = fotoDto.Galeria.DetalleGrupo,
					ElementoGeologicoId = elementoId,
				};

				_db.GaleriaElementosGeologicos.Add(galeria);
				await _db.SaveChangesAsync();

				var foto = new FotoElemento
				{
					TipoFoto = fotoDto.TipoFoto,
					FechaSubida = fotoDto.FechaSubida,
					CreadoPor = fotoDto.CreadoPor,
					DescripcionEspecifica = fotoDto.DescripcionEspecifica,
					Etiquetas = fotoDto.Etiquetas,
					Imagen = fotoDto.Imagen,
					ElementoGeologicoId = elementoId,
					GaleriaElementosGeologicoId = galeria.Id
				};

				_db.FotosElementos.Add(foto);
			}

			await _db.SaveChangesAsync();
		}
	}
}
