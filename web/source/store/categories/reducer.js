import { handleActions } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readCategory } from 'store/data/category';

export {
  getCollectionState,
  getPinnedItem as getPinnedCategory,
  getPinnedError,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const initialState = {
};

export default handleActions({
  ...actionHandlers('categories', readCategory),
}, initialState);
