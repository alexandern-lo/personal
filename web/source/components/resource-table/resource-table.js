import _ from 'lodash';

import Header, { columnProps as headerColumnProps } from './resource-table-header';
import Row, { columnProps as rowColumnProps } from './resource-table-row';

import styles from './resource-table.module.css';

const can = () => true;
const cannot = () => false;

export default class ResourceTable extends PureComponent {
  static propTypes = {
    tag: PropTypes.string,
    className: PropTypes.string,
    idKey: PropTypes.string.isRequired,
    columns: PropTypes.arrayOf(PropTypes.shape({
      ...headerColumnProps,
      ...rowColumnProps,
    })).isRequired,
    items: PropTypes.arrayOf(PropTypes.shape({})).isRequired,
    activeItem: PropTypes.shape({}),
    selected: PropTypes.arrayOf(PropTypes.shape({})),
    sortField: PropTypes.string,
    sortOrder: PropTypes.string,
    onSort: PropTypes.func,
    onActivate: PropTypes.func,
    canSelectAll: PropTypes.bool,
    canSelect: PropTypes.func,
    onSelect: PropTypes.func,
    onEdit: PropTypes.func,
    onDelete: PropTypes.func,
    canEdit: PropTypes.func,
    canDelete: PropTypes.func,
    wrapApiAction: PropTypes.func,
  };

  static defaultProps = {
    idKey: 'uid',
    canSelectAll: true,
  };

  onSelectItem = (item) => {
    const { selected = [] } = this.props;
    const idx = _.indexOf(selected, item);
    if (idx >= 0) {
      this.props.onSelect(_.without(selected, item));
    } else {
      this.props.onSelect(selected.concat([item]));
    }
  };

  onActivateItem = (item) => {
    const { activeItem, onActivate } = this.props;
    onActivate(item === activeItem ? null : item);
  };

  render() {
    const {
      tag, className, columns, items, idKey,
      activeItem, onActivate, onEdit, onDelete,
      selected, onSelect,
      sortField, sortOrder, onSort,
      canEdit = can,
      canDelete = can,
      canSelectAll,
      canSelect = canDelete,
      wrapApiAction,
    } = this.props;
    const activeKey = activeItem ? activeItem[idKey] : null;
    const isActive = activeKey ? (item => item[idKey] === activeKey) : () => false;
    const canSelectAny = onSelect && _.find(items, canSelect) != null;
    const canEditAny = onEdit && _.find(items, canEdit) != null;
    const canDeleteAny = onDelete && _.find(items, canDelete) != null;
    return (
      <table className={classNames(styles.table, className)}>
        <Header
          tag={tag}
          columns={columns}
          items={items}
          selected={selected}
          sortField={sortField}
          sortOrder={sortOrder}
          onSort={onSort}
          canSelectAll={canSelectAll}
          canSelect={canSelect}
          onSelect={onSelect}
          canEdit={onEdit ? canEdit : cannot}
          canDelete={canDelete}
          onDelete={onDelete}
        />
        <tbody>
          { items.map(item => (
            <Row
              tag={tag}
              key={item[idKey]}
              item={item}
              columns={columns}
              isActive={isActive(item)}
              isSelected={_.indexOf(selected, item) >= 0}
              onActivate={onActivate ? this.onActivateItem : null}
              canSelect={canSelect}
              onSelect={canSelectAny ? this.onSelectItem : null}
              canEdit={canEdit}
              onEdit={(canEditAny || canDeleteAny) ? onEdit : null}
              wrapApiAction={wrapApiAction}
            />
          ))}
        </tbody>
      </table>
    );
  }
}
