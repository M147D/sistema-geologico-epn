// src/hooks/useGeologiaService.js
import { useMemo } from 'react';
import { gisStubs } from '../stubs/gisStubs';
import { api } from '../lib/apiClient';

/** @typedef {import('../types/gis.js').GeologiaFeatureDto} GeologiaFeatureDto */
/** @typedef {import('../types/gis.js').ProvinciaFeatureDto} ProvinciaFeatureDto */
/** @typedef {import('../types/gis.js').EcuadorFeatureDto} EcuadorFeatureDto */
/** @typedef {import('../types/gis.js').GeoJsonFeatureCollection} GeoJsonFeatureCollection */

const USE_STUBS = import.meta.env.VITE_USE_STUBS === 'true';

/**
 * Hook para consumir los endpoints de datos geologicos (GIS) de la API.
 * Usa la instancia de Axios con JWT de apiClient.
 */
export const useGeologiaService = () => {
  return useMemo(() => {
    if (USE_STUBS) return gisStubs;
    return {
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
    };
  }, []);
};