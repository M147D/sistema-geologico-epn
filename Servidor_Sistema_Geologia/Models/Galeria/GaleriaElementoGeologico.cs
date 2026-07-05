using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.Galeria;

public class GaleriaElementoGeologico : EntidadAuditable
{
	public int ElementoGeologicoId { get; set; }
	[MaxLength(200)]
	[Display(Name = "Nombre del Grupo")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del grupo solo puede contener letras y espacios.")]
	public string DetalleGrupo { get; set; } = "Ninguna";
	public ElementoGeologico? ElementoGeologico { get; set; }
	public List<FotoElemento> Fotos { get; set; } = new List<FotoElemento>();
}