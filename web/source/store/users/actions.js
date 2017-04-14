import { createApiCallAction } from '../helpers/actions';
import buildActions from '../collections/actions_builder';

const collection = buildActions('users');
export const fetchUsers = collection.fetchItems;

export const searchUsers = createApiCallAction('users/SEARCH', 'fetchUsers');

export const userGrantAdmin = createApiCallAction('users/GRANT_ADMIN', 'userGrantAdmin');
export const userRevokeAdmin = createApiCallAction('users/REMOVE_ADMIN', 'userRevokeAdmin');

export const enableUser = createApiCallAction('users/ENABLE', 'enableUser');
export const disableUser = createApiCallAction('users/DISABLE', 'disableUser');

export const userResendInvite = createApiCallAction('users/RESEND_INVITE', 'userResendInvite');

export const inviteUser = createApiCallAction('users/INVITE', 'inviteUser');

export const deleteUsers = createApiCallAction('users/DELETE_ALL', 'deleteUsers');

export const fetchUsersByEvent = createApiCallAction('users/FETCH_USERS_BY_EVENT', 'getUsersByEvent');
