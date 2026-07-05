/**
 * @file Definiciones de tipos para la capa GIS
 * Espeja los DTOs del backend: Servidor_Sistema_Geologia/DTO/GIS/
 */

/**
 * Propiedades de una formación geológica (sin geometría)
 * Backend: GeologiaFeatureDto.cs
 *
 * @typedef {Object} GeologiaFeatureDto
 * @property {number} gid
 * @property {string|null} codA
 * @property {string|null} leyenda
 * @property {string|null} edad
 * @property {string|null} litologia
 * @property {string|null} colorRgb - Color RGB desde QML, formato "rgb(r, g, b)"
 * @property {string|null} labelQml - Nombre de la formación desde QML
 */

/**
 * Propiedades de una provincia (sin geometría)
 * Backend: ProvinciaFeatureDto.cs
 *
 * @typedef {Object} ProvinciaFeatureDto
 * @property {number} gid
 * @property {string|null} codigoProvincia
 * @property {string|null} nombreProvincia
 * @property {number|null} areaKm2
 * @property {number|null} poblacion
 */

/**
 * Propiedades del contorno de Ecuador (sin geometría)
 * Backend: EcuadorFeatureDto.cs
 *
 * @typedef {Object} EcuadorFeatureDto
 * @property {number} gid
 * @property {string|null} nombrePais
 * @property {string|null} isoCode
 * @property {number|null} areaKm2
 */

/**
 * Feature GeoJSON individual con propiedades tipadas
 *
 * @typedef {Object} GeoJsonFeature
 * @property {'Feature'} type
 * @property {number} id
 * @property {Object|null} geometry - Geometría GeoJSON (Polygon, MultiPolygon, etc.)
 * @property {GeologiaFeatureDto|ProvinciaFeatureDto|EcuadorFeatureDto} properties
 */

/**
 * FeatureCollection GeoJSON
 *
 * @typedef {Object} GeoJsonFeatureCollection
 * @property {'FeatureCollection'} type
 * @property {GeoJsonFeature[]} features
 */

export {};