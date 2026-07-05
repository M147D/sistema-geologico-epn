namespace Servidor_Sistema_Geologia.DTO.ElementosGeologicos;

public class ElementoGeologicoSingleResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ElementoGeologicoDetailDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}
