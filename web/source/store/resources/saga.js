import { actionDispatchers } from '../collections/saga_helpers';
import { getFetchParams } from './index';

export default {
  ...actionDispatchers('resources', {
    fetch: {
      apiCall: 'fetchResources',
      getFetchParams,
    },
    pin: {
      apiCall: 'fetchResourceById',
    },
  }),
};
