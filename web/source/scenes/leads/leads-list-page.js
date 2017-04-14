import _ from 'lodash';
import { connect } from 'store';
import { navigate, refresh } from 'store/navigate';

import { formatUtcDate } from 'helpers/dates';
import { saveFileResponse } from 'helpers/downloads';

import { crmConfigShape } from 'store/data/crm-config';
import {
  isSuperAdmin,
  isTenantAdmin,
  getSelectedCrmConfig,
} from 'store/auth';

import { collectionShape } from 'store/collections';
import { getCollectionState } from 'store/leads';
import { editLead } from 'store/data/lead';
import {
  updateLead,
  deleteLead,
  exportLeadsToFile,
  exportLeadsToCRM,
} from 'store/leads/actions';


import Breadcrumbs from 'components/sub-header/breadcrumbs';
import CollectionListPage from 'components/collections/list-page';

import LeadsExport from './leads-export';
import LeadPreview, { renderEmail } from './lead-preview';

const renderFirstEmail = (emails = []) => (emails[0] && renderEmail(emails[0]));
const renderTenant = (tenant = {}) => tenant.name;
const renderOwner = (user = {}) => user.email;

const userColumns = [
  { field: 'firstName', label: 'First name', sortField: 'first_name' },
  { field: 'lastName', label: 'Last name', sortField: 'last_name' },
  { field: 'jobTitle', label: 'Job title', sortField: 'job_title' },
  { field: 'companyName', label: 'Company', sortField: 'company_name' },
  { field: 'emails', label: 'Email', sortField: false, render: renderFirstEmail },
  { field: 'createdAt', label: 'Created on', sortField: 'created_at', render: formatUtcDate },
];

const superAdminColumns = userColumns.concat([
  { field: 'tenant', label: 'Tenant', sortField: false, render: renderTenant },
]);

const tentantAdminColumns = userColumns.concat([
  { field: 'owner', label: 'User', sortField: false, render: renderOwner },
]);

export const getColumns = ({ isSA, isTA }) => {
  if (isSA) return superAdminColumns;
  if (isTA) return tentantAdminColumns;
  return userColumns;
};


@connect({
  isSA: isSuperAdmin,
  isTA: isTenantAdmin,
  collection: getCollectionState,
  crmConfig: getSelectedCrmConfig,
}, {
  updateLead,
  deleteLead,
  exportLeadsToCRM,
  exportLeadsToFile,
  navigate,
  refresh,
})
export default class LeadsListPage extends Component {

  static propTypes = {
    location: PropTypes.shape({
      state: PropTypes.objectOf(PropTypes.string),
    }),
    isSA: PropTypes.bool,
    isTA: PropTypes.bool,

    collection: collectionShape.isRequired,
    crmConfig: crmConfigShape,

    updateLead: PropTypes.func.isRequired,
    deleteLead: PropTypes.func.isRequired,
    exportLeadsToCRM: PropTypes.func.isRequired,
    exportLeadsToFile: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
    refresh: PropTypes.func.isRequired,
  };

  onRefList = list => (this.list = list);

  onCreate = () => this.props.navigate('/leads/create');
  onEdit = lead => this.props.navigate(`/leads/${lead.uid}/edit`);
  onEditQuestionnaire = (lead) => {
    this.list.onClosePreview();
    this.props.navigate(`/leads/${lead.uid}/questionnaire`);
  };

  onChangeQualification = (lead, qualification) => {
    const { lead_uid, event_uid } = editLead(lead);
    return this.props.updateLead({ lead_uid, event_uid, qualification });
  };

  onInternalDelete = leads => (leads.length > 0
    ? this.props.deleteLead(leads.pop().uid)
        .then(() => this.onInternalDelete(leads))
    : Promise.resolve(true)
  );

  onExportToFile = (format) => {
    const { selected } = this.props.collection;
    this.list.handleApiActionCall(this.props.exportLeadsToFile({
      format,
      lead_uids: _.map(selected, lead => lead.uid),
    }).then(saveFileResponse));
  };

  onSetupCRM = () => this.props.navigate('/crm');

  onExportToCRM = () => {
    const { selected } = this.props.collection;
    this.list.handleApiActionCall(this.props.exportLeadsToCRM({
      lead_uids: _.map(selected, lead => lead.uid),
    }).then(resp => this.onCompleteExportToCRM(resp.data)));
  };

  onCompleteExportToCRM = (data = {}) => {
    const {
      total_created: created,
      total_updated: updated,
      total_failed: failed,
    } = data;
    if (failed > 0) {
      throw new Error(`${failed} lead${failed > 1 ? 's' : ''} failed to export`);
    }
    this.props.refresh(this.props.location, { exported: created + updated });
  };

  getNotification = () => {
    const { created, updated, exported } = this.props.location.state || {};
    if (created) {
      return `Lead ${created} has been successfully created`;
    }
    if (updated) {
      return `Lead ${updated} has been successfully updated`;
    }
    if (exported) {
      return 'Leads have been successfully exported';
    }
    return null;
  };

  getDeleteMessage = leads => `Delete lead${leads.length > 1 ? 's' : ''}?`;

  clearNotification = () => this.props.refresh(this.props.location, {});

  canEdit = () => true;
  canDelete = () => true;
  canEditQuestionnaire = lead => (
    lead.event && lead.event.questions && lead.event.questions.length > 0
  );

  renderPreview = ({ item, ...props }) => (
    <LeadPreview
      lead={item}
      {...props}
      onEditQuestionnaire={this.canEditQuestionnaire(item) ? this.onEditQuestionnaire : null}
      onChangeQualification={this.canEdit(item) ? this.onChangeQualification : null}
    />
  );

  render() {
    const { collection, crmConfig, isSA, isTA } = this.props;
    const { selected, total } = collection;
    const columns = getColumns({ isSA, isTA });
    const filters = isSA ? this.filtersConfig : null;
    const headerActions = [];
    if (total > 0) {
      headerActions.push((
        <LeadsExport
          key='export'
          count={selected.length}
          crmConfig={crmConfig}
          onExportToFile={this.onExportToFile}
          onSetupCRM={this.onSetupCRM}
          onExportToCRM={this.onExportToCRM}
        />
      ));
    }
    return (
      <CollectionListPage
        name='leads'
        title={<Breadcrumbs lead />}

        collection={collection}
        columns={columns}
        filters={filters}
        renderPreview={this.renderPreview}

        notification={this.getNotification()}
        clearNotification={this.clearNotification}

        onCreate={this.onCreate}
        createLabel='Create new lead'

        headerActions={headerActions}

        canEdit={this.canEdit}
        onEdit={this.onEdit}

        canDelete={this.canDelete}
        onDelete={this.onInternalDelete}
        getDeleteMessage={this.getDeleteMessage}

        ref={this.onRefList}
      />
    );
  }
}
