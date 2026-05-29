import React from 'react';
import { Box, CircularProgress } from '@mui/material';
import { useElementoDetail } from '../hooks/useElementoDetail';
import CardDetailElement from '../components/detail/CardDetailElement.jsx';
import DetailErrorState from '../components/detail/DetailErrorState.jsx';

const PageDetails = () => {
  const detail = useElementoDetail();

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

export default PageDetails;
