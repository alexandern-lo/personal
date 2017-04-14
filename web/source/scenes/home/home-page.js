import { connect } from 'store';

import {
  isSuperAdmin,
  isTenantAdmin,
  isUser,
  isAnon,
} from 'store/auth';

import { getError } from 'store/home';
import { clearError } from 'store/home/actions';

import PageContentWrapper from 'components/layout/page-content-wrapper';
import SubHeader from 'components/sub-header/sub-header';
import Breadcrumbs from 'components/sub-header/breadcrumbs';
import ErrorPanel from 'components/errors/error-panel';

import UserDashboard from './user-dashboard';
import TenantAdminDashboard from './tenant-admin-dashboard';
import SuperAdminDashboard from './super-admin-dashboard';

@connect({
  superAdmin: isSuperAdmin,
  tenantAdmin: isTenantAdmin,
  user: isUser,
  anon: isAnon,
  error: getError,
}, {
  clearError,
})
export default class HomePage extends Component {
  static propTypes = {
    superAdmin: PropTypes.bool,
    tenantAdmin: PropTypes.bool,
    user: PropTypes.bool,
    anon: PropTypes.bool,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    clearError: PropTypes.func,
  }

  clearError = () => this.props.clearError();

  renderDashboard = () => {
    const { user, tenantAdmin, anon, superAdmin } = this.props;
    if (user || anon) {
      return <UserDashboard />;
    }
    if (tenantAdmin) {
      return <TenantAdminDashboard />;
    }
    if (superAdmin) {
      return <SuperAdminDashboard />;
    }

    return null;
  }

  render() {
    const { error } = this.props;
    return (
      <PageContentWrapper>
        { error && <ErrorPanel error={error} onClear={this.clearError} /> }
        <SubHeader>
          <Breadcrumbs action='Home' />
        </SubHeader>
        {this.renderDashboard()}
      </PageContentWrapper>
    );
  }
}
