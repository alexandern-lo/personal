import _ from 'lodash';
import { reduxForm, validate } from 'components/forms';
import { connect } from 'store';

import { getPinnedEvent } from 'store/events';
import { eventShape } from 'store/data/event';

import { eventCoreLogin, eventCoreImport } from 'store/attendees/actions';

import EditingView from 'components/editing/editing-view';
import EditingTable from 'components/editing/editing-table';
import { InputRow, SelectRow, ErrorsRow } from 'components/editing/editing-rows';

import styles from './attendees-eventcore-import.module.css';

@connect({
  event: getPinnedEvent,
}, {
  eventCoreLogin,
  eventCoreImport,
})
export default class EventCoreImportDialog extends Component {
  static propTypes = {
    onClose: PropTypes.func.isRequired,

    event: eventShape.isRequired,
    eventCoreLogin: PropTypes.func.isRequired,
    eventCoreImport: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    this.state = { authorized: false };
  }

  onLogInSuccess = payload => this.setState({
    authorized: true,
    options: _.map(payload.data, r => ({ value: r.reportCustomizationId, label: r.reportName })),
  });

  onImportSuccess = () => {
    this.props.onClose();
  };

  onLogIn = (data) => {
    this.loginData = data;
    return this.props.eventCoreLogin({
      eventUid: this.props.event.uid,
      data,
    });
  };
  onImport = data => this.props.eventCoreImport({
    eventUid: this.props.event.uid,
    data,
  });

  renderLogInView = () => (
    <LoginForm
      onClose={this.props.onClose}
      onSubmit={this.onLogIn}
      onSubmitSuccess={this.onLogInSuccess}
    />
  );

  renderImportView = () => (
    <ImportForm
      onClose={this.props.onClose}
      onSubmit={this.onImport}
      onSubmitSuccess={this.onImportSuccess}
      options={this.state.options}
      initialValues={{
        ...this.loginData,
      }}
    />
  );

  render() {
    return !this.state.authorized ? this.renderLogInView() : this.renderImportView();
  }
}

const renderTitle = step => () => (
  <div className={styles.title}>
    <div>
      Import attendees from <span>EventCore</span>
    </div>
    <div className={styles.stepHint}>
      (Step {step})
    </div>
  </div>
);

const loginFormFields = {
  user_name: { required: true },
  password: { required: true },
  event_id: { required: true },
};

const LoginForm = reduxForm({
  form: 'event-core-login',
  ...validate(loginFormFields, { validateOnBlur: true }),
})(props => (
  <EditingView
    title={renderTitle(1)}
    submitButtonText='Log in'
    {...props}
  >
    <div className={styles.tableWrapper}>
      <EditingTable>
        <InputRow name='user_name' label='EventCore login' />
        <ErrorsRow name='user_name' />

        <InputRow name='password' label='Password' type='password' />
        <ErrorsRow name='password' />

        <InputRow name='event_id' label='Event ID' />
        <ErrorsRow name='event_id' />
      </EditingTable>
    </div>
  </EditingView>
));

const importFormFields = {
  report_id: { required: true },
};

const ImportForm = reduxForm({
  form: 'event-core-import',
  ...validate(importFormFields, { validateOnBlur: true }),
})(props => (
  <EditingView
    title={renderTitle(2)}
    submitButtonText='Import'
    {...props}
  >
    <div className={styles.tableWrapper}>
      <EditingTable>
        <InputRow name='event_id' label='Event ID' disabled />
        <SelectRow
          name='report_id'
          label='Report name'
          options={props.options}
          placeholder='Choose report name'
          disablePlaceholder
        />
      </EditingTable>
    </div>
  </EditingView>
));
