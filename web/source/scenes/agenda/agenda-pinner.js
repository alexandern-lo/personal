import { connect } from 'store';
import { pinAgendaByUid } from 'store/agenda/actions';
import { getPinnedAgenda } from 'store/agenda';
import { getPinnedEvent } from 'store/events';

import ProgressIndicator from 'components/loading-spinner/loading-spinner';

@connect({
  pinnedAgenda: getPinnedAgenda,
  pinnedEvent: getPinnedEvent,
}, {
  pinAgendaByUid,
})
export default class AgendaPinner extends Component {
  static propTypes = {
    routeParams: PropTypes.shape({
      event_uid: PropTypes.string,
      agenda_uid: PropTypes.string,
    }).isRequired,
    pinnedAgenda: PropTypes.shape({ uid: PropTypes.string }),
    pinnedEvent: PropTypes.shape({ uid: PropTypes.string }),
    pinAgendaByUid: PropTypes.func.isRequired,
    children: PropTypes.node.isRequired,
  };

  componentWillMount() {
    const { routeParams: { agenda_uid: agendaUid }, pinnedAgenda,
      pinnedEvent: { uid: eventUid } } = this.props;

    this.pinnedAgenda = pinnedAgenda;

    if (!pinnedAgenda || pinnedAgenda.uid !== agendaUid) {
      this.props.pinAgendaByUid({ eventUid, agendaUid });
      this.pinnedAgenda = null;
    }
  }

  componentWillUpdate(nextProps) {
    this.pinnedAgenda = nextProps.pinnedAgenda;
  }

  render() {
    if (!this.pinnedAgenda) {
      return (
        <ProgressIndicator />
      );
    }

    return this.props.children;
  }
}
