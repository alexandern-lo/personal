import { actionDispatchers } from '../collections/saga_helpers';
import { getFetchParams } from './index';

import { COLLECTION_NAME } from './actions';

export default {
  ...actionDispatchers(COLLECTION_NAME, {
    fetch: {
      apiCall: 'fetchCrmConfigs',
      getFetchParams,
    },
    pin: {
      apiCall: 'fetchCrmConfigById',
    },
  }),
};
