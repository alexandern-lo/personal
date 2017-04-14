import { connectActions } from 'store';

import { reduxForm, validate } from 'components/forms';
import EditingView from 'components/editing/editing-view';
import EditingTable from 'components/editing/editing-table';
import {
  ErrorsRow,
  InputRow,
  TextAreaRow,
} from 'components/editing/editing-rows';


import { searchEvents } from 'store/events/actions';
import { readEvent } from 'store/data/event';

import { addExpense } from 'store/home/actions';

import ErrorPanel from 'components/errors/error-panel';
import ModalView from 'components/modal-view/modal-view';
import FetchableList from 'components/fetchable/fetchable-list';

import styles from './add-expense-dialog.module.css';

@connectActions({
  addExpense,
})
export default class AddExpenseDialog extends Component {
  static propTypes = {
    onClose: PropTypes.func.isRequired,
    addExpense: PropTypes.func.isRequired,
    onSuccess: PropTypes.func.isRequired,
  }

  constructor(props) {
    super(props);

    this.state = { event: null, showAddExpense: false };
  }

  onSelectEvent = event => this.setState({ event });

  onError = error => this.setState({ error });
  onClearError = () => this.setState({ error: null });

  onAddExpense = ({ amount, comments }) => this.props.addExpense({
    eventUid: this.state.event.uid,
    expense: {
      amount,
      currency: 'usd',
    },
    comments,
  });

  renderEvent = (item, selected) => (
    <div
      className={classNames(
        styles.row,
        selected && styles.active,
      )}
    >
      <span>{item.name}</span>
    </div>
  );

  renderChooseEventDialog = () => (
    <ModalView
      title='Сhoose an event'
      onClose={this.props.onClose}
      button={{
        text: 'Continue',
        onClick: () => { this.setState({ showAddExpense: true }); },
        active: !!this.state.event,
      }}
    >
      <FetchableList
        width={480}
        height={234}
        rowHeight={42}
        renderItem={this.renderEvent}
        selected={this.state.event ? [this.state.event] : []}
        onSelect={this.onSelectEvent}
        fetchPageAction={searchEvents}
        reader={readEvent}
        searchable
        onError={this.onError}
      />
      {this.state.error &&
        <ErrorPanel error={this.state.error} onClear={this.onClearError} />}
    </ModalView>
  );

  renderAddExpenseDialog = () => (
    <AddExpenseForm
      onSubmit={this.onAddExpense}
      onSubmitSuccess={this.props.onSuccess}
      onClose={this.props.onClose}
    />
  );

  render() {
    return !this.state.showAddExpense ?
      this.renderChooseEventDialog() :
      this.renderAddExpenseDialog();
  }
}

const fields = {
  amount: {
    required: true,
    digits: { min: 1, max: 9 },
  },
  comments: {
    minLength: 2,
    maxLength: 90,
  },
};

const RenderAddExpenseForm = props => (
  <EditingView
    title='sdfsdf'
    submitButtonText='Import'
    {...props}
  >
    <div className={styles.tableWrapper}>
      <EditingTable>
        <InputRow label='Expense value' name='amount' required />
        <ErrorsRow name='amount' />

        <TextAreaRow label='Description' name='comments' maxLength={90} />
        <ErrorsRow name='description' />
      </EditingTable>
    </div>
  </EditingView>
);

const AddExpenseForm = reduxForm({
  form: 'add-expense',
  ...validate(fields, { validateOnBlur: true }),
})(RenderAddExpenseForm);
