import { connect } from 'store';
import { redirect } from 'store/navigate';
import { makeBlankEvent } from 'store/data/event';

import { isSuperAdmin } from 'store/auth';
import { createEvent } from 'store/events/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './event-edit-form';

@connect({
  superAdmin: isSuperAdmin,
}, {
  createEvent,
  redirect,
})
export default class CreateEventPage extends Component {

  static propTypes = {
    superAdmin: PropTypes.bool,
    createEvent: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onCreate = data => this.props.createEvent(data);

  onSuccess = (r, d, { values: event }) => this.props.redirect({
    pathname: '/events',
    state: { created: event.name },
  });

  onCancel = () => this.props.redirect('/events');

  render() {
    const { superAdmin } = this.props;
    return (
      <EditForm
        form='create-event'
        initialValues={makeBlankEvent()}
        title={<Breadcrumbs event action='Create new event' />}
        actionTitle='Create'
        isConference={superAdmin}
        onSubmit={this.onCreate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
