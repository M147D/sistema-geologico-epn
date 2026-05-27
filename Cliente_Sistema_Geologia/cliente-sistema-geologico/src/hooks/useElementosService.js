// src/hooks/useElementosService.js
import { useMemo } from 'react';
import { elementosStubs } from '../stubs/elementosStubs';
import { useApi } from '../context/ApiContext';

/** @typedef {import('../types/elementosGeologicos').ElementoGeologicoListDto} ElementoGeologicoListDto */
/** @typedef {import('../types/elementosGeologicos').ElementoGeologicoDetailDto} ElementoGeologicoDetailDto */
/** @typedef {import('../types/elementosGeologicos').ElementoGeologicoSingleResponseDto} ElementoGeologicoSingleResponseDto */
/** @typedef {import('../types/elementosGeologicos').ElementoGeologicoFilterDto} ElementoGeologicoFilterDto */
/** @typedef {import('../types/elementosGeologicos').CreateFosilDto} CreateFosilDto */
/** @typedef {import('../types/elementosGeologicos').CreateMineralDto} CreateMineralDto */
/** @typedef {import('../types/elementosGeologicos').CreateRocaDto} CreateRocaDto */
/** @typedef {import('../types/elementosGeologicos').UpdateFosilDto} UpdateFosilDto */
/** @typedef {import('../types/elementosGeologicos').UpdateMineralDto} UpdateMineralDto */
/** @typedef {import('../types/elementosGeologicos').UpdateRocaDto} UpdateRocaDto */
/** @typedef {import('../types/locations').CountryListDto} CountryListDto */
/** @typedef {import('../types/locations').ProvinceListDto} ProvinceListDto */
/** @typedef {import('../types/gallery').PhotoElementDto} PhotoElementDto */

const USE_STUBS = import.meta.env.VITE_USE_STUBS === 'true';

/**
 * Hook para gestionar las operaciones relacionadas con elementos geológicos
 */
export const useElementosService = () => {
  const { api } = useApi();

  return useMemo(() => {
    if (USE_STUBS) return elementosStubs;

    return ({
    /**
     * Obtiene todos los fósiles
     * @returns {Promise<(ElementoGeologicoListDto & {tipo: string})[]>} Lista de fósiles
     */
    async getFosiles() {
      const response = await api.get('/elementos-geologicos/fosiles');
      return Array.isArray(response.data)
        ? response.data.map(fosil => ({ ...fosil, tipo: 'fosil', tipoElemento: 'Fosil' }))
        : [];
    },

    /**
     * Obtiene todas las rocas
     * @returns {Promise<(ElementoGeologicoListDto & {tipo: string})[]>} Lista de rocas
     */
    async getRocas() {
      const response = await api.get('/elementos-geologicos/rocas');
      return Array.isArray(response.data)
        ? response.data.map(roca => ({ ...roca, tipo: 'roca', tipoElemento: 'Roca' }))
        : [];
    },

    /**
     * Obtiene todos los minerales
     * @returns {Promise<(ElementoGeologicoListDto & {tipo: string})[]>} Lista de minerales
     */
    async getMinerales() {
      const response = await api.get('/elementos-geologicos/minerales');
      return Array.isArray(response.data)
        ? response.data.map(mineral => ({ ...mineral, tipo: 'mineral', tipoElemento: 'Mineral' }))
        : [];
    },

    /**
     * Obtiene todos los elementos (fósiles, minerales y rocas)
     * @returns {Promise<(ElementoGeologicoListDto & {tipo: string})[]>} Lista combinada de elementos
     */
    async getTodosElementos() {
      const [fosilesResponse, mineralesResponse, rocasResponse] = await Promise.all([
        api.get('/elementos-geologicos/fosiles'),
        api.get('/elementos-geologicos/minerales'),
        api.get('/elementos-geologicos/rocas')
      ]);

      const fosiles = Array.isArray(fosilesResponse.data)
        ? fosilesResponse.data.map(fosil => ({ ...fosil, tipo: 'fosil', tipoElemento: 'Fosil' }))
        : [];

      const minerales = Array.isArray(mineralesResponse.data)
        ? mineralesResponse.data.map(mineral => ({ ...mineral, tipo: 'mineral', tipoElemento: 'Mineral' }))
        : [];

      const rocas = Array.isArray(rocasResponse.data)
        ? rocasResponse.data.map(roca => ({ ...roca, tipo: 'roca', tipoElemento: 'Roca' }))
        : [];

      return [...fosiles, ...minerales, ...rocas];
    },

    /**
     * Obtiene un elemento específico por ID
     * @param {number} id - ID del elemento
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Respuesta con el elemento geológico
     */
    async getElementById(id) {
      const response = await api.get(`/elementos-geologicos/${id}`);
      return response.data;
    },

    /**
     * Crea un nuevo fósil
     * @param {CreateFosilDto} fosilData - Datos del fósil
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Fósil creado
     */
    async createFosil(fosilData) {
      const response = await api.post('/elementos-geologicos/fosiles', fosilData);
      return response.data;
    },

    /**
     * Crea un nuevo mineral
     * @param {CreateMineralDto} mineralData - Datos del mineral
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Mineral creado
     */
    async createMineral(mineralData) {
      const response = await api.post('/elementos-geologicos/minerales', mineralData);
      return response.data;
    },

    /**
     * Crea una nueva roca
     * @param {CreateRocaDto} rocaData - Datos de la roca
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Roca creada
     */
    async createRoca(rocaData) {
      const response = await api.post('/elementos-geologicos/rocas', rocaData);
      return response.data;
    },

    /**
     * Actualiza un fósil
     * @param {number} id - ID del fósil
     * @param {UpdateFosilDto} updateData - Datos a actualizar
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Fósil actualizado
     */
    async updateFosil(id, updateData) {
      const response = await api.put(`/elementos-geologicos/fosiles/${id}`, updateData);
      return response.data;
    },

    /**
     * Actualiza un mineral
     * @param {number} id - ID del mineral
     * @param {UpdateMineralDto} updateData - Datos a actualizar
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Mineral actualizado
     */
    async updateMineral(id, updateData) {
      const response = await api.put(`/elementos-geologicos/minerales/${id}`, updateData);
      return response.data;
    },

    /**
     * Actualiza una roca
     * @param {number} id - ID de la roca
     * @param {UpdateRocaDto} updateData - Datos a actualizar
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Roca actualizada
     */
    async updateRoca(id, updateData) {
      const response = await api.put(`/elementos-geologicos/rocas/${id}`, updateData);
      return response.data;
    },

    /**
     * Elimina un elemento geológico (soft delete)
     * @param {number} id - ID del elemento
     * @returns {Promise<ElementoGeologicoSingleResponseDto>} Confirmación de eliminación
     */
    async deleteElemento(id) {
      const response = await api.delete(`/elementos-geologicos/${id}`);
      return response.data;
    },

    /**
     * Busca un elemento por código. Retorna { exists, id, tipo } o { exists: false }
     * @param {string} codigo - Código del elemento
     * @returns {Promise<{exists: boolean, id?: number, tipo?: string}>}
     */
    async getByCodigo(codigo) {
      try {
        const response = await api.get(`/elementos-geologicos/by-codigo/${encodeURIComponent(codigo)}`);
        return response.data;
      } catch (error) {
        if (error.response?.status === 404) {
          return { exists: false };
        }
        throw error;
      }
    },

    /**
     * Filtra elementos según los criterios proporcionados
     * @param {ElementoGeologicoFilterDto} filtros - Criterios de filtrado
     * @returns {Promise<ElementoGeologicoListDto[]>} Lista de elementos filtrados
     */
    async filtrarElementos(filtros = {}) {
      const construirQueryParams = (filtros) => {
        const queryParams = new URLSearchParams();

        Object.keys(filtros).forEach(key => {
          if (filtros[key] !== null && filtros[key] !== undefined && filtros[key] !== '') {
            queryParams.append(key, filtros[key]);
          }
        });

        return queryParams.toString();
      };

      const queryParams = construirQueryParams(filtros);

      // Si se especifica un tipo concreto, usar el endpoint específico
      if (filtros.tipo === 'Fosil') {
        const response = await api.get(`/elementos-geologicos/fosiles?${queryParams}`);
        return Array.isArray(response.data) ? response.data : [];
      } else if (filtros.tipo === 'Mineral') {
        const response = await api.get(`/elementos-geologicos/minerales?${queryParams}`);
        return Array.isArray(response.data) ? response.data : [];
      } else if (filtros.tipo === 'Roca') {
        const response = await api.get(`/elementos-geologicos/rocas?${queryParams}`);
        return Array.isArray(response.data) ? response.data : [];
      }

      // Si no se especifica tipo, usar el endpoint general
      const response = await api.get(`/elementos-geologicos?${queryParams}`);
      return Array.isArray(response.data) ? response.data : [];
    },

    /**
     * Obtiene países disponibles (solo activos)
     * @returns {Promise<CountryListDto[]>} Lista de países
     */
    async getPaises() {
      try {
        // Usar endpoint de países activos
        const response = await api.get('/paises/active');

        // El backend retorna: { success, message, data: { paises: [...], totalCount, ... } }
        if (response.data?.success && response.data?.data?.paises) {
          return response.data.data.paises;
        }

        // Fallback: intentar acceso directo si la estructura es diferente
        if (Array.isArray(response.data?.data)) {
          return response.data.data;
        }

        return [];
      } catch (error) {
        console.error('Error al obtener países:', error);
        return [];
      }
    },

    /**
     * Obtiene provincias de un país específico
     * @param {number} paisId - ID del país
     * @returns {Promise<ProvinceListDto[]>} Lista de provincias
     */
    async getProvincias(paisId) {
      try {
        // Usar endpoint específico para provincias por país
        const response = await api.get(`/provincias/by-pais/${paisId}`);

        // El backend retorna: { success, message, data: { provincias: [...], totalCount, ... } }
        if (response.data?.success && response.data?.data?.provincias) {
          return response.data.data.provincias;
        }

        // Fallback: intentar acceso directo si la estructura es diferente
        if (Array.isArray(response.data?.data)) {
          return response.data.data;
        }

        return [];
      } catch (error) {
        console.error(`Error al obtener provincias del país ${paisId}:`, error);
        return [];
      }
    },

    /**
     * Obtiene fotos de una galería
     * @param {number} galeriaId - ID de la galería
     * @returns {Promise<PhotoElementDto[]>} Lista de fotos
     */
    async getFotosGaleria(galeriaId) {
      const response = await api.get(`/foto-elementos/galeria/${galeriaId}`);
      return Array.isArray(response.data) ? response.data : [];
    },

    /**
     * Sube una foto a una galería
     * @param {number} galeriaId - ID de la galería
     * @param {FormData} formData - Datos de la foto
     * @returns {Promise<PhotoElementDto>} Foto creada
     */
    async uploadFoto(galeriaId, formData) {
      const response = await api.post(`/foto-elementos/galeria/${galeriaId}`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      return response.data;
    },

    /**
     * Obtiene la galería y fotos de un elemento (sin bytes de imagen)
     * @param {number} elementoId - ID del elemento geológico
     * @returns {Promise<{galeriaId: number|null, fotos: PhotoElementDto[]}>}
     */
    async getFotosByElemento(elementoId) {
      const response = await api.get(`/foto-elementos/por-elemento/${elementoId}`);
      return response.data;
    },

    /**
     * Sube una foto a un elemento por su ID. Crea la galería si no existe.
     * @param {number} elementoId - ID del elemento geológico
     * @param {FormData} formData - Datos de la foto
     * @returns {Promise<PhotoElementDto>} Foto creada
     */
    async uploadFotoByElemento(elementoId, formData) {
      const response = await api.post(`/foto-elementos/elemento/${elementoId}`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      return response.data;
    },

    /**
     * Actualiza una foto existente
     * @param {number} fotoId - ID de la foto
     * @param {FormData} formData - Datos a actualizar (TipoFoto, DescripcionEspecifica, ImagenFile opcional)
     * @returns {Promise<PhotoElementDto>} Foto actualizada
     */
    async updateFoto(fotoId, formData) {
      const response = await api.put(`/foto-elementos/${fotoId}`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      return response.data;
    },

    /**
     * Elimina una foto (soft-delete)
     * @param {number} fotoId - ID de la foto
     * @returns {Promise<void>}
     */
    async deleteFoto(fotoId) {
      await api.delete(`/foto-elementos/${fotoId}`);
    },

    /**
     * Restaura una foto eliminada por soft-delete (solo Admin)
     * @param {number} fotoId - ID de la foto
     * @returns {Promise<void>}
     */
    async restoreFoto(fotoId) {
      await api.patch(`/foto-elementos/${fotoId}/restaurar`);
    }
    });
  }, [api]);
};
