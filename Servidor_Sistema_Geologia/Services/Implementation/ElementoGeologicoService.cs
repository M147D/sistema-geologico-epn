using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class ElementoGeologicoService : IElementoGeologicoService
{
    private readonly IElementoGeologicoRepository _elementoRepository;
    private readonly ILogger<ElementoGeologicoService> _logger;

    public ElementoGeologicoService(
        IElementoGeologicoRepository elementoRepository,
        ILogger<ElementoGeologicoService> logger)
    {
        _elementoRepository = elementoRepository;
        _logger = logger;
    }

    public async Task<ElementoGeologicoResponseDto> GetByIdWithDetailsAsync(int id, int usuarioId)
    {
        try
        {
            var elemento = await _elementoRepository.GetByIdWithDetailsAsync(id);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Elemento geológico no encontrado",
                    Errors = new List<string> { $"No se encontró un elemento con ID: {id}" }
                };
            }

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Elemento geológico con detalles obtenido exitosamente",
                Data = MapToDetailDto(elemento)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elemento geológico con detalles ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> GetByCodigoAsync(string codigo, int usuarioId)
    {
        try
        {
            var elemento = await _elementoRepository.GetByCodigoAsync(codigo);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Elemento geológico no encontrado",
                    Errors = new List<string> { $"No se encontró un elemento con código: {codigo}" }
                };
            }

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Elemento geológico obtenido exitosamente",
                Data = MapToDetailDto(elemento)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elemento geológico con código: {Codigo}", codigo);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetAllAsync(ElementoGeologicoFilterDto filter)
    {
        try
        {
            var elementos = await _elementoRepository.GetAllAsync(filter);

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Elementos geológicos obtenidos exitosamente",
                Data = elementos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de elementos geológicos");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    private ElementoGeologicoDetailDto MapToDetailDto(ElementoGeologico elemento)
    {
        var dto = new ElementoGeologicoDetailDto
        {
            Id = elemento.Id,
            Nombre = elemento.Nombre,
            Codigo = elemento.Codigo,
            TipoElemento = elemento.GetType().Name,
            Edad = elemento.Edad,
            Donante = elemento.Donante,
            FechaIngreso = elemento.FechaIngreso,
            Ejemplares = elemento.Ejemplares,
            DocumentosRelacionados = elemento.DocumentosRelacionados,
            LaminaExiste = elemento.LaminaExiste,
            EstadoActivo = elemento.EstadoActivo,
            FechaCreacion = elemento.FechaCreacion,
            FechaActualizacion = elemento.FechaActualizacion,
            CreadoPor = elemento.UsuarioCreacion?.NombreCompleto,
            ActualizadoPor = elemento.UsuarioActualizacion?.NombreCompleto,
            EliminadoPor = elemento.UsuarioEliminacion?.NombreCompleto
        };

        if (elemento.Ubicacion != null)
        {
            dto.Ubicacion = new UbicacionDto
            {
                Id = elemento.Ubicacion.Id,
                Latitud = elemento.Ubicacion.Latitud,
                Longitud = elemento.Ubicacion.Longitud,
                Localidad = elemento.Ubicacion.Localidad,
                Leyenda = elemento.Ubicacion.Leyenda,
                EstadoActivo = elemento.Ubicacion.EstadoActivo,
                NombrePais = elemento.Ubicacion.Pais?.NombrePais,
                NombreProvincia = elemento.Ubicacion.Provincia?.NombreProvincia
            };
        }

        if (elemento.Galeria != null)
        {
            dto.Galeria = new GaleriaElementoGeologicoDto
            {
                Id = elemento.Galeria.Id,
                DetalleGrupo = elemento.Galeria.DetalleGrupo,
                TotalFotos = elemento.Galeria.Fotos?.Count ?? 0
            };
        }

        switch (elemento)
        {
            case Fosil fosil:
                dto.TipoFosil = fosil.TipoFosil;
                dto.Especie = fosil.Especie;
                dto.Periodo = fosil.Periodo;
                break;
            case Mineral mineral:
                dto.TipoMineral = mineral.TipoMineral;
                dto.LitologiaMineral = mineral.Litologia;
                break;
            case Roca roca:
                dto.TipoRoca = roca.TipoRoca;
                dto.LitologiaRoca = roca.Litologia;
                break;
        }

        return dto;
    }
}
