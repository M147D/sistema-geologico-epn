namespace Servidor_Sistema_Geologia.DTO.Create
{
    public class ElementoGeologicoCreateDto
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string? Nombre { get; set; }

        public int? Edad { get; set; }

        public string? Donante { get; set; }

        public int FechaIngreso { get; set; }

        public string? Codigo { get; set; }

        public int? Ejemplares { get; set; }

        public string? DocumentosRelacionados { get; set; }

        public string? LaminaURL { get; set; }

        public bool? LaminaExiste { get; set; }

        public UbicacionDto? Ubicacion { get; set; }

        public EstadoElementoDto? EstadoElemento { get; set; }

        public List<FotoElementoDto> Fotos { get; set; } = new List<FotoElementoDto>();
    }
}
