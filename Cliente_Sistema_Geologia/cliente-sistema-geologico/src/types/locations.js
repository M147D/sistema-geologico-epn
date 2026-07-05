/**
 * @file Definiciones de tipos para ubicaciones (países, provincias)
 * Espeja los DTOs del backend: Servidor_Sistema_Geologia/DTO/Locations/ y DTO/Ubicaciones/
 */

/**
 * País en vista de lista
 * Backend: CountryListDto.cs
 *
 * @typedef {Object} CountryListDto
 * @property {number} id
 * @property {string} nombrePais
 * @property {string} fechaCreacion - ISO 8601
 * @property {boolean} estadoActivo
 * @property {string|null} fechaActualizacion - ISO 8601
 * @property {number} totalProvincias
 */

/**
 * Provincia en vista de lista
 * Backend: ProvinceListDto.cs
 *
 * @typedef {Object} ProvinceListDto
 * @property {number} id
 * @property {string} nombreProvincia
 * @property {number} paisId
 * @property {string|null} nombrePais
 * @property {string} fechaCreacion - ISO 8601
 * @property {boolean} estadoActivo
 * @property {string|null} fechaActualizacion - ISO 8601
 * @property {number} totalUbicaciones
 */

/**
 * Ubicación completa con relaciones
 * Backend: UbicacionDto.cs
 *
 * @typedef {Object} UbicacionDto
 * @property {number} id
 * @property {string|null} latitud
 * @property {string|null} longitud
 * @property {string|null} localidad
 * @property {string|null} leyenda
 * @property {boolean} estadoActivo
 * @property {string} fechaCreacion - ISO 8601
 * @property {string|null} fechaActualizacion - ISO 8601
 * @property {string|null} nombrePais
 * @property {string|null} nombreProvincia
 * @property {number|null} paisId
 * @property {number|null} provinciaId
 */

// ─── Paginación y wrappers de respuesta ─────────────────────────────────────

/**
 * Resultado paginado de países
 * Backend: PaginatedCountriesDto.cs
 *
 * @typedef {Object} PaginatedCountriesDto
 * @property {CountryListDto[]} paises
 * @property {number} totalCount
 * @property {number} totalPages
 * @property {number} currentPage
 * @property {number} pageSize
 * @property {boolean} hasPrevious
 * @property {boolean} hasNext
 */

/**
 * Resultado paginado de provincias
 * Backend: PaginatedProvincesDto.cs
 *
 * @typedef {Object} PaginatedProvincesDto
 * @property {ProvinceListDto[]} provincias
 * @property {number} totalCount
 * @property {number} totalPages
 * @property {number} currentPage
 * @property {number} pageSize
 * @property {boolean} hasPrevious
 * @property {boolean} hasNext
 */

/**
 * Respuesta wrapper para lista de países
 * Backend: CountriesListResponseDto.cs
 *
 * @typedef {Object} CountriesListResponseDto
 * @property {boolean} success
 * @property {string} message
 * @property {PaginatedCountriesDto|null} data
 * @property {string[]} errors
 */

/**
 * Respuesta wrapper para lista de provincias
 * Backend: ProvincesListResponseDto.cs
 *
 * @typedef {Object} ProvincesListResponseDto
 * @property {boolean} success
 * @property {string} message
 * @property {PaginatedProvincesDto|null} data
 * @property {string[]} errors
 */

export {};
