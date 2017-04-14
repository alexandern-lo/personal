import { connectActions } from 'store';
import { redirect } from 'store/navigate';

import { createResource } from 'store/resources/actions';
import { dataResource } from 'store/data/resource';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './resource-edit-form';

@connectActions({
  createResource,
  redirect,
})
export default class ResourceCreatePage extends Component {
  static propTypes = {
    createResource: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onCreate = data => this.props.createResource(dataResource(data));

  onSuccess = (r, d, { values: resource }) => this.props.redirect({
    pathname: '/resources',
    state: { created: resource.name },
  });

  onCancel = () => this.props.redirect('/resources');

  render() {
    return (
      <EditForm
        title={<Breadcrumbs resource action='Upload resources' />}
        actionTitle='Create'
        onCancel={this.onCancel}
        onSubmit={this.onCreate}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
