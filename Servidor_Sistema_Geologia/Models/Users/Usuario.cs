
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

public class Usuario : IdentityUser<int>
{
	[Display(Name = "Nombre Completo")]
	public string? NombreCompleto { get; set; }

	[Required(ErrorMessage = "El rol es obligatorio")]
	[EnumDataType(typeof(RolUsuario), ErrorMessage = "El rol debe ser un valor valido de RolUsuario")]
	public RolUsuario Rol { get; set; } = RolUsuario.Free;
	
	[Display(Name = "Fecha de Creación")]
	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
	public DateTime FechaCreacion { get; set; } = DateTime.Now;
	
	public bool EstadoActivo { get; set; } = true;
}