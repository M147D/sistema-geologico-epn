using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

/// <summary>
/// Service for Roca entities - handles business logic and validation
/// Inherits from BaseElementoGeologicoService for common operations
/// </summary>
public class RocaService : BaseElementoGeologicoService<Roca, CreateRocaDto, UpdateRocaDto>, IRocaService
{
    private readonly IRocaRepository _rocaRepository;
    private readonly IElementoGeologicoRepository _historialRepository;

    public RocaService(
        IRocaRepository repository,
        IElementoGeologicoRepository historialRepository,
        ILogger<BaseElementoGeologicoService<Roca, CreateRocaDto, UpdateRocaDto>> logger,
        GestorSistemaGeologia context)
        : base(repository, logger, context)
    {
        _rocaRepository = repository;
        _historialRepository = historialRepository;
    }

    protected override string ElementTypeName => "Roca";

    // 🪨 ROCK-SPECIFIC METHODS
    public async Task<IEnumerable<Roca>> GetByLitologiaAsync(string litologia)
    {
        return await _rocaRepository.GetByLitologiaAsync(litologia);
    }

    public async Task<IEnumerable<Roca>> GetByTipoRocaAsync(SubtipoRoca tipoRoca)
    {
        return await _rocaRepository.GetByTipoRocaAsync(tipoRoca);
    }

    // 🎯 IMPLEMENTATION OF ABSTRACT METHODS
    protected override async Task<ValidationResult> ValidateCreateAsync(CreateRocaDto createDto)
    {
        // Validate unique code
        if (await _repository.ExistsByCodigoAsync(createDto.Codigo))
        {
            return ValidationResult.Error($"Ya existe una roca con el código '{createDto.Codigo}'");
        }

        // Rock-specific validations
        if (string.IsNullOrWhiteSpace(createDto.Litologia))
        {
            return ValidationResult.Error("La litología es obligatoria para una roca");
        }

        return ValidationResult.Success();
    }

    protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateRocaDto updateDto)
    {
        // Validate unique code (excluding current element)
        if (await _repository.ExistsByCodigoAsync(updateDto.Codigo, id))
        {
            return ValidationResult.Error($"Ya existe una roca con el código '{updateDto.Codigo}'");
        }

        // Rock-specific validations
        if (string.IsNullOrWhiteSpace(updateDto.Litologia))
        {
            return ValidationResult.Error("La litología es obligatoria para una roca");
        }

        return ValidationResult.Success();
    }

    protected override async Task<Roca> MapCreateDtoToEntityAsync(CreateRocaDto createDto)
    {
        // 🌍 Crear ubicación automáticamente si se proporcionan datos de ubicación
        Ubicacion? ubicacion = null;
        if (createDto.UbicacionId.HasValue)
        {
            // Usar ubicación existente
            ubicacion = await _context.Ubicaciones.FindAsync(createDto.UbicacionId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(createDto.Latitud) || !string.IsNullOrWhiteSpace(createDto.Longitud))
        {
            // Crear nueva ubicación automáticamente con manejo de transacciones
            Pais? pais = null;
            Provincia? provincia = null;
            
            // Step 1: Create or get País first
            if (!string.IsNullOrWhiteSpace(createDto.NombrePais))
            {
                pais = await ObtenerOCrearPaisAsync(createDto.NombrePais);
            }
            
            // Step 2: Create or get Provincia only if País exists
            if (pais != null && !string.IsNullOrWhiteSpace(createDto.NombreProvincia))
            {
                provincia = await ObtenerOCrearProvinciaAsync(createDto.NombreProvincia, pais.Id);
            }
            
            // Step 3: Create Ubicación with the IDs we have
            ubicacion = await CrearUbicacionAsync(createDto, pais?.Id, provincia?.Id);
        }

        // 🖼️ Crear galería automáticamente
        var galeria = await CrearGaleriaAsync(createDto.Nombre);
        
        return new Roca
        {
            Nombre = createDto.Nombre,
            Codigo = createDto.Codigo,
            Edad = createDto.Edad,
            Donante = createDto.Donante,
            FechaIngreso = createDto.FechaIngreso ?? DateTime.Now,
            Ejemplares = createDto.Ejemplares,
            DocumentosRelacionados = createDto.DocumentosRelacionados,
            LaminaExiste = createDto.LaminaExiste,
            UbicacionId = ubicacion?.Id ?? 1, // Default if no location
            GaleriaElementosGeologicoId = galeria.Id,
            
            // Rock-specific properties
            TipoRoca = createDto.TipoRoca,
            Litologia = createDto.Litologia,
            
            // Set timestamps
            FechaCreacion = DateTime.Now,
            EstadoActivo = true
        };
    }

    protected override async Task MapUpdateDtoToEntityAsync(UpdateRocaDto updateDto, Roca roca)
    {
        // Update base properties
        roca.Nombre = updateDto.Nombre;
        roca.Codigo = updateDto.Codigo;
        roca.Edad = updateDto.Edad;
        roca.Donante = updateDto.Donante;
        roca.FechaIngreso = updateDto.FechaIngreso ?? roca.FechaIngreso;
        roca.Ejemplares = updateDto.Ejemplares;
        roca.DocumentosRelacionados = updateDto.DocumentosRelacionados;
        roca.LaminaExiste = updateDto.LaminaExiste;
        
        if (updateDto.UbicacionId.HasValue)
        {
            roca.UbicacionId = updateDto.UbicacionId.Value;
        }

        // Update rock-specific properties
        roca.TipoRoca = updateDto.TipoRoca;
        roca.Litologia = updateDto.Litologia;
    }

    protected override ElementoGeologicoDetailDto MapToDetailDto(Roca roca)
    {
        return new ElementoGeologicoDetailDto
        {
            Id = roca.Id,
            Nombre = roca.Nombre,
            Codigo = roca.Codigo,
            TipoElemento = "Roca",
            Edad = roca.Edad,
            Donante = roca.Donante,
            FechaIngreso = roca.FechaIngreso,
            Ejemplares = roca.Ejemplares,
            DocumentosRelacionados = roca.DocumentosRelacionados,
            LaminaExiste = roca.LaminaExiste,
            EstadoActivo = roca.EstadoActivo,
            FechaCreacion = roca.FechaCreacion,
            FechaActualizacion = roca.FechaActualizacion,
            
            // Relations
            Ubicacion = roca.Ubicacion != null ? new UbicacionDto
            {
                Id = roca.Ubicacion.Id,
                Latitud = roca.Ubicacion.Latitud,
                Longitud = roca.Ubicacion.Longitud,
                Localidad = roca.Ubicacion.Localidad,
                Leyenda = roca.Ubicacion.Leyenda,
                PaisId = roca.Ubicacion.PaisId,
                ProvinciaId = roca.Ubicacion.ProvinciaId,
                EstadoActivo = roca.Ubicacion.EstadoActivo,
                FechaCreacion = roca.Ubicacion.FechaCreacion,
                FechaActualizacion = roca.Ubicacion.FechaActualizacion,
                NombrePais = roca.Ubicacion.Pais?.NombrePais,
                NombreProvincia = roca.Ubicacion.Provincia?.NombreProvincia
            } : null,
            
            Galeria = roca.Galeria != null ? new GaleriaElementoGeologicoDto
            {
                Id = roca.Galeria.Id,
                DetalleGrupo = roca.Galeria.DetalleGrupo,
                TotalFotos = roca.Galeria.Fotos?.Count ?? 0,
                Fotos = new List<FotoElementoDto>()
            } : null,
            
            // Rock-specific properties
            TipoRoca = roca.TipoRoca,
            LitologiaRoca = roca.Litologia,
            
            // Null for other types
            TipoFosil = null,
            Especie = null,
            Periodo = null,
            TipoMineral = null,
            LitologiaMineral = null
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
            _logger.LogError(ex, "Error registrando acceso para roca {ElementoId}", elementoId);
            // Don't throw - this shouldn't break the main operation
        }
    }
}