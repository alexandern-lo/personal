import { createAction } from '../helpers/actions';

export const internalSetupApi = createAction('@@API/SETUP');

export const apiAuthError = createAction('@@API/AUTH_ERROR');
