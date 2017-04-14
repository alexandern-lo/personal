import { createFetchAction, createApiCallAction } from '../helpers/actions';

export const fetchTerms = createFetchAction('terms/FETCH');

export const acceptTerms = createApiCallAction('terms/ACCEPT', 'acceptTerms');
