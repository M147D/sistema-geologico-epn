// src/context/DetailContext.jsx
import { createContext, useContext, useState, useEffect, useMemo } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { useElementos } from './ElementosContext';
import { invalidateImage } from '../utils/imageCache';

const DetailContext = createContext();

// eslint-disable-next-line react-refresh/only-export-components
export const useDetail = () => {
  const context = useContext(DetailContext);
  if (!context) throw new Error('useDetail must be used within DetailProvider');
  return context;
};

export const DetailProvider = ({ children }) => {
  const { id: idParam } = useParams();
  const id = parseInt(idParam);
  const navigate = useNavigate();
  const location = useLocation();

  const { user, isAdmin, canEdit, canDelete } = useAuth();
  const {
    elementos,
    obtenerElementoPorId,
    actualizarElemento,
    cargarFotosDeElemento,
    subirFotoAElemento,
    actualizarFoto,
    eliminarFoto,
    restaurarFoto,
    solicitarInforme,
    getImage,
    getImageThumbnail,
  } = useElementos();

  const [elemento, setElemento] = useState(location.state?.elemento || null);
  const [fotos, setFotos] = useState([]);
  const [loadingDetail, setLoadingDetail] = useState(true);

  const [isEditing, setIsEditing] = useState(false);
  const [editForm, setEditForm] = useState({});
  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState(null);
  const [saveSuccess, setSaveSuccess] = useState(false);

  useEffect(() => {
    setIsEditing(false);
    setEditForm({});
    setSaveError(null);
    setSaveSuccess(false);
    setLoadingDetail(true);

    if (!id) { setLoadingDetail(false); return; }

    obtenerElementoPorId(id)
      .then(async (data) => {
        if (data) {
          setElemento(data);
          try {
            const fotosData = await cargarFotosDeElemento(id);
            setFotos(fotosData?.fotos || []);
          } catch {
            setFotos(data.galeria?.fotos || []);
          }
        } else {
          setElemento(null);
        }
      })
      .catch(() => setElemento(null))
      .finally(() => setLoadingDetail(false));
  }, [id, obtenerElementoPorId, cargarFotosDeElemento]);

  const currentIndex = useMemo(() => {
    if (!elementos?.length) return -1;
    return elementos.findIndex(el => el.id === id);
  }, [elementos, id]);

  const totalElementos = elementos?.length || 0;

  const handleNavigate = (direction) => {
    if (currentIndex === -1 || totalElementos === 0) return;
    const newIndex = direction === 'prev'
      ? (currentIndex - 1 + totalElementos) % totalElementos
      : (currentIndex + 1) % totalElementos;
    const nextEl = elementos[newIndex];
    navigate(`/detalle/${nextEl.id}`, { state: { elemento: nextEl } });
  };

  const handleGoBack = () => {
    if (window.history.length > 1) navigate(-1);
    else navigate('/listar-elementos');
  };

  const handleStartEdit = () => {
    if (!elemento) return;
    setEditForm({
      nombre: elemento.nombre || '',
      codigo: elemento.codigo || '',
      edad: elemento.edad || '',
      donante: elemento.donante || '',
      fechaIngreso: elemento.fechaIngreso
        ? new Date(elemento.fechaIngreso).toISOString().split('T')[0]
        : '',
      ejemplares: elemento.ejemplares || 1,
      documentosRelacionados: elemento.documentosRelacionados || '',
      laminaExiste: elemento.laminaExiste || false,
      tipoFosil: elemento.tipoFosil ?? 0,
      especie: elemento.especie || '',
      periodo: elemento.periodo || '',
      tipoMineral: elemento.tipoMineral ?? 0,
      litologiaMineral: elemento.litologiaMineral || '',
      tipoRoca: elemento.tipoRoca ?? 0,
      litologiaRoca: elemento.litologiaRoca || '',
      localidad: elemento.ubicacion?.localidad || elemento.localidad || '',
      latitud: elemento.ubicacion?.latitud || elemento.latitud || '',
      longitud: elemento.ubicacion?.longitud || elemento.longitud || '',
    });
    setIsEditing(true);
    setSaveError(null);
    setSaveSuccess(false);
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setEditForm({});
    setSaveError(null);
  };

  const handleEditChange = (field, value) => {
    setEditForm(prev => ({ ...prev, [field]: value }));
  };

  const handleSave = async () => {
    setSaving(true);
    setSaveError(null);
    setSaveSuccess(false);
    try {
      const tipo = elemento.tipoElemento.toLowerCase();
      const baseData = {
        nombre: editForm.nombre,
        codigo: editForm.codigo,
        edad: editForm.edad,
        donante: editForm.donante,
        fechaIngreso: editForm.fechaIngreso || null,
        ejemplares: parseInt(editForm.ejemplares) || 1,
        documentosRelacionados: editForm.documentosRelacionados || null,
        laminaExiste: editForm.laminaExiste,
        ubicacionId: elemento.ubicacion?.id || null,
        usuarioId: user.id,
        localidad: editForm.localidad || null,
        latitud: editForm.latitud || null,
        longitud: editForm.longitud || null,
        nombrePais: elemento.ubicacion?.nombrePais || null,
        nombreProvincia: elemento.ubicacion?.nombreProvincia || null,
      };

      let updateData;
      if (tipo === 'fosil') {
        updateData = { ...baseData, tipoFosil: editForm.tipoFosil, especie: editForm.especie, periodo: editForm.periodo };
      } else if (tipo === 'mineral') {
        updateData = { ...baseData, tipoMineral: editForm.tipoMineral, litologia: editForm.litologiaMineral };
      } else {
        updateData = { ...baseData, tipoRoca: editForm.tipoRoca, litologia: editForm.litologiaRoca };
      }

      await actualizarElemento(id, updateData, tipo);
      const data = await obtenerElementoPorId(id);
      if (data) setElemento(data);

      setIsEditing(false);
      setSaveSuccess(true);
      setTimeout(() => setSaveSuccess(false), 3000);
    } catch (err) {
      const errors = err.response?.data?.errors;
      const errMsg = errors
        ? (Array.isArray(errors) ? errors.join(', ') : Object.values(errors).flat().join('; '))
        : null;
      setSaveError(errMsg || err.response?.data?.message || err.message || 'Error al guardar los cambios');
    } finally {
      setSaving(false);
    }
  };

  const refreshFotos = async () => {
    const data = await cargarFotosDeElemento(id);
    if (data) setFotos(data.fotos || []);
  };

  const handleUploadPhoto = async (_galeriaId, formData) => {
    await subirFotoAElemento(elemento.id, formData);
    await refreshFotos();
  };

  const handleSavePhoto = async (fotoId, formData) => {
    invalidateImage(fotoId);
    await actualizarFoto(fotoId, formData);
    await refreshFotos();
  };

  const handleDeletePhoto = async (fotoId) => {
    invalidateImage(fotoId);
    await eliminarFoto(fotoId);
    await refreshFotos();
  };

  const handleRestorePhoto = async (fotoId) => {
    invalidateImage(fotoId);
    await restaurarFoto(fotoId);
    await refreshFotos();
  };

  const value = {
    id,
    elemento,
    fotos,
    loadingDetail,
    user,
    isAdmin,
    canEdit,
    canDelete,
    currentIndex,
    totalElementos,
    handleNavigate,
    handleGoBack,
    isEditing,
    editForm,
    saving,
    saveError,
    saveSuccess,
    handleStartEdit,
    handleCancelEdit,
    handleEditChange,
    handleSave,
    clearSaveError: () => setSaveError(null),
    clearSaveSuccess: () => setSaveSuccess(false),
    handleSetSaveError: (msg) => setSaveError(msg),
    handleUploadPhoto,
    handleSavePhoto,
    handleDeletePhoto,
    handleRestorePhoto,
    handleSolicitarInforme: (dto) => solicitarInforme(id, dto),
    getImage,
    getImageThumbnail,
  };

  return (
    <DetailContext.Provider value={value}>
      {children}
    </DetailContext.Provider>
  );
};
