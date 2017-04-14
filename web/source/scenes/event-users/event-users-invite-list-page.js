import { redirect } from 'store/navigate';

import { connect } from 'store';
import { isSuperAdmin } from 'store/auth';

import { getPinnedEvent } from 'store/events';
import { eventShape } from 'store/data/event';

import { collectionShape } from 'store/collections';
import { getCollectionState, EVENT_USER_NOT_INVITED_CONTEXT } from 'store/event-users';
import { inviteEventUsers } from 'store/event-users/actions';

import { CancelButton } from 'components/sub-header/buttons';
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
}, {
  redirect,
  inviteEventUsers,
})
export default class EventUsersInviteList extends Component {

  static propTypes = {
    event: eventShape.isRequired,
    collection: collectionShape.isRequired,
    superAdmin: PropTypes.bool,
    redirect: PropTypes.func.isRequired,
    inviteEventUsers: PropTypes.func.isRequired,
  };

  onListRef = (ref) => {
    this.listRef = ref;
  };

  onInviteUsers = () => {
    const users = this.props.collection.selected;
    if (users.length <= 0) {
      return;
    }
    const promise = this.props.inviteEventUsers({
      eventUid: this.props.event.uid,
      data: users.map(u => u.uid),
    })
      .then(this.createOnInviteSuccess(users.length));
    this.listRef.handleApiActionCall(promise);
  };

  onCancel = () => this.props.redirect(`/events/${this.props.event.uid}/users`);

  createOnInviteSuccess = invited => () => this.props.redirect({
    pathname: `/events/${this.props.event.uid}/users`,
    state: { invited },
  });

  renderCancel = () => (<CancelButton onClick={this.onCancel}>Cancel</CancelButton>);

  render() {
    const { event, collection, superAdmin } = this.props;
    return (
      <CollectionListPage
        ref={this.onListRef}
        name='event-users'
        title={<Breadcrumbs event={event} eventUserInvite />}
        headerActions={[this.renderCancel()]}
        context={EVENT_USER_NOT_INVITED_CONTEXT + event.uid}
        collection={collection}
        columns={superAdmin ? systemAdminColumns : columns}
        filters={superAdmin ? systemAdminFilters : {}}
        onCreate={this.onInviteUsers}
        createLabel='Invite'
        canSearch
      />
    );
  }
}

