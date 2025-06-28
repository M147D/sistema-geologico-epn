using System.ComponentModel.DataAnnotations;

namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public abstract class CreateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria")]
    [RegularExpression(@"^[0-9]+(\.[0-9]+)?\s*(a|m|k)?$", ErrorMessage = "La edad debe ser un número seguido de 'a' (años), 'm' (millones) o 'k' (miles) opcionalmente")]
    [StringLength(50, ErrorMessage = "La edad no puede exceder los 50 caracteres")]
    public string Edad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El donante es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El donante debe tener entre 1 y 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El donante solo puede contener letras y espacios")]
    public string Donante { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
    [DataType(DataType.Date, ErrorMessage = "La fecha de ingreso debe ser una fecha válida")]
    public DateTime? FechaIngreso { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "El código solo puede contener letras y números")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El código debe tener entre 1 y 100 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de ejemplares es obligatorio")]
    [Range(1, 10000, ErrorMessage = "El número de ejemplares debe estar entre 1 y 10000")]
    public uint Ejemplares { get; set; } = 1;

    [StringLength(5000, ErrorMessage = "Los documentos relacionados no pueden exceder los 5000 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s,;.-]*$", ErrorMessage = "Los documentos relacionados solo pueden contener letras, números, espacios y algunos caracteres especiales")]
    public string? DocumentosRelacionados { get; set; }

    [Display(Name = "¿Existe lámina?")]
    public bool LaminaExiste { get; set; } = false;

    public int? UbicacionId { get; set; }
    
    public int UsuarioId { get; set; } // Para el historial de acceso
}

public class CreateFosilDto : CreateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El tipo de fósil es obligatorio")]
    [EnumDataType(typeof(SubtipoFosil), ErrorMessage = "El tipo de fósil no es válido")]
    public SubtipoFosil TipoFosil { get; set; } = SubtipoFosil.Desconocido;

    [StringLength(200, ErrorMessage = "La especie no puede exceder los 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "La especie solo puede contener letras y espacios")]
    public string Especie { get; set; } = "Por determinar";

    [StringLength(200, ErrorMessage = "El período no puede exceder los 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "El período solo puede contener letras y espacios")]
    public string Periodo { get; set; } = "Por determinar";
}

public class CreateMineralDto : CreateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El tipo de mineral es obligatorio")]
    [EnumDataType(typeof(SubtipoMineral), ErrorMessage = "El tipo de mineral no es válido")]
    public SubtipoMineral TipoMineral { get; set; } = SubtipoMineral.Desconocido;

    [StringLength(200, ErrorMessage = "La litología no puede exceder los 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "La litología solo puede contener letras y espacios")]
    public string Litologia { get; set; } = "Desconocida";
}

public class CreateRocaDto : CreateElementoGeologicoBaseDto
{
    [Required(ErrorMessage = "El tipo de roca es obligatorio")]
    [EnumDataType(typeof(SubtipoRoca), ErrorMessage = "El tipo de roca no es válido")]
    public SubtipoRoca TipoRoca { get; set; } = SubtipoRoca.Desconocido;

    [StringLength(200, ErrorMessage = "La litología no puede exceder los 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$", ErrorMessage = "La litología solo puede contener letras y espacios")]
    public string Litologia { get; set; } = "Desconocida";
}
