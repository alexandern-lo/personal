import { apiCaller } from '../helpers/sagas';
import { createApiCallResultAction } from '../helpers/actions';

import { fetchMobileSettings } from './actions';

export default {
  [fetchMobileSettings]: apiCaller('fetchMobileSettings', createApiCallResultAction(fetchMobileSettings)),
};
