import { connect } from 'store';
import { navigate } from 'store/navigate';

import {
  fetchUsersLeadGoals,
  resetUsersLeadGoals,
  setTotalLeadsByUserEvent,
} from 'store/home/actions';

import { getUsersLeadGoals, getTotalLeadsByUserEvent } from 'store/home';
import { usersLeadGoalsShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import ProgressBar from '../common-components/progress-bar';

import EventListDialog from './event-list-dialog';

import styles from './total-leads-by-user-widget.module.css';

@connect({
  users: getUsersLeadGoals,
  event: getTotalLeadsByUserEvent,
}, {
  getUsersLeadGoals: fetchUsersLeadGoals,
  setEvent: setTotalLeadsByUserEvent,
  resetUsersLeadGoals,
  navigate,
})
export default class TotalLeadsByUserWidget extends Component {
  static propTypes = {
    users: usersLeadGoalsShape,
    event: PropTypes.shape({ name: PropTypes.string }),
    setEvent: PropTypes.func,
    getUsersLeadGoals: PropTypes.func,
    resetUsersLeadGoals: PropTypes.func,
    navigate: PropTypes.func,
  }

  constructor(props) {
    super(props);

    this.state = { showEventListDialog: false, eventsFilters: null };
  }

  onEventFilterChange = (event) => {
    this.props.setEvent(event);

    this.setState({ showEventListDialog: false });
    this.props.resetUsersLeadGoals();
    this.props.getUsersLeadGoals();
  }

  openEventListDialog = (event) => {
    const { value } = event.target;
    if (value === 'reset_filter') {
      this.onEventFilterChange(null);
      return;
    }

    const eventsFilters = event.target.value === 'all' ? null :
      { start_after: `${new Date().getFullYear()}-01-01T00:00:00` };
    this.setState({ showEventListDialog: true, eventsFilters });
  }

  renderTableRow = (user) => {
    const progress = user.leadsGoal === 0 ? 100 : (user.leadsCount / user.leadsGoal) * 100;
    return (
      <tr key={user.uid}>
        <td className={styles.userLeadsInfoCell}>
          <div className={classNames(styles.userLeadsInfo, { [styles.overload]: progress >= 100 })}>
            <span>{`${user.firstName} ${user.lastName}`}</span>
            <span>
              <b>{user.leadsCount}</b> of {user.leadsGoal}
            </span>
          </div>
          <ProgressBar
            percent={progress}
          />
        </td>
        <td className={styles.leadsGoalCell}>
          {user.leadsGoal}
        </td>
      </tr>
    );
  }


  render() {
    const { users, event } = this.props;
    const { eventsFilters, showEventListDialog } = this.state;

    if (!users) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner className={styles.spinner} />
        </div>
      );
    }

    return (
      <div className={styles.rootContainer}>
        <div className={styles.header}>
          <div>
            <span>Total leads by user</span>
            <span>Lead goal</span>
          </div>
          <select value={(event && event.name) || 'All events'} onChange={this.openEventListDialog}>
            {event && <option disabled>{event && event.name}</option>}
            <option value='reset_filter'>All events</option>
            <option value='calendar_year'>Calendar year events</option>
            <option value={'all'}>Select Event</option>
          </select>
        </div>
        <table>
          <tbody>
            {users.map(user => this.renderTableRow(user))}
          </tbody>
        </table>
        <div className={styles.footer} onClick={() => { this.props.navigate('/users'); }}>
          <span>Show all users</span>
          <span>&nbsp;▶</span>
        </div>
        {showEventListDialog &&
          <EventListDialog
            onClose={() => this.setState({ showEventListDialog: false })}
            onSelect={this.onEventFilterChange}
            filters={eventsFilters}
          />}
      </div>
    );
  }
}
