using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.DTO.Gallery;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

/// <summary>
/// Service for Fossil entities - handles business logic and validation
/// Inherits from BaseElementoGeologicoService for common operations
/// </summary>
public class FosilService : BaseElementoGeologicoService<Fosil, CreateFosilDto, UpdateFosilDto>, IFosilService
{
    private readonly IFosilRepository _fosilRepository;
    private readonly IElementoGeologicoRepository _historialRepository; // For access history

    public FosilService(
        IFosilRepository repository,
        IElementoGeologicoRepository historialRepository,
        ILogger<BaseElementoGeologicoService<Fosil, CreateFosilDto, UpdateFosilDto>> logger,
        IUbicacionRepository ubicaciones,
        IUnitOfWork unitOfWork)
        : base(repository, logger, ubicaciones, unitOfWork)
    {
        _fosilRepository = repository;
        _historialRepository = historialRepository;
    }

    protected override string ElementTypeName => "Fósil";

    // 🦕 FOSSIL-SPECIFIC METHODS
    public async Task<IEnumerable<Fosil>> GetByPeriodoAsync(string periodo)
    {
        return await _fosilRepository.GetByPeriodoAsync(periodo);
    }

    public async Task<IEnumerable<Fosil>> GetByEspecieAsync(string especie)
    {
        return await _fosilRepository.GetByEspecieAsync(especie);
    }

    public async Task<IEnumerable<Fosil>> GetByTipoFosilAsync(SubtipoFosil tipoFosil)
    {
        return await _fosilRepository.GetByTipoFosilAsync(tipoFosil);
    }

    // 🎯 IMPLEMENTATION OF ABSTRACT METHODS
    protected override async Task<ValidationResult> ValidateCreateAsync(CreateFosilDto createDto)
    {
        // Validate unique code
        if (await _repository.ExistsByCodigoAsync(createDto.Codigo))
        {
            return ValidationResult.Error($"Ya existe un fósil con el código '{createDto.Codigo}'");
        }

        // Fossil-specific validations
        if (string.IsNullOrWhiteSpace(createDto.Especie))
        {
            return ValidationResult.Error("La especie es obligatoria para un fósil");
        }

        if (string.IsNullOrWhiteSpace(createDto.Periodo))
        {
            return ValidationResult.Error("El periodo es obligatorio para un fósil");
        }

        return ValidationResult.Success();
    }

    protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateFosilDto updateDto)
    {
        // Validate unique code (excluding current element)
        if (await _repository.ExistsByCodigoAsync(updateDto.Codigo, id))
        {
            return ValidationResult.Error($"Ya existe un fósil con el código '{updateDto.Codigo}'");
        }

        return ValidationResult.Success();
    }

    protected override async Task<Fosil> MapCreateDtoToEntityAsync(CreateFosilDto createDto)
    {
        // Ubicación como grafo: se inserta con el elemento en un solo SaveChanges.
        var ubicacion = await ResolverUbicacionAsync(createDto);

        return new Fosil
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

            // Fossil-specific properties
            TipoFosil = createDto.TipoFosil,
            Especie = createDto.Especie,
            Periodo = createDto.Periodo,
            
            // Set timestamps
            FechaCreacion = DateTime.Now,
            EstadoActivo = true
        };
    }

    protected override Task MapUpdateDtoToEntityAsync(UpdateFosilDto updateDto, Fosil fosil)
    {
        // Update base properties
        fosil.Nombre = updateDto.Nombre;
        fosil.Codigo = updateDto.Codigo;
        fosil.Edad = updateDto.Edad;
        fosil.Donante = updateDto.Donante;
        fosil.FechaIngreso = updateDto.FechaIngreso ?? fosil.FechaIngreso;
        fosil.Ejemplares = updateDto.Ejemplares;
        fosil.DocumentosRelacionados = updateDto.DocumentosRelacionados;
        fosil.LaminaExiste = updateDto.LaminaExiste;
        
        if (updateDto.UbicacionId.HasValue)
        {
            fosil.UbicacionId = updateDto.UbicacionId.Value;
        }

        // Update fossil-specific properties
        fosil.TipoFosil = updateDto.TipoFosil;
        fosil.Especie = updateDto.Especie;
        fosil.Periodo = updateDto.Periodo;
        return Task.CompletedTask;
    }

    protected override void ActualizarDesdeCreateDto(CreateFosilDto dto, Fosil fosil)
    {
        fosil.Nombre = dto.Nombre;
        fosil.Edad = dto.Edad;
        fosil.Donante = dto.Donante;
        fosil.FechaIngreso = dto.FechaIngreso ?? fosil.FechaIngreso;
        fosil.Ejemplares = dto.Ejemplares;
        fosil.DocumentosRelacionados = dto.DocumentosRelacionados;
        fosil.LaminaExiste = dto.LaminaExiste;
        fosil.TipoFosil = dto.TipoFosil;
        fosil.Especie = dto.Especie;
        fosil.Periodo = dto.Periodo;
    }

    protected override ElementoGeologicoDetailDto MapToDetailDto(Fosil fosil)
    {
        return new ElementoGeologicoDetailDto
        {
            Id = fosil.Id,
            Nombre = fosil.Nombre,
            Codigo = fosil.Codigo,
            TipoElemento = "Fosil",
            Edad = fosil.Edad,
            Donante = fosil.Donante,
            FechaIngreso = fosil.FechaIngreso,
            Ejemplares = fosil.Ejemplares,
            DocumentosRelacionados = fosil.DocumentosRelacionados,
            LaminaExiste = fosil.LaminaExiste,
            EstadoActivo = fosil.EstadoActivo,
            FechaCreacion = fosil.FechaCreacion,
            FechaActualizacion = fosil.FechaActualizacion,
            FechaEliminacion = fosil.FechaEliminacion,
            CreadoPor = fosil.UsuarioCreacion?.NombreCompleto,
            ActualizadoPor = fosil.UsuarioActualizacion?.NombreCompleto,
            EliminadoPor = fosil.UsuarioEliminacion?.NombreCompleto,

            // Relations
            Ubicacion = fosil.Ubicacion != null ? new UbicacionDto
            {
                Id = fosil.Ubicacion.Id,
                Latitud = fosil.Ubicacion.Latitud,
                Longitud = fosil.Ubicacion.Longitud,
                Localidad = fosil.Ubicacion.Localidad,
                Leyenda = fosil.Ubicacion.Leyenda,
                PaisId = fosil.Ubicacion.Provincia?.PaisId,
                ProvinciaId = fosil.Ubicacion.ProvinciaId,
                EstadoActivo = fosil.Ubicacion.EstadoActivo,
                FechaCreacion = fosil.Ubicacion.FechaCreacion,
                FechaActualizacion = fosil.Ubicacion.FechaActualizacion,
                NombrePais = fosil.Ubicacion.Provincia?.Pais?.NombrePais,
                NombreProvincia = fosil.Ubicacion.Provincia?.NombreProvincia
            } : null,
            
            Galeria = fosil.Galeria != null ? new GaleriaElementoGeologicoDto
            {
                Id = fosil.Galeria.Id,
                DetalleGrupo = fosil.Galeria.DetalleGrupo,
                TotalFotos = fosil.Galeria.Fotos?.Count ?? 0,
                Fotos = fosil.Galeria.Fotos?
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
            
            // Fossil-specific properties
            TipoFosil = fosil.TipoFosil,
            Especie = fosil.Especie,
            Periodo = fosil.Periodo,
            
            // Null for other types
            TipoMineral = null,
            LitologiaMineral = null,
            TipoRoca = null,
            LitologiaRoca = null
        };
    }

}