namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class ElementosGeologicosListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedElementosGeologicosDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}
