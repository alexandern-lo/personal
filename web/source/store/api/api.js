import { BASE_API_URL, TOKEN_KEY } from 'config/api.config';

import ApiBase from './base';

const json = o => JSON.stringify(o);

export default class Api extends ApiBase {

  constructor() {
    super({
      baseURL: BASE_API_URL,
    });
  }

  getAuthToken = () => sessionStorage.getItem(TOKEN_KEY);

  fetchProfile = params => this.get('/profile', { params });
  fetchInvite = code => this.get(`/users/invite/${code}`);
  setDefaultCrm = uid => this.put('/profile/crm', json(uid));
  fetchTerms = () => this.get('/profile/terms');
  acceptTerms = () => this.post('/profile/terms/accept');
  fetchSubscriptionPlans = params => this.get('/subscription/plans', { params });
  refreshSubscription = (uid = '') => this.post('/subscription/refresh', json(uid));
  startTrialSubscription = () => this.post('/subscription/start_trial');
  updateProfile = data => this.put('/profile', data);

  fetchCrmConfigs = params => this.get('/crm', { params });
  fetchCrmConfigById = uid => this.get(`/crm/${uid}`);
  createCrmConfig = data => this.post('/crm', data);
  updateCrmConfig = data => this.put(`/crm/${data.uid}`, data);
  deleteCrmConfig = uid => this.delete(`/crm/${uid}`);
  grantCrmConfig = data => this.put(`/crm/${data.uid}/grant_code`, data);

  fetchUsers = params => this.get('/users', { params });
  userGrantAdmin = uid => this.put('/users/grant_admin', json(uid));
  userRevokeAdmin = uid => this.put('/users/revoke_admin', json(uid));
  enableUser = uid => this.put('/users/enable', json(uid));
  disableUser = uid => this.put('/users/disable', json(uid));
  userResendInvite = uid => this.post(`/users/${uid}/resend_invite`);
  inviteUser = data => this.post('/users/invite', data, data.suid && { params: { suid: data.suid } });
  deleteUsers = data => this.post('/users/delete', data);
  getUsersByEvent = ({ eventUid, type = 'invited' }) => this.get(`/events/${eventUid}/users`, { params: { event_uid: eventUid, type } });

  fetchEvents = params => this.get('/events', { params });
  fetchEventById = uid => this.get(`/events/${uid}`);
  createEvent = data => this.post('/events', data);
  updateEvent = data => this.put(`/events/${data.event_uid}`, data);
  deleteEvent = uid => this.delete(`/events/${uid}`);
  deleteEvents = data => this.post('/events/delete', data);

  fetchLeads = params => this.get('/leads', { params });
  fetchLeadById = uid => this.get(`/leads/${uid}`);
  createLead = data => this.post('/leads', data);
  updateLead = data => this.put(`/leads/${data.lead_uid}`, data);
  deleteLead = uid => this.delete(`/leads/${uid}`);
  exportLeadsToFile = data => this.post('/leads/export/file', data, { fullResponse: true });
  exportLeadsToCRM = data => this.post('/leads/export/crm', data);

  fetchAgendaItems = ({ eventUid, ...params }) => this.get(`/events/${eventUid}/agenda_items`, { params });
  fetchAgendaByUid = ({ eventUid, agendaUid }) => this.get(`/events/${eventUid}/agenda_items/${agendaUid}`);
  createAgenda = ({ eventUid, ...params }) => this.post(`/events/${eventUid}/agenda_items`, params);
  updateAgenda = ({ eventUid, uid: agendaUid, ...params }) =>
    this.put(`/events/${eventUid}/agenda_items/${agendaUid}`, params);
  deleteAgenda = ({ eventUid, agendaUid }) => this.delete(`/events/${eventUid}/agenda_items/${agendaUid}`);
  deleteAgenadItems = ({ eventUid, agendaItemsUids }) =>
    this.post(`/events/${eventUid}/agenda_items/delete`, { agenda_item_uids: agendaItemsUids });

  requestFileUpload = fileName => this.post(`/users/resources/upload_token/${encodeURIComponent(fileName)}`);

  fetchQuestions = eventUid => this.get(`/events/${eventUid}/questions`);
  fetchQuestionById = ({ eventUid, uid }) => this.get(`/events/${eventUid}/questions/${uid}`);
  createQuestion = ({ eventUid, data }) => this.post(`/events/${eventUid}/questions`, data);
  updateQuestion = ({ eventUid, data }) => this.put(`/events/${eventUid}/questions/${data.uid}`, data);
  deleteQuestions = ({ eventUid, data }) => this.post(`/events/${eventUid}/questions/delete`, data);
  moveQuestion = ({ eventUid, uid, data }) => this.patch(`/events/${eventUid}/questions/${uid}/move`, data);

  fetchAttendees = ({ eventUid, filter = {}, ...params }) => this.post(`/events/${eventUid}/attendees/filter`, filter, { params });
  searchAttendees = params => this.get('/attendees', { params });
  fetchAttendeeByUid = ({ eventUid, attendeeUid }) => this.get(`/events/${eventUid}/attendees/${attendeeUid}`);
  createAttendee = ({ eventUid, ...params }) => this.post(`/events/${eventUid}/attendees`, params);
  updateAttendee = ({ eventUid, attendeeUid, ...params }) => this.put(`/events/${eventUid}/attendees/${attendeeUid}`, params);
  deleteAttendee = ({ eventUid, attendeeUid }) => this.delete(`/events/${eventUid}/attendees/${attendeeUid}`);
  importAttendees = ({ eventUid, file }) => this.post(`/events/${eventUid}/attendees/import`, file);
  eventCoreLogin = ({ eventUid, data }) => this.post(`/events/${eventUid}/attendees/eventcore_reports`, data);
  eventCoreImport = ({ eventUid, data }) => this.post(`/events/${eventUid}/attendees/eventcore_import`, data);

  fetchResources = params => this.get('/users/resources/', { params });
  fetchResourceById = uid => this.get(`/users/resources/${uid}`);
  createResource = data => this.post('/users/resources', data);
  updateResource = ({ uid, ...data }) => this.put(`/users/resources/${uid}`, data);
  deleteResource = uid => this.delete(`/users/resources/${uid}`);

  fetchCategories = ({ eventUid, params }) => this.get(`/events/${eventUid}/attendee_categories`, { params });
  fetchCategoryById = ({ eventUid, categoryUid }) => this.get(`/events/${eventUid}/attendee_categories/${categoryUid}`);
  createCategory = ({ eventUid, data }) => this.post(`/events/${eventUid}/attendee_categories`, data);
  updateCategory = ({ eventUid, data }) => this.put(`/events/${eventUid}/attendee_categories/${data.uid}`, data);
  deleteCategories = ({ eventUid, data }) => this.post(`/events/${eventUid}/attendee_categories/delete`, data);

  fetchEventUsers = ({ eventUid, params }) => this.get(`/events/${eventUid}/users`, { params });
  deleteEventUsers = ({ eventUid, data }) => this.post(`/events/${eventUid}/users/delete`, data);
  inviteEventUsers = ({ eventUid, data }) => this.post(`/events/${eventUid}/users/invite`, data);

  fetchMobileSettings = () => this.get('/mobile_settings');
  updateMobileSettings = data => this.put('/mobile_settings', data);

  fetchTenants = params => this.get('/tenants', { params });

  fetchBillingInfo = () => this.get('/subscription/billing_info');

  getDashboardSummary = params => this.get('/dashboard', { params: {
    events_sort_field: 'start_date',
    events_sort_order: 'asc',
    events_limit: 5,
    resources_sort_field: 'name',
    resources_sort_order: 'asc',
    resources_limit: 5,
    ...params,
  } });

  getRecentLeadsActivity = (limit = 5) => this.get('/leads/recent_activity', { params: { limit } });
  getDailyLeadsHistory = (limit = 30) => this.get('/dashboard/leads_history/daily', { params: { limit } });
  getUsersLeadGoals = ({ limit = 5, eventUids }) => this.post('/dashboard/users/leads_goals', { limit, event_uids: eventUids });
  getUsersExpenses = ({ limit = 5, eventUids }) => this.post('./dashboard/users/expenses', { limit, event_uids: eventUids });

  getSuperAdminDashboardSummary = () => this.get('dashboard/superadmin/summary');
  getNewUsersHistory = (limit = 30) => this.get('dashboard/superadmin/history/new_users/daily', { params: { limit } });

  getNewSubscriptionsHistory = ({ limit = 30, type }) => this.get('dashboard/superadmin/history/new_subscriptions/daily', { params: { limit, type } });
  getNewTrialSubscriptionsHistory = limit => this.getNewSubscriptionsHistory({ limit, type: 'trial' });
  getNewPaidSubscriptionsHistory = limit => this.getNewSubscriptionsHistory({ limit, type: 'paid' });

  getAverageLeadsHistory = (limit = 12) => this.get('dashboard/superadmin/history/average_leads/monthly', { params: { limit } });

  getAverageEventsHistory = ({ limit = 12, type }) => this.get('dashboard/superadmin/history/average_events/monthly', { params: { limit, type } });
  getAverageAllEventsHistory = limit => this.getAverageEventsHistory({ limit, type: 'all' });
  getAverageConferenceEventsHistory = limit => this.getAverageEventsHistory({ limit, type: 'conference' });

  addExpense = ({ eventUid, ...params }) => this.post(`/events/${eventUid}/expenses`, { event_uid: eventUid, ...params });
  adjustUserGoal = ({ eventUid, ...params }) => this.post(`/events/${eventUid}/goals`, { event_uid: eventUid, ...params });
  adjustTenantGoals = ({ eventUid, ...params }) => this.post(`/events/${eventUid}/goals/adjust`, { event_uid: eventUid, ...params });
}
