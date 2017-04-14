import { handleActions } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readResource } from 'store/data/resource';

export {
  getCollectionState,
  getPinnedItem as getPinnedResource,
  getPinnedError,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const defaultState = {
};

export default handleActions({
  ...actionHandlers('resources', readResource),
}, defaultState);
