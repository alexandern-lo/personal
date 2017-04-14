import { navigate, refresh } from 'store/navigate';

import { connect } from 'store';
import { collectionShape } from 'store/collections';
import { getCollectionState } from 'store/users';
import {
  userResendInvite,
  userGrantAdmin,
  userRevokeAdmin,
  enableUser,
  disableUser,
} from 'store/users/actions';

import { getProfile, getProfileUID } from 'store/auth';
import { profileShape } from 'store/data/profile';

import { getBillingInfo } from 'store/account';
import { fetchBillingInfo } from 'store/account/actions';
import { billingInfoShape } from 'store/data/billing-info';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';

import StatusForm from 'components/users-forms/users-list-status-form';
import RoleForm from 'components/users-forms/users-list-role-form';
import InvitationForm from 'components/users-forms/users-list-invitation-form';

import RightPanel from './account-settings-panel';
import RenewPanel from './renew-panel';

import styles from './account-settings-page.module.css';

const cannot = () => false;

@connect({
  collection: getCollectionState,
  profile: getProfile,
  profileUid: getProfileUID,
  billingInfo: getBillingInfo,
}, {
  navigate,
  refresh,
  userResendInvite,
  userGrantAdmin,
  userRevokeAdmin,
  enableUser,
  disableUser,
  fetchBillingInfo,
})
export default class AccountSettings extends Component {
  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    collection: collectionShape.isRequired,
    profile: profileShape.isRequired,
    profileUid: PropTypes.string.isRequired,
    billingInfo: billingInfoShape,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
    userResendInvite: PropTypes.func.isRequired,
    userGrantAdmin: PropTypes.func.isRequired,
    userRevokeAdmin: PropTypes.func.isRequired,
    enableUser: PropTypes.func.isRequired,
    disableUser: PropTypes.func.isRequired,
    fetchBillingInfo: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    this.state = {};
  }

  componentWillMount = () => {
    this.setState({
      ...this.state,
      showRenewPanel: true,
    });
  };

  componentWillUnmount = () => {
    if (this.editWindow) {
      if (this.editWindow.detachEvent) this.editWindow.detachEvent('onunload', this.onEditWindowClose);
      else this.editWindow.onunload = null;
      this.editWindow = null;
    }
  };

  onInviteUsers = () => this.props.navigate('/users/invite');

  onClearRenewPanel = () => this.setState({ ...this.state, showRenewPanel: false });

  onEditSubscription = () => {
    const { billingInfo } = this.props;

    if (billingInfo && billingInfo.editUrl) {
      const wnd = window.open('', '_blank', 'channelmode=yes,scrollbars=no,width=700,height=700');
      wnd.document.write(`<body><iframe src='${billingInfo.editUrl}' frameBorder='0' width='100%' height='100%'/></body>`);
      if (wnd.attachEvent) wnd.attachEvent('onunload', this.onEditWindowClose);
      else wnd.onunload = this.onEditWindowClose;
      this.editWindow = wnd;
    } else {
      this.props.navigate('/select_plan');
    }
  };

  onEditWindowClose = () => {
    this.editWindow = null;
    this.updateBillingInfo();
  };

  getNotification = () => {
    const { invited } = this.props.location.state || {};
    if (invited) {
      return 'User has been successfully invited';
    }
    return null;
  };

  setListRef = (ref) => {
    this.listRef = ref;
    if (!this.props.billingInfo) this.updateBillingInfo();
  };

  updateBillingInfo = () => {
    if (this.listRef) this.listRef.handleApiActionCall(this.props.fetchBillingInfo());
  };

  clearNotification = () => this.props.refresh(this.props.location, {});

  canEnableMoreUsers = () => {
    const { profile: { subscription: { activeUsers, maxUsers } } } = this.props;
    return activeUsers < maxUsers;
  };

  isMe = user => this.props.profileUid === user.uid;

  renderPageContent = table => (
    <table className={styles.contentTable}>
      <tbody>
        <tr>
          <td>Users</td>
          <td>Subscription management</td>
        </tr>
        <tr>
          <td className={styles.contentCell}>{table}</td>
          <td className={styles.contentCell}>
            <RightPanel
              subscription={this.props.profile.subscription}
              billingInfo={this.props.billingInfo || {}}
              onEditSubscription={this.onEditSubscription}
            />
          </td>
        </tr>
      </tbody>
    </table>
  );

  renderInvitation = (val, user, field, { wrapApiAction }) => (
    <InvitationForm
      user={user}
      userResendInvite={wrapApiAction(this.props.userResendInvite)}
      disabled={this.isMe(user)}
    />
  );
  renderLicense = (val, user, field, { wrapApiAction }) => (
    <StatusForm
      user={user}
      disabled={this.isMe(user)}
      enabledText='Assigned'
      disabledText='None'
      enableUser={wrapApiAction(this.props.enableUser)}
      disableUser={wrapApiAction(this.props.disableUser)}
      canEnable={this.canEnableMoreUsers()}
    />
  );
  renderRole = (val, user, field, { wrapApiAction }) => (
    <RoleForm
      user={user}
      disabled={this.isMe(user)}
      userGrantAdmin={wrapApiAction(this.props.userGrantAdmin)}
      userRevokeAdmin={wrapApiAction(this.props.userRevokeAdmin)}
    />
  );

  render() {
    const { collection, profile: { subscription } } = this.props;
    const { showRenewPanel } = this.state;
    return (
      <CollectionListPage
        ref={this.setListRef}
        name='users'
        title={<Breadcrumbs action='Account' />}

        collection={collection}
        columns={[
          { field: 'firstName', label: 'First name', sortField: 'first_name' },
          { field: 'lastName', label: 'Last name', sortField: 'last_name' },
          { field: 'invitation', label: 'Invitation Status', render: this.renderInvitation, sortField: false },
          { field: 'license', label: `License(${subscription.activeUsers}/${subscription.maxUsers})`, render: this.renderLicense, sortField: false },
          { field: 'role', label: 'Role', render: this.renderRole, sortField: false },
        ]}
        canSelectAll={false}
        canSelect={cannot}
        canDelete={cannot}
        onCreate={this.onInviteUsers}
        createLabel='Invite new users'
        notification={this.getNotification()}
        clearNotification={this.clearNotification}
        renderContent={this.renderPageContent}
      >
        {showRenewPanel &&
          <RenewPanel expire={subscription.expiresAt} onClear={this.onClearRenewPanel} />
        }
      </CollectionListPage>
    );
  }
}
