import { actionDispatchers } from '../collections/saga_helpers';
import { getFetchParams } from './index';

export default {
  ...actionDispatchers('users', {
    fetch: {
      apiCall: 'fetchUsers',
      getFetchParams,
    },
  }),
};
