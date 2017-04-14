import { handleActions } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readAgenda } from 'store/data/agenda';

export {
  getCollectionState,
  getPinnedItem as getPinnedAgenda,
  getPinnedError,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const defaultState = {
};

export default handleActions({
  ...actionHandlers('agenda', readAgenda),
}, defaultState);
