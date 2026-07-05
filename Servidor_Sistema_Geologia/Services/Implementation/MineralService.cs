using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.DTO.Gallery;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

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
        ILogger<BaseElementoGeologicoService<Mineral, CreateMineralDto, UpdateMineralDto>> logger,
        SistemaGeologicoDbContext context)
        : base(repository, logger, context)
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

        return ValidationResult.Success();
    }

    protected override async Task<Mineral> MapCreateDtoToEntityAsync(CreateMineralDto createDto)
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
            UbicacionId = ubicacion?.Id ?? 1, // Default if no location
            // Nota: GaleriaElementosGeologicoId ya no existe - la relación ahora es Galeria -> Elemento

            // Mineral-specific properties
            TipoMineral = createDto.TipoMineral,
            Litologia = createDto.Litologia,
            
            // Set timestamps
            FechaCreacion = DateTime.Now,
            EstadoActivo = true
        };
    }

    protected override Task MapUpdateDtoToEntityAsync(UpdateMineralDto updateDto, Mineral mineral)
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
        return Task.CompletedTask;
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
            FechaEliminacion = mineral.FechaEliminacion,
            CreadoPor = mineral.UsuarioCreacion?.NombreCompleto,
            ActualizadoPor = mineral.UsuarioActualizacion?.NombreCompleto,
            EliminadoPor = mineral.UsuarioEliminacion?.NombreCompleto,

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
                NombrePais = mineral.Ubicacion.Provincia?.Pais?.NombrePais,
                NombreProvincia = mineral.Ubicacion.Provincia?.NombreProvincia
            } : null,
            
            Galeria = mineral.Galeria != null ? new GaleriaElementoGeologicoDto
            {
                Id = mineral.Galeria.Id,
                DetalleGrupo = mineral.Galeria.DetalleGrupo,
                TotalFotos = mineral.Galeria.Fotos?.Count ?? 0,
                Fotos = mineral.Galeria.Fotos?
                    .Select(f => new FotoElementoDto
                    {
                        Id = f.Id,
                        GaleriaElementosGeologicoId = f.GaleriaElementosGeologicoId,
                        TipoFoto = f.TipoFoto,
                        DescripcionEspecifica = f.DescripcionEspecifica,
                        FechaCreacion = f.FechaCreacion,
                        FechaActualizacion = f.FechaActualizacion,
                        EstadoActivo = f.EstadoActivo,
                        ImagenUrl = $"/api/foto-elementos/imagen/{f.Id}"
                    }).ToList() ?? new List<FotoElementoDto>()
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

}