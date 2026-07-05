import { Link } from 'react-router-dom';
import { Paper, Stack, Button } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import DetailNavigation from './DetailNavigation';

const DetailTopBar = ({ onGoBack, currentIndex, totalElementos, onNavigate }) => {
  return (
    <Paper
      elevation={0}
      sx={{
        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        px: 2, py: 1, mb: 2,
        bgcolor: 'grey.50', border: 1, borderColor: 'divider', borderRadius: 2,
        flexWrap: 'wrap', gap: 1,
      }}
    >
      <Stack direction="row" spacing={1}>
        <Button startIcon={<ArrowBackIcon />} onClick={onGoBack} size="small">
          Volver
        </Button>
        <Button component={Link} to="/listar-elementos" size="small" variant="text">Lista</Button>
        <Button component={Link} to="/mapa" size="small" variant="text">Mapa</Button>
      </Stack>
      <DetailNavigation
        currentIndex={currentIndex}
        totalElementos={totalElementos}
        onNavigate={onNavigate}
      />
    </Paper>
  );
};

export default DetailTopBar;
