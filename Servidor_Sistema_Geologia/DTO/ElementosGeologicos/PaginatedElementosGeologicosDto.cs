namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class PaginatedElementosGeologicosDto
{
    public List<ElementoGeologicoListDto> ElementosGeologicos { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    
    // Estadísticas adicionales
    public Dictionary<string, int> TipoStats { get; set; } = new();
    public Dictionary<string, int> EstadoStats { get; set; } = new();
}
