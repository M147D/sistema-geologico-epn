namespace Servidor_Sistema_Geologia.DTO.Locations;

public class CountriesListResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaginatedCountriesDto? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}
