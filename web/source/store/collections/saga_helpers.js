import invariant from 'invariant';
import buildActions, { SAME_CONTEXT } from './actions_builder';
import { createApiCallResultAction } from '../helpers/actions';
import {
  fromStore,
  apiCaller,
  dispatchApiCall,
  actionDispatcher,
} from '../helpers/sagas';


export const collectionFetcher = (callName, getFetchParams, responseActionCreator) => {
  invariant(typeof callName === 'string', 'api callName should be string');
  invariant(typeof getFetchParams === 'function', 'fetch params getter should be function');
  invariant(typeof responseActionCreator === 'function', 'responseActionCreator should be function');
  return function* fetchCollection({ payload }) {
    const params = yield fromStore(getFetchParams, payload);
    yield dispatchApiCall(responseActionCreator, callName, params);
  };
};


export const actionDispatchers = (name, options = {}) => {
  const {
    fetchItems,
    pinItemById,
    sortItems,
    changePage,
    changePerPage,
    search,
    changeFilter,
  } = buildActions(name);
  const dispatchFetch = actionDispatcher(fetchItems, SAME_CONTEXT);
  const dispatchers = {
    [sortItems]: dispatchFetch,
    [changePage]: dispatchFetch,
    [changePerPage]: dispatchFetch,
    [search]: dispatchFetch,
    [changeFilter]: dispatchFetch,
  };
  if (options.fetch) {
    const { apiCall, getFetchParams } = options.fetch;
    const responseActionCreator = createApiCallResultAction(fetchItems);
    dispatchers[fetchItems] = collectionFetcher(apiCall, getFetchParams, responseActionCreator);
  }
  if (options.pin) {
    const { apiCall } = options.pin;
    const responseActionCreator = createApiCallResultAction(pinItemById);
    dispatchers[pinItemById] = apiCaller(apiCall, responseActionCreator);
  }
  return dispatchers;
};
