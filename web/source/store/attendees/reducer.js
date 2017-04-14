import { handleActions } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readAttendee } from 'store/data/attendee';

export {
  getCollectionState,
  getPinnedItem as getPinnedAttendee,
  getPinnedError,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const initialState = {
};

export default handleActions({
  ...actionHandlers('attendees', readAttendee),
}, initialState);
