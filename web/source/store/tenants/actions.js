import { createApiCallAction } from '../helpers/actions';

export const searchTenants = createApiCallAction('tenants/SEARCH', 'fetchTenants');
