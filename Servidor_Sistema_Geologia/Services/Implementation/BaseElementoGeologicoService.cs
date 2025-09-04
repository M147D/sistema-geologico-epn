using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Servidor_Sistema_Geologia.Services.Implementation;

/// <summary>
/// Base service implementation for ElementoGeologico inheritance hierarchy
/// Provides common business logic and validation for all geological elements
/// </summary>
/// <typeparam name="T">The geological element type (Fosil, Mineral, Roca)</typeparam>
/// <typeparam name="TCreateDto">The create DTO type for the element</typeparam>
/// <typeparam name="TUpdateDto">The update DTO type for the element</typeparam>
public abstract class BaseElementoGeologicoService<T, TCreateDto, TUpdateDto> : 
    IBaseElementoGeologicoService<T, TCreateDto, TUpdateDto>
    where T : ElementoGeologico, new()
    where TCreateDto : CreateElementoGeologicoBaseDto
    where TUpdateDto : UpdateElementoGeologicoBaseDto
{
    protected readonly IBaseElementoGeologicoRepository<T> _repository;
    protected readonly ILogger<BaseElementoGeologicoService<T, TCreateDto, TUpdateDto>> _logger;
    protected readonly GestorSistemaGeologia _context;
    protected abstract string ElementTypeName { get; }

    protected BaseElementoGeologicoService(
        IBaseElementoGeologicoRepository<T> repository,
        ILogger<BaseElementoGeologicoService<T, TCreateDto, TUpdateDto>> logger,
        GestorSistemaGeologia context)
    {
        _repository = repository;
        _logger = logger;
        _context = context;
    }

    // 🔍 CONSULTAS BASICAS
    public virtual async Task<ElementoGeologicoResponseDto> GetByIdAsync(int id, int usuarioId)
    {
        try
        {
            var elemento = await _repository.GetByIdWithDetailsAsync(id);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = $"{ElementTypeName} no encontrado"
                };
            }

            // Register access in history
            await RegisterAccessAsync(elemento.Id, usuarioId, AccionesUsuario.Visualizacion);

            var elementoDto = MapToDetailDto(elemento);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = $"{ElementTypeName} obtenido exitosamente",
                Data = elementoDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener {ElementType} con ID: {Id}", ElementTypeName, id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = $"Ha ocurrido un error inesperado al obtener el {ElementTypeName.ToLower()}"
            };
        }
    }

    public virtual async Task<PaginatedElementosGeologicosDto> GetAllAsync(ElementoGeologicoFilterDto filter)
    {
        try
        {
            return await _repository.GetAllAsync(filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de {ElementType}", ElementTypeName);
            return new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = new List<ElementoGeologicoListDto>(),
                TotalCount = 0,
                TotalPages = 0,
                CurrentPage = filter.Page,
                PageSize = filter.PageSize,
                HasPrevious = false,
                HasNext = false
            };
        }
    }

    // ✏️ OPERACIONES CRUD
    public virtual async Task<ElementoGeologicoResponseDto> CreateAsync(TCreateDto createDto, int usuarioId)
    {
        try
        {
            // Validate business rules
            var validationResult = await ValidateCreateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = validationResult.ErrorMessage
                };
            }

            // Map DTO to entity
            var elemento = await MapCreateDtoToEntityAsync(createDto);

            // Create entity
            var createdElemento = await _repository.CreateAsync(elemento);

            // Register creation in history
            await RegisterAccessAsync(createdElemento.Id, usuarioId, AccionesUsuario.Creacion);

            var elementoDto = MapToDetailDto(createdElemento);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = $"{ElementTypeName} creado exitosamente",
                Data = elementoDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear {ElementType}: {Nombre}", ElementTypeName, createDto.Nombre);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = $"Ha ocurrido un error inesperado al crear el {ElementTypeName.ToLower()}"
            };
        }
    }

    public virtual async Task<ElementoGeologicoResponseDto> UpdateAsync(int id, TUpdateDto updateDto, int usuarioId)
    {
        try
        {
            var elemento = await _repository.GetByIdAsync(id);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = $"{ElementTypeName} no encontrado"
                };
            }

            // Validate business rules
            var validationResult = await ValidateUpdateAsync(id, updateDto);
            if (!validationResult.IsValid)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = validationResult.ErrorMessage
                };
            }

            // Map DTO to entity
            await MapUpdateDtoToEntityAsync(updateDto, elemento);

            // Update entity
            var updatedElemento = await _repository.UpdateAsync(elemento);

            // Register update in history
            await RegisterAccessAsync(updatedElemento.Id, usuarioId, AccionesUsuario.Edicion);

            var elementoDto = MapToDetailDto(updatedElemento);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = $"{ElementTypeName} actualizado exitosamente",
                Data = elementoDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar {ElementType} con ID: {Id}", ElementTypeName, id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = $"Ha ocurrido un error inesperado al actualizar el {ElementTypeName.ToLower()}"
            };
        }
    }

    public virtual async Task<bool> DeleteAsync(int id, int usuarioId)
    {
        try
        {
            var elemento = await _repository.GetByIdAsync(id);
            if (elemento == null)
            {
                _logger.LogWarning("{ElementType} con ID {Id} no encontrado para eliminación", ElementTypeName, id);
                return false;
            }

            var success = await _repository.DeleteAsync(id);
            
            if (success)
            {
                // Register deletion in history
                await RegisterAccessAsync(id, usuarioId, AccionesUsuario.Eliminacion);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar {ElementType} con ID: {Id}", ElementTypeName, id);
            return false;
        }
    }

    public virtual async Task<bool> RestoreAsync(int id, int usuarioId)
    {
        try
        {
            var success = await _repository.RestoreAsync(id);
            
            if (success)
            {
                // Register restoration in history
                await RegisterAccessAsync(id, usuarioId, AccionesUsuario.Edicion);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar {ElementType} con ID: {Id}", ElementTypeName, id);
            return false;
        }
    }

    // ✅ VALIDACIONES
    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }

    public virtual async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
    {
        return await _repository.ExistsByCodigoAsync(codigo, excludeId);
    }

    // 📊 ESTADISTICAS
    public virtual async Task<Dictionary<string, int>> GetStatsAsync()
    {
        try
        {
            return new Dictionary<string, int>
            {
                ["Total"] = await _repository.GetTotalCountAsync(),
                ["Activos"] = await _repository.GetActiveCountAsync(),
                ["Inactivos"] = await _repository.GetTotalCountAsync() - await _repository.GetActiveCountAsync()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de {ElementType}", ElementTypeName);
            return new Dictionary<string, int>();
        }
    }

    public virtual async Task<List<T>> GetRecentAsync(int count = 10)
    {
        return await _repository.GetRecentAsync(count);
    }

    // 🎯 ABSTRACT METHODS FOR TYPE-SPECIFIC LOGIC
    protected abstract Task<ValidationResult> ValidateCreateAsync(TCreateDto createDto);
    protected abstract Task<ValidationResult> ValidateUpdateAsync(int id, TUpdateDto updateDto);
    protected abstract Task<T> MapCreateDtoToEntityAsync(TCreateDto createDto);
    protected abstract Task MapUpdateDtoToEntityAsync(TUpdateDto updateDto, T elemento);
    protected abstract ElementoGeologicoDetailDto MapToDetailDto(T elemento);
    protected abstract Task RegisterAccessAsync(int elementoId, int usuarioId, AccionesUsuario accion);

    // Helper class for validation results
    protected class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ValidationResult Success() => new() { IsValid = true };
        public static ValidationResult Error(string message) => new() { IsValid = false, ErrorMessage = message };
    }

    // 🌍 HELPER METHODS FOR LOCATION MANAGEMENT (adapted from Iteration1)
    protected virtual async Task<Pais?> ObtenerOCrearPaisAsync(string? nombrePais)
    {
        if (string.IsNullOrWhiteSpace(nombrePais))
            return null;

        // First try to find existing país
        var pais = await _context.Paises
            .FirstOrDefaultAsync(p => p.NombrePais == nombrePais && p.EstadoActivo);

        if (pais == null)
        {
            // Create new país with explicit transaction handling
            pais = new Pais 
            { 
                NombrePais = nombrePais,
                EstadoActivo = true,
                FechaCreacion = DateTime.Now
            };
            
            // Add and save immediately to get the ID
            _context.Paises.Add(pais);
            await _context.SaveChangesAsync();
            
            // Ensure entity is tracked properly
            _context.Entry(pais).Reload();
            
            _logger.LogInformation("Creado nuevo país: {NombrePais} con ID: {Id}", nombrePais, pais.Id);
        }

        return pais;
    }

    protected virtual async Task<Provincia?> ObtenerOCrearProvinciaAsync(string? nombreProvincia, int? paisId)
    {
        if (string.IsNullOrWhiteSpace(nombreProvincia) || !paisId.HasValue)
            return null;

        try
        {
            // Ensure the País exists in the database before creating Provincia
            var paisExists = await _context.Paises.AnyAsync(p => p.Id == paisId.Value && p.EstadoActivo);
            if (!paisExists)
            {
                _logger.LogWarning("País con ID {PaisId} no encontrado para crear provincia {NombreProvincia}", paisId, nombreProvincia);
                return null;
            }

            var provincia = await _context.Provincias
                .FirstOrDefaultAsync(p => p.NombreProvincia == nombreProvincia && 
                                        p.PaisId == paisId.Value && 
                                        p.EstadoActivo);

            if (provincia == null)
            {
                provincia = new Provincia
                {
                    NombreProvincia = nombreProvincia,
                    PaisId = paisId.Value,
                    EstadoActivo = true,
                    FechaCreacion = DateTime.Now
                };
                await _context.Provincias.AddAsync(provincia);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Creada nueva provincia: {NombreProvincia} para país ID: {PaisId}", nombreProvincia, paisId);
            }

            return provincia;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear provincia {NombreProvincia} para país ID: {PaisId}", nombreProvincia, paisId);
            return null; // Return null instead of throwing to prevent breaking the main operation
        }
    }

    protected virtual async Task<Ubicacion> CrearUbicacionAsync(TCreateDto createDto, int? paisId = null, int? provinciaId = null)
    {
        try
        {
            // Create location with basic data - foreign keys will be set if valid
            var ubicacion = new Ubicacion
            {
                Latitud = ObtenerValorUbicacion(createDto, "Latitud") ?? "0.0",
                Longitud = ObtenerValorUbicacion(createDto, "Longitud") ?? "0.0",
                Localidad = ObtenerValorUbicacion(createDto, "Localidad") ?? "Desconocida",
                Leyenda = ObtenerValorUbicacion(createDto, "Leyenda") ?? "Sin leyenda",
                PaisId = paisId,
                ProvinciaId = provinciaId,
                EstadoActivo = true,
                FechaCreacion = DateTime.Now
            };

            await _context.Ubicaciones.AddAsync(ubicacion);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Creada nueva ubicación: {Localidad} con País ID: {PaisId}, Provincia ID: {ProvinciaId}", 
                ubicacion.Localidad, paisId, provinciaId);

            return ubicacion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear ubicación para {Localidad} con País ID: {PaisId}, Provincia ID: {ProvinciaId}", 
                ObtenerValorUbicacion(createDto, "Localidad"), paisId, provinciaId);
            
            // Create location without foreign keys as fallback
            var ubicacionFallback = new Ubicacion
            {
                Latitud = ObtenerValorUbicacion(createDto, "Latitud") ?? "0.0",
                Longitud = ObtenerValorUbicacion(createDto, "Longitud") ?? "0.0",
                Localidad = ObtenerValorUbicacion(createDto, "Localidad") ?? "Desconocida",
                Leyenda = ObtenerValorUbicacion(createDto, "Leyenda") ?? "Sin leyenda",
                PaisId = null,
                ProvinciaId = null,
                EstadoActivo = true,
                FechaCreacion = DateTime.Now
            };

            await _context.Ubicaciones.AddAsync(ubicacionFallback);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Creada ubicación de respaldo sin relaciones geográficas: {Localidad}", ubicacionFallback.Localidad);

            return ubicacionFallback;
        }
    }

    protected virtual async Task<Galeria.GaleriaElementoGeologico> CrearGaleriaAsync(string nombreElemento)
    {
        var galeria = new Galeria.GaleriaElementoGeologico
        {
            DetalleGrupo = $"Galería de {nombreElemento}"
        };

        await _context.GaleriaElementosGeologicos.AddAsync(galeria);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Creada nueva galería para elemento: {NombreElemento}", nombreElemento);

        return galeria;
    }

    // Helper method to get location values from DTO using reflection
    private string? ObtenerValorUbicacion(TCreateDto dto, string propertyName)
    {
        var property = typeof(TCreateDto).GetProperty(propertyName);
        return property?.GetValue(dto)?.ToString();
    }
}