import { handleActions, isApiResultAction } from '../helpers/actions';

import {
  fetchMobileSettings,
  updateMobileSettings,
} from './actions';

const initialState = {
  mobileSettings: null,
  loading: false,
  error: null,
};

export const getMobileSettings = ({ mobileSettings }) => mobileSettings;
export const isLoading = ({ loading }) => loading;
export const getError = ({ error }) => error;

const onFetchMobileSettings = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, error: payload, loading: false };
    }
    const mobileSettings = payload.data;
    return { ...state, mobileSettings, error: null, loading: false };
  }
  return { ...state, error: null, loading: true };
};

const onUpdateMobileSettings = (state, action) => {
  if (isApiResultAction(action) && !action.error) {
    return { ...state, mobileSettings: action.payload.data };
  }
  return state;
};

export default handleActions({
  [fetchMobileSettings]: onFetchMobileSettings,
  [updateMobileSettings]: onUpdateMobileSettings,
}, initialState);
