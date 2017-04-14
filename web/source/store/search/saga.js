import _ from 'lodash';
import { fromStore, callApi, dispatch, actionDispatcher } from '../helpers/sagas';
import { createApiCallResultAction } from '../helpers/actions';

import {
  getActiveTab,
  getFetchParams,
} from './index';

import {
  search,
  sortItems,
  changePage,
  changePerPage,
} from './actions';

const searchResult = createApiCallResultAction(search);

const calls = {
  events: 'fetchEvents',
  leads: 'fetchLeads',
  attendees: 'searchAttendees',
};

function* performSearch({ payload }) {
  try {
    const { collection } = payload || {};
    const names = collection ? [collection] : Object.keys(calls);
    const fetchParams = yield _.map(names, name => fromStore(getFetchParams, name));
    const namedParams = _.zipObject(names, fetchParams);
    const resposes = yield _.map(names, name => callApi(calls[name], namedParams[name]));
    const namedResponses = _.zipObject(names, resposes);
    yield dispatch(searchResult(namedResponses));
  } catch (error) {
    yield dispatch(searchResult(error));
  }
}

function* collectionChange() {
  const collection = yield fromStore(getActiveTab);
  yield dispatch(search({ collection }));
}

export default {
  [search]: performSearch,
  [sortItems]: collectionChange,
  [changePage]: collectionChange,
  [changePerPage]: actionDispatcher(search),
};
