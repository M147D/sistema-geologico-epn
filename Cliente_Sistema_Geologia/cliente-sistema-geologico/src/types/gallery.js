/**
 * @file Definiciones de tipos para galería y fotos
 * Espeja los DTOs del backend: Servidor_Sistema_Geologia/DTO/Gallery/
 */

/**
 * Foto de un elemento geológico
 * Backend: PhotoElementDto.cs
 *
 * @typedef {Object} PhotoElementDto
 * @property {number} id
 * @property {number} galeriaElementosGeologicoId
 * @property {number} tipoFoto - Enum TipoFoto
 * @property {string} descripcionEspecifica
 * @property {string} imagenUrl
 * @property {string|null} detalleGrupoGaleria
 * @property {string|null} nombreElementoGeologico
 */

/**
 * Galería de un elemento geológico con sus fotos
 * Backend: GalleryGeologicalElementDto.cs
 *
 * @typedef {Object} GalleryGeologicalElementDto
 * @property {number} id
 * @property {string|null} detalleGrupo
 * @property {number} totalFotos
 * @property {PhotoElementDto[]} fotos
 */

export {};
