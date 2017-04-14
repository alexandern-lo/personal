import { connect } from 'store';
import { redirect } from 'store/navigate';

import {
  editEvent,
  eventShape,
  isConference,
} from 'store/data/event';

import { getPinnedEvent } from 'store/events';
import { updateEvent } from 'store/events/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './event-edit-form';

@connect({
  event: getPinnedEvent,
}, {
  updateEvent,
  redirect,
})
export default class EditEventPage extends Component {

  static propTypes = {
    event: eventShape.isRequired,
    updateEvent: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onUpdate = data => this.props.updateEvent(data);

  onSuccess = (r, d, { values: event }) => this.props.redirect({
    pathname: '/events',
    state: { updated: event.name },
  });

  onCancel = () => this.props.redirect('/events');

  render() {
    const { event } = this.props;
    return (
      <EditForm
        form='edit-event'
        title={<Breadcrumbs event={event} action='edit' />}
        actionTitle='Save'
        initialValues={editEvent(event)}
        isConference={isConference(event)}
        onSubmit={this.onUpdate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
