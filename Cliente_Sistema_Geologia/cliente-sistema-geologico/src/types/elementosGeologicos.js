/**
 * @file Definiciones de tipos para elementos geológicos
 * Espeja los DTOs del backend: Servidor_Sistema_Geologia/DTO/ElementosGeologicos/
 */

// ─── Tipos de respuesta de lista ────────────────────────────────────────────

/**
 * Elemento geológico en vista de lista (resumen)
 * Backend: ElementoGeologicoListDto.cs
 *
 * @typedef {Object} ElementoGeologicoListDto
 * @property {number} id
 * @property {string} nombre
 * @property {string} codigo
 * @property {string} tipoElemento - "Fosil" | "Mineral" | "Roca"
 * @property {string} edad
 * @property {string} donante
 * @property {string|null} fechaIngreso - ISO 8601
 * @property {number|null} ejemplares
 * @property {boolean} laminaExiste
 * @property {boolean} estadoActivo
 * @property {string} fechaCreacion - ISO 8601
 * @property {string|null} fechaActualizacion - ISO 8601
 * @property {number|null} ubicacionId
 * @property {string|null} localidad
 * @property {string|null} nombrePais
 * @property {string|null} nombreProvincia
 * @property {string|null} latitud
 * @property {string|null} longitud
 * @property {string|null} tipoEspecifico - "Vertebrado", "Silicato", "Ígnea", etc.
 * @property {string|null} especie - Solo para fósiles
 * @property {string|null} periodo - Solo para fósiles
 * @property {string|null} litologia - Para minerales y rocas
 * @property {number} totalFotos
 * @property {boolean} tieneGaleria
 */

// ─── Tipos de respuesta de detalle ──────────────────────────────────────────

/**
 * Elemento geológico en vista de detalle (completo)
 * Backend: ElementoGeologicoDetailDto.cs
 *
 * @typedef {Object} ElementoGeologicoDetailDto
 * @property {number} id
 * @property {string} nombre
 * @property {string} codigo
 * @property {string} tipoElemento - "Fosil" | "Mineral" | "Roca"
 * @property {string} edad
 * @property {string} donante
 * @property {string|null} fechaIngreso - ISO 8601
 * @property {number|null} ejemplares
 * @property {string|null} documentosRelacionados
 * @property {boolean} laminaExiste
 * @property {boolean} estadoActivo
 * @property {string} fechaCreacion - ISO 8601
 * @property {string|null} fechaActualizacion - ISO 8601
 * @property {import('./locations').UbicacionDto|null} ubicacion
 * @property {import('./gallery').GalleryGeologicalElementDto|null} galeria
 * @property {number|null} tipoFosil - Enum SubtipoFosil (0=Desconocido, 1=Vertebrado, 2=Invertebrado, 3=Paleobotánica, 4=Icnofósil, 5=Microfósil)
 * @property {string|null} especie
 * @property {string|null} periodo
 * @property {number|null} tipoMineral - Enum SubtipoMineral
 * @property {string|null} litologiaMineral
 * @property {number|null} tipoRoca - Enum SubtipoRoca
 * @property {string|null} litologiaRoca
 */

// ─── Tipos de creación ──────────────────────────────────────────────────────

/**
 * Campos base para crear un elemento geológico
 * Backend: CreateElementoGeologicoBaseDto.cs
 *
 * @typedef {Object} CreateElementoGeologicoBaseDto
 * @property {string} nombre
 * @property {string} edad
 * @property {string} donante
 * @property {string|null} fechaIngreso - ISO 8601
 * @property {string} codigo
 * @property {number} ejemplares
 * @property {string} [documentosRelacionados]
 * @property {boolean} laminaExiste
 * @property {number} [ubicacionId]
 * @property {number} usuarioId
 * @property {UbicacionCompletoDto} ubicacion
 */

/**
 * DTO para crear un fósil
 * Backend: CreateFosilDto.cs
 *
 * @typedef {CreateElementoGeologicoBaseDto & CreateFosilFields} CreateFosilDto
 */

/**
 * @typedef {Object} CreateFosilFields
 * @property {number} tipoFosil - Enum SubtipoFosil
 * @property {string} especie
 * @property {string} periodo
 */

/**
 * DTO para crear un mineral
 * Backend: CreateMineralDto.cs
 *
 * @typedef {CreateElementoGeologicoBaseDto & CreateMineralFields} CreateMineralDto
 */

/**
 * @typedef {Object} CreateMineralFields
 * @property {number} tipoMineral - Enum SubtipoMineral
 * @property {string} litologia
 */

/**
 * DTO para crear una roca
 * Backend: CreateRocaDto.cs
 *
 * @typedef {CreateElementoGeologicoBaseDto & CreateRocaFields} CreateRocaDto
 */

/**
 * @typedef {Object} CreateRocaFields
 * @property {number} tipoRoca - Enum SubtipoRoca
 * @property {string} litologia
 */

// ─── Tipos de actualización ─────────────────────────────────────────────────

/**
 * Campos base para actualizar un elemento geológico
 * Backend: UpdateElementoGeologicoBaseDto.cs
 *
 * @typedef {Object} UpdateElementoGeologicoBaseDto
 * @property {string} nombre
 * @property {string} edad
 * @property {string} donante
 * @property {string|null} fechaIngreso - ISO 8601
 * @property {string} codigo
 * @property {number} ejemplares
 * @property {string} [documentosRelacionados]
 * @property {boolean} laminaExiste
 * @property {number} [ubicacionId]
 * @property {number} usuarioId
 */

/**
 * DTO para actualizar un fósil
 * Backend: UpdateFosilDto.cs
 *
 * @typedef {UpdateElementoGeologicoBaseDto & UpdateFosilFields} UpdateFosilDto
 */

/**
 * @typedef {Object} UpdateFosilFields
 * @property {number} tipoFosil - Enum SubtipoFosil
 * @property {string} especie
 * @property {string} periodo
 */

/**
 * DTO para actualizar un mineral
 * Backend: UpdateMineralDto.cs
 *
 * @typedef {UpdateElementoGeologicoBaseDto & UpdateMineralFields} UpdateMineralDto
 */

/**
 * @typedef {Object} UpdateMineralFields
 * @property {number} tipoMineral - Enum SubtipoMineral
 * @property {string} litologia
 */

/**
 * DTO para actualizar una roca
 * Backend: UpdateRocaDto.cs
 *
 * @typedef {UpdateElementoGeologicoBaseDto & UpdateRocaFields} UpdateRocaDto
 */

/**
 * @typedef {Object} UpdateRocaFields
 * @property {number} tipoRoca - Enum SubtipoRoca
 * @property {string} litologia
 */

// ─── Tipos auxiliares (ubicación y galería para creación) ───────────────────

/**
 * Información de ubicación completa para crear/actualizar un elemento
 * Backend: UbicacionCompletoDto.cs
 *
 * @typedef {Object} UbicacionCompletoDto
 * @property {string} nombrePais
 * @property {string} [nombreProvincia]
 * @property {string} localidad
 * @property {string} latitud
 * @property {string} longitud
 * @property {string} leyenda
 */

/**
 * Información de galería completa para crear un elemento
 * Backend: GaleriaCompletoDto.cs
 *
 * @typedef {Object} GaleriaCompletoDto
 * @property {string} detalleGrupo
 * @property {FotoCompletoDto[]} fotos
 */

/**
 * Información de una foto para crear un elemento
 * Backend: FotoCompletoDto.cs
 *
 * @typedef {Object} FotoCompletoDto
 * @property {Uint8Array} imagen
 * @property {string} tipoFoto
 * @property {string} descripcionEspecifica
 */

// ─── Filtros ────────────────────────────────────────────────────────────────

/**
 * Filtros para buscar elementos geológicos
 * Backend: ElementoGeologicoFilterDto.cs
 *
 * @typedef {Object} ElementoGeologicoFilterDto
 * @property {string} [nombre]
 * @property {string} [codigo]
 * @property {string} [donante]
 * @property {string} [edad]
 * @property {string} [tipoElemento] - "Fosil" | "Mineral" | "Roca"
 * @property {number} [paisId]
 * @property {number} [provinciaId]
 * @property {number} [ubicacionId]
 * @property {string} [localidad]
 * @property {string} [fechaIngresoDesde] - ISO 8601
 * @property {string} [fechaIngresoHasta] - ISO 8601
 * @property {string} [fechaCreacionDesde] - ISO 8601
 * @property {string} [fechaCreacionHasta] - ISO 8601
 * @property {boolean} [laminaExiste]
 * @property {number} [ejemplaresMin]
 * @property {number} [ejemplaresMax]
 * @property {boolean} [estadoActivo]
 * @property {boolean} [includeInactive]
 * @property {number} [tipoFosil] - Enum SubtipoFosil
 * @property {number} [tipoMineral] - Enum SubtipoMineral
 * @property {number} [tipoRoca] - Enum SubtipoRoca
 * @property {string} [especie]
 * @property {string} [periodo]
 * @property {string} [litologia]
 * @property {number} [page]
 * @property {number} [pageSize]
 * @property {string} [sortBy]
 * @property {boolean} [sortDescending]
 * @property {boolean} [includeUbicacion]
 * @property {boolean} [includeGaleria]
 * @property {boolean} [includeFotos]
 */

// ─── Paginación y wrappers de respuesta ─────────────────────────────────────

/**
 * Resultado paginado de elementos geológicos
 * Backend: PaginatedElementosGeologicosDto.cs
 *
 * @typedef {Object} PaginatedElementosGeologicosDto
 * @property {ElementoGeologicoListDto[]} elementosGeologicos
 * @property {number} totalCount
 * @property {number} totalPages
 * @property {number} currentPage
 * @property {number} pageSize
 * @property {boolean} hasPrevious
 * @property {boolean} hasNext
 * @property {Object<string, number>} tipoStats - Conteo por tipo de elemento
 * @property {Object<string, number>} estadoStats - Conteo por estado
 */

/**
 * Respuesta wrapper para un solo elemento geológico
 * Backend: ElementoGeologicoSingleResponseDto.cs
 *
 * @typedef {Object} ElementoGeologicoSingleResponseDto
 * @property {boolean} success
 * @property {string} message
 * @property {ElementoGeologicoDetailDto|null} data
 * @property {string[]} errors
 */

/**
 * Respuesta wrapper para lista de elementos geológicos
 * Backend: ElementosGeologicosListResponseDto.cs
 *
 * @typedef {Object} ElementosGeologicosListResponseDto
 * @property {boolean} success
 * @property {string} message
 * @property {PaginatedElementosGeologicosDto|null} data
 * @property {string[]} errors
 */

export {};