import { connectActions } from 'store';
import {
  fetchDashboardSummary,
  fetchDailyLeadsHistory,
  fetchUsersLeadGoals,
  fetchUsersExpenses,
} from 'store/home/actions';

import LeadsStatsWidget from './tenant-components/leads-stats-widget';
import TotalLeadsByUserWidget from './tenant-components/total-leads-by-user-widget';
import TotalExpensesByUserWidget from './tenant-components/total-expenses-by-user-widget';
import EventsLeadsGoalsWidget from './common-components/events-leads-goals-widget';
import ResourcesWidget from './common-components/resources-widget';
import EventsExpensesWidget from './common-components/events-expenses-widget';

import styles from './tenant-admin-dashboard.module.css';

@connectActions({
  getDashboardSummary: fetchDashboardSummary,
  getDailyLeadsHistory: fetchDailyLeadsHistory,
  getUsersLeadGoals: fetchUsersLeadGoals,
  getUsersExpenses: fetchUsersExpenses,
})
export default class TenantAdminDashboard extends Component {
  static propTypes = {
    getDashboardSummary: PropTypes.func,
    getDailyLeadsHistory: PropTypes.func,
    getUsersLeadGoals: PropTypes.func,
    getUsersExpenses: PropTypes.func,
  }

  componentWillMount() {
    this.fetchDataFromServer();
    this.fetchingDataTimer = setInterval(this.fetchDataFromServer, 1000 * 60 * 5);
  }

  componentWillUnmount() {
    clearInterval(this.fetchingDataTimer);
  }

  onAddExpenseSuccess = () => {
    this.props.getDashboardSummary({ resources_limit: 4 });
    this.props.getUsersExpenses({});
  }

  onAdjustGoalSuccess = () => {
    this.props.getDashboardSummary({ resources_limit: 4 });
    this.props.getUsersLeadGoals({});
  }

  fetchDataFromServer = () => {
    this.props.getDashboardSummary({ resources_limit: 4 });
    this.props.getDailyLeadsHistory();
    this.props.getUsersLeadGoals({});
    this.props.getUsersExpenses({});
  }

  render() {
    return (
      <div className={styles.widgetsContainer}>
        <div className={styles.column}>
          <div className={classNames(styles.widget, styles.top, styles.topLeft)}>
            <LeadsStatsWidget />
          </div>
          <div className={classNames(styles.widget, styles.bottomLeft)}>
            <ResourcesWidget />
          </div>
        </div>
        <div className={styles.column}>
          <div className={classNames(styles.widget, styles.top, styles.middle)}>
            <EventsLeadsGoalsWidget onAdjustGoalSuccess={this.onAdjustGoalSuccess} />
          </div>
          <div className={classNames(styles.widget, styles.middle)}>
            <EventsExpensesWidget onAddExpenseSuccess={this.onAddExpenseSuccess} />
          </div>
        </div>
        <div className={styles.column}>
          <div className={classNames(styles.widget, styles.top, styles.right)}>
            <TotalLeadsByUserWidget />
          </div>
          <div className={classNames(styles.widget, styles.right)}>
            <TotalExpensesByUserWidget />
          </div>
        </div>
      </div>
    );
  }
}
