import { connect } from 'store';
import { redirect } from 'store/navigate';

import { crmConfigShape } from 'store/data/crm-config';

import { getPinnedCrmConfig } from 'store/crm-configs';
import { updateCrmConfig } from 'store/crm-configs/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './crm-config-form';

@connect({
  crmConfig: getPinnedCrmConfig,
}, {
  updateCrmConfig,
  redirect,
})
export default class EditCrmConfigPage extends Component {

  static propTypes = {
    crmConfig: crmConfigShape.isRequired,
    updateCrmConfig: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onUpdate = data => this.props.updateCrmConfig(data);

  onSuccess = (r, d, { values: crmConfig }) => this.props.redirect({
    pathname: '/crm',
    state: { updated: crmConfig.name },
  });

  onCancel = () => this.props.redirect('/crm');

  render() {
    const { crmConfig } = this.props;
    return (
      <EditForm
        form='edit-crm-config'
        title={<Breadcrumbs crmConfig={crmConfig} action='edit' />}
        actionTitle='Save'
        initialValues={crmConfig}
        onSubmit={this.onUpdate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
