import { getCollectionState } from 'store/leads';
import pinner from 'components/collections/pinner';

export default pinner({
  name: 'leads',
  getCollectionState,
  getItemIdFromParams: ({ lead_uid: uid }) => uid,
});
