using Servidor_Sistema_Geologia.DTO.Ubicaciones;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class PaisService : IPaisService
{
    private readonly IPaisRepository _paisRepository;
    private readonly ILogger<PaisService> _logger;

    public PaisService(IPaisRepository paisRepository, ILogger<PaisService> logger)
    {
        _paisRepository = paisRepository;
        _logger = logger;
    }

    // 🔍 CONSULTAS
    public async Task<PaisResponseDto> GetByIdAsync(int id)
    {
        try
        {
            var pais = await _paisRepository.GetByIdAsync(id);
            if (pais == null)
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"País no encontrado\"
                };
            }

            return new PaisResponseDto
            {
                Success = true,
                Message = \"País obtenido exitosamente\",
                Data = MapToPaisDetailDto(pais)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener país por ID: {Id}\", id);
            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener país\"
            };
        }
    }

    public async Task<PaisResponseDto> GetByIdWithProvinciasAsync(int id)
    {
        try
        {
            var pais = await _paisRepository.GetByIdWithProvinciasAsync(id);
            if (pais == null)
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"País no encontrado\"
                };
            }

            var paisDetail = MapToPaisDetailDto(pais);
            paisDetail.Provincias = pais.Provincias
                .Where(p => p.EstadoActivo)
                .Select(p => new ProvinciaListDto
                {
                    Id = p.Id,
                    NombreProvincia = p.NombreProvincia,
                    PaisId = p.PaisId,
                    NombrePais = pais.NombrePais,
                    FechaCreacion = p.FechaCreacion,
                    EstadoActivo = p.EstadoActivo,
                    FechaActualizacion = p.FechaActualizacion,
                    TotalUbicaciones = p.Ubicaciones?.Count(u => u.EstadoActivo) ?? 0
                })
                .ToList();

            return new PaisResponseDto
            {
                Success = true,
                Message = \"País con provincias obtenido exitosamente\",
                Data = paisDetail
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener país con provincias por ID: {Id}\", id);
            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener país\"
            };
        }
    }

    public async Task<PaisesListResponseDto> GetAllAsync(PaisFilterDto filter)
    {
        try
        {
            var result = await _paisRepository.GetAllAsync(filter);

            return new PaisesListResponseDto
            {
                Success = true,
                Message = \"Países obtenidos exitosamente\",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener lista de países\");
            return new PaisesListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener países\"
            };
        }
    }

    public async Task<PaisesListResponseDto> GetAllActiveAsync()
    {
        try
        {
            var paises = await _paisRepository.GetAllActiveAsync();

            var paisesList = paises.Select(p => new PaisListDto
            {
                Id = p.Id,
                NombrePais = p.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalProvincias = p.Provincias?.Count(pr => pr.EstadoActivo) ?? 0
            }).ToList();

            return new PaisesListResponseDto
            {
                Success = true,
                Message = \"Países activos obtenidos exitosamente\",
                Data = new PaginatedPaisesDto
                {
                    Paises = paisesList,
                    TotalCount = paisesList.Count,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = paisesList.Count,
                    HasPrevious = false,
                    HasNext = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener países activos\");
            return new PaisesListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener países activos\"
            };
        }
    }

    // ✏️ OPERACIONES CRUD
    public async Task<PaisResponseDto> CreateAsync(CreatePaisDto createDto)
    {
        try
        {
            // Verificar si el nombre ya existe
            if (await _paisRepository.ExistsByNameAsync(createDto.NombrePais))
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"Ya existe un país con este nombre\",
                    Errors = new List<string> { \"Nombre de país ya registrado\" }
                };
            }

            var pais = new Pais
            {
                NombrePais = createDto.NombrePais.Trim()
            };

            var createdPais = await _paisRepository.CreateAsync(pais);

            return new PaisResponseDto
            {
                Success = true,
                Message = \"País creado exitosamente\",
                Data = MapToPaisDetailDto(createdPais)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al crear país: {NombrePais}\", createDto.NombrePais);
            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al crear país\"
            };
        }
    }

    public async Task<PaisResponseDto> UpdateAsync(int id, UpdatePaisDto updateDto)
    {
        try
        {
            var pais = await _paisRepository.GetByIdAsync(id);
            if (pais == null)
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"País no encontrado\"
                };
            }

            // Verificar si el nuevo nombre ya existe en otro país
            if (await _paisRepository.ExistsByNameAsync(updateDto.NombrePais, id))
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"Ya existe otro país con este nombre\",
                    Errors = new List<string> { \"Nombre de país ya registrado por otro país\" }
                };
            }

            // Actualizar propiedades
            pais.NombrePais = updateDto.NombrePais.Trim();
            pais.EstadoActivo = updateDto.EstadoActivo;

            var updatedPais = await _paisRepository.UpdateAsync(pais);

            return new PaisResponseDto
            {
                Success = true,
                Message = \"País actualizado exitosamente\",
                Data = MapToPaisDetailDto(updatedPais)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al actualizar país: {Id}\", id);
            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al actualizar país\"
            };
        }
    }

    public async Task<PaisResponseDto> DeleteAsync(int id)
    {
        try
        {
            var pais = await _paisRepository.GetByIdAsync(id);
            if (pais == null)
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"País no encontrado\"
                };
            }

            // Verificar si ya está eliminado
            if (!pais.EstadoActivo)
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"El país ya ha sido eliminado previamente\"
                };
            }

            // Verificar si tiene provincias activas
            if (await _paisRepository.HasActiveProvinciasAsync(id))
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"No se puede eliminar el país porque tiene provincias activas asociadas\",
                    Errors = new List<string> { \"Elimine primero las provincias asociadas\" }
                };
            }

            var success = await _paisRepository.DeleteAsync(id);

            if (success)
            {
                return new PaisResponseDto
                {
                    Success = true,
                    Message = \"País eliminado exitosamente (mantiene registros para auditoría)\"
                };
            }

            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error al eliminar el país\"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al eliminar país: {Id}\", id);
            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al eliminar país\"
            };
        }
    }

    public async Task<PaisResponseDto> RestoreAsync(int id)
    {
        try
        {
            var pais = await _paisRepository.GetByIdAsync(id);
            if (pais == null)
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"País no encontrado\"
                };
            }

            // Verificar si ya está activo
            if (pais.EstadoActivo)
            {
                return new PaisResponseDto
                {
                    Success = false,
                    Message = \"El país ya está activo\"
                };
            }

            var success = await _paisRepository.RestoreAsync(id);

            if (success)
            {
                return new PaisResponseDto
                {
                    Success = true,
                    Message = \"País restaurado exitosamente\",
                    Data = MapToPaisDetailDto(await _paisRepository.GetByIdAsync(id))
                };
            }

            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error al restaurar el país\"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al restaurar país: {Id}\", id);
            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al restaurar país\"
            };
        }
    }

    // ✅ VALIDACIONES
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _paisRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al verificar existencia de país: {Id}\", id);
            return false;
        }
    }

    public async Task<bool> ExistsByNameAsync(string nombrePais, int? excludeId = null)
    {
        try
        {
            return await _paisRepository.ExistsByNameAsync(nombrePais, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al verificar existencia de país por nombre: {NombrePais}\", nombrePais);
            return false;
        }
    }

    public async Task<bool> CanDeleteAsync(int id)
    {
        try
        {
            return !await _paisRepository.HasActiveProvinciasAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al verificar si se puede eliminar país: {Id}\", id);
            return false;
        }
    }

    // 📊 ESTADÍSTICAS Y REPORTES
    public async Task<PaisResponseDto> GetStatsAsync()
    {
        try
        {
            var stats = await _paisRepository.GetStatsAsync();

            // Crear un objeto usando PaisDetailDto como contenedor de estadísticas
            var statsDto = new PaisDetailDto
            {
                Id = stats[\"Total\"],
                NombrePais = $\"Total: {stats[\"Total\"]}, Activos: {stats[\"Activos\"]}, Inactivos: {stats[\"Inactivos\"]}\",
                EstadoActivo = true,
                FechaCreacion = DateTime.Now
            };

            return new PaisResponseDto
            {
                Success = true,
                Message = \"Estadísticas obtenidas exitosamente\",
                Data = statsDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener estadísticas de países\");
            return new PaisResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener estadísticas\"
            };
        }
    }

    public async Task<PaisesListResponseDto> GetRecentAsync(int count = 10)
    {
        try
        {
            var paises = await _paisRepository.GetRecentAsync(count);

            var paisesList = paises.Select(p => new PaisListDto
            {
                Id = p.Id,
                NombrePais = p.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalProvincias = p.Provincias?.Count(pr => pr.EstadoActivo) ?? 0
            }).ToList();

            return new PaisesListResponseDto
            {
                Success = true,
                Message = \"Países recientes obtenidos exitosamente\",
                Data = new PaginatedPaisesDto
                {
                    Paises = paisesList,
                    TotalCount = paisesList.Count,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = paisesList.Count,
                    HasPrevious = false,
                    HasNext = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener países recientes\");
            return new PaisesListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener países recientes\"
            };
        }
    }

    public async Task<PaisesListResponseDto> GetInactiveAsync()
    {
        try
        {
            var filter = new PaisFilterDto
            {
                EstadoActivo = false,
                IncludeInactive = true,
                PageSize = 100 // Limite alto para obtener todos los inactivos
            };

            var result = await _paisRepository.GetAllAsync(filter);

            return new PaisesListResponseDto
            {
                Success = true,
                Message = \"Países eliminados obtenidos exitosamente\",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener países eliminados\");
            return new PaisesListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener países eliminados\"
            };
        }
    }

    // 🔄 MÉTODOS AUXILIARES
    private static PaisDetailDto MapToPaisDetailDto(Pais pais)
    {
        return new PaisDetailDto
        {
            Id = pais.Id,
            NombrePais = pais.NombrePais,
            FechaCreacion = pais.FechaCreacion,
            EstadoActivo = pais.EstadoActivo,
            FechaActualizacion = pais.FechaActualizacion,
            TotalUbicaciones = pais.Ubicaciones?.Count(u => u.EstadoActivo) ?? 0,
            Provincias = new List<ProvinciaListDto>() // Se llena en GetByIdWithProvinciasAsync si es necesario
        };
    }
}