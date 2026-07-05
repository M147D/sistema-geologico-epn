import { Box, CircularProgress } from '@mui/material';
import { DetailProvider, useDetail } from '../context/DetailContext';
import CardDetailElement from '../components/detail/CardDetailElement.jsx';
import DetailErrorState from '../components/detail/DetailErrorState.jsx';

const PageDetailsContent = () => {
  const detail = useDetail();

  if (detail.loadingDetail && !detail.elemento)
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
        <CircularProgress />
      </Box>
    );

  if (!detail.loadingDetail && !detail.elemento)
    return <DetailErrorState id={detail.id} onGoBack={detail.handleGoBack} />;

  return <CardDetailElement {...detail} />;
};

const PageDetails = () => (
  <DetailProvider>
    <PageDetailsContent />
  </DetailProvider>
);

export default PageDetails;
