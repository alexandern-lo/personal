import { connect } from 'store';
import { redirect } from 'store/navigate';

import { createAgenda } from 'store/agenda/actions';
import { getPinnedEvent } from 'store/events';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import AgendaEditForm from './agenda-edit-form';

@connect({
  event: getPinnedEvent,
}, {
  createAgenda,
  redirect,
})

export default class CreateAgendaPage extends Component {
  static propTypes = {
    event: PropTypes.shape({
      name: PropTypes.string,
      uid: PropTypes.string,
      startDate: PropTypes.string,
      endDate: PropTypes.string,
    }),
    redirect: PropTypes.func.isRequired,
    createAgenda: PropTypes.func.isRequired,
  };

  onCreate = data => this.props.createAgenda({ eventUid: this.props.event.uid, ...data });

  onSuccess = (a, b, { values: agenda }) => this.props.redirect({
    pathname: `/events/${this.props.event.uid}/agenda_items`,
    state: { created: agenda.name },
  });

  onCancel = () => this.props.redirect(`/events/${this.props.event.uid}/agenda_items`);

  render() {
    const { event } = this.props;
    return (
      <AgendaEditForm
        form='create-agenda'
        title={<Breadcrumbs event={event} agenda action='Create new agenda item' />}
        actionTitle='Create'
        minDate={event.startDate}
        maxDate={event.endDate}
        onSubmit={this.onCreate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
