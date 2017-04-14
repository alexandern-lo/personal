import { createApiCallAction } from '../helpers/actions';
import buildActions from '../collections/actions_builder';

const collection = buildActions('leads');

export const fetchEvents = collection.fetchItems;
export const pinLeadById = collection.pinItemById;

export const createLead = createApiCallAction('leads/CREATE', 'createLead');
export const updateLead = createApiCallAction('leads/UPDATE', 'updateLead');
export const deleteLead = createApiCallAction('leads/DELETE', 'deleteLead');
export const exportLeadsToFile = createApiCallAction('leads/EXPORT_FILE', 'exportLeadsToFile');
export const exportLeadsToCRM = createApiCallAction('leads/EXPORT_CRM', 'exportLeadsToCRM');
