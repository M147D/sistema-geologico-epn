using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class HistorialAcceso
{
	[Key]
	public int Id { get; set; }
	public int UsuarioId { get; set; } = 0;
	public int ElementoGeologicoId { get; set; } = 0;
	[Display(Name = "Fecha de Creación")]
	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
	public DateTime FechaAcceso { get; set; } = DateTime.UtcNow;
	[EnumDataType(typeof(AccionesUsuario), ErrorMessage = "La acción no es válida.")]
	public AccionesUsuario Accion { get; set; } = AccionesUsuario.Ninguna;
	public Usuario? Usuario { get; set; }
	public ElementoGeologico? ElementoGeologico { get; set; }
}