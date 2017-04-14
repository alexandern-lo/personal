import { getCollectionState } from 'store/attendees';
import pinner from 'components/collections/pinner';

export default pinner({
  name: 'attendees',
  getCollectionState,
  getItemIdFromParams: ({ uid }) => uid,
  getFetchParams: ({ event_uid: eventUid, uid: attendeeUid }) => ({ eventUid, attendeeUid }),
});
