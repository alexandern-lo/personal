import { createApiCallAction } from '../helpers/actions';

export const searchEvents = createApiCallAction('events/SEARCH', 'fetchEvents');

export const createEvent = createApiCallAction('events/CREATE', 'createEvent');
export const updateEvent = createApiCallAction('events/UPDATE', 'updateEvent');
export const deleteEvent = createApiCallAction('events/DELETE', 'deleteEvent');
export const deleteEvents = createApiCallAction('events/DELETE_ALL', 'deleteEvents');
