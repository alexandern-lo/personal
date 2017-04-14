import { connect } from 'store';
import { navigate, refresh } from 'store/navigate';

import { formatUtcTime, formatUtcDate, listOfDatesBetween } from 'helpers/dates';

import { eventShape } from 'store/data/event';
import { collectionShape } from 'store/collections';

import { isSuperAdmin } from 'store/auth';
import { getPinnedEvent } from 'store/events';
import { deleteAgendaItems } from 'store/agenda/actions';
import { getCollectionState } from 'store/agenda';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';
import AgendaPreview from './agenda-preview';

const tableColumnsConfig = [
  { field: 'name', label: 'Title' },
  { field: 'date', label: 'Date', render: formatUtcDate },
  { field: 'startTime', label: 'Start time', sortField: 'start_time', render: formatUtcTime },
  { field: 'endTime', label: 'End time', sortField: 'end_time', render: formatUtcTime },
  { field: 'location', label: 'Location' },
];

@connect({
  event: getPinnedEvent,
  collection: getCollectionState,
  isSuperAdmin,
}, {
  deleteAgendaItems,
  navigate,
  refresh,
})

export default class AgendaListPage extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    event: eventShape.isRequired,
    collection: collectionShape.isRequired,
    isSuperAdmin: PropTypes.bool,
    deleteAgendaItems: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    const { event: { startDate, endDate } } = props;
    const dates = {
      '': 'All dates',
    };
    if (startDate && endDate) {
      const list = listOfDatesBetween(startDate, endDate);
      list.forEach(d => (dates[d.toISOString()] = formatUtcDate(d)));
    }
    this.filtersConfig = {
      date: {
        title: 'Date',
        options: dates,
      },
    };
  }

  onCreate = () => this.props.navigate(`/events/${this.props.event.uid}/agenda_items/create`);
  onEdit = agenda => this.props.navigate(`/events/${this.props.event.uid}/agenda_items/${agenda.uid}/edit`);

  onPerformDelete = items => this.props.deleteAgendaItems({
    eventUid: this.props.event.uid,
    agendaItemsUids: items.map(a => a.uid),
  });

  getNotification = () => {
    const { created, updated } = this.props.location.state || {};
    if (created) {
      return `Agenda ${created} has been successfully created`;
    }
    if (updated) {
      return `Agenda ${updated} has been successfully updated`;
    }
    return null;
  };

  getDeleteMessage = items => `Delete agenda item${items.length > 1 ? 's' : ''}?`;

  clearNotification = () => this.props.refresh(this.props.location, {});

  canCreate = () => this.props.isSuperAdmin;
  canSelect = () => this.props.isSuperAdmin;
  canEdit = () => this.props.isSuperAdmin;
  canDelete = () => this.props.isSuperAdmin;

  renderPreview = ({ item, ...props }) => (
    <AgendaPreview agenda={item} {...props} />
  );

  render() {
    const { collection, event } = this.props;
    return (
      <CollectionListPage
        name='agenda'
        title={<Breadcrumbs event={event} agenda />}

        collection={collection}
        columns={tableColumnsConfig}
        filters={this.filtersConfig}
        renderPreview={this.renderPreview}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        onCreate={this.canCreate() ? this.onCreate : null}
        createLabel='Create new agenda item'

        canSelect={this.canSelect}

        canEdit={this.canEdit}
        onEdit={this.onEdit}

        canDelete={this.canDelete}
        onDelete={this.onPerformDelete}
        getDeleteMessage={this.getDeleteMessage}

        context={event.uid}
      />
    );
  }
}
