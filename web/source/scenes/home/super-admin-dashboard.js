import { connectActions } from 'store';

import {
  fetchSuperAdminDashboardSummary,
  fetchNewUsersHistory,
  fetchNewTrialSubscriptionsHistory,
  fetchNewPaidSubscriptionsHistory,
  fetchAverageLeadsHistory,
  fetchAverageAllEventsHistory,
  fetchAverageConferenceEventsHistory,
} from 'store/home/actions';

import TotalUsersWidget from './super-admin-components/total-users-widget';
import NewUsersWidget from './super-admin-components/new-users-widget';
import NewSubscriptionsWidget from './super-admin-components/new-subscriptions-widget';
import RecurlyWidget from './super-admin-components/recurly-widget';
import AverageLeadsWidget from './super-admin-components/average-leads-widget';
import AverageEventsWidget from './super-admin-components/average-events-widget';

import styles from './super-admin-dashboard.module.css';

@connectActions({
  getSuperAdminDashboardSummary: fetchSuperAdminDashboardSummary,
  getNewUsersHistory: fetchNewUsersHistory,
  getNewTrialSubscriptionsHistory: fetchNewTrialSubscriptionsHistory,
  getNewPaidSubscriptionsHistory: fetchNewPaidSubscriptionsHistory,
  getAverageLeadsHistory: fetchAverageLeadsHistory,
  getAverageAllEventsHistory: fetchAverageAllEventsHistory,
  getAverageConferenceEventsHistory: fetchAverageConferenceEventsHistory,
})
export default class SuperAdminDashboard extends Component {
  static propTypes = {
    getSuperAdminDashboardSummary: PropTypes.func,
    getNewUsersHistory: PropTypes.func,
    getNewTrialSubscriptionsHistory: PropTypes.func,
    getNewPaidSubscriptionsHistory: PropTypes.func,
    getAverageLeadsHistory: PropTypes.func,
    getAverageAllEventsHistory: PropTypes.func,
    getAverageConferenceEventsHistory: PropTypes.func,
  }

  constructor(props) {
    super(props);

    props.getSuperAdminDashboardSummary();
    props.getNewUsersHistory();
    props.getNewTrialSubscriptionsHistory();
    props.getNewPaidSubscriptionsHistory();
    props.getAverageLeadsHistory(new Date().getMonth + 1);
    props.getAverageAllEventsHistory(new Date().getMonth + 1);
    props.getAverageConferenceEventsHistory(new Date().getMonth + 1);
  }

  render() {
    return (
      <div className={styles.widgetsContainer}>
        <div>
          <div className={`${styles.widget} ${styles.left}`}>
            <TotalUsersWidget />
          </div>
          <div className={`${styles.widget} ${styles.middle}`}>
            <NewUsersWidget />
          </div>
          <div className={`${styles.widget} ${styles.right}`}>
            <NewSubscriptionsWidget />
          </div>
        </div>
        <div>
          <div className={`${styles.widget} ${styles.left}`}>
            <RecurlyWidget />
          </div>
          <div className={`${styles.widget} ${styles.middle}`}>
            <AverageLeadsWidget />
          </div>
          <div className={`${styles.widget} ${styles.right}`}>
            <AverageEventsWidget />
          </div>
        </div>
      </div>
    );
  }
}
