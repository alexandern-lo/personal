import { createApiCallAction } from '../helpers/actions';
import buildActions from '../collections/actions_builder';

const collection = buildActions('attendees');

export const fetchAttendees = collection.fetchItems;
export const pinAttendeeByUid = collection.pinItemById;

export const createAttendee = createApiCallAction('attendees/CREATE', 'createAttendee');
export const updateAttendee = createApiCallAction('attendees/UPDATE', 'updateAttendee');
export const deleteAttendee = createApiCallAction('attendees/DELETE', 'deleteAttendee');
export const deleteAttendees = createApiCallAction('attendees/DELETE_ALL', 'deleteAttendees');
export const importAttendees = createApiCallAction('attendees/IMPORT', 'importAttendees');

export const eventCoreLogin = createApiCallAction('attendees/EVENT_CODE/LOGIN', 'eventCoreLogin');
export const eventCoreImport = createApiCallAction('attendees/EVENT_CODE/IMPORT', 'eventCoreImport');
