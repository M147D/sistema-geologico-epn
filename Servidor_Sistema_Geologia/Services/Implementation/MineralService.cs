using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;
using Servidor_Sistema_Geologia.ElementosGeologicos;

namespace Servidor_Sistema_Geologia.Services.Implementation;

/// <summary>
/// Service for Mineral entities - handles business logic and validation
/// Inherits from BaseElementoGeologicoService for common operations
/// </summary>
public class MineralService : BaseElementoGeologicoService<Mineral, CreateMineralDto, UpdateMineralDto>, IMineralService
{
    private readonly IMineralRepository _mineralRepository;
    private readonly IElementoGeologicoRepository _historialRepository;

    public MineralService(
        IMineralRepository repository,
        IElementoGeologicoRepository historialRepository,
        ILogger<BaseElementoGeologicoService<Mineral, CreateMineralDto, UpdateMineralDto>> logger)
        : base(repository, logger)
    {
        _mineralRepository = repository;
        _historialRepository = historialRepository;
    }

    protected override string ElementTypeName => "Mineral";

    // 💎 MINERAL-SPECIFIC METHODS
    public async Task<IEnumerable<Mineral>> GetByLitologiaAsync(string litologia)
    {
        return await _mineralRepository.GetByLitologiaAsync(litologia);
    }

    public async Task<IEnumerable<Mineral>> GetByTipoMineralAsync(SubtipoMineral tipoMineral)
    {
        return await _mineralRepository.GetByTipoMineralAsync(tipoMineral);
    }

    // 🎯 IMPLEMENTATION OF ABSTRACT METHODS
    protected override async Task<ValidationResult> ValidateCreateAsync(CreateMineralDto createDto)
    {
        // Validate unique code
        if (await _repository.ExistsByCodigoAsync(createDto.Codigo))
        {
            return ValidationResult.Error($"Ya existe un mineral con el código '{createDto.Codigo}'");
        }

        // Mineral-specific validations
        if (string.IsNullOrWhiteSpace(createDto.Litologia))
        {
            return ValidationResult.Error("La litología es obligatoria para un mineral");
        }

        return ValidationResult.Success();
    }

    protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateMineralDto updateDto)
    {
        // Validate unique code (excluding current element)
        if (await _repository.ExistsByCodigoAsync(updateDto.Codigo, id))
        {
            return ValidationResult.Error($"Ya existe un mineral con el código '{updateDto.Codigo}'");
        }

        // Mineral-specific validations
        if (string.IsNullOrWhiteSpace(updateDto.Litologia))
        {
            return ValidationResult.Error("La litología es obligatoria para un mineral");
        }

        return ValidationResult.Success();
    }

    protected override async Task<Mineral> MapCreateDtoToEntityAsync(CreateMineralDto createDto)
    {
        var ubicacionId = createDto.UbicacionId ?? 1; // Default for now
        
        return new Mineral
        {
            Nombre = createDto.Nombre,
            Codigo = createDto.Codigo,
            Edad = createDto.Edad,
            Donante = createDto.Donante,
            FechaIngreso = createDto.FechaIngreso ?? DateTime.Now,
            Ejemplares = createDto.Ejemplares,
            DocumentosRelacionados = createDto.DocumentosRelacionados,
            LaminaExiste = createDto.LaminaExiste,
            UbicacionId = ubicacionId,
            
            // Mineral-specific properties
            TipoMineral = createDto.TipoMineral,
            Litologia = createDto.Litologia,
            
            // Gallery initialization
            Galeria = new Galeria.GaleriaElementoGeologico 
            { 
                DetalleGrupo = "Ninguna" 
            }
        };
    }

    protected override async Task MapUpdateDtoToEntityAsync(UpdateMineralDto updateDto, Mineral mineral)
    {
        // Update base properties
        mineral.Nombre = updateDto.Nombre;
        mineral.Codigo = updateDto.Codigo;
        mineral.Edad = updateDto.Edad;
        mineral.Donante = updateDto.Donante;
        mineral.FechaIngreso = updateDto.FechaIngreso ?? mineral.FechaIngreso;
        mineral.Ejemplares = updateDto.Ejemplares;
        mineral.DocumentosRelacionados = updateDto.DocumentosRelacionados;
        mineral.LaminaExiste = updateDto.LaminaExiste;
        
        if (updateDto.UbicacionId.HasValue)
        {
            mineral.UbicacionId = updateDto.UbicacionId.Value;
        }

        // Update mineral-specific properties
        mineral.TipoMineral = updateDto.TipoMineral;
        mineral.Litologia = updateDto.Litologia;
    }

    protected override ElementoGeologicoDetailDto MapToDetailDto(Mineral mineral)
    {
        return new ElementoGeologicoDetailDto
        {
            Id = mineral.Id,
            Nombre = mineral.Nombre,
            Codigo = mineral.Codigo,
            TipoElemento = "Mineral",
            Edad = mineral.Edad,
            Donante = mineral.Donante,
            FechaIngreso = mineral.FechaIngreso,
            Ejemplares = mineral.Ejemplares,
            DocumentosRelacionados = mineral.DocumentosRelacionados,
            LaminaExiste = mineral.LaminaExiste,
            EstadoActivo = mineral.EstadoActivo,
            FechaCreacion = mineral.FechaCreacion,
            FechaActualizacion = mineral.FechaActualizacion,
            
            // Relations
            Ubicacion = mineral.Ubicacion != null ? new UbicacionDto
            {
                Id = mineral.Ubicacion.Id,
                Latitud = mineral.Ubicacion.Latitud,
                Longitud = mineral.Ubicacion.Longitud,
                Localidad = mineral.Ubicacion.Localidad,
                Leyenda = mineral.Ubicacion.Leyenda,
                PaisId = mineral.Ubicacion.PaisId,
                ProvinciaId = mineral.Ubicacion.ProvinciaId,
                EstadoActivo = mineral.Ubicacion.EstadoActivo,
                FechaCreacion = mineral.Ubicacion.FechaCreacion,
                FechaActualizacion = mineral.Ubicacion.FechaActualizacion,
                NombrePais = mineral.Ubicacion.Pais?.NombrePais,
                NombreProvincia = mineral.Ubicacion.Provincia?.NombreProvincia
            } : null,
            
            Galeria = mineral.Galeria != null ? new GaleriaElementoGeologicoDto
            {
                Id = mineral.Galeria.Id,
                DetalleGrupo = mineral.Galeria.DetalleGrupo,
                TotalFotos = mineral.Galeria.Fotos?.Count ?? 0,
                Fotos = new List<FotoElementoDto>()
            } : null,
            
            // Mineral-specific properties
            TipoMineral = mineral.TipoMineral,
            LitologiaMineral = mineral.Litologia,
            
            // Null for other types
            TipoFosil = null,
            Especie = null,
            Periodo = null,
            TipoRoca = null,
            LitologiaRoca = null
        };
    }

    protected override async Task RegisterAccessAsync(int elementoId, int usuarioId, AccionesUsuario accion)
    {
        try
        {
            await _historialRepository.RegisterAccessAsync(elementoId, usuarioId, accion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando acceso para mineral {ElementoId}", elementoId);
            // Don't throw - this shouldn't break the main operation
        }
    }
}