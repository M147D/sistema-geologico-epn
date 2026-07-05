using Servidor_Sistema_Geologia;

namespace Servidor_Sistema_Geologia.DTO.Gallery;

public class FotoElementoDto
{
    public int Id { get; set; }
    public int GaleriaElementosGeologicoId { get; set; }
    public byte[] Imagen { get; set; } = Array.Empty<byte>();
    public TipoFoto TipoFoto { get; set; } = TipoFoto.Desconocido;
    public string DescripcionEspecifica { get; set; } = "Vacío";
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public bool EstadoActivo { get; set; }

    // Additional properties for extended information
    public string? ImagenUrl { get; set; }
    public string? DetalleGrupoGaleria { get; set; }
    public string? NombreElementoGeologico { get; set; }
}
