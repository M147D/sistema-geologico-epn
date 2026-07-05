import {
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Typography,
  List,
  Box
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import LayersIcon from '@mui/icons-material/Layers';
import FormationToolbar from './FormationToolbar';
import FormationItem from './FormationItem';
import FormationSearchBar from './FormationSearchBar';

/**
 * Componente que muestra la lista de formaciones geológicas
 * @param {Array} formations - Array de formaciones
 * @param {Set} visibleFormations - Set de IDs de formaciones visibles
 * @param {Function} onFormationToggle - Callback para toggle individual
 * @param {Function} onSelectAll - Callback para seleccionar todas
 * @param {Function} onDeselectAll - Callback para deseleccionar todas
 * @param {string} searchQuery - Texto de búsqueda actual
 * @param {Function} onSearchChange - Callback cuando cambia el texto de búsqueda
 */
const FormationList = ({
  formations,
  visibleFormations,
  onFormationToggle,
  onSelectAll,
  onDeselectAll,
  searchQuery,
  onSearchChange
}) => {
  const totalFormations = formations.length;
  const currentSelected = visibleFormations.size;
  const isAllSelected = totalFormations > 0 && currentSelected === totalFormations;
  const isNoneSelected = currentSelected === 0;

  return (
    <Accordion defaultExpanded disableGutters elevation={0}>
      <AccordionSummary
        expandIcon={<ExpandMoreIcon />}
        sx={{
          minHeight: 36,
          '& .MuiAccordionSummary-content': { my: 0.5 },
          bgcolor: 'background.default'
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <LayersIcon fontSize="small" color="action" />
          <Typography variant="body2" fontWeight="600">
            Formaciones ({currentSelected}/{totalFormations})
          </Typography>
        </Box>
      </AccordionSummary>

      <AccordionDetails sx={{ p: 0 }}>
        {/* Barra de búsqueda */}
        <FormationSearchBar
          searchQuery={searchQuery}
          onSearchChange={onSearchChange}
          resultCount={formations.length}
        />

        {formations.length > 0 ? (
          <>
            <FormationToolbar
              isAllSelected={isAllSelected}
              isNoneSelected={isNoneSelected}
              onSelectAll={onSelectAll}
              onDeselectAll={onDeselectAll}
            />

            <List dense sx={{ py: 0, maxHeight: '30vh', overflowY: 'auto' }}>
              {formations.map((formation) => (
                <FormationItem
                  key={formation.codA}
                  formation={formation}
                  isChecked={visibleFormations.has(formation.codA)}
                  onToggle={onFormationToggle}
                />
              ))}
            </List>
          </>
        ) : (
          <Typography
            variant="caption"
            color="text.secondary"
            sx={{ display: 'block', textAlign: 'center', p: 2 }}
          >
            {searchQuery
              ? 'No se encontraron formaciones con ese criterio'
              : 'No hay formaciones disponibles'}
          </Typography>
        )}
      </AccordionDetails>
    </Accordion>
  );
};

export default FormationList;
