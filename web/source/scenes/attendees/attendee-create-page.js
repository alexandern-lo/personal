import { connect } from 'store';
import { redirect } from 'store/navigate';
import { prepareDataForUpload, makeBlankAttendee } from 'store/data/attendee';
import { createAttendee } from 'store/attendees/actions';
import { getPinnedEvent } from 'store/events';

import Breadcrumbs from 'components/sub-header/breadcrumbs';

import AttendeeEditForm from './attendee-edit-form';

@connect({
  event: getPinnedEvent,
}, {
  createAttendee,
  redirect,
})
export default class CreateAttendeePage extends Component {
  static propTypes = {
    event: PropTypes.shape({
      name: PropTypes.string,
      uid: PropTypes.string,
      categories: PropTypes.arrayOf(PropTypes.shape({
        uid: PropTypes.string,
        name: PropTypes.string,
        options: PropTypes.arrayOf(PropTypes.shape({
          uid: PropTypes.string,
          name: PropTypes.string,
        })),
      })),
    }),
    redirect: PropTypes.func.isRequired,
    createAttendee: PropTypes.func.isRequired,
  };

  onCreate = data => this.props.createAttendee({
    eventUid: this.props.event.uid,
    ...prepareDataForUpload(data),
  });

  onSuccess = (a, b, { values: attendee }) => this.props.redirect({
    pathname: `/events/${this.props.event.uid}/attendees`,
    state: { created: `${attendee.first_name} ${attendee.last_name}` },
  });

  onCancel = () => this.props.redirect(`/events/${this.props.event.uid}/attendees`);

  render() {
    const { event } = this.props;

    return (
      <AttendeeEditForm
        form='create-attendee'
        initialValues={makeBlankAttendee(event)}
        title={<Breadcrumbs event={event} attendee action='Add new attendee' />}
        actionTitle='Add'
        onSubmit={this.onCreate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
