import { handleActions, isApiResultAction } from 'store/helpers/actions';
import { actionHandlers } from 'store/collections/reducer_helpers';
import { readUser } from 'store/data/user';
import { onUpdateItem } from 'store/collections';
import { userGrantAdmin, userRevokeAdmin, enableUser, disableUser, userResendInvite } from './actions';

export {
  getCollectionState,
  getFetchParams,
} from 'store/collections/reducer_helpers';

const defaultState = {
};

const onUserUpdate = (state, action) => {
  const { error, payload } = action;
  if (isApiResultAction(action) && !error) {
    const user = readUser(payload.data);
    return onUpdateItem(state, user);
  }
  return state;
};

export default handleActions({
  ...actionHandlers('users', readUser),
  [userGrantAdmin]: onUserUpdate,
  [userRevokeAdmin]: onUserUpdate,
  [enableUser]: onUserUpdate,
  [disableUser]: onUserUpdate,
  [userResendInvite]: onUserUpdate,
}, defaultState);
