import { connect } from 'store';

import { getSuperAdminDashboardSummary, getNewUsersHistory } from 'store/home';
import { SADashboardSummaryShape, chartHistoryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import LineChart from '../common-components/line-chart';
import { numberCommaFormat } from '../helpers';

import styles from './new-users-widget.module.css';

const NewUsersWidget = ({ dashboardSummary, newUsersHistory }) => {
  if (!dashboardSummary || !newUsersHistory) {
    return (
      <div className={styles.spinnerContainer}>
        <Spinner />
      </div>
    );
  }

  const { users: { yesterday } } = dashboardSummary;

  return (
    <div className={styles.rootContainer}>
      <div className={styles.widgetTitle}>New users registered</div>
      <div className={styles.widgetAfterTitle}>(Last day)</div>
      <div className={styles.newUsersCount}>{numberCommaFormat(yesterday.total)}</div>
      <div className={styles.chartTitle}>Last 30 days registered</div>
      <div className={styles.lineChartContainer}>
        <LineChart
          charts={{
            points: newUsersHistory.map(point => ({ x: point.date, y: point.value })),
          }}
        />
      </div>
    </div>
  );
};

export default connect({
  dashboardSummary: getSuperAdminDashboardSummary,
  newUsersHistory: getNewUsersHistory,
})(NewUsersWidget);

NewUsersWidget.propTypes = {
  dashboardSummary: SADashboardSummaryShape,
  newUsersHistory: chartHistoryShape,
};
