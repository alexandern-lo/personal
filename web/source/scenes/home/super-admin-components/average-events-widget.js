import { connect } from 'store';

import { getSuperAdminDashboardSummary, getAverageEventsHistory } from 'store/home';
import { SADashboardSummaryShape, chartHistoryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import LineChart from '../common-components/line-chart';
import { numberCommaFormat } from '../helpers';

import styles from './average-events-widget.module.css';

@connect({
  dashboardSummary: getSuperAdminDashboardSummary,
  eventsHistory: getAverageEventsHistory,
})
export default class AverageEventsWidget extends Component {
  static propTypes = {
    dashboardSummary: SADashboardSummaryShape,
    eventsHistory: PropTypes.shape({
      all: chartHistoryShape,
      conference: chartHistoryShape,
    }),
  }

  constructor(props) {
    super(props);

    this.state = { showAll: true, showConference: true };
  }

  getCharts = () => {
    const charts = [];
    const { eventsHistory } = this.props;

    if (this.state.showConference) {
      charts.push({
        points: eventsHistory.conference.map(
          point => ({ x: point.date, y: point.value.toFixed(2) }),
        ),
        config: {
          backgroundColor: 'rgba(152, 152, 152, 0.25)',
          color: 'rgba(152, 152, 152, 0.25)',
        },
      });
    }

    if (this.state.showAll) {
      charts.push({
        points: eventsHistory.all.map(
          point => ({ x: point.date, y: point.value.toFixed(2) }),
        ),
      });
    }

    return charts;
  }

  toggleAllEventsChartButton = () => {}

  render() {
    const { dashboardSummary, eventsHistory } = this.props;
    const { showAll, showConference } = this.state;

    if (!dashboardSummary || !eventsHistory) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner />
        </div>
      );
    }

    const { conference, total } = dashboardSummary.events.allTime;

    return (
      <div className={styles.rootContainer}>
        <div className={styles.widgetInfoContainer}>
          <div>
            <div className={styles.widgetTitle}>Total events in system</div>
            <div className={styles.statsCount}>{numberCommaFormat(total)}</div>
          </div>
          <div>
            <div className={styles.widgetTitle}>Conference events</div>
            <div className={styles.statsCount}>{numberCommaFormat(conference)}</div>
          </div>
        </div>
        <div className={styles.controlPanel}>
          <div
            className={classNames(styles.controlButton, { [styles.passiveButton]: !showAll })}
            onClick={() => { this.setState({ showAll: !showAll, showConference: true }); }}
          >
            Average events / users
          </div>
          <div
            className={
              classNames(styles.controlButton, { [styles.passiveButton]: !showConference })
            }
            onClick={() => { this.setState({ showConference: !showConference, showAll: true }); }}
          >
            Conference events / users
          </div>
        </div>
        <div className={styles.lineChartContainer}>
          <LineChart
            charts={this.getCharts()}
            options={{ redraw: true, monthLabels: true }}
          />
        </div>
      </div>
    );
  }
}
