namespace Servidor_Sistema_Geologia.DTO.Locations;

public class ProvincesListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedProvincesDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}
