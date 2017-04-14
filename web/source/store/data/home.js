import typeReader from '../helpers/type_reader';
import typeShape, { required, optional } from '../helpers/type_shape';

const moneyType = (from, isRequired = true) => ({
  type: {
    amount: required(Number),
    currency: required(String),
  },
  from,
  required: isRequired,
});

const chartHistoryType = [{
  date: required(String),
  value: required(Number),
}];

const leadsStatsType = {
  type: {
    allTimeCount: required(Number, 'alltime_count'),
    allTimeGoal: required(Number, 'alltime_goal'),
    lastPeriodCount: required(Number, 'last_period_count'),
    lastPeriodGoal: required(Number, 'last_period_goal'),
    thisYearExpenses: moneyType('this_year_expenses'),
    thisYearCpl: moneyType('this_year_cpl'),
  },
  from: 'leads_statistics',
  required: true,
};

const dashboardSummaryType = {
  leadsStats: leadsStatsType,
  events: [{
    uid: required(String, 'event_uid'),
    name: required(String),
    leadsGoal: required(Number, 'leads_goal'),
    leadsCount: required(Number, 'leads_count'),
    totalExpenses: moneyType('total_expenses'),
  }],
  resources: [{
    uid: required(String, 'resource_uid'),
    name: required(String),
    type: required(String),
    url: required(String),
    sentCount: required(Number, 'sent_count'),
    openedCount: required(Number, 'opened_count'),
  }],
};

const recentLeadsActivityType = [{
  leadUid: required(String, 'lead_uid'),
  firstName: required(String, 'first_name'),
  lastName: optional(String, 'last_name'),
  photoUrl: optional(String, 'photo_url'),
  eventUid: required(String, 'event_uid'),
  eventName: required(String, 'event_name'),
  performedAction: required(String, 'performed_action'),
  performedAt: required(String, 'performed_at'),
}];

const usersLeadGoalsType = [{
  uid: required(String, 'user_uid'),
  firstName: required(String, 'first_name'),
  lastName: required(String, 'last_name'),
  leadsGoal: required(String, 'leads_goal'),
  leadsCount: required(String, 'leads_count'),
}];

const usersExpensesType = {
  totalExpenses: moneyType('total_expenses'),
  userExpenses: {
    type: [{
      uid: required(String, 'user_uid'),
      firstName: required(String, 'first_name'),
      lastName: required(String, 'last_name'),
      amount: moneyType('amount'),
    }],
    from: 'user_expenses',
    required: true,
  },
};

const subscriptionType = (from, isRequired = true) => ({
  type: {
    total: required(Number),
    trial: required(Number),
    paid: required(Number),
  },
  from,
  required: isRequired,
});

const SADashboardSummaryType = {
  users: {
    allTime: subscriptionType('all_time'),
    yesterday: subscriptionType('yesterday'),
  },
  subscriptions: {
    allTime: subscriptionType('all_time'),
    lastPeriod: subscriptionType('last_period'),
  },
  leads: {
    allTime: subscriptionType('all_time'),
  },
  events: {
    allTime: {
      type: {
        total: required(Number),
        conference: required(Number),
      },
      from: 'all_time',
      required: true,
    },
  },
};

export const readDashboardSummary = typeReader(dashboardSummaryType);
export const dashboardSummaryShape = typeShape(dashboardSummaryType);

export const readRecentLeadsActivity = typeReader(recentLeadsActivityType);
export const recentLeadsActivityShape = typeShape(recentLeadsActivityType);

export const readChartHistory = typeReader(chartHistoryType);
export const chartHistoryShape = typeShape(chartHistoryType);

export const readUsersLeadGoals = typeReader(usersLeadGoalsType);
export const usersLeadGoalsShape = typeShape(usersLeadGoalsType);

export const readUsersExpenses = typeReader(usersExpensesType);
export const usersExpensesShape = typeShape(usersExpensesType);

export const readSADashboardSummary = typeReader(SADashboardSummaryType);
export const SADashboardSummaryShape = typeShape(SADashboardSummaryType);
