import { connect } from 'store';
import { navigate, refresh } from 'store/navigate';

import { formatUtcDate } from 'helpers/dates';

import { collectionShape } from 'store/collections';
import { getCollectionState } from 'store/resources';
import { deleteResource } from 'store/resources/actions';
import { getDisplayType } from 'store/data/resource';

import { isSuperAdmin, isTenantAdmin } from 'store/auth';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';
import { renderTenantsFilter } from 'components/sub-header/filters/tenants-filter';
import { renderUsersFilter } from 'components/sub-header/filters/users-filter';

import ResourcePreview, { renderOwner } from './resource-preview';

const renderName = (name, resource) => (resource.url
  ? <a href={resource.url} download target='_blank' rel='noopener noreferrer'>{name}</a>
  : name
);

const renderType = (_, resource) => getDisplayType(resource);
const renderTenant = tenant => tenant && tenant.name;

const nameColumn = { field: 'name', label: 'Name', render: renderName };
const typeColumn = { field: 'type', label: 'Type', render: renderType };
const dateColumn = { field: 'createdAt', label: 'Upload date', sortField: 'created_at', render: formatUtcDate };
const ownerColumn = { field: 'user', label: 'Owner', sortField: false, render: renderOwner };
const tenantColumn = { field: 'tenant', label: 'Tenant', sortField: false, render: renderTenant };

const SA_COLUMNS = [nameColumn, typeColumn, tenantColumn, ownerColumn, dateColumn];
const TA_COLUMNS = [nameColumn, typeColumn, ownerColumn, dateColumn];
const USER_COLUMNS = [nameColumn, typeColumn, dateColumn];

const SA_FILTERS = {
  tenant: { render: renderTenantsFilter },
};

const TA_FILTERS = {
  user: { render: renderUsersFilter },
};

@connect({
  isSA: isSuperAdmin,
  isTA: isTenantAdmin,
  collection: getCollectionState,
}, {
  deleteResource,
  navigate,
  refresh,
})
export default class ResourcesList extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    isTA: PropTypes.bool,
    isSA: PropTypes.bool,

    collection: collectionShape.isRequired,
    deleteResource: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  onCreate = () => this.props.navigate('/resources/create');
  onEdit = resource => this.props.navigate(`/resources/${resource.uid}/edit`);

  onInternalDelete = resources => (resources.length > 0
    ? this.props.deleteResource(resources.pop().uid)
        .then(() => this.onInternalDelete(resources))
    : Promise.resolve(true)
  );

  getNotification = () => {
    const { created, updated } = this.props.location.state || {};
    if (created) {
      return `New resource ${created} is successfully created`;
    }
    if (updated) {
      return `Resource ${updated} is successfully updated`;
    }
    return null;
  };

  getDeleteMessage = resources => `Delete resource${resources.length > 1 ? 's' : ''}?`;

  getColumns = () => {
    const { isTA, isSA } = this.props;
    if (isSA) return SA_COLUMNS;
    if (isTA) return TA_COLUMNS;
    return USER_COLUMNS;
  };

  getFilters = () => {
    const { isTA, isSA } = this.props;
    if (isSA) return SA_FILTERS;
    if (isTA) return TA_FILTERS;
    return null;
  };

  clearNotification = () => this.props.refresh(this.props.location, {});

  renderPreview = ({ item, ...props }) => (
    <ResourcePreview resource={item} {...props} />
  );

  render() {
    const { collection } = this.props;
    const columns = this.getColumns();
    const filters = this.getFilters();
    return (
      <CollectionListPage
        name='resources'
        title={<Breadcrumbs resource />}

        collection={collection}
        columns={columns}
        filters={filters}
        renderPreview={this.renderPreview}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        onCreate={this.onCreate}
        createLabel='Upload resources'

        canEdit={this.canEdit}
        onEdit={this.onEdit}

        canDelete={this.canDelete}
        onDelete={this.onInternalDelete}
        getDeleteMessage={this.getDeleteMessage}
      />
    );
  }
}
