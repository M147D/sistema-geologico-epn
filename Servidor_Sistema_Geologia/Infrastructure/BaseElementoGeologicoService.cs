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

		protected async Task GuardarFotosAsync(int elementoId, IEnumerable<FotoElementoDto> fotosDto)
		{
			if (fotosDto == null || !fotosDto.Any()) return;

			// Verifica si ya existe una galería para este elemento
			var galeria = await _db.GaleriaElementosGeologicos
				.FirstOrDefaultAsync(g => g.ElementoGeologicoId == elementoId);

			// Se crean todas las fotos
			var fotosParaAgregar = fotosDto.Select(fotoDto => new FotoElemento
			{
				TipoFoto = fotoDto.TipoFoto,
				FechaSubida = fotoDto.FechaSubida ?? DateTime.Now,
				CreadoPor = fotoDto.CreadoPor ?? "Sistema",
				DescripcionEspecifica = fotoDto.DescripcionEspecifica,
				Etiquetas = fotoDto.Etiquetas,
				Imagen = fotoDto.Imagen,
				GaleriaElementosGeologicoId = galeria.Id
			}).ToList();

			// Se guardan las fotos en el contexto
			_db.FotosElementos.AddRange(fotosParaAgregar);
			await _db.SaveChangesAsync();
		}
	}
}