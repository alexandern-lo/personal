import { connect } from 'store';

import { getSuperAdminDashboardSummary } from 'store/home';
import { SADashboardSummaryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import DoughnutChart from '../common-components/doughnut-chart';
import { numberCommaFormat } from '../helpers';
import styles from './total-users-widget.module.css';

const TotalUsersWidget = ({ dashboardSummary }) => {
  if (!dashboardSummary) {
    return (
      <div className={styles.spinnerContainer}>
        <Spinner />
      </div>
    );
  }

  const { users: { allTime: users } } = dashboardSummary;

  let trialPercent = users.paid === 0 ? 100 : (users.trial / (users.paid + users.trial)) * 100;
  trialPercent = trialPercent.toFixed(2);
  const paidPercent = (100 - trialPercent).toFixed(2);

  return (
    <div className={styles.rootContainer}>
      <div className={styles.widgetTitle}>Total users</div>
      <div className={styles.widgetAfterTitle}>(All time registered)</div>
      <div className={styles.registeredUsersCount}>{numberCommaFormat(users.total)}</div>
      <div className={styles.chartContainer}>
        <div className={styles.chartLegend}>
          <div>
            <div className={classNames(styles.legendTitle, styles.paidIcon)}>
              Paid
            </div>
            <div className={styles.legendCountInfo}>{numberCommaFormat(users.paid)}</div>
          </div>
          <div>
            <div className={classNames(styles.legendTitle, styles.trialIcon)}>
              Free trial
            </div>
            <div className={styles.legendCountInfo}>{numberCommaFormat(users.trial)}</div>
          </div>
        </div>
        <div className={styles.doughnutChartContainer}>
          <DoughnutChart
            data={[paidPercent, trialPercent]}
            size={220}
          />
        </div>
      </div>
    </div>
  );
};

export default connect({ dashboardSummary: getSuperAdminDashboardSummary })(TotalUsersWidget);

TotalUsersWidget.propTypes = {
  dashboardSummary: SADashboardSummaryShape,
};
