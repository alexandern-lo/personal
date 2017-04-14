import { handleAction } from 'redux-actions';
import { internalSetupApi } from './actions';

export const haveToken = (api) => {
  const token = api && api.getAuthToken();
  return token && token.length > 0;
};

export default handleAction(internalSetupApi, (_, { payload }) => (payload), {});
