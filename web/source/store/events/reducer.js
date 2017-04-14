import { handleActions } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readEvent } from 'store/data/event';

export {
  getCollectionState,
  getPinnedItem as getPinnedEvent,
  getPinnedError,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const initialState = {
};

export default handleActions({
  ...actionHandlers('events', readEvent),
}, initialState);
