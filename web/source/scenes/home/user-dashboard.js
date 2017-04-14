import { connectActions } from 'store';
import {
  fetchDashboardSummary,
  fetchRecentLeadsActivity,
  fetchDailyLeadsHistory,
} from 'store/home/actions';

import TotalLeadsChartWidget from './common-components/total-leads-chart-widget';
import TotalLeadsGoalsDoughnutWidget from './common-components/total-leads-goals-doughnut-widget';
import EventsLeadsGoalsWidget from './common-components/events-leads-goals-widget';
import EventsExpensesWidget from './common-components/events-expenses-widget';
import ResourcesWidget from './common-components/resources-widget';
import RecentLeadsActivityWidget from './user-components/recent-leads-activity-widget';

import styles from './user-dashboard.module.css';

@connectActions({
  getDashboardSummary: fetchDashboardSummary,
  getDailyLeadsHistory: fetchDailyLeadsHistory,
  getRecentLeadsActivity: fetchRecentLeadsActivity,
})
export default class UserDashboard extends Component {
  static propTypes = {
    getDashboardSummary: PropTypes.func,
    getRecentLeadsActivity: PropTypes.func,
    getDailyLeadsHistory: PropTypes.func,
  }

  componentWillMount() {
    this.fetchDataFromServer();
    this.fetchingDataTimer = setInterval(this.fetchDataFromServer, 1000 * 60 * 5);
  }

  componentWillUnmount() {
    clearInterval(this.fetchingDataTimer);
  }

  onAddExpenseSuccess = () => {
    this.props.getDashboardSummary();
  }

  onAdjustGoalSuccess = () => {
    this.props.getDashboardSummary();
  }

  fetchDataFromServer = () => {
    this.props.getDashboardSummary();
    this.props.getDailyLeadsHistory();
    this.props.getRecentLeadsActivity();
  }

  render() {
    return (
      <div className={styles.widgetsContainer}>
        <div>
          <div className={`${styles.widget} ${styles.left}`}>
            <TotalLeadsChartWidget />
          </div>
          <div className={`${styles.widget} ${styles.middle}`}>
            <EventsLeadsGoalsWidget onAdjustGoalSuccess={this.onAdjustGoalSuccess} />
          </div>
          <div className={`${styles.widget} ${styles.right}`}>
            <ResourcesWidget showFooter />
          </div>
        </div>
        <div>
          <div className={`${styles.widget} ${styles.left}`}>
            <TotalLeadsGoalsDoughnutWidget />
          </div>
          <div className={`${styles.widget} ${styles.middle}`}>
            <EventsExpensesWidget onAddExpenseSuccess={this.onAddExpenseSuccess} />
          </div>
          <div className={`${styles.widget} ${styles.right}`}>
            <RecentLeadsActivityWidget />
          </div>
        </div>
      </div>
    );
  }
}
