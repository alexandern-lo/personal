import { handleActions, isApiResultAction } from 'store/helpers/actions';
import { actionHandlers, replaceItem } from 'store/collections/reducer_helpers';
import { readLead } from 'store/data/lead';

import { updateLead } from './actions';

export {
  getCollectionState,
  getPinnedItem as getPinnedLead,
  getPinnedError,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const initialState = {
};

const onUpdate = (state, action) => {
  if (isApiResultAction(action)) {
    const { payload, error } = action;
    if (error) return state;
    const lead = readLead(payload.data);
    return replaceItem(state, lead, item => item.uid === lead.uid);
  }
  return state;
};

export default handleActions({
  [updateLead]: onUpdate,
  ...actionHandlers('leads', readLead),
}, initialState);
