import {
  callApi,
  fromStore,
  dispatch,
  apiCaller,
} from '../helpers/sagas';

import { createApiCallResultAction } from '../helpers/actions';
import { actionDispatchers } from '../collections/saga_helpers';

import { getPinnedEvent } from '../events';
import { getCollectionState } from './index';

import {
  fetchAttendees,
  pinAttendeeByUid,
} from './actions';

const attendeesFetched = createApiCallResultAction(fetchAttendees);
function* fetcher() {
  const event = yield fromStore(getPinnedEvent);
  const { search, filters, sortField,
    sortOrder, page, perPage } = yield fromStore(getCollectionState);
  try {
    const result = yield callApi('fetchAttendees', {
      eventUid: event.uid,
      filter: {
        query: search,
        ...filters,
      },
      sortField,
      sortOrder,
      page,
      perPage,
    });
    yield dispatch(attendeesFetched(result));
  } catch (error) {
    yield dispatch(attendeesFetched(error));
  }
}

export default {
  [fetchAttendees]: fetcher,
  [pinAttendeeByUid]: apiCaller('fetchAttendeeByUid', createApiCallResultAction(pinAttendeeByUid)),
  ...actionDispatchers('attendees'),
};
