import { takeLatest, takeEvery, fork } from 'redux-saga/lib/internal/io';

import {
  isApiResultAction,
  isErrorAction,
} from './actions';

const toArray = arg => (Array.isArray(arg) ? arg : [arg]);

function bindArray(array) {
  const handlers = array.map(toArray);
  return function* runSagas() {
    yield handlers.map(args => fork(...args));
  };
}

function matchAction(actionType, cancelPrevious) {
  return (action) => {
    if (action.type !== actionType) return false;
    if (isApiResultAction(action) || isErrorAction(action)) return false;
    const shouldCancel = action.meta && action.meta.cancelPrevious;
    return shouldCancel ? cancelPrevious : !cancelPrevious;
  };
}

function bindAction(actionType, handler) {
  const args = toArray(handler);
  return function* handleAction() {
    yield [
      takeLatest(matchAction(actionType, true), ...args),
      takeEvery(matchAction(actionType, false), ...args),
    ];
  };
}

export function bindSaga(saga) {
  if (saga.call) return saga;
  if (Array.isArray(saga)) return bindArray(saga);
  const sagas = Object.keys(saga).map(actionType => bindAction(actionType, saga[actionType]));
  return bindArray(sagas);
}

export default function bindSagas(sagas) {
  const bound = {};
  Object.keys(sagas).forEach((name) => {
    bound[name] = bindSaga(sagas[name]);
  });
  return bound;
}
