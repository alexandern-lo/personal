import { createApiCallAction } from '../helpers/actions';
import buildActions from '../collections/actions_builder';

const collection = buildActions('agenda');

export const fetchAgendaItems = collection.fetchItems;
export const pinAgendaByUid = collection.pinItemById;

export const createAgenda = createApiCallAction('agenda/CREATE', 'createAgenda');
export const updateAgenda = createApiCallAction('agenda/EDIT', 'updateAgenda');
export const deleteAgenda = createApiCallAction('agenda/DELETE', 'deleteAgenda');
export const deleteAgendaItems = createApiCallAction('agenda/MASS_DELETE', 'deleteAgenadItems');
