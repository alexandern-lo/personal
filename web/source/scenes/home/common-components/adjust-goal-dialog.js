import { connect } from 'store';

import {
 adjustUserGoal as adjustUserGoalAction,
 adjustTenantGoals as adjustTenantGoalsAction,
} from 'store/home/actions';

import { reduxForm, validate } from 'components/forms';
import EditingView from 'components/editing/editing-view';
import EditingTable from 'components/editing/editing-table';
import {
  ErrorsRow,
  InputRow,
} from 'components/editing/editing-rows';


import { searchEvents } from 'store/events/actions';
import { readEvent } from 'store/data/event';
import { fetchUsersByEvent } from 'store/users/actions';
import { readUser } from 'store/data/user';

import { isTenantAdmin } from 'store/auth';

import ErrorPanel from 'components/errors/error-panel';
import ModalView from 'components/modal-view/modal-view';
import FetchableList from 'components/fetchable/fetchable-list';

import styles from './adjust-goal-dialog.module.css';

@connect({
  tenantAdmin: isTenantAdmin,
}, {
  adjustUserGoal: adjustUserGoalAction,
  adjustTenantGoals: adjustTenantGoalsAction,
})

export default class AddExpenseDialog extends Component {
  static propTypes = {
    onClose: PropTypes.func.isRequired,
    onSuccess: PropTypes.func.isRequired,
    tenantAdmin: PropTypes.bool,
    adjustUserGoal: PropTypes.func,
    adjustTenantGoals: PropTypes.func,
  }

  constructor(props) {
    super(props);

    this.state = {
      event: null,
      users: [],
      showAddExpense: false,
      eventChosen: false,
      userChosen: false,
    };
  }

  onAdjustGoal = ({ leads_goal }) => {
    const { tenantAdmin, adjustUserGoal, adjustTenantGoals } = this.props;
    const { event, users } = this.state;
    if (tenantAdmin) {
      return adjustTenantGoals({
        eventUid: event.uid,
        user_uids: users.map(user => user.uid),
        leads_goal,
      });
    }

    return adjustUserGoal({ eventUid: event.uid, leads_goal });
  }

  onSelectUser = (user) => {
    const { users } = this.state;
    const index = users.indexOf(user);

    if (index >= 0) {
      users.splice(index, 1);
    } else {
      users.push(user);
    }

    this.setState({ users: [...users] });
  };

  onError = error => this.setState({ error });

  fetchUsersByEvent = () => fetchUsersByEvent({ eventUid: this.state.event.uid });

  renderEvent = (event, selected) => (
    <div
      className={classNames(
        styles.row,
        { [styles.active]: selected },
      )}
    >
      <span>{event.name}</span>
    </div>
  );

  renderUser = (user, selected) => (
    <div
      className={classNames(
        styles.row,
        styles.user,
        { [styles.selected]: selected },
      )}
    >
      <span>{`${user.firstName} ${user.lastName}`}</span>
    </div>
  );

  renderChooseEventDialog = () => (
    <ModalView
      title='Сhoose an event'
      onClose={this.props.onClose}
      button={{
        text: 'Continue',
        onClick: () => { this.setState({ eventChosen: true }); },
        active: !!this.state.event,
      }}
    >
      <FetchableList
        width={480}
        height={234}
        rowHeight={42}
        renderItem={this.renderEvent}
        selected={[this.state.event]}
        onSelect={event => this.setState({ event })}
        fetchPageAction={searchEvents}
        reader={readEvent}
        searchable
        onError={this.onError}
      />
      {this.state.error &&
        <ErrorPanel
          error={this.state.error}
          onClear={() => this.setState({ error: null })}
        />}
    </ModalView>
  );

  renderChooseUserDialog = () => (
    <ModalView
      title='Choose User'
      onClose={this.props.onClose}
      button={{
        text: 'Continue',
        onClick: () => this.setState({ userChosen: true }),
        active: this.state.users.length > 0,
      }}
    >
      <FetchableList
        width={480}
        height={234}
        rowHeight={42}
        renderItem={this.renderUser}
        selected={this.state.users}
        onSelect={this.onSelectUser}
        fetchPageAction={this.fetchUsersByEvent}
        reader={readUser}
        searchable
        onError={this.onError}
      />
      {this.state.error &&
        <ErrorPanel
          error={this.state.error}
          onClear={() => this.setState({ error: null })}
        />}
    </ModalView>
  )

  renderAdjustGoalDialog = () => (
    <AdjustGoalForm
      onSubmit={this.onAdjustGoal}
      onSubmitSuccess={this.props.onSuccess}
      onClose={this.props.onClose}
    />
  );

  render() {
    const { tenantAdmin } = this.props;
    const { userChosen, eventChosen } = this.state;

    if (userChosen) {
      return this.renderAdjustGoalDialog();
    }

    if (eventChosen) {
      return tenantAdmin ? this.renderChooseUserDialog() : this.renderAdjustGoalDialog();
    }

    return this.renderChooseEventDialog();
  }
}

const fields = {
  leads_goal: {
    required: true,
    digits: { min: 1, max: 9 },
  },
};

const RenderAdjustGoalForm = props => (
  <EditingView
    title='Adjust goal'
    submitButtonText='Adjust'
    {...props}
  >
    <div className={styles.tableWrapper}>
      <EditingTable>
        <InputRow label='Leads goal' name='leads_goal' required />
        <ErrorsRow name='leads_goal' />
      </EditingTable>
    </div>
  </EditingView>
);

const AdjustGoalForm = reduxForm({
  form: 'add-expense',
  ...validate(fields, { validateOnBlur: true }),
})(RenderAdjustGoalForm);
