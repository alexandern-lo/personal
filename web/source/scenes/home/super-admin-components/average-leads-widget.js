import moment from 'moment';
import { connect } from 'store';

import { getSuperAdminDashboardSummary, getAverageLeadsHistory } from 'store/home';
import { SADashboardSummaryShape, chartHistoryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import LineChart from '../common-components/line-chart';
import { numberCommaFormat } from '../helpers';

import styles from './average-leads-widget.module.css';

const AverageLeadsWidget = ({ dashboardSummary, leadsHistory }) => {
  if (!dashboardSummary || !leadsHistory) {
    return (
      <div className={styles.spinnerContainer}>
        <Spinner />
      </div>
    );
  }

  const { allTime: { total: totalLeads } } = dashboardSummary.leads;

  return (
    <div className={styles.rootContainer}>
      <div className={styles.widgetTitle}>Total leads in system</div>
      <div className={styles.widgetAfterTitle}>
        ({moment.months()[new Date().getMonth()]}, {new Date().getFullYear()})
      </div>
      <div className={styles.totalLeadsCount}>{numberCommaFormat(totalLeads)}</div>
      <div className={styles.chartTitle}>Average leads and users</div>
      <div className={styles.lineChartContainer}>
        <LineChart
          charts={{
            points: leadsHistory.map(point => ({ x: point.date, y: point.value.toFixed(2) })),
          }}
          options={{ monthLabels: true }}
        />
      </div>
    </div>
  );
};

export default connect({
  dashboardSummary: getSuperAdminDashboardSummary,
  leadsHistory: getAverageLeadsHistory,
})(AverageLeadsWidget);

AverageLeadsWidget.propTypes = {
  dashboardSummary: SADashboardSummaryShape,
  leadsHistory: chartHistoryShape,
};
