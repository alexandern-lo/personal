import { connect } from 'store';
import { redirect } from 'store/navigate';
import { attendeeShape, editAttendee, prepareDataForUpload } from 'store/data/attendee';

import { updateAttendee } from 'store/attendees/actions';
import { getPinnedEvent } from 'store/events';
import { getPinnedAttendee } from 'store/attendees';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import AttendeeEditForm from './attendee-edit-form';

@connect({
  event: getPinnedEvent,
  attendee: getPinnedAttendee,
}, {
  updateAttendee,
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
    attendee: attendeeShape,
    redirect: PropTypes.func.isRequired,
    updateAttendee: PropTypes.func.isRequired,
  };

  onUpdate = data => this.props.updateAttendee({
    eventUid: this.props.event.uid,
    attendeeUid: this.props.attendee.uid,
    ...prepareDataForUpload(data),
  });

  onSuccess = (a, b, { values: attendee }) => this.props.redirect({
    pathname: `/events/${this.props.event.uid}/attendees`,
    state: { created: `${attendee.first_name} ${attendee.last_name}` },
  });

  onCancel = () => this.props.redirect(`/events/${this.props.event.uid}/attendees`);

  render() {
    const { event, attendee } = this.props;
    return (
      <AttendeeEditForm
        form='edit-attendee'
        initialValues={editAttendee(attendee, event)}
        title={<Breadcrumbs event={event} attendee={attendee} action='edit' />}
        actionTitle='Save'
        onSubmit={this.onUpdate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
