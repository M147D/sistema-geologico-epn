// src/pages/PageForm.jsx
import { useState } from 'react';
import FormElement from "../components/crud/FormElement.jsx";
import { useElementos } from '../context/ElementosContext';
import { Box, Tabs, Tab, Paper, Container, Typography } from '@mui/material';
import BiotechIcon from '@mui/icons-material/Biotech';
import DiamondIcon from '@mui/icons-material/Diamond';
import LandscapeIcon from '@mui/icons-material/Landscape';

const PageForm = () => {
    const [tipoElemento, setTipoElemento] = useState('fosil');
    const { crearElemento, loading } = useElementos();

    return (
        <Container maxWidth="xl" sx={{ my: 4 }}>
            <Paper elevation={2} sx={{ overflow: 'hidden' }}>
                <Box sx={{ px: 3, pt: 3, pb: 0, borderBottom: 1, borderColor: 'divider' }}>
                    <Typography variant="h5" fontWeight={600} gutterBottom>
                        Registro de Elemento Geológico
                    </Typography>
                    <Tabs value={tipoElemento} onChange={(_, v) => setTipoElemento(v)}>
                        <Tab icon={<BiotechIcon />} iconPosition="start" label="Fósil" value="fosil" />
                        <Tab icon={<LandscapeIcon />} iconPosition="start" label="Roca" value="roca" />
                        <Tab icon={<DiamondIcon />} iconPosition="start" label="Mineral" value="mineral" />
                    </Tabs>
                </Box>
                <Box sx={{ p: 3 }}>
                    <FormElement tipo={tipoElemento} crearElemento={crearElemento} loading={loading} />
                </Box>
            </Paper>
        </Container>
    );
};

export default PageForm;