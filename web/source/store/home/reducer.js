import { handleActions, isApiResultAction } from 'store/helpers/actions';
import {
  readDashboardSummary,
  readRecentLeadsActivity,
  readChartHistory,
  readUsersLeadGoals,
  readUsersExpenses,
  readSADashboardSummary,
} from 'store/data/home';
import {
  fetchDashboardSummary,
  fetchRecentLeadsActivity,
  fetchDailyLeadsHistory,
  fetchUsersLeadGoals,
  fetchUsersExpenses,

  fetchSuperAdminDashboardSummary,
  fetchNewUsersHistory,
  fetchNewTrialSubscriptionsHistory,
  fetchNewPaidSubscriptionsHistory,
  fetchAverageLeadsHistory,
  fetchAverageAllEventsHistory,
  fetchAverageConferenceEventsHistory,

  clearError,
  resetUsersLeadGoals,
  resetUsersExpenses,
  setTotalLeadsByUserEvent,
  setTotalExpensesByUserEvent,
} from './actions';

const initialState = {
  dashboardSummary: null,
  recentLeadsActivity: null,
  leadsHistory: null,
  usersLeadGoals: null,
  usersExpenses: null,
  superAdminDashboardSummary: null,
  newUsersHistory: null,
  newTrialSubscriptionsHistory: null,
  newPaidSubscriptionsHistory: null,
  averageLeadsHistory: null,
  averageAllEventsHistory: null,
  averageConferenceEventsHistory: null,

  error: null,
  totalLeadsByUserEvent: null,
  totalExpensesByUserEvent: null,
};

export const getDashboardSummary = ({ dashboardSummary }) => dashboardSummary;
export const getRecentLeadsActivity = ({ recentLeadsActivity }) => recentLeadsActivity;
export const getLeadsHistory = ({ leadsHistory }) => leadsHistory;
export const getUsersLeadGoals = ({ usersLeadGoals }) => usersLeadGoals;
export const getUsersExpenses = ({ usersExpenses }) => usersExpenses;

export const getSuperAdminDashboardSummary =
  ({ superAdminDashboardSummary }) => superAdminDashboardSummary;
export const getNewUsersHistory = ({ newUsersHistory }) => newUsersHistory;

export const getNewSubscriptionsHistory = ({
  newTrialSubscriptionsHistory, newPaidSubscriptionsHistory,
}) => (newTrialSubscriptionsHistory && newPaidSubscriptionsHistory &&
  {
    trial: newTrialSubscriptionsHistory,
    paid: newPaidSubscriptionsHistory,
  }
);

export const getAverageLeadsHistory = ({ averageLeadsHistory }) => averageLeadsHistory;
export const getAverageEventsHistory = ({
  averageAllEventsHistory, averageConferenceEventsHistory,
}) => (averageAllEventsHistory && averageConferenceEventsHistory &&
  {
    all: averageAllEventsHistory,
    conference: averageConferenceEventsHistory,
  }
);

export const getError = ({ error }) => error;
export const getTotalLeadsByUserEvent = ({ totalLeadsByUserEvent }) => totalLeadsByUserEvent;
export const getTotalExpensesByUserEvent =
  ({ totalExpensesByUserEvent }) => totalExpensesByUserEvent;

const onFetchData = (name, reader, state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;

    if (error) {
      return { ...state, error: payload };
    }

    const { data } = payload;
    return { ...state, [name]: reader(data) };
  }

  return state;
};

export default handleActions({
  [fetchDashboardSummary]: onFetchData.bind(this, 'dashboardSummary', readDashboardSummary),
  [fetchRecentLeadsActivity]: onFetchData.bind(this, 'recentLeadsActivity', readRecentLeadsActivity),
  [fetchDailyLeadsHistory]: onFetchData.bind(this, 'leadsHistory', readChartHistory),
  [fetchUsersLeadGoals]: onFetchData.bind(this, 'usersLeadGoals', readUsersLeadGoals),
  [fetchUsersExpenses]: onFetchData.bind(this, 'usersExpenses', readUsersExpenses),
  [fetchSuperAdminDashboardSummary]: onFetchData.bind(this, 'superAdminDashboardSummary', readSADashboardSummary),
  [fetchNewUsersHistory]: onFetchData.bind(this, 'newUsersHistory', readChartHistory),
  [fetchNewTrialSubscriptionsHistory]: onFetchData.bind(this, 'newTrialSubscriptionsHistory', readChartHistory),
  [fetchNewPaidSubscriptionsHistory]: onFetchData.bind(this, 'newPaidSubscriptionsHistory', readChartHistory),
  [fetchAverageLeadsHistory]: onFetchData.bind(this, 'averageLeadsHistory', readChartHistory),
  [fetchAverageAllEventsHistory]: onFetchData.bind(this, 'averageAllEventsHistory', readChartHistory),
  [fetchAverageConferenceEventsHistory]: onFetchData.bind(this, 'averageConferenceEventsHistory', readChartHistory),
  [clearError]: state => ({ ...state, error: null }),
  [resetUsersLeadGoals]: state => ({ ...state, usersLeadGoals: null }),
  [resetUsersExpenses]: state => ({ ...state, usersExpenses: null }),
  [setTotalLeadsByUserEvent]: (state, action) => ({
    ...state,
    totalLeadsByUserEvent: action.payload,
  }),
  [setTotalExpensesByUserEvent]: (state, action) => ({
    ...state,
    totalExpensesByUserEvent: action.payload,
  }),
}, initialState);
