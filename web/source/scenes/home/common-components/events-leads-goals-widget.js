import { connect } from 'store';
import { navigate } from 'store/navigate';
import { getDashboardSummary } from 'store/home';
import { dashboardSummaryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import ProgressBar from './progress-bar';

import AdjustGoalDialog from './adjust-goal-dialog';

import styles from './events-leads-goals-widget.module.css';

@connect({
  dashboardSummary: getDashboardSummary,
}, {
  navigate,
})
export default class EventsLeadsGoalsWidget extends Component {
  static propTypes = {
    dashboardSummary: dashboardSummaryShape,
    navigate: PropTypes.func,
    onAdjustGoalSuccess: PropTypes.func,
  }

  constructor(props) {
    super(props);

    this.state = { showAdjustGoalDialog: false };
  }

  onAdjustGoalSucces = () => {
    this.setState({ showAdjustGoalDialog: false });
    this.props.onAdjustGoalSuccess();
  }

  renderTableRow = (event) => {
    const progress = event.leadsGoal === 0 ? 100 : (event.leadsCount / event.leadsGoal) * 100;
    return (
      <tr key={event.uid}>
        <td className={styles.eventsLeadsInfoCell}>
          <div
            className={classNames(styles.eventsLeadsInfo, { [styles.overload]: progress >= 100 })}
          >
            <span>{event.name}</span>
            <span><b>{event.leadsCount}</b> of {event.leadsGoal}</span>
          </div>
          <ProgressBar percent={progress} />
        </td>
        <td className={styles.adjustGoalCell}>
          <div className={styles.adjustGoal}>
            <div className={styles.goalIcon} />
            <div>{event.leadsGoal}</div>
          </div>
        </td>
      </tr>
    );
  }

  render() {
    const { dashboardSummary } = this.props;
    const { showAdjustGoalDialog } = this.state;
    if (!dashboardSummary) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner />
        </div>
      );
    }

    const { events = [] } = dashboardSummary;

    return (
      <div className={styles.rootContainer}>
        <table>
          <tbody>
            <tr>
              <th className={styles.eventsLeadsInfoCell}>
                <div className={styles.eventsLeadsHeader}>
                  <span>Events</span>
                  <span>Leads</span>
                </div>
              </th>
              <th className={styles.adjustGoalHeader}>
                Adjust Goal
              </th>
            </tr>
            {events.map(event => this.renderTableRow(event))}
          </tbody>
        </table>
        <div className={styles.footer}>
          <div onClick={() => { this.props.navigate('/events'); }}>
            <span>Show all events</span>
            <span>&nbsp;▶</span>
          </div>
          <div
            className={styles.adjustGoalButton}
            onClick={() => this.setState({ showAdjustGoalDialog: true })}
          >
            Adjust goal
          </div>
        </div>
        {showAdjustGoalDialog &&
          <AdjustGoalDialog
            onClose={() => this.setState({ showAdjustGoalDialog: false })}
            onSuccess={this.onAdjustGoalSucces}
          />
        }
      </div>
    );
  }
}
