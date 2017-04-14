import _ from 'lodash';
import { connect } from 'store';
import { navigate, refresh } from 'store/navigate';

import { isSuperAdmin, isTenantAdmin } from 'store/auth';

import { getPinnedEvent } from 'store/events';
import { eventShape } from 'store/data/event';

import { collectionShape } from 'store/collections';
import { deleteCategories } from 'store/categories/actions';
import { getCollectionState } from 'store/categories';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';

import styles from './categories-list-page.module.css';

const renderOptions = category => (
  <ul className={styles.optionsList}>
    { _.map(category, option => (
      <li key={option.uid}>{option.name}</li>
    ))}
  </ul>
);

const tableColumnsConfig = [
  { field: 'name', label: 'Name' },
  { field: 'options', label: 'Possible values', sortField: false, render: renderOptions },
];

@connect({
  event: getPinnedEvent,
  collection: getCollectionState,
  superAdmin: isSuperAdmin,
  tenantAdmin: isTenantAdmin,
}, {
  deleteCategories,
  navigate,
  refresh,
})
export default class EventCategoriesListPage extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    collection: collectionShape.isRequired,

    event: eventShape.isRequired,

    superAdmin: PropTypes.bool,

    deleteCategories: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  onRefList = list => (this.list = list);

  onCreate = () => this.props.navigate(`/events/${this.props.event.uid}/categories/create`);
  onEdit = category => this.props.navigate(`/events/${this.props.event.uid}/categories/${category.uid}/edit`);
  onDelete = categories => this.props.deleteCategories({
    eventUid: this.props.event.uid,
    data: categories.map(e => e.uid),
  });

  getNotification = () => {
    const { created, updated } = this.props.location.state || {};
    if (created) {
      return `New category ${created} is successfully created`;
    }
    if (updated) {
      return `Category ${updated} is successfully updated`;
    }
    return null;
  };

  getDeleteMessage = events => `Delete categor${events.length > 1 ? 'ies' : 'y'}?`;

  clearNotification = () => this.props.refresh(this.props.location, {});

  canDelete = () => this.props.superAdmin;
  canEdit = () => this.props.superAdmin;

  render() {
    const { event, collection, superAdmin } = this.props;
    return (
      <CollectionListPage
        className={styles.collection}
        name='categories'
        title={<Breadcrumbs event={event} category />}
        collection={collection}
        columns={tableColumnsConfig}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        onCreate={superAdmin ? this.onCreate : null}
        createLabel='Create new category'

        canEdit={this.canEdit}
        onEdit={this.onEdit}

        canDelete={this.canDelete}
        onDelete={this.onDelete}
        getDeleteMessage={this.getDeleteMessage}

        canSearch={false}

        ref={this.onRefList}
      />
    );
  }
}
