namespace Servidor_Sistema_Geologia.DTO.Locations;

public class PaginatedCountriesDto
{
    public List<CountryListDto> Paises { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
