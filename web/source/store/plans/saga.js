import { apiCaller } from '../helpers/sagas';
import { createApiCallResultAction } from '../helpers/actions';

import { fetchPlans } from './actions';

export default {
  [fetchPlans]: apiCaller('fetchSubscriptionPlans', createApiCallResultAction(fetchPlans)),
};
