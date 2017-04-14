import { navigate, refresh } from 'store/navigate';

import { isSuperAdmin } from 'store/auth';

import { connect } from 'store';
import { collectionShape } from 'store/collections';

import { getPinnedEvent } from 'store/events';
import { eventShape } from 'store/data/event';

import { deleteAttendee } from 'store/attendees/actions';
import { getCollectionState } from 'store/attendees';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';
import CategoriesDropdown from 'components/categories-dropdown/categories-dropdown';

import AttendeesPreview from './attendees-preview';
import AttendeesImport from './attendees-import';
import EventCoreImportDialog from './attendees-eventcore-import';

const columns = [
  { field: 'firstName', label: 'First name', sortField: 'first_name' },
  { field: 'lastName', label: 'Last name', sortField: 'last_name' },
  { field: 'title', label: 'Job title' },
  { field: 'company', label: 'Company' },
  { field: 'email', label: 'Email', sortField: false },
  { field: 'phone', label: 'PhoneNumber', sortField: false },
];

export const getColumns = () => columns;

@connect({
  collection: getCollectionState,
  superAdmin: isSuperAdmin,
  event: getPinnedEvent,
}, {
  deleteAttendee,
  navigate,
  refresh,
})
export default class AttendeesListPage extends Component {
  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    collection: collectionShape.isRequired,
    superAdmin: PropTypes.bool,
    event: eventShape.isRequired,

    deleteAttendee: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);

    const { categories } = this.props.event;
    const renderCategories = ({ name, onChange, value }) => (
      <CategoriesDropdown
        key={name}
        onChange={onChange}
        categories={categories}
        chosenItems={value || []}
      />
    );

    this.categoryFilterConfig = {
      categories: {
        title: 'Category',
        render: renderCategories,
      },
    };

    this.state = { showEventCoreImport: false };
  }

  onRefList = list => (this.list = list);

  onTabFilterSelected = (f) => {
    const { collection: { filters } } = this.props;
    if (!filters.range || filters.range !== f) {
      this.list.onSelectFilter({ range: f });
    }
  };

  onCreate = () => this.props.navigate(`/events/${this.props.event.uid}/attendees/create`);
  onEdit = attendee => this.props.navigate(`/events/${this.props.event.uid}/attendees/${attendee.uid}/edit`);
  onDelete = (attendees) => {
    const { uid: eventUid } = this.props.event;
    const attendee = attendees.pop();
    return attendee ?
      this.props.deleteAttendee({ eventUid, attendeeUid: attendee.uid })
      .then(() => this.onDelete(attendees)) :
      Promise.resolve(true);
  };

  onImportFromCSV = () => this.props.navigate(`/events/${this.props.event.uid}/attendees/import`);
  onImportFromEventCore = () => this.setState({ showEventCoreImport: true });

  getNotification = () => {
    const { created, updated, imported } = this.props.location.state || {};
    if (created) {
      return `Attendee ${created} has been successfully created`;
    }
    if (updated) {
      return `Attendee ${updated} has been successfully updated`;
    }
    if (imported) {
      return 'Attendees have been successfully imported';
    }
    return null;
  };

  getDeleteMessage = attendees => `Delete attendee${attendees.length > 1 ? 's' : ''}?`;

  getHeaderActions = () => (!this.props.superAdmin ? null : [
    <AttendeesImport
      key='import'
      onImportFromCSV={this.onImportFromCSV}
      onImportFromEventCore={this.onImportFromEventCore}
    />,
  ]);

  clearNotification = () => this.props.refresh(this.props.location, {});

  canEdit = () => this.props.superAdmin;

  canDelete = () => this.props.superAdmin;

  canCreate = () => this.props.superAdmin;

  renderPreview = ({ item, ...props }) => (
    <AttendeesPreview attendee={item} event={this.props.event} {...props} />
  );

  render() {
    const { collection, event, superAdmin } = this.props;
    const { showEventCoreImport } = this.state;
    return (
      <CollectionListPage
        name='attendees'
        title={<Breadcrumbs event={event} attendee />}
        collection={collection}
        columns={columns}
        renderPreview={this.renderPreview}
        filters={this.categoryFilterConfig}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        canCreate={superAdmin}
        onCreate={this.onCreate}
        createLabel='Create new attendee'

        headerActions={this.getHeaderActions()}

        canEdit={this.canEdit}
        onEdit={this.onEdit}

        canDelete={this.canDelete}
        onDelete={this.onDelete}
        getDeleteMessage={this.getDeleteMessage}

        context={event.uid}
        ref={this.onRefList}
      >
        {
          showEventCoreImport &&
          <EventCoreImportDialog
            onClose={() => { this.setState({ showEventCoreImport: false }); }}
          />
        }
      </CollectionListPage>
    );
  }
}
