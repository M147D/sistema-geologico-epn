using Servidor_Sistema_Geologia.DTO.Ubicaciones;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class ProvinciaService : IProvinciaService
{
    private readonly IProvinciaRepository _provinciaRepository;
    private readonly IPaisRepository _paisRepository;
    private readonly ILogger<ProvinciaService> _logger;

    public ProvinciaService(
        IProvinciaRepository provinciaRepository, 
        IPaisRepository paisRepository,
        ILogger<ProvinciaService> logger)
    {
        _provinciaRepository = provinciaRepository;
        _paisRepository = paisRepository;
        _logger = logger;
    }

    // 🔍 CONSULTAS
    public async Task<ProvinciaResponseDto> GetByIdAsync(int id)
    {
        try
        {
            var provincia = await _provinciaRepository.GetByIdAsync(id);
            if (provincia == null)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"Provincia no encontrada\"
                };
            }

            return new ProvinciaResponseDto
            {
                Success = true,
                Message = \"Provincia obtenida exitosamente\",
                Data = await MapToProvinciaDetailDtoAsync(provincia)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener provincia por ID: {Id}\", id);
            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener provincia\"
            };
        }
    }

    public async Task<ProvinciaResponseDto> GetByIdWithPaisAsync(int id)
    {
        try
        {
            var provincia = await _provinciaRepository.GetByIdWithPaisAsync(id);
            if (provincia == null)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"Provincia no encontrada\"
                };
            }

            return new ProvinciaResponseDto
            {
                Success = true,
                Message = \"Provincia con información del país obtenida exitosamente\",
                Data = await MapToProvinciaDetailDtoAsync(provincia)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener provincia con país por ID: {Id}\", id);
            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener provincia\"
            };
        }
    }

    public async Task<ProvinciasListResponseDto> GetAllAsync(ProvinciaFilterDto filter)
    {
        try
        {
            var result = await _provinciaRepository.GetAllAsync(filter);

            return new ProvinciasListResponseDto
            {
                Success = true,
                Message = \"Provincias obtenidas exitosamente\",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener lista de provincias\");
            return new ProvinciasListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener provincias\"
            };
        }
    }

    public async Task<ProvinciasListResponseDto> GetAllActiveAsync()
    {
        try
        {
            var provincias = await _provinciaRepository.GetAllActiveAsync();

            var provinciasList = provincias.Select(p => new ProvinciaListDto
            {
                Id = p.Id,
                NombreProvincia = p.NombreProvincia,
                PaisId = p.PaisId,
                NombrePais = p.Pais?.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalUbicaciones = p.Ubicaciones?.Count(u => u.EstadoActivo) ?? 0
            }).ToList();

            return new ProvinciasListResponseDto
            {
                Success = true,
                Message = \"Provincias activas obtenidas exitosamente\",
                Data = new PaginatedProvinciasDto
                {
                    Provincias = provinciasList,
                    TotalCount = provinciasList.Count,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = provinciasList.Count,
                    HasPrevious = false,
                    HasNext = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener provincias activas\");
            return new ProvinciasListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener provincias activas\"
            };
        }
    }

    public async Task<ProvinciasListResponseDto> GetByPaisIdAsync(int paisId, bool includeInactive = false)
    {
        try
        {
            var provincias = await _provinciaRepository.GetByPaisIdAsync(paisId, includeInactive);

            var provinciasList = provincias.Select(p => new ProvinciaListDto
            {
                Id = p.Id,
                NombreProvincia = p.NombreProvincia,
                PaisId = p.PaisId,
                NombrePais = p.Pais?.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalUbicaciones = p.Ubicaciones?.Count(u => u.EstadoActivo) ?? 0
            }).ToList();

            return new ProvinciasListResponseDto
            {
                Success = true,
                Message = \"Provincias del país obtenidas exitosamente\",
                Data = new PaginatedProvinciasDto
                {
                    Provincias = provinciasList,
                    TotalCount = provinciasList.Count,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = provinciasList.Count,
                    HasPrevious = false,
                    HasNext = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener provincias por país: {PaisId}\", paisId);
            return new ProvinciasListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener provincias del país\"
            };
        }
    }

    // ✏️ OPERACIONES CRUD
    public async Task<ProvinciaResponseDto> CreateAsync(CreateProvinciaDto createDto)
    {
        try
        {
            // Verificar que el país existe y está activo
            var pais = await _paisRepository.GetByIdAsync(createDto.PaisId);
            if (pais == null || !pais.EstadoActivo)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"El país especificado no existe o no está activo\",
                    Errors = new List<string> { \"País inválido\" }
                };
            }

            // Verificar si el nombre ya existe en ese país
            if (await _provinciaRepository.ExistsByNameInPaisAsync(createDto.NombreProvincia, createDto.PaisId))
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"Ya existe una provincia con este nombre en el país seleccionado\",
                    Errors = new List<string> { \"Nombre de provincia ya registrado en este país\" }
                };
            }

            var provincia = new Provincia
            {
                NombreProvincia = createDto.NombreProvincia.Trim(),
                PaisId = createDto.PaisId
            };

            var createdProvincia = await _provinciaRepository.CreateAsync(provincia);

            return new ProvinciaResponseDto
            {
                Success = true,
                Message = \"Provincia creada exitosamente\",
                Data = await MapToProvinciaDetailDtoAsync(createdProvincia)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al crear provincia: {NombreProvincia}\", createDto.NombreProvincia);
            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al crear provincia\"
            };
        }
    }

    public async Task<ProvinciaResponseDto> UpdateAsync(int id, UpdateProvinciaDto updateDto)
    {
        try
        {
            var provincia = await _provinciaRepository.GetByIdAsync(id);
            if (provincia == null)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"Provincia no encontrada\"
                };
            }

            // Verificar que el país existe y está activo
            var pais = await _paisRepository.GetByIdAsync(updateDto.PaisId);
            if (pais == null || !pais.EstadoActivo)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"El país especificado no existe o no está activo\",
                    Errors = new List<string> { \"País inválido\" }
                };
            }

            // Verificar si el nuevo nombre ya existe en otro provincia del mismo país
            if (await _provinciaRepository.ExistsByNameInPaisAsync(updateDto.NombreProvincia, updateDto.PaisId, id))
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"Ya existe otra provincia con este nombre en el país seleccionado\",
                    Errors = new List<string> { \"Nombre de provincia ya registrado por otra provincia en este país\" }
                };
            }

            // Actualizar propiedades
            provincia.NombreProvincia = updateDto.NombreProvincia.Trim();
            provincia.PaisId = updateDto.PaisId;
            provincia.EstadoActivo = updateDto.EstadoActivo;

            var updatedProvincia = await _provinciaRepository.UpdateAsync(provincia);

            return new ProvinciaResponseDto
            {
                Success = true,
                Message = \"Provincia actualizada exitosamente\",
                Data = await MapToProvinciaDetailDtoAsync(updatedProvincia)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al actualizar provincia: {Id}\", id);
            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al actualizar provincia\"
            };
        }
    }

    public async Task<ProvinciaResponseDto> DeleteAsync(int id)
    {
        try
        {
            var provincia = await _provinciaRepository.GetByIdAsync(id);
            if (provincia == null)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"Provincia no encontrada\"
                };
            }

            // Verificar si ya está eliminada
            if (!provincia.EstadoActivo)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"La provincia ya ha sido eliminada previamente\"
                };
            }

            // Verificar si tiene ubicaciones activas
            if (await _provinciaRepository.HasActiveUbicacionesAsync(id))
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"No se puede eliminar la provincia porque tiene ubicaciones activas asociadas\",
                    Errors = new List<string> { \"Elimine primero las ubicaciones asociadas\" }
                };
            }

            var success = await _provinciaRepository.DeleteAsync(id);

            if (success)
            {
                return new ProvinciaResponseDto
                {
                    Success = true,
                    Message = \"Provincia eliminada exitosamente (mantiene registros para auditoría)\"
                };
            }

            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error al eliminar la provincia\"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al eliminar provincia: {Id}\", id);
            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al eliminar provincia\"
            };
        }
    }

    public async Task<ProvinciaResponseDto> RestoreAsync(int id)
    {
        try
        {
            var provincia = await _provinciaRepository.GetByIdAsync(id);
            if (provincia == null)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"Provincia no encontrada\"
                };
            }

            // Verificar si ya está activa
            if (provincia.EstadoActivo)
            {
                return new ProvinciaResponseDto
                {
                    Success = false,
                    Message = \"La provincia ya está activa\"
                };
            }

            var success = await _provinciaRepository.RestoreAsync(id);

            if (success)
            {
                return new ProvinciaResponseDto
                {
                    Success = true,
                    Message = \"Provincia restaurada exitosamente\",
                    Data = await MapToProvinciaDetailDtoAsync(await _provinciaRepository.GetByIdAsync(id))
                };
            }

            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error al restaurar la provincia\"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al restaurar provincia: {Id}\", id);
            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al restaurar provincia\"
            };
        }
    }

    // ✅ VALIDACIONES
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _provinciaRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al verificar existencia de provincia: {Id}\", id);
            return false;
        }
    }

    public async Task<bool> ExistsByNameInPaisAsync(string nombreProvincia, int paisId, int? excludeId = null)
    {
        try
        {
            return await _provinciaRepository.ExistsByNameInPaisAsync(nombreProvincia, paisId, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al verificar existencia de provincia por nombre: {NombreProvincia} en país {PaisId}\", nombreProvincia, paisId);
            return false;
        }
    }

    public async Task<bool> CanDeleteAsync(int id)
    {
        try
        {
            return !await _provinciaRepository.HasActiveUbicacionesAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al verificar si se puede eliminar provincia: {Id}\", id);
            return false;
        }
    }

    // 📊 ESTADÍSTICAS Y REPORTES
    public async Task<ProvinciaResponseDto> GetStatsAsync()
    {
        try
        {
            var stats = await _provinciaRepository.GetStatsAsync();

            // Crear un objeto usando ProvinciaDetailDto como contenedor de estadísticas
            var statsDto = new ProvinciaDetailDto
            {
                Id = stats[\"Total\"],
                NombreProvincia = $\"Total: {stats[\"Total\"]}, Activos: {stats[\"Activos\"]}, Inactivos: {stats[\"Inactivos\"]}\",
                EstadoActivo = true,
                FechaCreacion = DateTime.Now
            };

            return new ProvinciaResponseDto
            {
                Success = true,
                Message = \"Estadísticas obtenidas exitosamente\",
                Data = statsDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener estadísticas de provincias\");
            return new ProvinciaResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener estadísticas\"
            };
        }
    }

    public async Task<ProvinciasListResponseDto> GetRecentAsync(int count = 10)
    {
        try
        {
            var provincias = await _provinciaRepository.GetRecentAsync(count);

            var provinciasList = provincias.Select(p => new ProvinciaListDto
            {
                Id = p.Id,
                NombreProvincia = p.NombreProvincia,
                PaisId = p.PaisId,
                NombrePais = p.Pais?.NombrePais,
                FechaCreacion = p.FechaCreacion,
                EstadoActivo = p.EstadoActivo,
                FechaActualizacion = p.FechaActualizacion,
                TotalUbicaciones = p.Ubicaciones?.Count(u => u.EstadoActivo) ?? 0
            }).ToList();

            return new ProvinciasListResponseDto
            {
                Success = true,
                Message = \"Provincias recientes obtenidas exitosamente\",
                Data = new PaginatedProvinciasDto
                {
                    Provincias = provinciasList,
                    TotalCount = provinciasList.Count,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = provinciasList.Count,
                    HasPrevious = false,
                    HasNext = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener provincias recientes\");
            return new ProvinciasListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener provincias recientes\"
            };
        }
    }

    public async Task<ProvinciasListResponseDto> GetInactiveAsync()
    {
        try
        {
            var filter = new ProvinciaFilterDto
            {
                EstadoActivo = false,
                IncludeInactive = true,
                PageSize = 100 // Limite alto para obtener todos los inactivos
            };

            var result = await _provinciaRepository.GetAllAsync(filter);

            return new ProvinciasListResponseDto
            {
                Success = true,
                Message = \"Provincias eliminadas obtenidas exitosamente\",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, \"Error al obtener provincias eliminadas\");
            return new ProvinciasListResponseDto
            {
                Success = false,
                Message = \"Error interno del servidor al obtener provincias eliminadas\"
            };
        }
    }

    // 🔄 MÉTODOS AUXILIARES
    private async Task<ProvinciaDetailDto> MapToProvinciaDetailDtoAsync(Provincia provincia)
    {
        // Obtener información del país si no está cargada
        var pais = provincia.Pais ?? await _paisRepository.GetByIdAsync(provincia.PaisId);

        return new ProvinciaDetailDto
        {
            Id = provincia.Id,
            NombreProvincia = provincia.NombreProvincia,
            PaisId = provincia.PaisId,
            NombrePais = pais?.NombrePais,
            FechaCreacion = provincia.FechaCreacion,
            EstadoActivo = provincia.EstadoActivo,
            FechaActualizacion = provincia.FechaActualizacion,
            TotalUbicaciones = provincia.Ubicaciones?.Count(u => u.EstadoActivo) ?? 0
        };
    }
}