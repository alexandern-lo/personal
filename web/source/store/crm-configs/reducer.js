import { handleActions } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readCrmConfig } from 'store/data/crm-config';

import { COLLECTION_NAME } from './actions';

export {
  getCollectionState,
  getPinnedItem as getPinnedCrmConfig,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const initialState = {
};

export default handleActions({
  ...actionHandlers(COLLECTION_NAME, readCrmConfig),
}, initialState);
