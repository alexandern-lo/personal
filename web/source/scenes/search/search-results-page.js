import { connect } from 'store';
import {
  getError,
  isLoading,
  getActiveTab,
  getEvents,
  getLeads,
  getAttendees,
} from 'store/search';
import {
  clearError,
  changeTab,
  changePage,
  changePerPage,
  previewItem,
  sortItems,
} from 'store/search/actions';

import { getProfileUID, isSuperAdmin, isTenantAdmin } from 'store/auth';

import PageContentWrapper from 'components/layout/page-content-wrapper';
import PreviewPanelContainer from 'components/layout/preview-panel-container';
import Spinner from 'components/loading-spinner/loading-spinner';
import ErrorPanel from 'components/errors/error-panel';
import Table from 'components/resource-table/resource-table';
import Pagination from 'components/sub-header/pagination';

import { getColumns as getEventsColumns } from 'scenes/events/events-list-page';
import { getColumns as getLeadsColumns } from 'scenes/leads/leads-list-page';
import { getColumns as getAttendeesColumns } from 'scenes/attendees/attendees-list-page';

import EventPreview from 'scenes/events/event-preview';
import LeadPreview from 'scenes/leads/lead-preview';
import AttendeesPreview from 'scenes/attendees/attendees-preview';

import { isConference, isOwnedBy } from 'store/data/event';

import styles from './search-results-page.module.css';

const perPageOptions = [10, 25, 50];

const collectionShape = {
  items: PropTypes.arrayOf(PropTypes.object),
  preview: PropTypes.shape({}),
  total: PropTypes.number.isRequired,
  page: PropTypes.number.isRequired,
  totalPages: PropTypes.number.isRequired,
  perPage: PropTypes.number.isRequired,
};

const getColumns = (tab, roles) => {
  switch (tab) {
    case 'events': return getEventsColumns(roles);
    case 'leads': return getLeadsColumns(roles);
    case 'attendees': return getAttendeesColumns(roles);
    default: return null;
  }
};

@connect({
  isSA: isSuperAdmin,
  isTA: isTenantAdmin,
  myUID: getProfileUID,
  loading: isLoading,
  error: getError,
  activeTab: getActiveTab,
  events: getEvents,
  leads: getLeads,
  attendees: getAttendees,
}, {
  clearError,
  changeTab,
  changePage,
  changePerPage,
  previewItem,
  sortItems,
})
export default class SearchResultsPage extends Component {
  static propTypes = {
    isSA: PropTypes.bool,
    isTA: PropTypes.bool,
    myUID: PropTypes.string,
    loading: PropTypes.bool,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    activeTab: PropTypes.string.isRequired,
    events: PropTypes.shape(collectionShape),
    leads: PropTypes.shape(collectionShape),
    attendees: PropTypes.shape(collectionShape),
    clearError: PropTypes.func.isRequired,
    changeTab: PropTypes.func.isRequired,
    changePage: PropTypes.func.isRequired,
    changePerPage: PropTypes.func.isRequired,
    previewItem: PropTypes.func.isRequired,
    sortItems: PropTypes.func.isRequired,
  };

  onChangeTab = (tab) => {
    this.onClosePreview();
    this.props.changeTab(tab);
  };

  onClosePreview = () => this.props.previewItem(null);

  onNavigate = (path) => {
    this.onClosePreview();
    window.open(path, '_blank');
  };

  onEditEvent = event => this.onNavigate(`/events/${event.uid}/edit`);
  onEditLead = lead => this.onNavigate(`/leads/${lead.uid}/edit`);
  onEditLeadQuestionnaire = lead => this.onNavigate(`/leads/${lead.uid}/questionnaire`);
  onEditAttendee = attendee => this.onNavigate(`/events/${attendee.eventUid}/attendees/${attendee.uid}/edit`);

  getCollection = (name) => {
    switch (name) {
      case 'events': return this.props.events;
      case 'leads': return this.props.leads;
      case 'attendees': return this.props.attendees;
      default: return null;
    }
  };

  renderFilters = () => {
    const { activeTab } = this.props;
    const { page, perPage, totalPages } = this.getCollection(activeTab);
    const tab = (name, label) => {
      const { total } = this.getCollection(name);
      return (
        <button
          className={classNames({ [styles.active]: name === activeTab })}
          onClick={() => this.onChangeTab(name)}
        >{label} ({total})</button>
      );
    };
    return (
      <div className={styles.header}>
        <div className={styles.tabs}>
          { tab('events', 'Events') }
          { tab('leads', 'Leads') }
          { tab('attendees', 'Attendees') }
        </div>
        <Pagination
          page={page}
          totalPages={totalPages}
          perPage={perPage}
          perPageOptions={perPageOptions}
          onChangePage={this.props.changePage}
          onChangePerPage={this.props.changePerPage}
        />
      </div>
    );
  };

  renderTable = () => {
    const { activeTab, isSA, isTA } = this.props;
    const { items, preview, sortField, sortOrder } = this.getCollection(activeTab);
    return (
      <Table
        key={activeTab}
        className={styles.table}
        items={items || []}
        columns={getColumns(activeTab, { isSA, isTA })}
        sortField={sortField}
        sortOrder={sortOrder}
        onSort={this.props.sortItems}
        activeItem={preview}
        onActivate={this.props.previewItem}
      />
    );
  };

  renderPreview = () => {
    switch (this.props.activeTab) {
      case 'events': return this.renderEventsPreview();
      case 'leads': return this.renderLeadsPreview();
      case 'attendees': return this.renderAttendeesPreview();
      default: return null;
    }
  };

  renderEventsPreview = () => {
    const { isSA, myUID } = this.props;
    const { preview: event } = this.props.events;
    if (!event) return null;
    const canEdit = isConference(event) ? isSA : isOwnedBy(event, myUID);
    return (
      <EventPreview
        className={styles.preview}
        event={event}
        isSuperAdmin={isSA}
        onEdit={canEdit ? this.onEditEvent : null}
        onNavigate={this.onNavigate}
        onClose={this.onClosePreview}
      />
    );
  };

  renderLeadsPreview = () => {
    const { preview: lead } = this.props.leads;
    if (!lead) return null;
    const canEditQuestionnaire = (
      lead.event && lead.event.questions && lead.event.questions.length > 0
    );
    return (
      <LeadPreview
        className={styles.preview}
        lead={lead}
        onEdit={this.onEditLead}
        onEditQuestionnaire={canEditQuestionnaire ? this.onEditLeadQuestionnaire : null}
        onClose={this.onClosePreview}
      />
    );
  };

  renderAttendeesPreview = () => {
    const { isSA, attendees } = this.props;
    const { preview: attendee } = attendees;
    if (!attendee) return null;
    const event = {};
    return (
      <AttendeesPreview
        className={styles.preview}
        attendee={attendee}
        event={event}
        onEdit={isSA ? this.onEditAttendee : null}
        onClose={this.onClosePreview}
      />
    );
  };

  render() {
    const { loading, error } = this.props;
    return (
      <PageContentWrapper>
        { loading && <Spinner /> }
        { error && <ErrorPanel error={error} onClear={this.props.clearError} /> }
        { this.renderFilters() }
        <PreviewPanelContainer previewPanel={this.renderPreview()}>
          { this.renderTable() }
        </PreviewPanelContainer>
      </PageContentWrapper>
    );
  }
}
