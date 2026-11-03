using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.DTO.Validation;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

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
    protected readonly IUbicacionRepository _ubicaciones;
    protected readonly IUnitOfWork _unitOfWork;
    protected abstract string ElementTypeName { get; }

    /// <summary>Máximo de filas por carga masiva (cada fila pasa por validación con regex).</summary>
    public const int MaxFilasLote = 2000;

    // Servicio sin DbContext: todo acceso a datos pasa por repositorios (REV-01, P-02).
    protected BaseElementoGeologicoService(
        IBaseElementoGeologicoRepository<T> repository,
        ILogger<BaseElementoGeologicoService<T, TCreateDto, TUpdateDto>> logger,
        IUbicacionRepository ubicaciones,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _logger = logger;
        _ubicaciones = ubicaciones;
        _unitOfWork = unitOfWork;
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

            // Register access in history - Ya no necesario con EntidadAuditable
            // await RegisterAccessAsync(elemento.Id, usuarioId, AccionesUsuario.Visualizacion);

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
            elemento.UsuarioCreacionId = usuarioId;

            // Elemento, ubicación (con provincia/país nuevos si hacen falta) y galería
            // se insertan en un solo SaveChanges: o se crean todos o ninguno (REV-01, H-08).
            elemento.Galeria = NuevaGaleria(elemento.Nombre);
            var createdElemento = await _repository.CreateAsync(elemento);

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
            elemento.UsuarioActualizacionId = usuarioId;

            // Update entity
            var updatedElemento = await _repository.UpdateAsync(elemento);

            // Register update in history
            // await RegisterAccessAsync(updatedElemento.Id, usuarioId, AccionesUsuario.Edicion);

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

            elemento.EstadoActivo = false;
            elemento.FechaEliminacion = DateTime.Now;
            elemento.UsuarioEliminacionId = usuarioId;

            await _repository.UpdateAsync(elemento);

            return true;
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
            var restaurado = await _repository.RestoreAsync(id, usuarioId);
            if (!restaurado)
                _logger.LogWarning("{ElementType} con ID {Id} no encontrado para restauración", ElementTypeName, id);
            return restaurado;
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

    // Helper class for validation results
    protected class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ValidationResult Success() => new() { IsValid = true };
        public static ValidationResult Error(string message) => new() { IsValid = false, ErrorMessage = message };
    }

    // 🌍 UBICACIÓN: se arma como grafo y se guarda con el elemento.
    // El país se guarda solo en la provincia (REV-01, H-05); sin provincia se usa la
    // "Desconocida" de ese país. Las coordenadas se validan y se guardan como número (H-06).
    protected async Task<Ubicacion?> ResolverUbicacionAsync(TCreateDto createDto)
    {
        if (createDto.UbicacionId.HasValue)
        {
            return await _ubicaciones.GetByIdAsync(createDto.UbicacionId.Value)
                ?? throw new ArgumentException($"La ubicación {createDto.UbicacionId.Value} no existe");
        }

        var datos = createDto.Ubicacion;
        if (datos == null) return null;

        Provincia provincia;
        if (!string.IsNullOrWhiteSpace(datos.NombrePais))
        {
            var pais = await _ubicaciones.ObtenerOCrearPaisAsync(datos.NombrePais);
            provincia = string.IsNullOrWhiteSpace(datos.NombreProvincia)
                ? await _ubicaciones.ObtenerProvinciaDesconocidaAsync(pais)
                : await _ubicaciones.ObtenerOCrearProvinciaAsync(datos.NombreProvincia, pais);
        }
        else
        {
            provincia = await _ubicaciones.ObtenerProvinciaDesconocidaGlobalAsync();
        }

        return new Ubicacion
        {
            Provincia = provincia,
            Latitud = Coordenada.Parse(datos.Latitud, -90, 90),
            Longitud = Coordenada.Parse(datos.Longitud, -180, 180),
            Localidad = string.IsNullOrWhiteSpace(datos.Localidad) ? "Desconocida" : datos.Localidad,
            Leyenda = string.IsNullOrWhiteSpace(datos.Leyenda) ? "Sin leyenda" : datos.Leyenda,
            EstadoActivo = true,
            FechaCreacion = DateTime.Now
        };
    }

    protected static Galeria.GaleriaElementoGeologico NuevaGaleria(string nombreElemento) => new()
    {
        DetalleGrupo = $"Galeria de {nombreElemento}",
        EstadoActivo = true,
        FechaCreacion = DateTime.Now
    };

    // 📦 CARGA MASIVA (Excel): todas las filas o ninguna (REV-01, H-14).
    // Misma regla que tenía el cliente: si el código ya existe en un elemento activo del
    // mismo tipo se actualiza; si pertenece a otro tipo se crea con sufijo ".D{fila}".
    public virtual async Task<LoteResultadoDto> CargarLoteAsync(IReadOnlyList<TCreateDto> filas, int usuarioId)
    {
        var resultado = new LoteResultadoDto { TotalFilas = filas.Count };
        if (filas.Count == 0 || filas.Count > MaxFilasLote)
        {
            resultado.Errores.Add(new LoteErrorDto { Fila = 0, Mensaje = $"El lote debe tener entre 1 y {MaxFilasLote} filas" });
            return resultado;
        }

        try
        {
            await _unitOfWork.EnTransaccionAsync(async () =>
            {
                for (var i = 0; i < filas.Count; i++)
                {
                    var fila = filas[i];
                    var existente = await _repository.FindAnyByCodigoAsync(fila.Codigo);
                    if (existente is T mismoTipo)
                    {
                        if (!mismoTipo.EstadoActivo)
                        {
                            resultado.Errores.Add(new LoteErrorDto { Fila = i + 1, Codigo = fila.Codigo, Mensaje = "El código pertenece a un elemento eliminado" });
                            continue;
                        }
                        ActualizarDesdeCreateDto(fila, mismoTipo);
                        mismoTipo.UsuarioActualizacionId = usuarioId;
                        await _repository.UpdateAsync(mismoTipo);
                        resultado.Actualizados++;
                        continue;
                    }
                    if (existente != null)
                        fila.Codigo = $"{fila.Codigo}.D{i + 1}";

                    var validacion = await ValidateCreateAsync(fila);
                    if (!validacion.IsValid)
                    {
                        resultado.Errores.Add(new LoteErrorDto { Fila = i + 1, Codigo = fila.Codigo, Mensaje = validacion.ErrorMessage });
                        continue;
                    }
                    var elemento = await MapCreateDtoToEntityAsync(fila);
                    elemento.UsuarioCreacionId = usuarioId;
                    elemento.Galeria = NuevaGaleria(elemento.Nombre);
                    await _repository.CreateAsync(elemento);
                    resultado.Creados++;
                }

                if (resultado.Errores.Count > 0)
                    _unitOfWork.MarcarParaRevertir();
                return true;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en la carga masiva de {ElementType}", ElementTypeName);
            resultado.Errores.Add(new LoteErrorDto { Fila = 0, Mensaje = "Error al guardar el lote; no se guardó ninguna fila" });
        }

        resultado.Success = resultado.Errores.Count == 0;
        if (!resultado.Success) { resultado.Creados = 0; resultado.Actualizados = 0; }
        return resultado;
    }

    /// <summary>Copia los campos editables de una fila de carga masiva sobre un elemento existente (sin tocar la ubicación).</summary>
    protected abstract void ActualizarDesdeCreateDto(TCreateDto dto, T elemento);
}
