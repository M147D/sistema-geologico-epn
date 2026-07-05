// src/context/ExcelContext.jsx
import { createContext, useContext, useState } from 'react';
import * as XLSX from 'xlsx';
import { useElementos } from './ElementosContext';
import { TIPO_FOSIL_MAP, TIPO_MINERAL_MAP, TIPO_ROCA_MAP } from '../constants/tipoMaps';

const ExcelContext = createContext();

// eslint-disable-next-line react-refresh/only-export-components
export const useExcel = () => {
  const context = useContext(ExcelContext);
  if (!context) throw new Error('useExcel must be used within ExcelProvider');
  return context;
};

const normalizeKeys = (obj) => {
  const tildeMap = { 'á': 'a', 'é': 'e', 'í': 'i', 'ó': 'o', 'ú': 'u', 'ñ': 'n' };
  const normalized = {};
  for (const [key, value] of Object.entries(obj)) {
    const normalizedKey = key
      .toLowerCase()
      .trim()
      .replace(/[áéíóúñ]/g, c => tildeMap[c] || c)
      .replace(/\s+/g, '');
    normalized[normalizedKey] = value;
  }
  return normalized;
};

const mapRow = (raw) => {
  const n = normalizeKeys(raw);
  return {
    nombre: n.nombre ?? null,
    edad: n.edad ?? n.ano ?? null,
    donante: n.donante ?? null,
    codigo: n.codigo ?? null,
    ejemplares: n.ejemplar ?? n.ejemplares ?? null,
    laminaExiste: n.laminaexiste ?? null,
    documentosRelacionados: n.documentosrelacionados ?? null,
    especie: n.especie ?? null,
    periodo: n.periodo ?? null,
    tipoFosil: n.tipofosil ?? n.subtipo ?? n.tipo ?? null,
    tipoMineral: n.tipomineral ?? n.subtipo ?? n.tipo ?? null,
    tipoRoca: n.tiporoca ?? n.subtipo ?? n.tipo ?? null,
    litologia: n.litologia ?? null,
    latitud: n.latitud ?? null,
    longitud: n.longitud ?? null,
    localidad: n.localidad ?? null,
    leyenda: n.leyenda ?? null,
    nombrePais: n.nombrepais ?? n.pais ?? null,
    nombreProvincia: n.nombreprovincia ?? n.provincia ?? null,
  };
};

const normStr = (s) =>
  String(s).toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '').replace(/[-\s]/g, '');

const lookupTipo = (rawValue, map) => {
  if (rawValue === null || rawValue === undefined) return null;
  if (rawValue in map) return map[rawValue];
  const num = parseInt(rawValue, 10);
  if (!isNaN(num)) return num;
  const normRaw = normStr(rawValue);
  const entry = Object.entries(map).find(([k]) => normStr(k) === normRaw);
  return entry ? entry[1] : null;
};

const buildElementData = (row, tipo, index) => {
  const base = {
    nombre: row.nombre ? String(row.nombre).replace(/[^a-zA-ZáéíóúÁÉÍÓÚñÑ\s]/g, '').trim() || 'Sin nombre' : 'Sin nombre',
    edad: row.edad != null && String(row.edad).trim() !== '' ? String(row.edad).replace(/[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s.]/g, '').trim() || '0' : '0',
    donante: row.donante ? String(row.donante).replace(/[^a-zA-ZáéíóúÁÉÍÓÚñÑ\s]/g, '').trim() || 'Instituto Geologico' : 'Instituto Geologico',
    fechaIngreso: new Date().toISOString().split('T')[0],
    codigo: row.codigo ? String(row.codigo).replace(/[^a-zA-Z0-9.]/g, '') : 'EXC' + String(index + 1).padStart(5, '0'),
    ejemplares: parseInt(row.ejemplares) || 1,
    laminaExiste: row.laminaExiste === 'true' || row.laminaExiste === true || false,
    documentosRelacionados: row.documentosRelacionados ? String(row.documentosRelacionados).trim() : '',
    usuarioId: 1,
    ubicacion: {
      nombrePais: row.nombrePais || '',
      nombreProvincia: row.nombreProvincia || '',
      localidad: row.localidad || '',
      latitud: row.latitud != null && String(row.latitud).trim() !== '' ? String(row.latitud) : '',
      longitud: row.longitud != null && String(row.longitud).trim() !== '' ? String(row.longitud) : '',
      leyenda: row.leyenda || 'Sin leyenda',
    },
  };

  if (tipo === 'fosil') return { ...base, especie: row.especie || 'Por determinar', periodo: row.periodo || 'Por determinar', tipoFosil: lookupTipo(row.tipoFosil, TIPO_FOSIL_MAP) ?? 0 };
  if (tipo === 'mineral') return { ...base, tipoMineral: lookupTipo(row.tipoMineral, TIPO_MINERAL_MAP) ?? 0, litologia: row.litologia || 'Desconocida' };
  return { ...base, tipoRoca: lookupTipo(row.tipoRoca, TIPO_ROCA_MAP) ?? 0, litologia: row.litologia || 'Desconocida' };
};

export const ExcelProvider = ({ children }) => {
  const { crearElemento, actualizarElemento, buscarPorCodigo } = useElementos();
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [processingStatus, setProcessingStatus] = useState('');

  const readExcelFile = (file) => new Promise((resolve, reject) => {
    setLoading(true);
    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const workbook = XLSX.read(e.target.result, { type: 'binary' });
        const worksheet = workbook.Sheets[workbook.SheetNames[0]];
        setData(XLSX.utils.sheet_to_json(worksheet).map(mapRow));
        setLoading(false);
        resolve();
      } catch (error) {
        setLoading(false);
        reject(error);
      }
    };
    reader.onerror = (e) => { setLoading(false); reject(e); };
    reader.readAsBinaryString(file);
  });

  const saveToDatabase = async (tipoElemento) => {
    setLoading(true);
    const tipoNombre = tipoElemento === 'fosil' ? 'fósiles' : tipoElemento === 'mineral' ? 'minerales' : 'rocas';
    setProcessingStatus(`Guardando ${tipoNombre} en la base de datos...`);

    let createCount = 0, updateCount = 0, errorCount = 0, lastError = '';

    for (let i = 0; i < data.length; i++) {
      try {
        const elementData = buildElementData(data[i], tipoElemento, i);
        const existing = await buscarPorCodigo(elementData.codigo);

        if (existing.exists) {
          const { ubicacion, usuarioId, ...updateData } = elementData;
          if (existing.tipo === tipoElemento) {
            await actualizarElemento(existing.id, updateData, existing.tipo);
            updateCount++;
          } else {
            elementData.codigo = elementData.codigo + '.D' + String(i + 1);
            await crearElemento(elementData, tipoElemento);
            createCount++;
          }
        } else {
          await crearElemento(elementData, tipoElemento);
          createCount++;
        }
      } catch (error) {
        errorCount++;
        const errorMsg = error?.response?.data?.message || error?.response?.data?.errors || error?.message || 'Error desconocido';
        lastError = typeof errorMsg === 'object' ? JSON.stringify(errorMsg) : errorMsg;
      }
      setProcessingStatus(`Procesando: ${i + 1}/${data.length} - ${createCount} creados, ${updateCount} actualizados, ${errorCount} errores`);
    }

    setLoading(false);
    const statusMsg = `Completado: ${createCount} creados, ${updateCount} actualizados, ${errorCount} errores`;
    setProcessingStatus(errorCount > 0 ? `${statusMsg}. Ultimo error: ${lastError}` : statusMsg);
  };

  const resetData = () => {
    setData([]);
    setProcessingStatus('');
  };

  return (
    <ExcelContext.Provider value={{ data, loading, processingStatus, readExcelFile, saveToDatabase, resetData }}>
      {children}
    </ExcelContext.Provider>
  );
};