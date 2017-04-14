import { reduxForm, handleSubmitError, clearSubmitError } from 'components/forms';

import { collectionShape } from 'store/collections';
import buildActions from 'store/collections/actions_builder';

import Spinner from 'components/loading-spinner/loading-spinner';
import ErrorPanel from 'components/errors/error-panel';
import PageContentWrapper from 'components/layout/page-content-wrapper';
import PreviewPanelContainer from 'components/layout/preview-panel-container';

import Table from 'components/resource-table/resource-table';
import SubHeader from 'components/sub-header/sub-header';
import SubHeaderFilters from 'components/sub-header/sub-header-filters';
import Pagination from 'components/sub-header/pagination';

import { CreateButton } from 'components/sub-header/buttons';

import { SuccessDialog, DeleteDialog } from 'components/dialogs';

const perPageOptions = [10, 25, 50];

const can = (item, pred) => (pred ? pred(item) : true);

export default class CollectionsListPage extends Component {
  static propTypes = {
    tag: PropTypes.string,
    className: PropTypes.string,
    name: PropTypes.string.isRequired,
    context: PropTypes.string,
    title: PropTypes.node.isRequired,
    collection: collectionShape.isRequired,
    columns: PropTypes.arrayOf(PropTypes.object),
    filters: PropTypes.objectOf(PropTypes.object),

    notification: PropTypes.string,
    clearNotification: PropTypes.func,

    renderSubHeader: PropTypes.func,
    renderContent: PropTypes.func,
    renderPreview: PropTypes.func,

    headerActions: PropTypes.arrayOf(PropTypes.oneOfType([
      PropTypes.node,
      PropTypes.shape({
        render: PropTypes.func.isRequired,
      }),
    ])),

    canCreate: PropTypes.bool,
    onCreate: PropTypes.func,
    createLabel: PropTypes.string,

    canSearch: PropTypes.bool,

    canSelectAll: PropTypes.bool,
    canSelect: PropTypes.func,

    canEdit: PropTypes.func,
    onEdit: PropTypes.func,

    canDelete: PropTypes.func,
    onDelete: PropTypes.func,
    getDeleteMessage: PropTypes.func,

    children: PropTypes.node,
  };

  static defaultProps = {
    canCreate: true,
  }

  constructor(props) {
    super(props);
    this.collectionActions = buildActions(props.name);
  }

  componentDidMount = () => {
    const { notification, context } = this.props;
    const { items, context: collectionContext } = this.props.collection;
    const sameContext = context || collectionContext
      ? context === collectionContext
      : true;
    if (!sameContext || !items || !items.length || notification) {
      this.onRefresh();
    }
  };

  onRefForm = form => (this.form = form);

  onRefresh = () => {
    this.dispatch(this.collectionActions.fetchItems(this.props.context));
  };

  onSelectItems = (items) => {
    this.dispatch(this.collectionActions.selectItems(items));
  };

  onSort = (sort) => {
    this.dispatch(this.collectionActions.sortItems(sort));
  };

  onSearch = (search) => {
    this.dispatch(this.collectionActions.search(search));
  };

  onSelectFilter = (filters) => {
    this.dispatch(this.collectionActions.changeFilter(filters));
  };

  onChangePage = (page) => {
    this.dispatch(this.collectionActions.changePage(page));
  };

  onChangePerPage = (perPage) => {
    this.dispatch(this.collectionActions.changePerPage(perPage));
  };

  onPreview = (item) => {
    if (this.props.renderPreview) {
      this.dispatch(this.collectionActions.previewItem(item));
    }
  };

  onClosePreview = () => {
    this.dispatch(this.collectionActions.previewItem(null));
  };

  onEditItem = (item) => {
    this.onClosePreview();
    this.props.onEdit(item);
  };

  onAskDeleteItem = (item) => {
    this.setState({ itemsToDelete: [item], deleteError: null });
  };
  onAskDeleteItems = (items) => {
    this.setState({ itemsToDelete: items, deleteError: null });
  };

  onPerformDelete = () => {
    this.onClosePreview();
    return this.props.onDelete(this.getItemsToDelete().slice());
  };

  onCancelDelete = () => this.setState({ itemsToDelete: null });

  onCompletedDelete = () => {
    this.setState({ itemsToDelete: null });
    this.onClosePreview();
    this.onRefresh();
  };

  onFailedDelete = ({ _error: error }) => {
    this.setState({ itemsToDelete: null, deleteError: error });
    this.onRefresh();
  };

  getItemsToDelete = () => this.state && this.state.itemsToDelete;

  clearCollectionError = () => {
    this.dispatch(this.collectionActions.clearFetchError());
  };

  clearDeleteError = () => this.setState({ deleteError: null });

  dispatch = action => this.form.wrappedInstance.props.dispatch(action);

  wrapApiAction = fn => (...args) => this.form.wrappedInstance.handleApiActionCall(fn(...args));
  handleApiActionCall = promise => this.form.wrappedInstance.handleApiActionCall(promise);

  renderFilters = () => {
    const { collection, canSearch, filters: filtersConfig } = this.props;
    const { search, filters, totalPages, page, perPage } = collection;
    return (
      <SubHeaderFilters
        searchTerm={search}
        onSearch={canSearch === false ? null : this.onSearch}
        filters={filters}
        filtersConfig={filtersConfig}
        onFilterChange={this.onSelectFilter}
      >
        <Pagination
          page={page}
          totalPages={totalPages}
          perPage={perPage}
          perPageOptions={perPageOptions}
          onChangePage={this.onChangePage}
          onChangePerPage={this.onChangePerPage}
        />
      </SubHeaderFilters>
    );
  };

  renderPreview = () => {
    const {
      tag, collection: { preview }, renderPreview,
      canEdit, onEdit, canDelete, onDelete,
    } = this.props;
    if (!preview || !renderPreview) return null;
    return renderPreview({
      tag,
      item: preview,
      onEdit: onEdit && can(preview, canEdit) ? this.onEditItem : null,
      onDelete: onDelete && can(preview, canDelete) ? this.onAskDeleteItem : null,
      onClose: this.onClosePreview,
    });
  };

  renderTable = () => {
    const {
      tag, collection, columns, onEdit, onDelete,
      canSelectAll, canSelect, canEdit, canDelete,
      renderContent,
    } = this.props;
    const { items, preview, selected, sortField, sortOrder } = collection;
    const table = (
      <Table
        tag={tag}
        items={items || []} columns={columns}
        sortField={sortField} sortOrder={sortOrder} onSort={this.onSort}
        activeItem={preview} onActivate={this.onPreview}
        selected={selected} canSelectAll={canSelectAll}
        canSelect={canSelect} onSelect={this.onSelectItems}
        canEdit={canEdit} onEdit={onEdit}
        canDelete={canDelete} onDelete={onDelete ? this.onAskDeleteItems : null}
        wrapApiAction={this.wrapApiAction}
      />
    );
    return renderContent ? renderContent(table) : table;
  };

  renderSubHeader = () => {
    const { title, headerActions, onCreate, createLabel, renderSubHeader, canCreate } = this.props;
    if (renderSubHeader) return renderSubHeader();
    return (
      <SubHeader>
        { title }
        { headerActions && headerActions.map(action => (
          React.isValidElement(action) ? action : action.render()
        ))}
        { canCreate && onCreate && (
          <CreateButton withIcon onClick={onCreate}>
            { createLabel }
          </CreateButton>
        )}
      </SubHeader>
    );
  };

  renderDeleteDialog = () => {
    const { getDeleteMessage = () => 'Delete selected items?' } = this.props;
    const itemsToDelete = this.getItemsToDelete();
    return (itemsToDelete && itemsToDelete.length > 0) ? (
      <DeleteDialog
        message={getDeleteMessage(itemsToDelete)}
        onSubmit={this.onPerformDelete}
        onCancel={this.onCancelDelete}
        onSubmitSuccess={this.onCompletedDelete}
        onSubmitFail={this.onFailedDelete}
      />
    ) : null;
  };

  render() {
    const { name, className, collection, notification, clearNotification, children } = this.props;
    const { deleteError } = this.state || {};
    const { loading, error } = collection;
    return (
      <CollectionListForm
        form={`${name}-collection-form`}
        className={className}
        loading={loading}
        notification={notification}
        clearNotification={clearNotification}
        ref={this.onRefForm}
      >
        { error && <ErrorPanel error={error} onClear={this.clearCollectionError} /> }
        { deleteError && <ErrorPanel error={deleteError} onClear={this.clearDeleteError} /> }
        { this.renderDeleteDialog() }
        { this.renderSubHeader() }
        { this.renderFilters() }
        { children }
        <PreviewPanelContainer previewPanel={this.renderPreview()}>
          { this.renderTable() }
        </PreviewPanelContainer>
      </CollectionListForm>
    );
  }
}

@reduxForm()
class CollectionListForm extends Component { // eslint-disable-line react/no-multi-comp
  static propTypes = {
    className: PropTypes.string,
    loading: PropTypes.bool,
    notification: PropTypes.string,
    clearNotification: PropTypes.func.isRequired,
    handleSubmit: PropTypes.func.isRequired,
    submitting: PropTypes.bool,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    children: PropTypes.node,
  };

  handleApiActionCall = promise => this.props.handleSubmit(() => handleSubmitError(promise))();

  clearApiActionError = () => clearSubmitError(this.props);

  render() {
    const {
      className, loading, submitting, error,
      notification, clearNotification,
      children,
    } = this.props;
    return (
      <PageContentWrapper className={className}>
        { (loading || submitting) && !notification && <Spinner /> }
        { notification && <SuccessDialog message={notification} onOk={clearNotification} /> }
        { error && <ErrorPanel error={error} onClear={this.clearApiActionError} /> }
        { children }
      </PageContentWrapper>
    );
  }
}
