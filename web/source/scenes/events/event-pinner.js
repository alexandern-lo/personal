import { getCollectionState } from 'store/events';
import pinner from 'components/collections/pinner';

export default pinner({
  name: 'events',
  getCollectionState,
  getItemIdFromParams: ({ event_uid: uid }) => uid,
});
