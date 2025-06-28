using Servidor_Sistema_Geologia.DTO.ElementosGeologicos;
using Servidor_Sistema_Geologia.DTO.Ubicaciones;
using Servidor_Sistema_Geologia.ElementosGeologicos;
using Servidor_Sistema_Geologia.Repositories.Interfaces;
using Servidor_Sistema_Geologia.Services.Interfaces;

namespace Servidor_Sistema_Geologia.Services.Implementation;

public class ElementoGeologicoService : IElementoGeologicoService
{
    private readonly IElementoGeologicoRepository _elementoRepository;
    private readonly ILogger<ElementoGeologicoService> _logger;

    public ElementoGeologicoService(
        IElementoGeologicoRepository elementoRepository,
        ILogger<ElementoGeologicoService> logger)
    {
        _elementoRepository = elementoRepository;
        _logger = logger;
    }

    // 🔍 CONSULTAS GENERALES
    public async Task<ElementoGeologicoResponseDto> GetByIdAsync(int id, int usuarioId)
    {
        try
        {
            var elemento = await _elementoRepository.GetByIdAsync(id);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Elemento geológico no encontrado",
                    Errors = new List<string> { $"No se encontró un elemento con ID: {id}" }
                };
            }

            // Registrar visualización en el historial
            await _elementoRepository.RegisterAccessAsync(id, usuarioId, AccionesUsuario.Visualizacion);

            var elementoDto = MapToDetailDto(elemento);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Elemento geológico obtenido exitosamente",
                Data = elementoDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elemento geológico con ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> GetByIdWithDetailsAsync(int id, int usuarioId)
    {
        try
        {
            var elemento = await _elementoRepository.GetByIdWithDetailsAsync(id);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Elemento geológico no encontrado",
                    Errors = new List<string> { $"No se encontró un elemento con ID: {id}" }
                };
            }

            // Registrar visualización en el historial
            await _elementoRepository.RegisterAccessAsync(id, usuarioId, AccionesUsuario.Visualizacion);

            var elementoDto = MapToDetailDto(elemento);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Elemento geológico con detalles obtenido exitosamente",
                Data = elementoDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elemento geológico con detalles ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> GetByCodigoAsync(string codigo, int usuarioId)
    {
        try
        {
            var elemento = await _elementoRepository.GetByCodigoAsync(codigo);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Elemento geológico no encontrado",
                    Errors = new List<string> { $"No se encontró un elemento con código: {codigo}" }
                };
            }

            // Registrar visualización en el historial
            await _elementoRepository.RegisterAccessAsync(elemento.Id, usuarioId, AccionesUsuario.Visualizacion);

            var elementoDto = MapToDetailDto(elemento);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Elemento geológico obtenido exitosamente",
                Data = elementoDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elemento geológico con código: {Codigo}", codigo);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetAllAsync(ElementoGeologicoFilterDto filter)
    {
        try
        {
            var elementos = await _elementoRepository.GetAllAsync(filter);

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Elementos geológicos obtenidos exitosamente",
                Data = elementos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de elementos geológicos");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetAllActiveAsync()
    {
        try
        {
            var elementos = await _elementoRepository.GetAllActiveAsync();
            var elementosDto = elementos.Select(MapToListDto).ToList();

            var paginatedResult = new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = elementosDto,
                TotalCount = elementosDto.Count,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = elementosDto.Count,
                HasPrevious = false,
                HasNext = false
            };

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Elementos geológicos activos obtenidos exitosamente",
                Data = paginatedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos geológicos activos");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    // 🔍 CONSULTAS ESPECÍFICAS POR TIPO
    public async Task<ElementosGeologicosListResponseDto> GetFosilesAsync(ElementoGeologicoFilterDto filter)
    {
        try
        {
            var fosiles = await _elementoRepository.GetFosilesAsync(filter);

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Fósiles obtenidos exitosamente",
                Data = fosiles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener fósiles");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetMineralesAsync(ElementoGeologicoFilterDto filter)
    {
        try
        {
            var minerales = await _elementoRepository.GetMineralesAsync(filter);

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Minerales obtenidos exitosamente",
                Data = minerales
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener minerales");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetRocasAsync(ElementoGeologicoFilterDto filter)
    {
        try
        {
            var rocas = await _elementoRepository.GetRocasAsync(filter);

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Rocas obtenidas exitosamente",
                Data = rocas
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener rocas");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    // 🔍 CONSULTAS POR UBICACIÓN
    public async Task<ElementosGeologicosListResponseDto> GetByUbicacionAsync(int ubicacionId)
    {
        try
        {
            var elementos = await _elementoRepository.GetByUbicacionAsync(ubicacionId);
            var elementosDto = elementos.Select(MapToListDto).ToList();

            var paginatedResult = new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = elementosDto,
                TotalCount = elementosDto.Count,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = elementosDto.Count,
                HasPrevious = false,
                HasNext = false
            };

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = $"Elementos geológicos de la ubicación {ubicacionId} obtenidos exitosamente",
                Data = paginatedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos por ubicación: {UbicacionId}", ubicacionId);
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetByPaisAsync(int paisId)
    {
        try
        {
            var elementos = await _elementoRepository.GetByPaisAsync(paisId);
            var elementosDto = elementos.Select(MapToListDto).ToList();

            var paginatedResult = new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = elementosDto,
                TotalCount = elementosDto.Count,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = elementosDto.Count,
                HasPrevious = false,
                HasNext = false
            };

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = $"Elementos geológicos del país {paisId} obtenidos exitosamente",
                Data = paginatedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos por país: {PaisId}", paisId);
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetByProvinciaAsync(int provinciaId)
    {
        try
        {
            var elementos = await _elementoRepository.GetByProvinciaAsync(provinciaId);
            var elementosDto = elementos.Select(MapToListDto).ToList();

            var paginatedResult = new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = elementosDto,
                TotalCount = elementosDto.Count,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = elementosDto.Count,
                HasPrevious = false,
                HasNext = false
            };

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = $"Elementos geológicos de la provincia {provinciaId} obtenidos exitosamente",
                Data = paginatedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos por provincia: {ProvinciaId}", provinciaId);
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    // ✏️ OPERACIONES CRUD - FÓSILES
    public async Task<ElementoGeologicoResponseDto> CreateFosilAsync(CreateFosilDto createDto)
    {
        try
        {
            // Validar código único
            if (await _elementoRepository.ExistsByCodigoAsync(createDto.Codigo))
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Código ya existe",
                    Errors = new List<string> { $"Ya existe un elemento con el código: {createDto.Codigo}" }
                };
            }

            var fosil = new Fosil
            {
                Nombre = createDto.Nombre,
                Edad = createDto.Edad,
                Donante = createDto.Donante,
                FechaIngreso = createDto.FechaIngreso ?? throw new ArgumentNullException(nameof(createDto.FechaIngreso), "FechaIngreso no puede ser nulo"),
                Codigo = createDto.Codigo,
                Ejemplares = createDto.Ejemplares,
                DocumentosRelacionados = createDto.DocumentosRelacionados,
                LaminaExiste = createDto.LaminaExiste,
                UbicacionId = createDto.UbicacionId ?? throw new ArgumentNullException(nameof(createDto.UbicacionId), "UbicacionId no puede ser nulo"),
                TipoFosil = createDto.TipoFosil,
                Especie = createDto.Especie,
                Periodo = createDto.Periodo
            };

            var fosilCreado = await _elementoRepository.CreateFosilAsync(fosil);

            // Registrar creación en el historial
            await _elementoRepository.RegisterAccessAsync(fosilCreado.Id, createDto.UsuarioId, AccionesUsuario.Creacion);

            var fosilDto = MapToDetailDto(fosilCreado);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Fósil creado exitosamente",
                Data = fosilDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear fósil: {Nombre}", createDto.Nombre);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al crear el fósil" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> UpdateFosilAsync(int id, UpdateFosilDto updateDto)
    {
        try
        {
            var fosilExistente = await _elementoRepository.GetByIdAsync<Fosil>(id);
            if (fosilExistente == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Fósil no encontrado",
                    Errors = new List<string> { $"No se encontró un fósil con ID: {id}" }
                };
            }

            // Validar código único (excluyendo el elemento actual)
            if (await _elementoRepository.ExistsByCodigoAsync(updateDto.Codigo, id))
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Código ya existe",
                    Errors = new List<string> { $"Ya existe otro elemento con el código: {updateDto.Codigo}" }
                };
            }

            // Actualizar propiedades
            fosilExistente.Nombre = updateDto.Nombre;
            fosilExistente.Edad = updateDto.Edad;
            fosilExistente.Donante = updateDto.Donante;
            if (updateDto.FechaIngreso.HasValue)
                fosilExistente.FechaIngreso = updateDto.FechaIngreso.Value;
            else
                throw new ArgumentNullException(nameof(updateDto.FechaIngreso), "FechaIngreso no puede ser nulo");
            fosilExistente.Codigo = updateDto.Codigo;
            fosilExistente.Ejemplares = updateDto.Ejemplares;
            fosilExistente.DocumentosRelacionados = updateDto.DocumentosRelacionados;
            fosilExistente.LaminaExiste = updateDto.LaminaExiste;
            if (updateDto.UbicacionId.HasValue)
                fosilExistente.UbicacionId = updateDto.UbicacionId.Value;
            else
                throw new ArgumentNullException(nameof(updateDto.UbicacionId), "UbicacionId no puede ser nulo");
            fosilExistente.TipoFosil = updateDto.TipoFosil;
            fosilExistente.Especie = updateDto.Especie;
            fosilExistente.Periodo = updateDto.Periodo;

            var fosilActualizado = await _elementoRepository.UpdateFosilAsync(fosilExistente);

            // Registrar edición en el historial
            await _elementoRepository.RegisterAccessAsync(id, updateDto.UsuarioId, AccionesUsuario.Edicion);

            var fosilDto = MapToDetailDto(fosilActualizado);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Fósil actualizado exitosamente",
                Data = fosilDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar fósil ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al actualizar el fósil" }
            };
        }
    }

    // ✏️ OPERACIONES CRUD - MINERALES
    public async Task<ElementoGeologicoResponseDto> CreateMineralAsync(CreateMineralDto createDto)
    {
        try
        {
            // Validar código único
            if (await _elementoRepository.ExistsByCodigoAsync(createDto.Codigo))
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Código ya existe",
                    Errors = new List<string> { $"Ya existe un elemento con el código: {createDto.Codigo}" }
                };
            }

            var mineral = new Mineral
            {
                Nombre = createDto.Nombre,
                Edad = createDto.Edad,
                Donante = createDto.Donante,
                FechaIngreso = createDto.FechaIngreso ?? throw new ArgumentNullException(nameof(createDto.FechaIngreso), "FechaIngreso no puede ser nulo"),
                Codigo = createDto.Codigo,
                Ejemplares = createDto.Ejemplares,
                DocumentosRelacionados = createDto.DocumentosRelacionados,
                LaminaExiste = createDto.LaminaExiste,
                UbicacionId = createDto.UbicacionId ?? throw new ArgumentNullException(nameof(createDto.UbicacionId), "UbicacionId no puede ser nulo"),
                TipoMineral = createDto.TipoMineral,
                Litologia = createDto.Litologia
            };

            var mineralCreado = await _elementoRepository.CreateMineralAsync(mineral);

            // Registrar creación en el historial
            await _elementoRepository.RegisterAccessAsync(mineralCreado.Id, createDto.UsuarioId, AccionesUsuario.Creacion);

            var mineralDto = MapToDetailDto(mineralCreado);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Mineral creado exitosamente",
                Data = mineralDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear mineral: {Nombre}", createDto.Nombre);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al crear el mineral" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> UpdateMineralAsync(int id, UpdateMineralDto updateDto)
    {
        try
        {
            var mineralExistente = await _elementoRepository.GetByIdAsync<Mineral>(id);
            if (mineralExistente == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Mineral no encontrado",
                    Errors = new List<string> { $"No se encontró un mineral con ID: {id}" }
                };
            }

            // Validar código único (excluyendo el elemento actual)
            if (await _elementoRepository.ExistsByCodigoAsync(updateDto.Codigo, id))
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Código ya existe",
                    Errors = new List<string> { $"Ya existe otro elemento con el código: {updateDto.Codigo}" }
                };
            }

            // Actualizar propiedades
            mineralExistente.Nombre = updateDto.Nombre;
            mineralExistente.Edad = updateDto.Edad;
            mineralExistente.Donante = updateDto.Donante;
            if (updateDto.FechaIngreso.HasValue)
                mineralExistente.FechaIngreso = updateDto.FechaIngreso.Value;
            else
                throw new ArgumentNullException(nameof(updateDto.FechaIngreso), "FechaIngreso no puede ser nulo");
            mineralExistente.Codigo = updateDto.Codigo;
            mineralExistente.Ejemplares = updateDto.Ejemplares;
            mineralExistente.DocumentosRelacionados = updateDto.DocumentosRelacionados;
            mineralExistente.LaminaExiste = updateDto.LaminaExiste;
            if (updateDto.UbicacionId.HasValue)
                mineralExistente.UbicacionId = updateDto.UbicacionId.Value;
            else
                throw new ArgumentNullException(nameof(updateDto.UbicacionId), "UbicacionId no puede ser nulo");
            mineralExistente.TipoMineral = updateDto.TipoMineral;
            mineralExistente.Litologia = updateDto.Litologia;

            var mineralActualizado = await _elementoRepository.UpdateMineralAsync(mineralExistente);

            // Registrar edición en el historial
            await _elementoRepository.RegisterAccessAsync(id, updateDto.UsuarioId, AccionesUsuario.Edicion);

            var mineralDto = MapToDetailDto(mineralActualizado);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Mineral actualizado exitosamente",
                Data = mineralDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar mineral ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al actualizar el mineral" }
            };
        }
    }

    // ✏️ OPERACIONES CRUD - ROCAS
    public async Task<ElementoGeologicoResponseDto> CreateRocaAsync(CreateRocaDto createDto)
    {
        try
        {
            // Validar código único
            if (await _elementoRepository.ExistsByCodigoAsync(createDto.Codigo))
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Código ya existe",
                    Errors = new List<string> { $"Ya existe un elemento con el código: {createDto.Codigo}" }
                };
            }

            var roca = new Roca
            {
                Nombre = createDto.Nombre,
                Edad = createDto.Edad,
                Donante = createDto.Donante,
                FechaIngreso = createDto.FechaIngreso ?? throw new ArgumentNullException(nameof(createDto.FechaIngreso), "FechaIngreso no puede ser nulo"),
                Codigo = createDto.Codigo,
                Ejemplares = createDto.Ejemplares,
                DocumentosRelacionados = createDto.DocumentosRelacionados,
                LaminaExiste = createDto.LaminaExiste,
                UbicacionId = createDto.UbicacionId ?? throw new ArgumentNullException(nameof(createDto.UbicacionId), "UbicacionId no puede ser nulo"),
                TipoRoca = createDto.TipoRoca,
                Litologia = createDto.Litologia
            };

            var rocaCreada = await _elementoRepository.CreateRocaAsync(roca);

            // Registrar creación en el historial
            await _elementoRepository.RegisterAccessAsync(rocaCreada.Id, createDto.UsuarioId, AccionesUsuario.Creacion);

            var rocaDto = MapToDetailDto(rocaCreada);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Roca creada exitosamente",
                Data = rocaDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear roca: {Nombre}", createDto.Nombre);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al crear la roca" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> UpdateRocaAsync(int id, UpdateRocaDto updateDto)
    {
        try
        {
            var rocaExistente = await _elementoRepository.GetByIdAsync<Roca>(id);
            if (rocaExistente == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Roca no encontrada",
                    Errors = new List<string> { $"No se encontró una roca con ID: {id}" }
                };
            }

            // Validar código único (excluyendo el elemento actual)
            if (await _elementoRepository.ExistsByCodigoAsync(updateDto.Codigo, id))
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Código ya existe",
                    Errors = new List<string> { $"Ya existe otro elemento con el código: {updateDto.Codigo}" }
                };
            }

            // Actualizar propiedades
            rocaExistente.Nombre = updateDto.Nombre;
            rocaExistente.Edad = updateDto.Edad;
            rocaExistente.Donante = updateDto.Donante;
            if (updateDto.FechaIngreso.HasValue)
                rocaExistente.FechaIngreso = updateDto.FechaIngreso.Value;
            else
                throw new ArgumentNullException(nameof(updateDto.FechaIngreso), "FechaIngreso no puede ser nulo");
            rocaExistente.Codigo = updateDto.Codigo;
            rocaExistente.Ejemplares = updateDto.Ejemplares;
            rocaExistente.DocumentosRelacionados = updateDto.DocumentosRelacionados;
            rocaExistente.LaminaExiste = updateDto.LaminaExiste;
            if (updateDto.UbicacionId.HasValue)
                rocaExistente.UbicacionId = updateDto.UbicacionId.Value;
            else
                throw new ArgumentNullException(nameof(updateDto.UbicacionId), "UbicacionId no puede ser nulo");
            rocaExistente.TipoRoca = updateDto.TipoRoca;
            rocaExistente.Litologia = updateDto.Litologia;

            var rocaActualizada = await _elementoRepository.UpdateRocaAsync(rocaExistente);

            // Registrar edición en el historial
            await _elementoRepository.RegisterAccessAsync(id, updateDto.UsuarioId, AccionesUsuario.Edicion);

            var rocaDto = MapToDetailDto(rocaActualizada);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Roca actualizada exitosamente",
                Data = rocaDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar roca ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al actualizar la roca" }
            };
        }
    }

    // ✏️ OPERACIONES COMUNES
    public async Task<ElementoGeologicoResponseDto> DeleteAsync(int id, int usuarioId)
    {
        try
        {
            var elemento = await _elementoRepository.GetByIdAsync(id);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Elemento geológico no encontrado",
                    Errors = new List<string> { $"No se encontró un elemento con ID: {id}" }
                };
            }

            if (!elemento.EstadoActivo)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "El elemento ya está eliminado",
                    Errors = new List<string> { "El elemento geológico ya se encuentra eliminado" }
                };
            }

            var eliminado = await _elementoRepository.DeleteAsync(id);
            if (!eliminado)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "No se pudo eliminar el elemento",
                    Errors = new List<string> { "Ha ocurrido un error al eliminar el elemento" }
                };
            }

            // Registrar eliminación en el historial
            await _elementoRepository.RegisterAccessAsync(id, usuarioId, AccionesUsuario.Eliminacion);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Elemento geológico eliminado exitosamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar elemento geológico ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al eliminar el elemento" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> RestoreAsync(int id, int usuarioId)
    {
        try
        {
            var elemento = await _elementoRepository.GetByIdAsync(id);
            if (elemento == null)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "Elemento geológico no encontrado",
                    Errors = new List<string> { $"No se encontró un elemento con ID: {id}" }
                };
            }

            if (elemento.EstadoActivo)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "El elemento ya está activo",
                    Errors = new List<string> { "El elemento geológico ya se encuentra activo" }
                };
            }

            var restaurado = await _elementoRepository.RestoreAsync(id);
            if (!restaurado)
            {
                return new ElementoGeologicoResponseDto
                {
                    Success = false,
                    Message = "No se pudo restaurar el elemento",
                    Errors = new List<string> { "Ha ocurrido un error al restaurar el elemento" }
                };
            }

            // Registrar restauración en el historial como creación
            await _elementoRepository.RegisterAccessAsync(id, usuarioId, AccionesUsuario.Creacion);

            var elementoRestaurado = await _elementoRepository.GetByIdAsync(id);
            var elementoDto = MapToDetailDto(elementoRestaurado!);

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Elemento geológico restaurado exitosamente",
                Data = elementoDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar elemento geológico ID: {Id}", id);
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al restaurar el elemento" }
            };
        }
    }

    // ✅ VALIDACIONES
    public async Task<bool> ExistsAsync(int id)
    {
        return await _elementoRepository.ExistsAsync(id);
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
    {
        return await _elementoRepository.ExistsByCodigoAsync(codigo, excludeId);
    }

    public async Task<bool> CanDeleteAsync(int id)
    {
        // Un elemento se puede eliminar si no tiene dependencias críticas
        // Por ejemplo, si no tiene fotos en proceso o referencias importantes
        var hasGaleria = await _elementoRepository.HasGaleriaAsync(id);
        var hasFotos = await _elementoRepository.HasFotosAsync(id);
        
        // Por ahora permitimos eliminar elementos aunque tengan galería/fotos
        // ya que es un soft delete
        return true;
    }

    // 📊 ESTADÍSTICAS Y REPORTES
    public async Task<ElementoGeologicoResponseDto> GetStatsAsync()
    {
        try
        {
            var stats = await _elementoRepository.GetStatsAsync();

            // Crear un DTO temporal para estadísticas
            var statsDto = new ElementoGeologicoDetailDto
            {
                Id = 0,
                Nombre = "Estadísticas del Sistema"
            };

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Estadísticas obtenidas exitosamente",
                Data = statsDto // Esto es temporal, idealmente se debería crear un DTO específico para stats
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas");
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado al obtener estadísticas" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetRecentAsync(int count = 10)
    {
        try
        {
            var elementos = await _elementoRepository.GetRecentAsync(count);
            var elementosDto = elementos.Select(MapToListDto).ToList();

            var paginatedResult = new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = elementosDto,
                TotalCount = elementosDto.Count,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = elementosDto.Count,
                HasPrevious = false,
                HasNext = false
            };

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Elementos geológicos recientes obtenidos exitosamente",
                Data = paginatedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos recientes");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetInactiveAsync()
    {
        try
        {
            var filter = new ElementoGeologicoFilterDto
            {
                EstadoActivo = false,
                IncludeInactive = true,
                PageSize = 1000 // Para obtener todos los inactivos
            };

            var elementos = await _elementoRepository.GetAllAsync(filter);

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = "Elementos geológicos eliminados obtenidos exitosamente",
                Data = elementos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos inactivos");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementoGeologicoResponseDto> GetDashboardStatsAsync()
    {
        try
        {
            var dashboardStats = await _elementoRepository.GetDashboardStatsAsync();

            // Crear un DTO temporal para el dashboard
            var dashboardDto = new ElementoGeologicoDetailDto
            {
                Id = 0,
                Nombre = "Dashboard del Sistema"
            };

            return new ElementoGeologicoResponseDto
            {
                Success = true,
                Message = "Estadísticas del dashboard obtenidas exitosamente",
                Data = dashboardDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del dashboard");
            return new ElementoGeologicoResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    // 🔍 BÚSQUEDAS AVANZADAS
    public async Task<ElementosGeologicosListResponseDto> SearchAsync(string searchTerm, ElementoGeologicoFilterDto? filter = null)
    {
        try
        {
            filter ??= new ElementoGeologicoFilterDto();
            
            // Aplicar el término de búsqueda a múltiples campos
            filter.Nombre = searchTerm;
            // También se podría buscar en otros campos como código, donante, etc.

            var elementos = await _elementoRepository.GetAllAsync(filter);

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = $"Búsqueda completada para '{searchTerm}'",
                Data = elementos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda: {SearchTerm}", searchTerm);
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado en la búsqueda" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetByDateRangeAsync(DateTime desde, DateTime hasta)
    {
        try
        {
            var elementos = await _elementoRepository.GetElementosByDateRangeAsync(desde, hasta);
            var elementosDto = elementos.Select(MapToListDto).ToList();

            var paginatedResult = new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = elementosDto,
                TotalCount = elementosDto.Count,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = elementosDto.Count,
                HasPrevious = false,
                HasNext = false
            };

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = $"Elementos del rango {desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy} obtenidos exitosamente",
                Data = paginatedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos por rango de fechas");
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    public async Task<ElementosGeologicosListResponseDto> GetByDonanteAsync(string donante)
    {
        try
        {
            var elementos = await _elementoRepository.GetElementosByDonanteAsync(donante);
            var elementosDto = elementos.Select(MapToListDto).ToList();

            var paginatedResult = new PaginatedElementosGeologicosDto
            {
                ElementosGeologicos = elementosDto,
                TotalCount = elementosDto.Count,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = elementosDto.Count,
                HasPrevious = false,
                HasNext = false
            };

            return new ElementosGeologicosListResponseDto
            {
                Success = true,
                Message = $"Elementos del donante '{donante}' obtenidos exitosamente",
                Data = paginatedResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener elementos por donante: {Donante}", donante);
            return new ElementosGeologicosListResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Errors = new List<string> { "Ha ocurrido un error inesperado" }
            };
        }
    }

    // 🔄 HISTORIAL
    public async Task<List<HistorialAcceso>> GetHistorialAsync(int elementoId)
    {
        return await _elementoRepository.GetHistorialAsync(elementoId);
    }

    public async Task RegisterAccessAsync(int elementoId, int usuarioId, AccionesUsuario accion)
    {
        await _elementoRepository.RegisterAccessAsync(elementoId, usuarioId, accion);
    }

    // 🗺️ MÉTODOS DE MAPEO
    private ElementoGeologicoDetailDto MapToDetailDto(ElementoGeologico elemento)
    {
        var dto = new ElementoGeologicoDetailDto
        {
            Id = elemento.Id,
            Nombre = elemento.Nombre,
            Codigo = elemento.Codigo,
            TipoElemento = elemento.GetType().Name,
            Edad = elemento.Edad,
            Donante = elemento.Donante,
            FechaIngreso = elemento.FechaIngreso,
            Ejemplares = elemento.Ejemplares,
            DocumentosRelacionados = elemento.DocumentosRelacionados,
            LaminaExiste = elemento.LaminaExiste,
            EstadoActivo = elemento.EstadoActivo,
            FechaCreacion = elemento.FechaCreacion,
            FechaActualizacion = elemento.FechaActualizacion
        };

        // Mapear ubicación si existe
        if (elemento.Ubicacion != null)
        {
            dto.Ubicacion = new UbicacionDto
            {
                Id = elemento.Ubicacion.Id,
                Latitud = elemento.Ubicacion.Latitud,
                Longitud = elemento.Ubicacion.Longitud,
                Localidad = elemento.Ubicacion.Localidad,
                Leyenda = elemento.Ubicacion.Leyenda,
                EstadoActivo = elemento.Ubicacion.EstadoActivo,
                // Mapear país y provincia si existen
                NombrePais = elemento.Ubicacion.Pais?.NombrePais,
                NombreProvincia = elemento.Ubicacion.Provincia?.NombreProvincia
            };
        }

        // Mapear galería si existe
        if (elemento.Galeria != null)
        {
            dto.Galeria = new GaleriaElementoGeologicoDto
            {
                Id = elemento.Galeria.Id,
                DetalleGrupo = elemento.Galeria.DetalleGrupo,
                TotalFotos = elemento.Galeria.Fotos?.Count ?? 0
            };
        }

        // Mapear propiedades específicas por tipo
        switch (elemento)
        {
            case Fosil fosil:
                dto.TipoFosil = fosil.TipoFosil;
                dto.Especie = fosil.Especie;
                dto.Periodo = fosil.Periodo;
                break;
            case Mineral mineral:
                dto.TipoMineral = mineral.TipoMineral;
                dto.LitologiaMineral = mineral.Litologia;
                break;
            case Roca roca:
                dto.TipoRoca = roca.TipoRoca;
                dto.LitologiaRoca = roca.Litologia;
                break;
        }

        return dto;
    }

    private ElementoGeologicoListDto MapToListDto(ElementoGeologico elemento)
    {
        var dto = new ElementoGeologicoListDto
        {
            Id = elemento.Id,
            Nombre = elemento.Nombre,
            Codigo = elemento.Codigo,
            TipoElemento = elemento.GetType().Name,
            Edad = elemento.Edad,
            Donante = elemento.Donante,
            FechaIngreso = elemento.FechaIngreso,
            Ejemplares = elemento.Ejemplares,
            LaminaExiste = elemento.LaminaExiste,
            EstadoActivo = elemento.EstadoActivo,
            FechaCreacion = elemento.FechaCreacion,
            FechaActualizacion = elemento.FechaActualizacion,
            
            // Información de ubicación
            Localidad = elemento.Ubicacion?.Localidad,
            NombrePais = elemento.Ubicacion?.Pais?.NombrePais,
            NombreProvincia = elemento.Ubicacion?.Provincia?.NombreProvincia,
            
            // Información de galería
            TotalFotos = elemento.Galeria?.Fotos?.Count ?? 0,
            TieneGaleria = elemento.Galeria != null
        };

        // Mapear información específica por tipo
        switch (elemento)
        {
            case Fosil fosil:
                dto.TipoEspecifico = fosil.TipoFosil.ToString();
                dto.Especie = fosil.Especie;
                dto.Periodo = fosil.Periodo;
                break;
            case Mineral mineral:
                dto.TipoEspecifico = mineral.TipoMineral.ToString();
                dto.Litologia = mineral.Litologia;
                break;
            case Roca roca:
                dto.TipoEspecifico = roca.TipoRoca.ToString();
                dto.Litologia = roca.Litologia;
                break;
        }

        return dto;
    }
}
