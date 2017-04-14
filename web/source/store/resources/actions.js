import { createApiCallAction } from '../helpers/actions';

export const createResource = createApiCallAction('resources/CREATE', 'createResource');
export const updateResource = createApiCallAction('resources/UPDATE', 'updateResource');
export const deleteResource = createApiCallAction('resources/DELETE', 'deleteResource');
