import { navigate, refresh } from 'store/navigate';

import { formatUtcDate } from 'helpers/dates';
import { getProfileUID, isSuperAdmin, isTenantAdmin } from 'store/auth';

import { connect } from 'store';
import { collectionShape } from 'store/collections';
import { isConference, isPersonal, isOwnedBy, INDUSTRIES } from 'store/data/event';
import { deleteEvents } from 'store/events/actions';
import { getCollectionState } from 'store/events';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';
import DateFiltersPair from 'components/sub-header/date-filters-pair';
import { renderTenantsFilter } from 'components/sub-header/filters/tenants-filter';

import EventPreview from './event-preview';

const eventTypeFilter = {
  title: 'Event type',
  options: {
    '': 'All events',
    conference: 'conference',
    personal: 'personal',
  },
};

const industryFilter = {
  title: 'Industry',
  options: INDUSTRIES.reduce((opts, i) => {
    opts[i] = i; return opts; // eslint-disable-line no-param-reassign
  }, { '': 'All industries' }),
};

const renderDatesFilter = ({ name, filters, onFilterChange }) => (
  <DateFiltersPair
    key={name}
    filters={filters}
    onFilterChange={onFilterChange}
    start={{ title: 'Start after', field: 'start_after' }}
    end={{ title: 'End before', field: 'end_before' }}
  />
);
renderDatesFilter.propTypes = {
  name: PropTypes.string.isRequired,
  filters: PropTypes.objectOf(PropTypes.string).isRequired,
  onFilterChange: PropTypes.func.isRequired,
};

const adminFiltersConfig = {
  event_type: eventTypeFilter,
  tenant: { render: renderTenantsFilter },
  industry: industryFilter,
  dates: { render: renderDatesFilter },
};

const filtersConfig = {
  event_type: eventTypeFilter,
  industry: industryFilter,
  dates: { render: renderDatesFilter },
};

const renderEventType = (type) => {
  if (type === 'conference') return 'Conference';
  if (type === 'personal') return 'Personal';
  return 'Unknown';
};

const userColumns = [
  { field: 'type', label: 'Event type', sortField: false, render: renderEventType },
  { field: 'name', label: 'Event name' },
  { field: 'city', label: 'City' },
  { field: 'state', label: 'State' },
  { field: 'country', label: 'Country' },
  { field: 'startDate', label: 'Start date', sortField: 'start_date', render: formatUtcDate },
  { field: 'endDate', label: 'End date', sortField: 'end_date', render: formatUtcDate },
  { field: 'industry', label: 'Industry' },
];

const adminColumns = userColumns.concat([
  { field: 'tenant', label: 'Tenant' },
]);

export const getColumns = ({ isSA }) => {
  if (isSA) return adminColumns;
  return userColumns;
};

@connect({
  collection: getCollectionState,
  myUID: getProfileUID,
  isSA: isSuperAdmin,
  isTA: isTenantAdmin,
}, {
  deleteEvents,
  navigate,
  refresh,
})
export default class EventsListPage extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    collection: collectionShape.isRequired,

    myUID: PropTypes.string,
    isSA: PropTypes.bool,
    isTA: PropTypes.bool,

    deleteEvents: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  onRefList = list => (this.list = list);

  onNavigateToEventEntities = (path) => {
    this.props.navigate(path);
    this.list.onClosePreview();
  };

  onCreate = () => this.props.navigate('/events/create');
  onEdit = event => this.props.navigate(`/events/${event.uid}/edit`);
  onDelete = events => this.props.deleteEvents(events.map(e => e.uid));

  getNotification = () => {
    const { created, updated } = this.props.location.state || {};
    if (created) {
      return `Event ${created} has been successfully created`;
    }
    if (updated) {
      return `Event ${updated} has been successfully updated`;
    }
    return null;
  };

  getDeleteMessage = events => `Delete event${events.length > 1 ? 's' : ''}?`;

  clearNotification = () => this.props.refresh(this.props.location, {});

  canEdit = event => (isConference(event)
    ? this.props.isSA
    : isOwnedBy(event, this.props.myUID)
  );

  canDelete = (event) => {
    const { myUID, isSA, isTA } = this.props;
    if (isSA) return true;
    if (!isPersonal(event)) return false;
    return isTA ? true : isOwnedBy(event, myUID);
  };

  renderPreview = ({ item, ...props }) => (
    <EventPreview
      event={item}
      isSuperAdmin={this.props.isSA}
      onNavigate={this.onNavigateToEventEntities}
      {...props}
    />
  );

  render() {
    const { collection, isSA } = this.props;
    return (
      <CollectionListPage
        name='events'
        title={<Breadcrumbs event />}
        collection={collection}
        columns={getColumns({ isSA })}
        filters={isSA ? adminFiltersConfig : filtersConfig}
        renderPreview={this.renderPreview}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        onCreate={this.onCreate}
        createLabel='Create new event'

        canEdit={this.canEdit}
        onEdit={this.onEdit}

        canDelete={this.canDelete}
        onDelete={this.onDelete}
        getDeleteMessage={this.getDeleteMessage}

        ref={this.onRefList}
      />
    );
  }
}
