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
        SistemaGeologicoDbContext context)
        : base(repository, logger, context)
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
            UbicacionId = ubicacion?.Id ?? 1, // Default if no location
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
                PaisId = fosil.Ubicacion.PaisId,
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