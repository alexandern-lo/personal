import {
  callApi,
  fromStore,
  dispatch,
  apiCaller,
} from '../helpers/sagas';

import { createApiCallResultAction } from '../helpers/actions';
import { actionDispatchers } from '../collections/saga_helpers';

import { getPinnedEvent } from '../events';
import { getFetchParams } from './index';

import {
  fetchAgendaItems,
  pinAgendaByUid,
} from './actions';

const agendaItemsFetched = createApiCallResultAction(fetchAgendaItems);

function* fetcher({ payload }) {
  const event = yield fromStore(getPinnedEvent);
  const params = yield fromStore(getFetchParams, payload);
  try {
    const result = yield callApi('fetchAgendaItems', { eventUid: event.uid, ...params });
    yield dispatch(agendaItemsFetched(result));
  } catch (error) {
    yield dispatch(agendaItemsFetched(error));
  }
}

export default {
  [fetchAgendaItems]: fetcher,
  [pinAgendaByUid]: apiCaller('fetchAgendaByUid', createApiCallResultAction(pinAgendaByUid)),
  ...actionDispatchers('agenda'),
};
