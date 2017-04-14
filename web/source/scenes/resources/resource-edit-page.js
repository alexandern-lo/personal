import { connect } from 'store';
import { redirect } from 'store/navigate';

import { getPinnedResource } from 'store/resources';
import { updateResource } from 'store/resources/actions';

import { editResource, resourceShape, dataResource } from 'store/data/resource';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './resource-edit-form';

@connect({
  resource: getPinnedResource,
}, {
  updateResource,
  redirect,
})
export default class ResourceEditPage extends Component {

  static propTypes = {
    resource: resourceShape.isRequired,
    updateResource: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onUpdate = data => this.props.updateResource({
    uid: data.uid,
    ...editResource(data),
    ...dataResource(data),
  });

  onSuccess = (r, d, { values: resource }) => this.props.redirect({
    pathname: '/resources',
    state: { updated: resource.name },
  });

  onCancel = () => this.props.redirect('/resources');

  render() {
    const { resource } = this.props;
    return (
      <EditForm
        title={<Breadcrumbs resource={resource} action='edit' />}
        actionTitle='Save'
        initialValues={editResource(resource)}
        onSubmit={this.onUpdate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
