import _ from 'lodash';
import { formatUtcDate } from 'helpers/dates';

import styles from './resource-table.module.css';

export const columnProps = {
  field: PropTypes.string,
  render: PropTypes.func,
};

const can = () => true;

export default class ResourceTableRow extends PureComponent {
  static propTypes = {
    columns: PropTypes.arrayOf(PropTypes.shape(columnProps)).isRequired,
    item: PropTypes.shape({}).isRequired,
    isSelected: PropTypes.bool,
    isActive: PropTypes.bool,
    onActivate: PropTypes.func,
    onSelect: PropTypes.func,
    canSelect: PropTypes.func,
    onEdit: PropTypes.func,
    canEdit: PropTypes.func,
    wrapApiAction: PropTypes.func,
  };

  onActivate = () => this.props.onActivate(this.props.item);
  onSelect = () => this.props.onSelect(this.props.item);
  onEdit = () => this.props.onEdit(this.props.item);

  renderValue = (item, field, render) => {
    const value = item[field];
    const { wrapApiAction } = this.props;
    if (render) return render(value, item, field, { wrapApiAction });
    return _.isArray(value) ? _.join(value, ', ') : value;
  };

  render() {
    const {
      item, columns,
      isSelected, isActive,
      onActivate, onSelect, onEdit,
      canSelect = can,
      canEdit = can,
    } = this.props;
    return (
      <tr
        className={classNames(styles.row, {
          [styles.active]: isActive,
          'active-row': isActive,
        })}
      >
        { onSelect && (
          <td className={classNames('select-row-action', { selected: isSelected })}>
            { canSelect(item) && (
              <button onClick={this.onSelect} />
            )}
          </td>
        )}
        { onEdit && (
          <td className='edit-row-action'>
            { canEdit(item) && (
              <button onClick={this.onEdit} />
            )}
          </td>
        )}
        { columns.map(({ field, render, className }, idx) => (
          <td
            key={field || idx}
            className={classNames(className, 'content-cell', styles.contentCell, {
              [styles.dateCell]: render === formatUtcDate,
            })}
            onClick={onActivate ? this.onActivate : null}
          >
            { this.renderValue(item, field, render) }
          </td>
        ))}
      </tr>
    );
  }
}
