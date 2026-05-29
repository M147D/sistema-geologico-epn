import React, { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { yupResolver } from '@hookform/resolvers/yup';
import { compressForUpload } from '../../utils/imageProcessor';
import * as yup from 'yup';
import { useElementos } from '../../context/ElementosContext';
import { useAuth } from '../../context/AuthContext';
import { Box, Divider } from "@mui/material";
import BasicInfoSection from './BasicInfoSection';
import LocationSection from './LocationSection';
import PhotoSection from './PhotoSection';
import SubmitButton from './SubmitButton';
import Notifications from './Notifications';

const fileToBase64 = (file) => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      const base64 = reader.result.split(',')[1];
      resolve(base64);
    };
    reader.onerror = error => reject(error);
  });
};

const baseSchema = {
  nombre: yup.string()
    .required('El nombre es obligatorio')
    .min(1, 'El nombre debe tener al menos 1 carácter')
    .max(200, 'El nombre no puede exceder 200 caracteres'),
  edad: yup.string()
    .max(50, 'La edad no puede exceder 50 caracteres'),
  donante: yup.string()
    .max(200, 'El donante no puede exceder 200 caracteres'),
  codigo: yup.string()
    .max(100, 'El código no puede exceder 100 caracteres'),
  ejemplares: yup.number()
    .min(1, 'Debe haber al menos 1 ejemplar')
    .max(10000, 'No puede exceder 10000 ejemplares'),
  documentosRelacionados: yup.string()
    .max(5000, 'Los documentos no pueden exceder 5000 caracteres'),
  nombrePais: yup.string()
    .required('El país es obligatorio'),
  nombreProvincia: yup.string()
    .nullable(),
  localidad: yup.string(),
  latitud: yup.string(),
  longitud: yup.string(),
  leyenda: yup.string()
};

const fosilSchema = yup.object().shape({ ...baseSchema, especie: yup.string(), periodo: yup.string(), tipoFosil: yup.string() });
const mineralSchema = yup.object().shape({ ...baseSchema, tipoMineral: yup.string(), litologia: yup.string() });
const rocaSchema = yup.object().shape({ ...baseSchema, tipoRoca: yup.string(), litologia: yup.string() });

const getValidationSchema = (tipo) => {
  if (tipo === 'fosil') return fosilSchema;
  if (tipo === 'mineral') return mineralSchema;
  return rocaSchema;
};

const FormElement = ({ tipo }) => {
  const { crearElemento, loading, error: contextError } = useElementos();
  const { user } = useAuth();
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState(null);
  const [selectedFile, setSelectedFile] = useState(null);
  const [previewImage, setPreviewImage] = useState(null);

  const { register, handleSubmit, control, reset, formState: { errors } } = useForm({
    resolver: yupResolver(getValidationSchema(tipo)),
    mode: 'onBlur',
    defaultValues: {
      especie: "", periodo: "", tipoFosil: "Desconocido",
      tipoMineral: "Desconocido", tipoRoca: "Desconocido", litologia: "",
      nombre: "", edad: "", donante: "", codigo: "", ejemplares: 1,
      documentosRelacionados: "", laminaURL: "", laminaExiste: false,
      latitud: "", longitud: "", localidad: "", leyenda: "",
      nombreProvincia: "", nombrePais: "",
      tipoFoto: "Fotografia", descripcionFoto: "",
    },
  });

  useEffect(() => {
    reset();
    setPreviewImage(null);
    setSelectedFile(null);
  }, [tipo, reset]);

  const handleFileChange = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const maxSize = 10 * 1024 * 1024;
    if (file.size > maxSize) {
      setError(`El archivo es demasiado grande. El tamaño máximo es ${maxSize / (1024 * 1024)}MB`);
      return;
    }

    const compressed = await compressForUpload(file);
    setSelectedFile(compressed);
    const objectUrl = URL.createObjectURL(compressed);
    setPreviewImage(objectUrl);
  };

  const prepareDataForAPI = async (data) => {
    const usuarioId = user?.id || 1;
    const tipoFosilMap = { 'Desconocido': 0, 'Vertebrado': 1, 'Invertebrado': 2, 'Paleobotánica': 3, 'Icnofósil': 4, 'Microfósil': 5 };
    const tipoMineralMap = { 'Desconocido': 0, 'Silicato': 1, 'Carbonato': 2, 'Metálico': 3, 'Oxido': 4, 'Sulfuro': 5, 'Sulfato': 6, 'Fosfato': 7, 'Vanadato': 8 };
    const tipoRocaMap = { 'Desconocido': 0, 'Ígnea': 1, 'Sedimentaria': 2, 'Metamórfica': 3, 'Meteorito': 4, 'PiroVolcanoclástica': 5 };

    const payload = {
      tipo: tipo.charAt(0).toUpperCase() + tipo.slice(1),
      nombre: data.nombre,
      edad: data.edad?.toString().trim() || "0",
      donante: data.donante || "Sin Donante",
      fechaIngreso: new Date().toISOString().split('T')[0],
      codigo: data.codigo || `CODIGO${Date.now()}`,
      ejemplares: parseInt(data.ejemplares) || 1,
      laminaExiste: data.laminaExiste || false,
      ubicacion: {
        nombrePais: data.nombrePais || "",
        nombreProvincia: data.nombreProvincia || null,
        localidad: data.localidad || "Desconocida",
        latitud: data.latitud?.toString() || "0.0",
        longitud: data.longitud?.toString() || "0.0",
        leyenda: data.leyenda || "Sin leyenda"
      }
    };

    if (data.documentosRelacionados?.trim()) {
      payload.documentosRelacionados = data.documentosRelacionados.trim();
    }

    if (selectedFile) {
      try {
        const imagenBase64 = await fileToBase64(selectedFile);
        payload.galeria = {
          detalleGrupo: "Ninguna",
          fotos: [{
            imagen: imagenBase64,
            tipoFoto: data.tipoFoto || "Desconocido",
            descripcionEspecifica: data.descripcionFoto || "Vacío"
          }]
        };
      } catch (err) {
        console.error('Error al convertir imagen a base64:', err);
      }
    }

    if (tipo === "fosil") {
      payload.especie = data.especie || "Por determinar";
      payload.periodo = data.periodo || "Por determinar";
      payload.tipoFosil = tipoFosilMap[data.tipoFosil] ?? 0;
    } else if (tipo === "mineral") {
      payload.tipoMineral = tipoMineralMap[data.tipoMineral] ?? 0;
      payload.litologia = data.litologia || "Desconocida";
    } else if (tipo === "roca") {
      payload.tipoRoca = tipoRocaMap[data.tipoRoca] ?? 0;
      payload.litologia = data.litologia || "Desconocida";
    }

    return payload;
  };

  const onSubmit = async (data) => {
    setError(null);
    try {
      if (!data.nombre) throw new Error('Faltan campos requeridos');
      if (!data.nombrePais || data.nombrePais.trim() === '') throw new Error('El país es obligatorio');

      const apiData = await prepareDataForAPI(data);
      console.log('📤 Enviando elemento al backend:', JSON.stringify(apiData, null, 2));

      const response = await crearElemento(apiData, tipo);
      console.log('✅ Respuesta del servidor:', response);

      setSuccess(true);
      reset();
      setPreviewImage(null);
      setSelectedFile(null);
    } catch (error) {
      console.error(`❌ Error al crear ${tipo}:`, error);
      setError(error.message || `Error al crear ${tipo}`);
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)}>
      {/* Fila 1: Información básica | Ubicación */}
      <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 4, mb: 4 }}>
        <Box sx={{ flex: 1 }}>
          <BasicInfoSection tipo={tipo} register={register} errors={errors} control={control} />
        </Box>
        <Box sx={{ flex: 1 }}>
          <LocationSection register={register} control={control} errors={errors} />
        </Box>
      </Box>

      {/* Fila 2: Fotografía — ancho completo */}
      <PhotoSection
        register={register}
        handleFileChange={handleFileChange}
        selectedFile={selectedFile}
        previewImage={previewImage}
        tipo={tipo}
        control={control}
      />

      <Divider sx={{ my: 3 }} />
      <SubmitButton loading={loading} tipo={tipo} />

      <Notifications
        success={success}
        setSuccess={setSuccess}
        error={error}
        setError={setError}
        tipo={tipo}
      />
    </Box>
  );
};

export default FormElement;
