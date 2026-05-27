// src/hooks/useGeologiaGisService.js
import { useMemo } from 'react';
import { gisStubs } from '../stubs/gisStubs';
import { useApi } from '../context/ApiContext';

/** @typedef {import('../types/gis.js').GeologiaFeatureDto} GeologiaFeatureDto */
/** @typedef {import('../types/gis.js').ProvinciaFeatureDto} ProvinciaFeatureDto */
/** @typedef {import('../types/gis.js').EcuadorFeatureDto} EcuadorFeatureDto */
/** @typedef {import('../types/gis.js').GeologiaStatisticsDto} GeologiaStatisticsDto */
/** @typedef {import('../types/gis.js').GeoJsonFeatureCollection} GeoJsonFeatureCollection */
/** @typedef {import('../types/gis.js').GeologiaAtPointResponse} GeologiaAtPointResponse */
/** @typedef {import('../types/gis.js').GeologiaStatisticsResponse} GeologiaStatisticsResponse */

const USE_STUBS = import.meta.env.VITE_USE_STUBS === 'true';

/**
 * Hook para consumir los endpoints de datos geologicos (GIS) de la API.
 * Usa la instancia de Axios con JWT de ApiContext.
 */
export const useGeologiaGisService = () => {
  const { api } = useApi();

  return useMemo(() => {
    if (USE_STUBS) return gisStubs;
    return {
    /**
     * Obtiene todas las formaciones geologicas en formato GeoJSON
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getAllGeologia() {
      const response = await api.get('/geologiagis/geologia');
      return response.data;
    },

    /**
     * Obtiene todas las provincias en formato GeoJSON
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getProvincias() {
      const response = await api.get('/geologiagis/provincias');
      return response.data;
    },

    /**
     * Obtiene el contorno de Ecuador en formato GeoJSON
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getEcuador() {
      const response = await api.get('/geologiagis/ecuador');
      return response.data;
    },

    /**
     * Obtiene formaciones geologicas dentro de un area especifica (bounding box)
     * @param {number} minLat
     * @param {number} minLon
     * @param {number} maxLat
     * @param {number} maxLon
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getGeologiaByBounds(minLat, minLon, maxLat, maxLon) {
      const response = await api.get('/geologiagis/geologia/bounds', {
        params: { minLat, minLon, maxLat, maxLon }
      });
      return response.data;
    },

    /**
     * Obtiene formaciones geologicas simplificadas
     * @param {number} [tolerance=0.01]
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getSimplifiedGeologia(tolerance = 0.01) {
      const response = await api.get('/geologiagis/geologia/simplified', {
        params: { tolerance }
      });
      return response.data;
    },

    /**
     * Obtiene informacion geologica en un punto especifico
     * @param {number} lat
     * @param {number} lon
     * @returns {Promise<GeologiaAtPointResponse>}
     */
    async getGeologiaAtPoint(lat, lon) {
      const response = await api.get('/geologiagis/geologia/point', {
        params: { lat, lon }
      });
      return response.data;
    },

    /**
     * Obtiene formaciones geologicas filtradas por litologia
     * @param {string} litologia
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getGeologiaByLitologia(litologia) {
      const response = await api.get(`/geologiagis/geologia/litologia/${litologia}`);
      return response.data;
    },

    /**
     * Obtiene formaciones geologicas filtradas por edad geologica
     * @param {string} edad
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getGeologiaByEdad(edad) {
      const response = await api.get(`/geologiagis/geologia/edad/${edad}`);
      return response.data;
    },

    /**
     * Obtiene formaciones geologicas de una provincia especifica
     * @param {string} provincia
     * @returns {Promise<GeoJsonFeatureCollection>}
     */
    async getGeologiaByProvincia(provincia) {
      const response = await api.get(`/geologiagis/geologia/provincia/${provincia}`);
      return response.data;
    },

    /**
     * Obtiene estadisticas de las formaciones geologicas
     * @returns {Promise<GeologiaStatisticsResponse>}
     */
    async getStatistics() {
      const response = await api.get('/geologiagis/estadisticas');
      return response.data;
    },

    /**
     * Obtiene informacion sobre el servicio GIS
     * @returns {Promise<Object>}
     */
    async getInfo() {
      const response = await api.get('/geologiagis/info');
      return response.data;
    }
    };
  }, [api]);
};