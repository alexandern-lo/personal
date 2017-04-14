import _ from 'lodash';
import { SortableElement, SortableHandle } from 'react-sortable-hoc';
import styles from './sortable-table.module.css';

const DragHandle = SortableHandle(() => <div />);

export const columnProps = {
  field: PropTypes.string,
  render: PropTypes.func,
};

@SortableElement
export default class SortableTableRow extends PureComponent {

  static propTypes = {
    columns: PropTypes.arrayOf(PropTypes.shape(columnProps)).isRequired,
    item: PropTypes.shape({}).isRequired,
    isSelected: PropTypes.bool,
    isActive: PropTypes.bool,
    onActivate: PropTypes.func,
    onSelect: PropTypes.func,
    onEdit: PropTypes.func,
  };

  onActivate = () => this.props.onActivate(this.props.item);
  onSelect = () => this.props.onSelect(this.props.item);
  onEdit = () => this.props.onEdit(this.props.item);

  renderValue = (item, field, render) => {
    const value = item[field];
    if (render) return render(value, item, field);
    if (!field) return null;
    return _.isArray(value) ? _.join(value, ', ') : value;
  }

  render() {
    const { item, columns, isSelected, isActive, onActivate, onSelect, onEdit } = this.props;
    return (
      <tr
        className={classNames(styles.row, {
          'active-row': isActive,
        })}
      >
        <td className={styles.dragHandle}><DragHandle /></td>
        { onSelect && (
          <td
            className={classNames('select-row-action', {
              selected: isSelected,
            })}
            onClick={this.onSelect}
          >
            <button />
          </td>
        )}
        { onEdit && (
          <td
            className='edit-row-action'
            onClick={this.onEdit}
          >
            <button />
          </td>
        )}
        { columns.map(({ field, render }, idx) => (
          <td
            key={field || idx}
            className='content-cell'
            onClick={onActivate ? this.onActivate : null}
          >
            { this.renderValue(item, field, render) }
          </td>
        ))}
      </tr>
    );
  }
}
