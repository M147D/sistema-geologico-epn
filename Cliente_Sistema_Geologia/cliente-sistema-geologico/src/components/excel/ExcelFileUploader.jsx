// src/components/excel/ExcelFileUploader.jsx
import { Box, Alert, Typography, Stack } from '@mui/material';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';

const ExcelFileUploader = ({ uploadStatus, fileName, onFileUpload }) => {
  return (
    <Box sx={{ mb: 3 }}>
      {/* Zona de carga */}
      <input
        accept=".xlsx,.xls"
        style={{ display: 'none' }}
        id="excel-file-upload"
        type="file"
        onChange={onFileUpload}
      />
      <label htmlFor="excel-file-upload" style={{ display: 'block', cursor: 'pointer' }}>
        <Box
          sx={{
            border: '2px dashed',
            borderColor: uploadStatus === 'success' ? 'success.main' : 'grey.300',
            borderRadius: 2,
            p: 4,
            textAlign: 'center',
            bgcolor: uploadStatus === 'success' ? 'success.50' : 'grey.50',
            transition: 'all 0.2s',
            '&:hover': { borderColor: 'primary.main', bgcolor: 'action.hover' },
          }}
        >
          {fileName && uploadStatus === 'success' ? (
            <Stack alignItems="center" spacing={0.5}>
              <InsertDriveFileIcon sx={{ fontSize: 40, color: 'success.main' }} />
              <Typography variant="body1" fontWeight={500} color="success.main">
                {fileName}
              </Typography>
              <Typography variant="caption" color="text.secondary">Clic para cambiar archivo</Typography>
            </Stack>
          ) : (
            <Stack alignItems="center" spacing={0.5}>
              <CloudUploadIcon sx={{ fontSize: 40, color: 'text.disabled' }} />
              <Typography variant="body1" color="text.secondary">
                Clic para seleccionar archivo Excel
              </Typography>
              <Typography variant="caption" color="text.disabled">
                Formato: .xlsx
              </Typography>
            </Stack>
          )}
        </Box>
      </label>

      {uploadStatus === 'error-name' && (
        <Alert severity="error" sx={{ mt: 1.5 }}>
          El archivo debe tener extensión .xlsx
        </Alert>
      )}
      {uploadStatus === 'error-read' && (
        <Alert severity="error" sx={{ mt: 1.5 }}>
          Error al leer el archivo. Verifica que sea un archivo Excel válido.
        </Alert>
      )}
    </Box>
  );
};

export default ExcelFileUploader;
