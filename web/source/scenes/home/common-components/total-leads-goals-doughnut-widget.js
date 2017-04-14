import { connect } from 'store';

import Spinner from 'components/loading-spinner/loading-spinner';

import {
  getDashboardSummary,
} from 'store/home';

import {
  dashboardSummaryShape,
} from 'store/data/home';

import DoughnutChart from './doughnut-chart';

import styles from './total-leads-goals-doughnut-widget.module.css';

export const TotalLeadsGoalsDoughnutWidget = ({ dashboardSummary }) => {
  if (!dashboardSummary) {
    return (
      <div className={styles.spinnerContainer}>
        <Spinner />
      </div>
    );
  }

  const { allTimeCount, allTimeGoal } = dashboardSummary.leadsStats;

  let allTimeCountPercent = allTimeGoal === 0 ? 100 : (allTimeGoal - allTimeCount) / allTimeGoal;
  allTimeCountPercent = allTimeCountPercent > 100 ? 100 : allTimeCountPercent.toFixed(2);
  const allTimeGoalPercent = (100 - allTimeCountPercent).toFixed(2);

  return (
    <div className={styles.rootContainer}>
      <div className={styles.doughnutContainer}>
        <DoughnutChart
          data={[allTimeCountPercent, allTimeGoalPercent]}
          options={{ redraw: true }}
        />
      </div>
      <div className={styles.legendContainer}>
        <div className={styles.leads}>
          Leads
        </div>
        <div className={styles.goals}>
          Goals
        </div>
      </div>
    </div>
  );
};

export default connect({
  dashboardSummary: getDashboardSummary,
})(TotalLeadsGoalsDoughnutWidget);

TotalLeadsGoalsDoughnutWidget.propTypes = {
  dashboardSummary: dashboardSummaryShape,
};
