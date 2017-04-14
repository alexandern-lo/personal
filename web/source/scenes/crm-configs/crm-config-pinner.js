import { getCollectionState } from 'store/crm-configs';
import pinner from 'components/collections/pinner';

export default pinner({
  name: 'crm-configs',
  getCollectionState,
  getItemIdFromParams: ({ crm_config_uid: uid }) => uid,
});
