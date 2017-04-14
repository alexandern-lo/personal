import _ from 'lodash';
import { handleActions, isApiResultAction } from '../helpers/actions';

import { readPlan } from '../data/subscription-plan';
import { fetchPlans } from './actions';

const initialState = {
  plans: null,
  loading: false,
  error: null,
};

export const getPlans = ({ plans }) => plans;
export const isLoading = ({ loading }) => loading;
export const getError = ({ error }) => error;

const onFetchPlans = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, error: payload, loading: false };
    }
    const plans = _.map(payload.data, readPlan);
    return { ...state, plans, error: null, loading: false };
  }
  return { ...state, error: null, loading: true };
};

export default handleActions({
  [fetchPlans]: onFetchPlans,
}, initialState);
