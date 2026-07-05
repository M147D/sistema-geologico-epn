import { Typography, Button, Box } from '@mui/material';
import NavigateBeforeIcon from '@mui/icons-material/NavigateBefore';
import NavigateNextIcon from '@mui/icons-material/NavigateNext';

const DetailNavigation = ({ currentIndex, totalElementos, onNavigate }) => {
  if (totalElementos === 0 || currentIndex === -1) return null;

  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
      <Button
        startIcon={<NavigateBeforeIcon />}
        onClick={() => onNavigate('prev')}
        size="small"
        variant="outlined"
      >
        Anterior
      </Button>
      <Typography variant="body2" color="text.secondary" sx={{ px: 1, whiteSpace: 'nowrap' }}>
        {currentIndex + 1} / {totalElementos}
      </Typography>
      <Button
        endIcon={<NavigateNextIcon />}
        onClick={() => onNavigate('next')}
        size="small"
        variant="outlined"
      >
        Siguiente
      </Button>
    </Box>
  );
};

export default DetailNavigation;
