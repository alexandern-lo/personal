import { actionDispatchers } from '../collections/saga_helpers';
import { getCollectionState, getFetchParams, EVENT_USER_INVITED_CONTEXT } from './index';
import { getPinnedEvent } from '../events';

const getFetchEventUsersParams = store => ({
  eventUid: getPinnedEvent(store).uid,
  params: {
    ...getFetchParams(store),
    type: getCollectionState(store).context.startsWith(EVENT_USER_INVITED_CONTEXT) ? 'invited' : 'not_invited',
  },
});

export default {
  ...actionDispatchers('event-users', {
    fetch: {
      apiCall: 'fetchEventUsers',
      getFetchParams: getFetchEventUsersParams,
    },
  }),
};
