using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.DTO.Gallery;
using Servidor_Sistema_Geologia.Galeria;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class FotoElementoService : IFotoElementoService
{
    private readonly IFotoElementoRepository _fotoRepository;
    private readonly ILogger<FotoElementoService> _logger;

    public FotoElementoService(
        IFotoElementoRepository fotoRepository,
        ILogger<FotoElementoService> logger)
    {
        _fotoRepository = fotoRepository;
        _logger = logger;
    }

    public async Task<FotoElementoDto?> GetByIdAsync(int id)
    {
        try
        {
            var foto = await _fotoRepository.GetByIdAsync(id);
            return foto != null ? MapToDto(foto) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener foto con ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<FotoElementoDto>> GetByGaleriaIdAsync(int galeriaId)
    {
        try
        {
            var fotos = await _fotoRepository.GetByGaleriaIdAsync(galeriaId);
            return fotos.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener fotos de galería {GaleriaId}", galeriaId);
            throw;
        }
    }

    public async Task<FotoElementoDto> CreateAsync(CreateFotoElementoDto createDto, int galeriaId, int usuarioId)
    {
        try
        {
            var foto = new FotoElemento
            {
                GaleriaElementosGeologicoId = galeriaId,
                TipoFoto = createDto.TipoFoto,
                DescripcionEspecifica = createDto.DescripcionEspecifica,
                Imagen = createDto.Imagen ?? new byte[0],
                EstadoActivo = true,
                FechaCreacion = DateTime.Now
            };

            var createdFoto = await _fotoRepository.CreateAsync(foto);
            return MapToDto(createdFoto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear foto para galería {GaleriaId}", galeriaId);
            throw;
        }
    }

    public async Task<FotoElementoDto> UpdateAsync(int id, UpdateFotoElementoDto updateDto, int usuarioId)
    {
        try
        {
            var foto = await _fotoRepository.GetByIdAsync(id);
            if (foto == null)
            {
                throw new KeyNotFoundException($"Foto con ID {id} no encontrada");
            }

            // Actualizar propiedades
            foto.TipoFoto = updateDto.TipoFoto;
            foto.DescripcionEspecifica = updateDto.DescripcionEspecifica;

            // Solo actualizar imagen si se proporciona
            if (updateDto.Imagen != null && updateDto.Imagen.Length > 0)
            {
                foto.Imagen = updateDto.Imagen;
            }

            var updatedFoto = await _fotoRepository.UpdateAsync(foto);
            return MapToDto(updatedFoto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar foto con ID {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, int usuarioId)
    {
        try
        {
            if (!await _fotoRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Foto con ID {id} no encontrada");
            }

            await _fotoRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar foto con ID {Id}", id);
            throw;
        }
    }

    public async Task RestoreAsync(int id, int usuarioId)
    {
        try
        {
            if (!await _fotoRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Foto con ID {id} no encontrada");
            }

            await _fotoRepository.RestoreAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar foto con ID {Id}", id);
            throw;
        }
    }

    public async Task<byte[]?> GetImagenAsync(int id)
    {
        try
        {
            var foto = await _fotoRepository.GetByIdAsync(id);
            return foto?.Imagen;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener imagen de foto con ID {Id}", id);
            throw;
        }
    }

    public async Task<int?> GetOrCreateGaleriaIdAsync(int elementoId)
    {
        try
        {
            return await _fotoRepository.GetOrCreateGaleriaIdAsync(elementoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener o crear galería para elemento {ElementoId}", elementoId);
            throw;
        }
    }

    public async Task<ElementoFotosResponseDto?> GetFotosByElementoAsync(int elementoId, bool isAdmin)
    {
        try
        {
            var result = await _fotoRepository.GetGaleriaConFotosAsync(elementoId, soloActivos: !isAdmin);
            if (result == null) return null;

            var (galeriaId, fotos) = result.Value;
            return new ElementoFotosResponseDto
            {
                GaleriaId = galeriaId,
                Fotos = fotos.Select(f => new FotoElementoDto
                {
                    Id = f.Id,
                    GaleriaElementosGeologicoId = f.GaleriaElementosGeologicoId,
                    TipoFoto = f.TipoFoto,
                    DescripcionEspecifica = f.DescripcionEspecifica,
                    FechaCreacion = f.FechaCreacion,
                    FechaActualizacion = f.FechaActualizacion,
                    EstadoActivo = f.EstadoActivo,
                    ImagenUrl = $"/api/foto-elementos/imagen/{f.Id}"
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener fotos del elemento {ElementoId}", elementoId);
            throw;
        }
    }

    // Mapeo manual de entidad a DTO
    private FotoElementoDto MapToDto(FotoElemento foto)
    {
        return new FotoElementoDto
        {
            Id = foto.Id,
            GaleriaElementosGeologicoId = foto.GaleriaElementosGeologicoId,
            TipoFoto = foto.TipoFoto,
            DescripcionEspecifica = foto.DescripcionEspecifica,
            ImagenUrl = $"/api/foto-elementos/imagen/{foto.Id}",
            DetalleGrupoGaleria = foto.Galeria?.DetalleGrupo,
            NombreElementoGeologico = foto.Galeria?.ElementoGeologico?.Nombre
        };
    }
}