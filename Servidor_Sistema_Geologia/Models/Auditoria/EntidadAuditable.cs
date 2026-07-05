using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia;

/// <summary>
/// Clase base abstracta que proporciona funcionalidad de auditoría completa
/// para todas las entidades del sistema.
/// </summary>
public abstract class EntidadAuditable
{
    [Key]
    public int Id { get; set; }

    // AUDITORÍA - CREACIÓN
    [Display(Name = "Fecha de Creación")]
    [DataType(DataType.DateTime)]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [Display(Name = "Usuario que Creó")]
    public int? UsuarioCreacionId { get; set; }

    // AUDITORÍA - ACTUALIZACIÓN
    [Display(Name = "Fecha de Última Actualización")]
    [DataType(DataType.DateTime)]
    public DateTime? FechaActualizacion { get; set; }

    [Display(Name = "Usuario que Actualizó")]
    public int? UsuarioActualizacionId { get; set; }

    // AUDITORÍA - ELIMINACIÓN (SOFT DELETE)
    [Display(Name = "Fecha de Eliminación")]
    [DataType(DataType.DateTime)]
    public DateTime? FechaEliminacion { get; set; }

    [Display(Name = "Usuario que Eliminó")]
    public int? UsuarioEliminacionId { get; set; }

    [Display(Name = "Estado Activo")]
    public bool EstadoActivo { get; set; } = true;

    // NAVEGACIÓN - RELACIONES CON USUARIOS
    public Usuario? UsuarioCreacion { get; set; }
    public Usuario? UsuarioActualizacion { get; set; }
    public Usuario? UsuarioEliminacion { get; set; }
}
