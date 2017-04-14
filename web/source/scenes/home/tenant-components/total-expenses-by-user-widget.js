import { connect } from 'store';
import { navigate } from 'store/navigate';

import {
  fetchUsersExpenses,
  resetUsersExpenses,
  setTotalExpensesByUserEvent,
} from 'store/home/actions';

import { getUsersExpenses, getTotalExpensesByUserEvent } from 'store/home';
import { usersExpensesShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';

import EventListDialog from './event-list-dialog';

import styles from './total-expenses-by-user-widget.module.css';

@connect({
  users: getUsersExpenses,
  event: getTotalExpensesByUserEvent,
}, {
  getUsersExpenses: fetchUsersExpenses,
  setEvent: setTotalExpensesByUserEvent,
  resetUsersExpenses,
  navigate,
})
export default class TotalExpensesByUserWidget extends Component {
  static propTypes = {
    users: usersExpensesShape,
    event: PropTypes.shape({ name: PropTypes.string }),
    getUsersExpenses: PropTypes.func,
    resetUsersExpenses: PropTypes.func,
    setEvent: PropTypes.func,
    navigate: PropTypes.func,
  }

  constructor(props) {
    super(props);

    this.state = { showEventListDialog: false, eventsFilters: null };
  }

  onEventFilterChange = (event) => {
    this.props.setEvent(event);

    this.setState({ showEventListDialog: false });
    this.props.resetUsersExpenses();
    this.props.getUsersExpenses();
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

  renderTableRow = user => (
    <tr key={user.uid}>
      <td className={styles.userExpensesInfoCell}>
        <div className={styles.userInfo}>
          {`${user.firstName} ${user.lastName}`}
        </div>
      </td>
      <td className={styles.expensesCell}>
        ${user.amount.amount}
      </td>
    </tr>
  );

  render() {
    const { users, event } = this.props;
    const { eventsFilters, showEventListDialog } = this.state;

    if (!users) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner />
        </div>
      );
    }
    return (
      <div className={styles.rootContainer}>
        <div className={styles.header}>
          <div>
            <span>Total Expenses</span>
            <span>${users.totalExpenses.amount}</span>
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
            {users.userExpenses.map(user => this.renderTableRow(user))}
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
