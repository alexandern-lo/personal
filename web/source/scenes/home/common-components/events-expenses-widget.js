import { connect } from 'store';

import { getDashboardSummary } from 'store/home';
import { dashboardSummaryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';
import ProgressBar from './progress-bar';

import AddExpenseDialog from './add-expense-dialog';

import styles from './events-expenses-widget.module.css';

@connect({
  dashboardSummary: getDashboardSummary,
})
export default class EventsExpenses extends Component {
  static propTypes = {
    dashboardSummary: dashboardSummaryShape,
    onAddExpenseSuccess: PropTypes.func.isRequired,
  }

  constructor(props) {
    super(props);

    this.state = { showAddExpense: false };
  }

  onAddExpense = () => this.setState({ showAddExpense: true });
  onAddExpenseClose = () => this.setState({ showAddExpense: false });
  onAddExpenseSuccess = () => {
    this.setState({ showAddExpense: false });
    this.props.onAddExpenseSuccess();
  }

  renderTableRow = (event) => {
    const { thisYearExpenses } = this.props.dashboardSummary.leadsStats;
    const progress = thisYearExpenses.amount === 0 ? 100 :
              (event.totalExpenses.amount / thisYearExpenses.amount) * 100;

    return (
      <tr key={event.uid}>
        <td className={styles.eventExpensesInfoCell}>
          <div
            className={classNames(styles.eventExpensesInfo, { [styles.overload]: progress >= 100 })}
          >
            <span>{event.name}</span>
            <span>
              Spent <b>${event.totalExpenses.amount}</b> of
              {` $${this.props.dashboardSummary.leadsStats.thisYearExpenses.amount}`}
            </span>
          </div>
          <ProgressBar percent={progress} />
        </td>
        <td className={styles.costPerLeadCell}>
          ${(event.leadsCount === 0 ? 0
            : (event.totalExpenses.amount / event.leadsCount)).toFixed(2)}
        </td>
      </tr>
    );
  }

  render() {
    const { dashboardSummary } = this.props;
    const { showAddExpense } = this.state;
    if (!dashboardSummary) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner />
        </div>
      );
    }

    const { events = [], leadsStats: { thisYearExpenses, thisYearCpl } } = dashboardSummary;

    return (
      <div className={styles.rootContainer}>
        <table>
          <tbody>
            <tr>
              <th className={styles.eventExpensesInfoCell}>
                Events
              </th>
              <th className={styles.costPerLeadHeader}>
                Cost per lead
              </th>
            </tr>
            {events.map(event => this.renderTableRow(event))}
          </tbody>
        </table>
        <div className={styles.footer}>
          <div className={styles.footerInfo}>
            <div>
              <span>Yealy expenses to date</span>
              <span><b>${thisYearExpenses.amount.toFixed(2)}</b></span>
            </div>
            <div>
              <span>Average cost per lead</span>
              <span><b>${thisYearCpl.amount.toFixed(2)}</b></span>
            </div>
          </div>
          <div className={styles.addExpenseButton} onClick={this.onAddExpense}>
            + Add expense
          </div>
        </div>
        {showAddExpense &&
          <AddExpenseDialog
            onClose={this.onAddExpenseClose}
            onSuccess={this.onAddExpenseSuccess}
          />
        }
      </div>
    );
  }
}
