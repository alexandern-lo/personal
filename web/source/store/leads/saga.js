import { actionDispatchers } from '../collections/saga_helpers';
import { getFetchParams } from './index';

export default {
  ...actionDispatchers('leads', {
    fetch: {
      apiCall: 'fetchLeads',
      getFetchParams,
    },
    pin: {
      apiCall: 'fetchLeadById',
    },
  }),
};
