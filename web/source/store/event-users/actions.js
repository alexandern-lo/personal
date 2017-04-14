import { createApiCallAction } from '../helpers/actions';
import buildActions from '../collections/actions_builder';

const collection = buildActions('event-users');

export const fetchUsers = collection.fetchItems;
export const deleteEventUsers = createApiCallAction('event-users/DELETE_ALL', 'deleteEventUsers');
export const inviteEventUsers = createApiCallAction('event-users/INVITE', 'inviteEventUsers');
