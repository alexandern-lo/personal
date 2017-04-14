import { getCollectionState } from 'store/resources';
import pinner from 'components/collections/pinner';

export default pinner({
  name: 'resources',
  getCollectionState,
  getItemIdFromParams: ({ resource_uid: uid }) => uid,
});
