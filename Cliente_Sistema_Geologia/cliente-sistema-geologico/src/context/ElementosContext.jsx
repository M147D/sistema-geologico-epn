import { createContext, useContext, useState, useEffect, useCallback, useRef } from 'react';
import { useElementosService } from '../hooks/useElementosService';
import { useAuth } from './AuthContext';

const ElementosContext = createContext();

// eslint-disable-next-line react-refresh/only-export-components
export const useElementos = () => {
  const context = useContext(ElementosContext);
  if (!context) {
    throw new Error('useElementos must be used within ElementosProvider');
  }
  return context;
};

export const ElementosProvider = ({ children }) => {
  const elementosService = useElementosService();
  const { isAuthenticated } = useAuth();

  // Estados principales
  const [elementos, setElementos] = useState([]);
  const [fosiles, setFosiles] = useState([]);
  const [minerales, setMinerales] = useState([]);
  const [rocas, setRocas] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const [resultadosBusqueda, setResultadosBusqueda] = useState([]);

  // Estados de ubicaciones (para filtros)
  const [paises, setPaises] = useState([]);
  const [provincias, setProvincias] = useState([]);
  const [ubicacionesLoaded, setUbicacionesLoaded] = useState(false);

  // Cache flags
  const [elementosLoaded, setElementosLoaded] = useState(false);
  const [lastRefresh, setLastRefresh] = useState(null);

  // Refs so useCallback deps don't include loaded flags (avoids cascade re-fires)
  const elementosLoadedRef = useRef(false);
  elementosLoadedRef.current = elementosLoaded;
  const ubicacionesLoadedRef = useRef(false);
  ubicacionesLoadedRef.current = ubicacionesLoaded;
  // Guards against StrictMode double-invocation (refs persist across double-effect)
  const loadingElementosRef = useRef(false);
  const loadingUbicacionesRef = useRef(false);

  // --- Lectura ---

  const cargarTodosLosElementos = useCallback(async (force = false) => {
    if (!isAuthenticated) return;
    if (elementosLoadedRef.current && !force) return;
    if (loadingElementosRef.current && !force) return;
    loadingElementosRef.current = true;

    setLoading(true);
    setError(null);

    try {
      const [fosilesData, mineralesData, rocasData] = await Promise.all([
        elementosService.getFosiles(),
        elementosService.getMinerales(),
        elementosService.getRocas()
      ]);

      setFosiles(fosilesData || []);
      setMinerales(mineralesData || []);
      setRocas(rocasData || []);

      const todosElementos = [
        ...(fosilesData || []).map(f => ({ ...f, tipoElemento: 'Fosil' })),
        ...(mineralesData || []).map(m => ({ ...m, tipoElemento: 'Mineral' })),
        ...(rocasData || []).map(r => ({ ...r, tipoElemento: 'Roca' }))
      ];

      setElementos(todosElementos);
      setElementosLoaded(true);
      setLastRefresh(new Date());

    } catch (err) {
      console.error('Error al cargar elementos:', err);
      setError('Error al cargar elementos geológicos');
    } finally {
      setLoading(false);
      loadingElementosRef.current = false;
    }
  }, [elementosService, isAuthenticated]);

  const obtenerElementoPorId = useCallback(async (id) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');

    const response = await elementosService.getElementById(id);
    if (response?.success && response?.data) {
      return response.data;
    }
    return null;
  }, [elementosService, isAuthenticated]);

  const buscarPorCodigo = useCallback(async (codigo) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');
    return elementosService.getByCodigo(codigo);
  }, [elementosService, isAuthenticated]);

  const cargarUbicaciones = useCallback(async () => {
    if (!isAuthenticated) return;
    if (ubicacionesLoadedRef.current) return;
    if (loadingUbicacionesRef.current) return;
    loadingUbicacionesRef.current = true;

    try {
      const paisesData = await elementosService.getPaises();
      setPaises(paisesData || []);
      setUbicacionesLoaded(true);
    } catch (err) {
      console.error('Error al cargar ubicaciones:', err);
    } finally {
      loadingUbicacionesRef.current = false;
    }
  }, [elementosService, isAuthenticated]);

  const cargarProvinciasPorPais = useCallback(async (paisIdentifier) => {
    if (!isAuthenticated) return;

    try {
      let paisId = paisIdentifier;

      if (typeof paisIdentifier === 'string') {
        const pais = paises.find(p => p.nombrePais === paisIdentifier);
        if (!pais) {
          setProvincias([]);
          return;
        }
        paisId = pais.id;
      }

      const provinciasData = await elementosService.getProvincias(paisId);
      setProvincias(provinciasData || []);
    } catch (err) {
      console.error('Error al cargar provincias:', err);
      setProvincias([]);
    }
  }, [elementosService, isAuthenticated, paises]);

  // --- Filtros ---

  const limpiarFiltros = useCallback(() => {
    setResultadosBusqueda([]);
  }, []);

  const sincronizarResultados = useCallback((resultados) => {
    setResultadosBusqueda(resultados);
  }, []);

  // --- Escritura: Elementos ---

  const crearElemento = useCallback(async (elementoData, tipo) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');

    setLoading(true);
    setError(null);

    try {
      let nuevoElemento;

      if (tipo === 'fosil') {
        nuevoElemento = await elementosService.createFosil(elementoData);
      } else if (tipo === 'mineral') {
        nuevoElemento = await elementosService.createMineral(elementoData);
      } else if (tipo === 'roca') {
        nuevoElemento = await elementosService.createRoca(elementoData);
      }

      if (nuevoElemento) {
        const elementoConTipo = { ...nuevoElemento, tipoElemento: tipo.charAt(0).toUpperCase() + tipo.slice(1) };

        if (tipo === 'fosil') setFosiles(prev => [...prev, nuevoElemento]);
        else if (tipo === 'mineral') setMinerales(prev => [...prev, nuevoElemento]);
        else if (tipo === 'roca') setRocas(prev => [...prev, nuevoElemento]);

        setElementos(prev => [...prev, elementoConTipo]);
      }

      return nuevoElemento;
    } catch (err) {
      console.error('Error al crear elemento:', err);
      setError(`Error al crear ${tipo}`);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [elementosService, isAuthenticated]);

  const actualizarElemento = useCallback(async (id, elementoData, tipo) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');

    setLoading(true);
    setError(null);

    try {
      let elementoActualizado;

      if (tipo === 'fosil') {
        elementoActualizado = await elementosService.updateFosil(id, elementoData);
      } else if (tipo === 'mineral') {
        elementoActualizado = await elementosService.updateMineral(id, elementoData);
      } else if (tipo === 'roca') {
        elementoActualizado = await elementosService.updateRoca(id, elementoData);
      }

      if (elementoActualizado) {
        const actualizarArray = (prev) =>
          prev.map(item => item.id === id ? elementoActualizado : item);

        if (tipo === 'fosil') setFosiles(actualizarArray);
        else if (tipo === 'mineral') setMinerales(actualizarArray);
        else if (tipo === 'roca') setRocas(actualizarArray);

        setElementos(prev => prev.map(item =>
          item.id === id ? { ...elementoActualizado, tipoElemento: tipo.charAt(0).toUpperCase() + tipo.slice(1) } : item
        ));
      }

      return elementoActualizado;
    } catch (err) {
      console.error('Error al actualizar elemento:', err);
      setError(`Error al actualizar ${tipo}`);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [elementosService, isAuthenticated]);

  const eliminarElemento = useCallback(async (id) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');

    setLoading(true);
    setError(null);

    try {
      await elementosService.deleteElemento(id);

      const filtrarArray = (prev) => prev.filter(item => item.id !== id);
      setFosiles(filtrarArray);
      setMinerales(filtrarArray);
      setRocas(filtrarArray);
      setElementos(filtrarArray);
      setResultadosBusqueda(filtrarArray);
    } catch (err) {
      console.error('Error al eliminar elemento:', err);
      setError('Error al eliminar elemento');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [elementosService, isAuthenticated]);

  // --- Escritura: Fotos ---

  const subirFotoAElemento = useCallback(async (elementoId, formData) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');
    return elementosService.uploadFotoByElemento(elementoId, formData);
  }, [elementosService, isAuthenticated]);

  const cargarFotosDeElemento = useCallback(async (elementoId) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');
    return elementosService.getFotosByElemento(elementoId);
  }, [elementosService, isAuthenticated]);

  const actualizarFoto = useCallback(async (fotoId, formData) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');
    return elementosService.updateFoto(fotoId, formData);
  }, [elementosService, isAuthenticated]);

  const eliminarFoto = useCallback(async (fotoId) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');
    return elementosService.deleteFoto(fotoId);
  }, [elementosService, isAuthenticated]);

  const restaurarFoto = useCallback(async (fotoId) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');
    return elementosService.restoreFoto(fotoId);
  }, [elementosService, isAuthenticated]);

  const solicitarInforme = useCallback(async (id, dto) => {
    if (!isAuthenticated) throw new Error('Usuario no autenticado');
    return elementosService.solicitarInforme(id, dto);
  }, [elementosService, isAuthenticated]);

  const getImage = useCallback((fotoId) => elementosService.getImage(fotoId), [elementosService]);
  const getImageThumbnail = useCallback((fotoId) => elementosService.getImageThumbnail(fotoId), [elementosService]);

  // Cargar datos iniciales cuando el usuario esté autenticado.
  useEffect(() => {
    if (isAuthenticated) {
      cargarTodosLosElementos();
      cargarUbicaciones();
    }
  }, [isAuthenticated, cargarTodosLosElementos, cargarUbicaciones]);

  const value = {
    // Estados
    elementos,
    fosiles,
    minerales,
    rocas,
    loading,
    error,
    elementosLoaded,
    lastRefresh,

    // Búsqueda
    resultadosBusqueda,

    // Ubicaciones
    paises,
    provincias,

    // Lectura
    cargarTodosLosElementos,
    obtenerElementoPorId,
    buscarPorCodigo,
    cargarProvinciasPorPais,
    limpiarFiltros,
    sincronizarResultados,

    // Escritura: Elementos
    crearElemento,
    actualizarElemento,
    eliminarElemento,

    // Escritura: Fotos
    subirFotoAElemento,
    cargarFotosDeElemento,
    actualizarFoto,
    eliminarFoto,
    restaurarFoto,

    // Acciones transversales
    solicitarInforme,
    getImage,
    getImageThumbnail,

    // Utilidades
    setError: (err) => setError(err),
    clearError: () => setError(null)
  };

  return (
    <ElementosContext.Provider value={value}>
      {children}
    </ElementosContext.Provider>
  );
};

export default ElementosContext;