import { actionDispatchers } from '../collections/saga_helpers';
import { getFetchParams } from './index';

export default {
  ...actionDispatchers('events', {
    fetch: {
      apiCall: 'fetchEvents',
      getFetchParams,
    },
    pin: {
      apiCall: 'fetchEventById',
    },
  }),
};
