import _ from 'lodash';
import { SortableContainer } from 'react-sortable-hoc';

import Header, { columnProps as headerColumnProps } from './sortable-table-header';
import Row, { columnProps as rowColumnProps } from './sortable-table-row';

import styles from './sortable-table.module.css';

@SortableContainer
export default class SortableTable extends Component {
  static propTypes = {
    className: PropTypes.string,
    idKey: PropTypes.string.isRequired,
    indexKey: PropTypes.string.isRequired,
    columns: PropTypes.arrayOf(PropTypes.shape({
      ...headerColumnProps,
      ...rowColumnProps,
    })).isRequired,
    items: PropTypes.arrayOf(PropTypes.shape({})).isRequired,
    activeItem: PropTypes.shape({}),
    selected: PropTypes.arrayOf(PropTypes.shape({})),
    // active: PropTypes.object,
    onActivate: PropTypes.func,
    onSelect: PropTypes.func,
    onSelectAll: PropTypes.func,
    onEdit: PropTypes.func,
    onDelete: PropTypes.func,
  };

  static defaultProps = {
    idKey: 'uid',
    indexKey: 'position',
  };

  renderHeader = () => {
    const { columns, items, selected, onDelete,
      onSelectAll } = this.props;
    return (
      <Header
        items={items}
        columns={columns}
        onSelectAll={onSelectAll}
        onDelete={onDelete}
        allSelected={selected.length > 0 && _.difference(items, selected).length === 0}
      />
    );
  };

  render() {
    const { className, columns, items, idKey, activeItem, onActivate, onEdit,
      onSelect, selected, indexKey } = this.props;
    const activeKey = activeItem ? activeItem[idKey] : null;
    const isActive = activeKey ? (item => item[idKey] === activeKey) : () => false;
    return (
      <table className={classNames(styles.table, className)}>
        { this.renderHeader() }
        <tbody>
          { items.map(item => (
            <Row
              key={item[idKey]}
              index={item[indexKey]}
              item={item}
              columns={columns}
              isActive={isActive(item)}
              isSelected={_.indexOf(selected, item) >= 0}
              onActivate={onActivate}
              onEdit={onEdit}
              onSelect={onSelect}
            />
          ))}
        </tbody>
      </table>
    );
  }
}
