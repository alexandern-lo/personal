import { navigate, refresh } from 'store/navigate';

import { connect } from 'store';
import { isSuperAdmin, isTenantAdmin } from 'store/auth';

import { getPinnedEvent } from 'store/events';
import { eventShape } from 'store/data/event';

import { collectionShape } from 'store/collections';
import { getCollectionState, EVENT_USER_INVITED_CONTEXT } from 'store/event-users';
import { deleteEventUsers } from 'store/event-users/actions';

import CollectionListPage from 'components/collections/list-page';
import Breadcrumbs from 'components/sub-header/breadcrumbs';
import { renderTenantsFilter } from 'components/sub-header/filters/tenants-filter';

const columns = [
  { field: 'firstName', label: 'First name', sortField: 'first_name' },
  { field: 'lastName', label: 'Last name', sortField: 'last_name' },
  { field: 'email', label: 'E-mail' },
];

const systemAdminColumns = columns.concat([
  { field: 'tenant', label: 'Tenant', sortField: false },
]);

const systemAdminFilters = {
  tenant: { render: renderTenantsFilter },
};

@connect({
  event: getPinnedEvent,
  collection: getCollectionState,
  superAdmin: isSuperAdmin,
  tenantAdmin: isTenantAdmin,
}, {
  navigate,
  refresh,
  deleteEventUsers,
})
export default class EventUsersList extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    event: eventShape.isRequired,
    collection: collectionShape.isRequired,
    superAdmin: PropTypes.bool,
    tenantAdmin: PropTypes.bool,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
    deleteEventUsers: PropTypes.func.isRequired,
  };

  onDelete = users => this.props.deleteEventUsers({
    eventUid: this.props.event.uid,
    data: users.map(u => u.uid),
  });
  onInviteUsers = () => this.props.navigate(`/events/${this.props.event.uid}/users/invite`);
  getNotification = () => {
    const { invited } = this.props.location.state || {};
    if (invited) {
      const num = parseInt(invited, 10);
      return invited > 1
        ? `${num} users have been successfully invited`
        : 'User has been successfully invited';
    }
    return null;
  };
  getDeleteMessage = users => `Delete selected user${users.length > 1 ? 's' : ''}?`;
  clearNotification = () => this.props.refresh(this.props.location, {});

  isAdmin = () => this.props.superAdmin || this.props.tenantAdmin;
  render() {
    const { event, collection, superAdmin, tenantAdmin } = this.props;
    const admin = superAdmin || tenantAdmin;
    return (
      <CollectionListPage
        name='event-users'
        title={<Breadcrumbs event={event} eventUser />}
        context={EVENT_USER_INVITED_CONTEXT + event.uid}
        collection={collection}
        columns={superAdmin ? systemAdminColumns : columns}
        filters={superAdmin ? systemAdminFilters : {}}
        canDelete={() => admin}
        onDelete={this.onDelete}
        canSelectAll={false}
        canSelect={() => admin}
        getDeleteMessage={this.getDeleteMessage}
        notification={this.getNotification()}
        clearNotification={this.clearNotification}
        onCreate={admin && this.onInviteUsers}
        createLabel={admin && 'Invite new users'}
        canSearch
      />
    );
  }
}
