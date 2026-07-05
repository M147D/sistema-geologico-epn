// src/pages/PageExcel.jsx
import { useState } from 'react';
import { Container, Paper, Typography, Box, CircularProgress, Tabs, Tab } from '@mui/material';
import BiotechIcon from '@mui/icons-material/Biotech';
import DiamondIcon from '@mui/icons-material/Diamond';
import LandscapeIcon from '@mui/icons-material/Landscape';
import ExcelFileUploader from '../components/excel/ExcelFileUploader.jsx';
import ExcelDataPreview from '../components/excel/ExcelDataPreview.jsx';
import { ExcelProvider, useExcel } from '../context/ExcelContext';

const PageExcelContent = () => {
  const { data, loading, processingStatus, readExcelFile, saveToDatabase, resetData } = useExcel();
  const [tipoElemento, setTipoElemento] = useState('roca');
  const [file, setFile] = useState(null);
  const [uploadStatus, setUploadStatus] = useState('');

  const handleFileUpload = async (event) => {
    const selectedFile = event.target.files[0];
    if (!selectedFile) return;
    if (!selectedFile.name.endsWith('.xlsx')) {
      setUploadStatus('error-name');
      return;
    }
    setFile(selectedFile);
    try {
      await readExcelFile(selectedFile);
      setUploadStatus('success');
    } catch {
      setUploadStatus('error-read');
    }
  };

  const handleTipoChange = (_, newValue) => {
    setTipoElemento(newValue);
    setFile(null);
    setUploadStatus('');
    resetData();
  };

  return (
    <Container maxWidth="xl" sx={{ my: 4 }}>
      <Paper elevation={2} sx={{ overflow: 'hidden' }}>
        <Box sx={{ px: 3, pt: 3, pb: 0, borderBottom: 1, borderColor: 'divider' }}>
          <Typography variant="h5" fontWeight={600} gutterBottom>
            Carga Masiva desde Excel
          </Typography>
          <Tabs value={tipoElemento} onChange={handleTipoChange}>
            <Tab icon={<BiotechIcon />} iconPosition="start" label="Fósil"   value="fosil"   />
            <Tab icon={<LandscapeIcon />} iconPosition="start" label="Roca"  value="roca"    />
            <Tab icon={<DiamondIcon />} iconPosition="start" label="Mineral" value="mineral" />
          </Tabs>
        </Box>

        <Box sx={{ p: 3 }}>
          <ExcelFileUploader
            uploadStatus={uploadStatus}
            fileName={file?.name}
            onFileUpload={handleFileUpload}
          />

          {loading && !processingStatus && (
            <Box display="flex" justifyContent="center" alignItems="center" height="60vh">
              <CircularProgress />
            </Box>
          )}

          <ExcelDataPreview
            data={data}
            tipoElemento={tipoElemento}
            loading={loading}
            processingStatus={processingStatus}
            onSaveToDatabase={() => saveToDatabase(tipoElemento)}
          />
        </Box>
      </Paper>
    </Container>
  );
};

const PageExcel = () => (
  <ExcelProvider>
    <PageExcelContent />
  </ExcelProvider>
);

export default PageExcel;
