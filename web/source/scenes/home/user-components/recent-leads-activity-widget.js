import moment from 'moment';

import { connect } from 'store';
import { navigate } from 'store/navigate';

import { getRecentLeadsActivity } from 'store/home';
import { recentLeadsActivityShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';

import { formatUtcDate } from 'helpers/dates';

import styles from './recent-leads-activity-widget.module.css';

@connect({
  leadsActivity: getRecentLeadsActivity,
}, {
  navigate,
})
export default class RecentLeadsActivityWidget extends Component {
  static propTypes = {
    leadsActivity: recentLeadsActivityShape,
    navigate: PropTypes.func,
  }

  getPerfomedAtTimeString = (date) => {
    const minuteInMs = 1000 * 60;
    const currentDate = new Date();
    const perfomedAtDate = new Date(date);

    const millisTimeDiff = currentDate.getTime() - perfomedAtDate.getTime();

    if (millisTimeDiff < minuteInMs) {
      return 'just now';
    }

    if (millisTimeDiff < 2 * minuteInMs) {
      return 'a minute ago';
    }

    if (millisTimeDiff < 60 * minuteInMs) {
      return `${(millisTimeDiff / minuteInMs).toFixed(0)} minutes ago`;
    }

    if (currentDate.getDate() === perfomedAtDate.getDate()) {
      return `Today at ${moment(date).format('h:mm A')}`;
    }

    if (currentDate.getDate() - 1 === perfomedAtDate.getDate()) {
      return `Yesterday at ${moment(date).format('h:mm A')}`;
    }

    return formatUtcDate(date);
  }

  renderTableRow = lead => (
    <tr key={lead.leadUid}>
      <td className={styles.leadsInfoCell}>
        <div>{`${lead.firstName} ${lead.lastName || ''}`}</div>
        <div><b>Event:</b> {lead.eventName}</div>
      </td>
      <td className={styles.leadsActivityCell}>
        <div>{this.getPerfomedAtTimeString(lead.performedAt)}</div>
        <div>{lead.performedAction}</div>
      </td>
    </tr>
  );

  render() {
    const { leadsActivity } = this.props;
    if (!leadsActivity) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner />
        </div>
      );
    }

    return (
      <div className={styles.rootContainer}>
        <table>
          <tbody>
            <tr>
              <th className={styles.leadsInfoCell}>Recent Leads Activity</th>
              <th className={styles.leadsActivityCell} />
            </tr>
            {leadsActivity.map(lead => this.renderTableRow(lead))}
          </tbody>
        </table>
        <div className={styles.footer} onClick={() => { this.props.navigate('/leads'); }}>
          <span>Show all documents</span>
          <span>&nbsp;▶</span>
        </div>
      </div>
    );
  }
}
