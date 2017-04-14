import { connect } from 'store';
import { navigate, refresh } from 'store/navigate';

import { collectionShape } from 'store/collections';
import {
  crmConfigShape,
  getAuthorizationUrl,
  SALESFORCE_TYPE,
  DYNAMICS365_TYPE,
} from 'store/data/crm-config';
import { getSelectedCrmConfig } from 'store/auth';
import { getCollectionState } from 'store/crm-configs';
import { setDefaultCrm, deleteCrmConfig } from 'store/crm-configs/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';

import styles from './crm-configs-list-page.module.css';

const displayTypes = {
  [SALESFORCE_TYPE]: 'Salesforce',
  [DYNAMICS365_TYPE]: 'Dynamics 365',
};

const renderType = type => displayTypes[type];

@connect({
  collection: getCollectionState,
  defaultConfig: getSelectedCrmConfig,
}, {
  setDefaultCrm,
  deleteCrmConfig,
  navigate,
  refresh,
})
export default class CRMConfigsList extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    collection: collectionShape.isRequired,
    defaultConfig: crmConfigShape,
    setDefaultCrm: PropTypes.func.isRequired,
    deleteCrmConfig: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    this.columns = [
      { field: 'name', label: 'Name' },
      { field: 'url', label: 'URL', sortField: false },
      { field: 'type', label: 'CRM System', render: renderType },
      { field: '', label: '', render: this.renderAuth },
    ];
  }

  onCreate = () => this.props.navigate('/crm/create');

  onEdit = crmConfig => this.props.navigate(`/crm/${crmConfig.uid}/edit`);

  onInternalDelete = configs => (configs.length > 0
    ? this.props.deleteCrmConfig(configs.pop().uid)
        .then(() => this.onInternalDelete(configs))
    : Promise.resolve(true)
  );

  onSetDefault = crmConfig => this.props.setDefaultCrm(crmConfig.uid);

  onAuthorize = crmConfig => (window.location = getAuthorizationUrl(crmConfig));

  getNotification = () => {
    const { created, updated } = this.props.location.state || {};
    if (created) {
      return `New config ${created} is successfully created`;
    }
    if (updated) {
      return `Config ${updated} is successfully updated`;
    }
    return null;
  };

  getDeleteMessage = configs => `Delete selected confuration${configs.length > 1 ? 's' : ''}?`;

  clearNotification = () => this.props.refresh(this.props.location, {});

  renderAuth = (_, crmConfig, __, { wrapApiAction }) => (
    <CrmAuth
      crmConfig={crmConfig}
      defaultConfig={this.props.defaultConfig}
      onSetDefault={wrapApiAction(this.onSetDefault)}
      onAuthorize={this.onAuthorize}
    />
  );

  render() {
    const { collection, defaultConfig } = this.props;
    return (
      <CollectionListPage
        tag={defaultConfig ? defaultConfig.uid : null}
        name='crm-configs'
        title={<Breadcrumbs crmConfig />}

        collection={collection}
        columns={this.columns}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        canSearch={false}

        onCreate={this.onCreate}
        createLabel='New configuration'

        onEdit={this.onEdit}
        onDelete={this.onInternalDelete}
        getDeleteMessage={this.getDeleteMessage}
      />
    );
  }
}

// eslint-disable-next-line react/no-multi-comp
class CrmAuth extends Component {
  static propTypes = {
    crmConfig: crmConfigShape.isRequired,
    defaultConfig: crmConfigShape,
    onSetDefault: PropTypes.func.isRequired,
    onAuthorize: PropTypes.func.isRequired,
  };

  onSetDefault = () => this.props.onSetDefault(this.props.crmConfig);
  onAuthorize = () => this.props.onAuthorize(this.props.crmConfig);

  render() {
    const { crmConfig, defaultConfig } = this.props;
    const { authorized } = crmConfig;
    const isDefault = defaultConfig && defaultConfig.uid === crmConfig.uid;
    return (
      <div>
        <button
          className={styles.setDefaultButton}
          onClick={this.onSetDefault}
          disabled={isDefault}
        >
          Set as default
        </button>
        { !authorized && (
          <button
            className={styles.authorizeButton}
            onClick={this.onAuthorize}
          >
            Authorize
          </button>
        )}
      </div>
    );
  }
}
