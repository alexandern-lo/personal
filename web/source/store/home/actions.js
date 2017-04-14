import { createApiCallAction, createAction, createFetchAction } from '../helpers/actions';

export const fetchDashboardSummary = createApiCallAction('home/DASHBOARD_SUMMARY', 'getDashboardSummary');
export const fetchRecentLeadsActivity = createApiCallAction('home/RECENT_LEADS_ACTIVITY', 'getRecentLeadsActivity');
export const fetchDailyLeadsHistory = createApiCallAction('home/LEADS_HISTORY', 'getDailyLeadsHistory');
export const fetchUsersLeadGoals = createFetchAction('home/USERS_LEAD_GOALS');
export const fetchUsersExpenses = createFetchAction('home/USERS_EXPENSES');

export const fetchSuperAdminDashboardSummary =
  createApiCallAction('home/SA_DASHBOARD_SUMMARY', 'getSuperAdminDashboardSummary');
export const fetchNewUsersHistory = createApiCallAction('home/NEW_USERS_HISTORY', 'getNewUsersHistory');
export const fetchNewTrialSubscriptionsHistory = createApiCallAction('/home/NEW_TRIAL_SUBSCRIPTIONS_HISTORY', 'getNewTrialSubscriptionsHistory');
export const fetchNewPaidSubscriptionsHistory = createApiCallAction('/home/NEW_PAID_SUBSCRIPTIONS_HISTORY', 'getNewPaidSubscriptionsHistory');
export const fetchAverageLeadsHistory = createApiCallAction('home/AVERAGE_LEADS_HISTORY', 'getAverageLeadsHistory');
export const fetchAverageAllEventsHistory = createApiCallAction('home/AVERAGE_ALL_EVENTS_HISTORY', 'getAverageAllEventsHistory');
export const fetchAverageConferenceEventsHistory = createApiCallAction('home/AVERAGE_CONFERENCE_EVENTS_HISTORY', 'getAverageConferenceEventsHistory');

export const clearError = createAction('home/CLEAR_ERROR');
export const addExpense = createApiCallAction('home/ADD_EXPENSE', 'addExpense');
export const adjustUserGoal = createApiCallAction('home/ADJUST_USER_GOAL', 'adjustUserGoal');
export const adjustTenantGoals = createApiCallAction('home/ADJUST_TENANT_GOALS', 'adjustTenantGoals');

export const resetUsersLeadGoals = createAction('home/RESET_USERS_LEAD_GOALS');
export const resetUsersExpenses = createAction('home/RESET_USERS_EXPENSES');
export const setTotalLeadsByUserEvent = createAction('home/SET_TOTAL_LEADS_BY_USER_EVENT');
export const setTotalExpensesByUserEvent = createAction('home/SET_TOTAL_EXPENSES_BY_USER_EVENT');
