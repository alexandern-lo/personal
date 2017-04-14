import styles from './sortable-table.module.css';

const ResourceTableRow = ({
  columns,
  // anySelected,
  allSelected,
  onSelectAll,
  onDelete,
}) => (
  <thead>
    <tr>
      <th className={styles.dragHandle} />
      { onSelectAll &&
        <th
          onClick={onSelectAll}
          className={classNames('select-table-action', { selected: allSelected })}
        >
          <button />
        </th>
      }
      { onDelete &&
        <th
          onClick={onDelete}
          className={classNames('delete-table-action', {
            // inactive: !anySelected,
          })}
        >
          { <button /> }
        </th>
      }
      { columns.map(({ field, label }) => (
        <th
          key={field}
          className={classNames('content-cell')}
        >
          {label}
        </th>
      ))}
    </tr>
  </thead>
);
export default ResourceTableRow;

export const columnProps = {
  label: PropTypes.string,
  field: PropTypes.string,
};

ResourceTableRow.propTypes = {
  columns: PropTypes.arrayOf(PropTypes.shape(columnProps)).isRequired,
  // anySelected: PropTypes.bool,
  allSelected: PropTypes.bool,
  onSelectAll: PropTypes.func,
  onDelete: PropTypes.func,
};
