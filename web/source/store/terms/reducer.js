import { handleActions, isApiResultAction } from '../helpers/actions';

import { readTerms } from '../data/terms';
import { fetchTerms } from './actions';

const initialState = {
  terms: null,
  loading: false,
  error: null,
};

export const getLatestTerms = ({ terms }) => terms;
export const isLoading = ({ loading }) => loading;
export const getError = ({ error }) => error;

const onFetchTerms = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, error: payload, loading: false };
    }
    const terms = readTerms(payload.data);
    return { ...state, terms, error: null, loading: false };
  }
  return { ...state, error: null, loading: true };
};

export default handleActions({
  [fetchTerms]: onFetchTerms,
}, initialState);
