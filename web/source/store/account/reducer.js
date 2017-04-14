import { handleActions, isApiResultAction } from 'store/helpers/actions';
import { fetchBillingInfo } from './actions';
import { readBillingInfo } from '../data/billing-info';

const defaultState = {
  billingInfo: null,
};

export const getBillingInfo = ({ billingInfo }) => billingInfo;

const onFetchBillingInfo = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return state;
    }
    const billingInfo = readBillingInfo(payload.data);
    return { ...state, billingInfo };
  }
  return state;
};

export default handleActions({
  [fetchBillingInfo]: onFetchBillingInfo,
}, defaultState);
