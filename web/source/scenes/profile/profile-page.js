import { connect } from 'store';
import { refresh } from 'store/navigate';

import { profileShape } from 'store/data/profile';
import { getProfile, isTenantAdmin } from 'store/auth';
import { updateProfile, resetPassword } from 'store/auth/actions';

import { editUser } from 'store/data/user';
import { editTenant } from 'store/data/tenant';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import { BlueMessagePanel } from 'components/message-panel/message-panel';

import ProfileForm from './profile-form';

@connect({
  profile: getProfile,
  tenant: isTenantAdmin,
}, {
  updateProfile,
  resetPassword,
  refresh,
})
export default class AttendeesEditForm extends Component {
  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.shape({}),
    }),
    profile: profileShape.isRequired,
    tenant: PropTypes.bool.isRequired,
    updateProfile: PropTypes.func.isRequired,
    resetPassword: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  onUpdate = data => this.props.updateProfile(data);

  onSuccess = () => this.props.refresh(this.props.location, { success: true });

  onCancel = () => this.profileForm.reset();

  onResetPassword = () => this.props.resetPassword();

  getNotification = () => {
    const { success } = this.props.location.state || {};
    if (success) {
      return 'Profile has been successfully updated';
    }
    return null;
  };

  clearNotification = () => this.props.refresh(this.props.location, {});

  isMessagePanelVisible = () => {
    const { newUser } = this.props.location.state || {};
    return !!newUser;
  };

  renderMessagePanel = () => this.isMessagePanelVisible() && (
    <BlueMessagePanel
      message='So we can optimize your experience, please complete your profile.'
      onClear={this.clearNotification}
    />
  );

  render() {
    const { profile: { user, tenant } } = this.props;
    return (
      <ProfileForm
        ref={(form) => { this.profileForm = form; }}
        initialValues={{ user: editUser(user), tenant: editTenant(tenant) }}
        disableCompanyDetails={!this.props.tenant}
        form='profile'
        title={<Breadcrumbs action='Profile' />}
        actionTitle='Save'
        onResetPassword={this.onResetPassword}
        onSubmit={this.onUpdate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        panel={this.renderMessagePanel()}
      />
    );
  }
}
