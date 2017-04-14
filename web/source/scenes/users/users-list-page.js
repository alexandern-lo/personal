import { navigate, refresh } from 'store/navigate';

import { connect } from 'store';
import { collectionShape } from 'store/collections';
import { getCollectionState } from 'store/users';
import {
  userGrantAdmin,
  userRevokeAdmin,
  enableUser,
  disableUser,
  deleteUsers,
} from 'store/users/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';

import RoleForm from 'components/users-forms/users-list-role-form';
import StatusForm from 'components/users-forms/users-list-status-form';

import { renderTenantsFilter } from 'components/sub-header/filters/tenants-filter';

const filtersConfig = {
  role: {
    title: 'Role',
    options: {
      '': 'All roles',
      user: 'Seat user',
      admin: 'Tenant admin',
    },
  },
  status: {
    title: 'Status',
    options: {
      '': 'All statuses',
      enabled: 'Enabled',
      disabled: 'Disabled',
      invited: 'Invited',
    },
  },
  tenant: { render: renderTenantsFilter },
};

const can = () => true;

@connect({
  collection: getCollectionState,
}, {
  navigate,
  refresh,
  userGrantAdmin,
  userRevokeAdmin,
  enableUser,
  disableUser,
  deleteUsers,
})
export default class UsersList extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    collection: collectionShape.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
    userGrantAdmin: PropTypes.func.isRequired,
    userRevokeAdmin: PropTypes.func.isRequired,
    enableUser: PropTypes.func.isRequired,
    disableUser: PropTypes.func.isRequired,
    deleteUsers: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    this.columns = [
      { field: 'firstName', label: 'First name', sortField: 'first_name' },
      { field: 'lastName', label: 'Last name', sortField: 'last_name' },
      { field: 'email', label: 'E-mail' },
      { field: 'tenant', label: 'Tenant' },
      { field: 'role', label: 'Role', sortField: false, render: this.renderRole },
      { field: 'status', label: 'Status', sortField: false, render: this.renderStatus },
    ];
  }

  onInviteUser = () => this.props.navigate('/users/invite');
  onDeleteUsers = users => this.props.deleteUsers(users.map(u => u.uid));
  getDeleteMessage = users => `Delete selected user${users.length > 1 ? 's' : ''}?`;

  getNotification = () => {
    const { invited } = this.props.location.state || {};
    if (invited) {
      return `Invitation was successfully sent to ${invited}`;
    }
    return null;
  };
  clearNotification = () => this.props.refresh(this.props.location, {});

  renderStatus = (val, user, field, { wrapApiAction }) => (
    <StatusForm
      user={user}
      enableUser={wrapApiAction(this.props.enableUser)}
      disableUser={wrapApiAction(this.props.disableUser)}
    />
  );
  renderRole = (val, user, field, { wrapApiAction }) => (
    <RoleForm
      user={user}
      userGrantAdmin={wrapApiAction(this.props.userGrantAdmin)}
      userRevokeAdmin={wrapApiAction(this.props.userRevokeAdmin)}
    />
  );

  render() {
    const { collection } = this.props;
    return (
      <CollectionListPage
        name='users'
        title={<Breadcrumbs action='Users' />}

        collection={collection}
        columns={this.columns}
        filters={filtersConfig}

        canSelectAll={false}
        canSelect={can}

        canDelete={can}
        onDelete={this.onDeleteUsers}
        getDeleteMessage={this.getDeleteMessage}

        onCreate={this.onInviteUser}
        createLabel='Invite new user'
        notification={this.getNotification()}
        clearNotification={this.clearNotification}
      />
    );
  }
}
