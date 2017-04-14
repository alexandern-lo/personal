import { connect } from 'store';
import { redirect, refresh } from 'store/navigate';
import { isSuperAdmin } from 'store/auth';
import {
  getMobileSettings,
  getError,
  isLoading,
} from 'store/mobile-settings';
import {
  fetchMobileSettings,
  updateMobileSettings,
} from 'store/mobile-settings/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import SettingsForm from './mobile-settings-form';

@connect({
  isSA: isSuperAdmin,
  mobileSettings: getMobileSettings,
  error: getError,
  loading: isLoading,
}, {
  fetchMobileSettings,
  updateMobileSettings,
  redirect,
  refresh,
})
export default class MobileSettingsPage extends Component {
  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.shape({}),
    }),
    isSA: PropTypes.bool,
    mobileSettings: PropTypes.shape({}),
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    loading: PropTypes.bool,
    fetchMobileSettings: PropTypes.func.isRequired,
    updateMobileSettings: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  componentWillMount() {
    if (!this.props.isSA) {
      this.props.redirect('/');
    }
    this.props.fetchMobileSettings();
  }

  onRefForm = form => (this.form = form);

  onUpdateSettings = data => this.props.updateMobileSettings(data);

  onClearError = () => this.props.fetchMobileSettings();

  onSuccess = () => this.props.refresh(this.props.location, { success: true });

  onCancel = () => this.form.reset();

  getNotification = () => {
    const { success } = this.props.location.state || {};
    if (success) {
      return 'The link was successfully updated in mobile apps';
    }
    return null;
  };

  clearNotification = () => this.props.refresh(this.props.location, {});

  render() {
    const { mobileSettings, error, loading } = this.props;
    return (
      <SettingsForm
        title={<Breadcrumbs action='Mobile app settings' />}
        actionTitle='Save'
        loading={loading}
        loadingError={error}
        clearLoadingError={this.onClearError}
        initialValues={mobileSettings}
        onSubmit={this.onUpdateSettings}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
        notification={this.getNotification()}
        clearNotification={this.clearNotification}
        ref={this.onRefForm}
      />
    );
  }
}
