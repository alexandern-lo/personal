import { handleActions } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readUser } from 'store/data/user';

export {
  getCollectionState,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const defaultState = {
};

export default handleActions({
  ...actionHandlers('event-users', readUser),
}, defaultState);
