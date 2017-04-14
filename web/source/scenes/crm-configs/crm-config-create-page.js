import { connectActions } from 'store';
import { redirect } from 'store/navigate';

import { createCrmConfig } from 'store/crm-configs/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './crm-config-form';

@connectActions({
  createCrmConfig,
  redirect,
})
export default class CreateCrmConfigPage extends Component {

  static propTypes = {
    createCrmConfig: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onCreate = data => this.props.createCrmConfig(data);

  onSuccess = (r, d, { values: crmConfig }) => this.props.redirect({
    pathname: '/crm',
    state: { created: crmConfig.name },
  });

  onCancel = () => this.props.redirect('/crm');

  render() {
    return (
      <EditForm
        form='create-crm-config'
        title={<Breadcrumbs crmConfig action='New configuration' />}
        actionTitle='Create new'
        onSubmit={this.onCreate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
