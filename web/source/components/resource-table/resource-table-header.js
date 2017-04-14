import _ from 'lodash';

import styles from './resource-table.module.css';

const invertSort = (sorted, sortOrder) => {
  if (!sorted) return 'asc';
  return sortOrder === 'desc' ? 'asc' : 'desc';
};

const sameItems = (first, second) => (
  first && second && first.length > 0 && first.length === second.length &&
  _.difference(first, second).length === 0
);

const can = () => true;

export const columnProps = {
  label: PropTypes.string,
  field: PropTypes.string,
  sortField: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.bool,
  ]),
};

export default class ResourceTableHeader extends PureComponent {
  static propTypes = {
    columns: PropTypes.arrayOf(PropTypes.shape(columnProps)).isRequired,
    items: PropTypes.arrayOf(PropTypes.object).isRequired,
    selected: PropTypes.arrayOf(PropTypes.object),
    sortField: PropTypes.string,
    sortOrder: PropTypes.string,
    onSort: PropTypes.func,
    canSelectAll: PropTypes.bool,
    canSelect: PropTypes.func,
    onSelect: PropTypes.func,
    canEdit: PropTypes.func,
    canDelete: PropTypes.func,
    onDelete: PropTypes.func,
  };

  onSelectAll = () => {
    const all = this.itemsToSelect();
    const { selected, onSelect } = this.props;
    onSelect(sameItems(all, selected) ? [] : all);
  };

  onDelete = () => this.props.onDelete(this.props.selected);

  itemsToSelect = () => _.filter(this.props.items, this.props.canSelect);

  renderColumn = ({ field, label, sortField: _sortField }) => {
    const { sortField, sortOrder, onSort } = this.props;
    const columnSortField = _sortField || field;
    const sortable = _sortField !== false;
    const sorted = columnSortField == sortField; // eslint-disable-line eqeqeq
    const desc = sortOrder === 'desc';
    const sort = field && sortable
      ? () => onSort({ sortField: columnSortField, sortOrder: invertSort(sorted, sortOrder) })
      : null;
    return (
      <th
        key={field}
        onClick={sort}
        className={classNames('content-cell', {
          [styles.headerSortable]: sortable,
          [styles.headerSort]: sorted,
          [styles.headerSortDesc]: sorted && desc,
        })}
      >
        {label}
      </th>
    );
  };

  render() {
    const {
      items, columns, selected, onSelect, onDelete,
      canSelectAll,
      canSelect = can,
      canEdit = can,
      canDelete = can,
    } = this.props;
    const allSelected = sameItems(selected, this.itemsToSelect());
    const anySelected = selected && selected.length > 0;
    const canSelectAny = anySelected || _.find(items, canSelect) != null;
    const canEditAny = _.find(items, canEdit) != null;
    const canDeleteAll = anySelected && _.every(selected, canDelete);
    return (
      <thead>
        <tr>
          { onSelect && canSelectAny && canSelectAll &&
            <th className={classNames('select-table-action', { selected: allSelected })}>
              <button onClick={this.onSelectAll} />
            </th>
          }
          { ((onDelete && canSelectAny) || canEditAny) &&
            <th className={classNames('delete-table-action', { inactive: !canDeleteAll })}>
              { onDelete && canSelectAny && (
                <button onClick={canDeleteAll ? this.onDelete : null} />
              )}
            </th>
          }
          { columns.map(this.renderColumn) }
        </tr>
      </thead>
    );
  }
}
