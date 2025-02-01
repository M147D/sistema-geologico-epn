using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Models
{
	public class GaleriaElementoGeologico
	{
		[Key]
		public int Id { get; set; }

		public int? ElementoGeologicoId { get; set; }

		public string? DetalleGrupo { get; set; }

		public ElementoGeologico? ElementoGeologico { get; set; }

		public List<FotoElemento> Fotos { get; set; } = new List<FotoElemento>();
	}
}
