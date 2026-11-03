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
        IUbicacionRepository ubicaciones,
        IUnitOfWork unitOfWork)
        : base(repository, logger, ubicaciones, unitOfWork)
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
        // Ubicación como grafo: se inserta con el elemento en un solo SaveChanges.
        var ubicacion = await ResolverUbicacionAsync(createDto);

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
            Ubicacion = ubicacion, // sin id literal por defecto (REV-01, H-03)
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

    protected override void ActualizarDesdeCreateDto(CreateMineralDto dto, Mineral mineral)
    {
        mineral.Nombre = dto.Nombre;
        mineral.Edad = dto.Edad;
        mineral.Donante = dto.Donante;
        mineral.FechaIngreso = dto.FechaIngreso ?? mineral.FechaIngreso;
        mineral.Ejemplares = dto.Ejemplares;
        mineral.DocumentosRelacionados = dto.DocumentosRelacionados;
        mineral.LaminaExiste = dto.LaminaExiste;
        mineral.TipoMineral = dto.TipoMineral;
        mineral.Litologia = dto.Litologia;
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
                PaisId = mineral.Ubicacion.Provincia?.PaisId,
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