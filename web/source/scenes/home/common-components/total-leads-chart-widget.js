import { connect } from 'store';

import Spinner from 'components/loading-spinner/loading-spinner';

import {
  getDashboardSummary,
  getLeadsHistory,
} from 'store/home';

import {
  dashboardSummaryShape,
  chartHistoryShape,
} from 'store/data/home';

import LineChart from './line-chart';

import styles from './total-leads-chart-widget.module.css';

const TotalLeadsChartWidget = ({ dashboardSummary, leadsHistory }) => {
  if (!dashboardSummary || !leadsHistory) {
    return (
      <div className={styles.spinnerContainer}>
        <Spinner />
      </div>
    );
  }

  const { leadsStats } = dashboardSummary;

  return (
    <div className={styles.rootContainer}>
      <div className={styles.title}>
        Leads
      </div>
      <div className={styles.afterTitle}>
        (Last 30 days)
      </div>
      <div className={styles.lastPeriodCount}>
        {leadsStats.lastPeriodCount}
      </div>
      <div className={styles.allTime}>
        <b>{leadsStats.allTimeCount}</b> All time
      </div>
      <div className={styles.lineChartContainer}>
        <LineChart
          charts={{
            points: leadsHistory.map(point => ({ x: point.date, y: point.value })),
          }}
          options={{ redraw: true }}
        />
      </div>
    </div>
  );
};

export default connect({
  dashboardSummary: getDashboardSummary,
  leadsHistory: getLeadsHistory,
})(TotalLeadsChartWidget);

TotalLeadsChartWidget.propTypes = {
  dashboardSummary: dashboardSummaryShape,
  leadsHistory: chartHistoryShape,
};
