import { createApiCallAction } from '../helpers/actions';

export const fetchBillingInfo = createApiCallAction('account/billing-info/FETCH', 'fetchBillingInfo');
