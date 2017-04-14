import {
  createAction,
} from 'redux-actions';

export {
  createAction,
  handleActions,
} from 'redux-actions';

export {
  createApiCallAction,
  createApiCallResultAction,
  isApiResultAction,
  isErrorAction,
} from '../api/helpers';


export const createFetchAction = (type, payloadCreator) => (
  createAction(type, payloadCreator, () => ({ cancelPrevious: true }))
);
