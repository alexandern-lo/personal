import { connect } from 'store';

import { getSuperAdminDashboardSummary, getNewSubscriptionsHistory } from 'store/home';
import { SADashboardSummaryShape, chartHistoryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import LineChart from '../common-components/line-chart';
import { numberCommaFormat } from '../helpers';

import styles from './new-subscriptions-widget.module.css';

@connect({
  dashboardSummary: getSuperAdminDashboardSummary,
  newSubscriptionsHistory: getNewSubscriptionsHistory,
})
export default class NewSubscriptionsWidget extends Component {
  static propTypes = {
    dashboardSummary: SADashboardSummaryShape,
    newSubscriptionsHistory: PropTypes.shape({
      paid: chartHistoryShape,
      trial: chartHistoryShape,
    }),
  }

  constructor(props) {
    super(props);

    this.state = ({ showTrial: true, showPaid: true });
  }

  getCharts = () => {
    const charts = [];
    const { newSubscriptionsHistory } = this.props;

    if (this.state.showTrial) {
      charts.push({
        points: newSubscriptionsHistory.trial.map(point => ({ x: point.date, y: point.value })),
        config: {
          backgroundColor: 'rgba(152, 152, 152, 0.25)',
          color: 'rgba(152, 152, 152, 0.25)',
        },
      });
    }

    if (this.state.showPaid) {
      charts.push({
        points: newSubscriptionsHistory.paid.map(point => ({ x: point.date, y: point.value })),
      });
    }

    return charts;
  }

  render() {
    const { dashboardSummary, newSubscriptionsHistory } = this.props;
    const { showTrial, showPaid } = this.state;

    if (!dashboardSummary || !newSubscriptionsHistory) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner className={styles.spinner} />
        </div>
      );
    }

    const { paid, trial } = dashboardSummary.subscriptions.lastPeriod;
    const charts = this.getCharts();

    return (
      <div className={styles.rootContainer}>
        <div className={styles.widgetInfoContainer}>
          <div>
            <div className={styles.widgetTitle}>New subscriptions</div>
            <div className={styles.widgetAfterTitle}>(last 30 days)</div>
            <div className={styles.statsCount}>{numberCommaFormat(paid)}</div>
          </div>
          <div>
            <div className={styles.widgetTitle}>New free trials</div>
            <div className={styles.widgetAfterTitle}>(last 30 days)</div>
            <div className={styles.statsCount}>{numberCommaFormat(trial)}</div>
          </div>
        </div>
        <div className={styles.controlPanel}>
          <div
            className={classNames(styles.subscriptionButton, { [styles.passiveButton]: !showPaid })}
            onClick={() => { this.setState({ showPaid: !showPaid, showTrial: true }); }}
          >
            Subscription
          </div>
          <div
            className={
              classNames(styles.subscriptionButton, { [styles.passiveButton]: !showTrial })
            }
            onClick={() => { this.setState({ showTrial: !showTrial, showPaid: true }); }}
          >
            Trial
          </div>
        </div>
        <div className={styles.lineChartContainer}>
          <LineChart
            charts={charts}
            options={{ redraw: true }}
          />
        </div>
      </div>
    );
  }
}
