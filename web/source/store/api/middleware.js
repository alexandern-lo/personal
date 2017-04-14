import invariant from 'invariant';
import ApiError from './error';
import { getApi } from './index';
import { apiAuthError } from './actions';
import {
  isApiResultAction,
  isApiCallAction,
  buildResultActionData,
} from './helpers';


export default ({ getState, dispatch }) => next => (action) => {
  if (isApiResultAction(action) && action.error && action.payload instanceof ApiError) {
    switch (action.payload.status) {
      case 401: // invalid token
      case 402: // invalid subscription
      case 423: // disabled
        next(action);
        return dispatch(apiAuthError(action.payload));
      default: break;
    }
  }
  if (!isApiCallAction(action)) {
    return next(action);
  }
  const api = getApi(getState());
  const { type, apiCall, payload, meta } = action;
  const method = typeof apiCall === 'function' ? apiCall : api[apiCall];
  invariant(typeof method === 'function', `there is no '${apiCall}' api method for '${type}' api action`);
  const promise = method.call(api, payload, dispatch);
  invariant(typeof promise.then === 'function', `${apiCall} api method should return promise`);
  promise
  .then(result => dispatch(buildResultActionData(type, result, meta)))
  .catch((error) => {
    dispatch(buildResultActionData(type, error, meta));
    throw error;
  });
  next(action);
  return promise;
};
