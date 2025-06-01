using System.ComponentModel.DataAnnotations;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia;

public abstract class ElementoGeologico
{
	[Key]
	public int Id { get; set; }
	[Required(ErrorMessage = "El estado es obligatorio")]
	[EnumDataType(typeof(EstadosElemento), ErrorMessage = "El estado debe ser un valor valido de EstadosElemento")]
	public EstadosElemento Estado { get; set; } = EstadosElemento.Creado;
	[Required(ErrorMessage = "El nombre es obligatorio")]
	[StringLength(200, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 200 caracteres")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
	public string Nombre { get; set; } = string.Empty;
	[Required(ErrorMessage = "La edad es obligatoria")]
	[RegularExpression(@"^[0-9]+(\.[0-9]+)?\s*(a|m|k)?$", ErrorMessage = "La edad debe ser un numero entre 0 y 1000, seguido de 'a' (años), 'm' (millones) o 'k' (miles) opcionalmente")]
	[StringLength(50, ErrorMessage = "La edad no puede exceder los 50 caracteres")]
	public string Edad { get; set; } = string.Empty;
	[Required(ErrorMessage = "El donante es obligatorio")]
	[StringLength(200, MinimumLength = 1, ErrorMessage = "El donante debe tener entre 1 y 200 caracteres")]
	[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El donante solo puede contener letras y espacios")]
	public string? Donante { get; set; }
	[Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
	[DataType(DataType.Date, ErrorMessage = "La fecha de ingreso debe ser una fecha valida")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	[RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La fecha de ingreso debe estar en el formato yyyy-MM-dd")]
	[Range(typeof(DateTime), "1900-01-01", "2100-12-31", ErrorMessage = "La fecha de ingreso debe estar entre 1900 y 2100")]
	public DateTime? FechaIngreso { get; set; } = DateTime.Now;
	[Required(ErrorMessage = "El codigo es obligatorio")]
	[RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "El codigo solo puede contener letras y numeros")]
	[StringLength(100, MinimumLength = 1, ErrorMessage = "El codigo debe tener entre 1 y 100 caracteres")]
	public string? Codigo { get; set; } = string.Empty;
	[Required(ErrorMessage = "El numero de ejemplares es obligatorio")]
	[Range(1, 10000, ErrorMessage = "El numero de ejemplares debe estar entre 1 y 10000")]
	public uint? Ejemplares { get; set; } = 1;
	[RegularExpression(@"^[a-zA-Z0-9\s,;.-]+$", ErrorMessage = "Los documentos relacionados solo pueden contener letras, numeros, espacios y los siguientes caracteres: ,;.-")]
	[StringLength(5000, ErrorMessage = "Los documentos relacionados no pueden exceder los 500 caracteres")]
	public string? DocumentosRelacionados { get; set; }
	[Required(ErrorMessage = "La existencia de lamina es obligatoria")]
	[RegularExpression(@"^(true|false)$", ErrorMessage = "La existencia de lamina debe ser true o false")]
	[Display(Name = "¿Existe lamina?")]
	[DisplayFormat(ConvertEmptyStringToNull = false)]
	public bool LaminaExiste { get; set; } = false;
	public int? UbicacionId { get; set; }
	public int? GaleriaElementosGeologicoId { get; set; }
	public Ubicacion? Ubicacion { get; set; }
	public GaleriaElementoGeologico? Galeria { get; set; }
}