import invariant from 'invariant';
import { select, call, put } from 'redux-saga/lib/effects';
import { getApi as _getApi } from '../api';

export { call } from 'redux-saga/lib/effects';
export const fromStore = select;
export const dispatch = put;

export const getApi = () => fromStore(_getApi);

export function* callApi(callName, ...args) {
  invariant(typeof callName === 'string', 'api callName should be string');
  const api = yield getApi();
  invariant(typeof api[callName] === 'function', `api should have method ${callName}`);
  return yield call(api[callName], ...args);
}

export function* dispatchApiCall(responseActionCreator, callName, ...args) {
  invariant(typeof responseActionCreator === 'function', 'responseActionCreator should be function');
  invariant(typeof callName === 'string', 'api callName should be string');
  const api = yield getApi();
  invariant(typeof api[callName] === 'function', `api should have method ${callName}`);
  try {
    const responseData = yield callApi(callName, ...args);
    yield dispatch(responseActionCreator(responseData));
  } catch (error) {
    yield dispatch(responseActionCreator(error));
  }
}

export const apiCaller = (callName, responseActionCreator, errorActionCreator) => {
  invariant(typeof callName === 'string', 'api callName should be string');
  invariant(typeof responseActionCreator === 'function', 'responseActionCreator should be function');
  if (errorActionCreator) {
    invariant(typeof errorActionCreator === 'function', 'errorActionCreator should be function');
  }
  return function* apiCallerSaga({ payload }) {
    try {
      const responseData = yield callApi(callName, payload);
      yield dispatch(responseActionCreator(responseData));
    } catch (error) {
      const actionCreator = errorActionCreator || responseActionCreator;
      yield dispatch(actionCreator(error));
    }
  };
};

export const actionDispatcher = (actionCreator, payload) => {
  invariant(typeof actionCreator === 'function', 'actionCreator should be function');
  const transformer = typeof payload === 'function' ? payload : () => payload;
  return function* actionDispatcherSaga(triggerAction) {
    yield dispatch(actionCreator(transformer(triggerAction)));
  };
};
