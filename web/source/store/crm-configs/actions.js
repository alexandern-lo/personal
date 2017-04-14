import { createApiCallAction } from '../helpers/actions';

export const COLLECTION_NAME = 'crm-configs';

export const createCrmConfig = createApiCallAction('crm-configs/CREATE', 'createCrmConfig');
export const updateCrmConfig = createApiCallAction('crm-configs/UPDATE', 'updateCrmConfig');
export const deleteCrmConfig = createApiCallAction('crm-configs/DELETE', 'deleteCrmConfig');
export const grantCrmConfig = createApiCallAction('crm-configs/GRANT', 'grantCrmConfig');

export const setDefaultCrm = createApiCallAction('crm-configs/SET_DEFAULT', 'setDefaultCrm');
