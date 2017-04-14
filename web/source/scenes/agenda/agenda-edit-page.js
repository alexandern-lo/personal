import { connect } from 'store';
import { redirect } from 'store/navigate';

import { updateAgenda } from 'store/agenda/actions';
import { getPinnedEvent } from 'store/events';
import { getPinnedAgenda } from 'store/agenda';
import { editAgenda } from 'store/data/agenda';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import AgendaEditForm from './agenda-edit-form';

@connect({
  event: getPinnedEvent,
  agenda: getPinnedAgenda,
}, {
  redirect,
  updateAgenda,
})
export default class EditAgendaPage extends Component {

  static propTypes = {
    event: PropTypes.shape({ name: PropTypes.string, uid: PropTypes.string }),
    agenda: PropTypes.shape({ name: PropTypes.string, uid: PropTypes.string }),
    updateAgenda: PropTypes.func.isRequired,
    redirect: PropTypes.func,
  };

  onUpdate = data => this.props.updateAgenda({ eventUid: this.props.event.uid, ...data });

  onSuccess = (a, b, { values: agenda }) => this.props.redirect({
    pathname: `/events/${this.props.event.uid}/agenda_items`,
    state: { updated: agenda.name },
  });

  onCancel = () => this.props.redirect(`/events/${this.props.event.uid}/agenda_items`);

  render() {
    const { event, agenda } = this.props;
    return (
      <AgendaEditForm
        form='edit-agenda'
        title={<Breadcrumbs event={event} agenda={agenda} action='edit' />}
        actionTitle='Save'
        initialValues={editAgenda(agenda)}
        minDate={event.startDate}
        maxDate={event.endDate}
        onSubmit={this.onUpdate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
