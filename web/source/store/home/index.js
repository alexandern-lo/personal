import bind from '../helpers/bind';
import reducer, * as getters from './reducer';

const { binder, wrapAll } = bind(reducer);
export default binder;

export const {
  getDashboardSummary,
  getRecentLeadsActivity,
  getLeadsHistory,
  getUsersLeadGoals,
  getUsersExpenses,
  getSuperAdminDashboardSummary,
  getNewUsersHistory,
  getNewSubscriptionsHistory,
  getAverageLeadsHistory,
  getAverageEventsHistory,

  getError,
  getTotalLeadsByUserEvent,
  getTotalExpensesByUserEvent,
} = wrapAll(getters);
