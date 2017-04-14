import { createFetchAction, createApiCallAction } from '../helpers/actions';

export const fetchMobileSettings = createFetchAction('mobile-settings/FETCH');
export const updateMobileSettings = createApiCallAction('mobile-settings/UPDATE', 'updateMobileSettings');
