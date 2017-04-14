import { apiCaller } from '../helpers/sagas';
import { createApiCallResultAction } from '../helpers/actions';

import { fetchTerms } from './actions';

export default {
  [fetchTerms]: apiCaller('fetchTerms', createApiCallResultAction(fetchTerms)),
};
