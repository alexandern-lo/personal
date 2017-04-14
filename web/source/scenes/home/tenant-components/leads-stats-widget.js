import TotalLeadsChartWidget from '../common-components/total-leads-chart-widget';
import TotalLeadsGoalsDoughnutWidget from '../common-components/total-leads-goals-doughnut-widget';

import styles from './leads-stats-widget.module.css';

export default class extends Component {
  constructor(props) {
    super(props);

    this.state = { activeComponent: 'line-chart' };
  }

  showLineChart = () => {
    this.setState({ activeComponent: 'line-chart' });
  }

  showDougnutChart = () => {
    this.setState({ activeComponent: 'doughnut-chart' });
  }

  render() {
    const { activeComponent } = this.state;

    return (
      <div className={styles.rootContainer}>
        <div className={styles.menu}>
          <div
            className={classNames({ [styles.activeButton]: activeComponent === 'line-chart' })}
            onClick={this.showLineChart}
          >
            Total leads
          </div>
          <div
            className={classNames({ [styles.activeButton]: activeComponent === 'doughnut-chart' })}
            onClick={this.showDougnutChart}
          >
            Total events leads & goals
          </div>
        </div>
        <div className={styles.widgetContainer}>
          {
            this.state.activeComponent === 'line-chart' ?
              <TotalLeadsChartWidget />
              : <TotalLeadsGoalsDoughnutWidget />
          }
        </div>
      </div>
    );
  }
}
