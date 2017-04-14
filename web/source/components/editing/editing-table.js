import styles from './editing-table.module.css';

const EditingTable = ({ className, naked, children }) => (
  <table className={classNames(styles.table, className)}>
    { naked
      ? children
      : (
        <tbody>
          { children }
        </tbody>
      )
    }
  </table>
);
export default EditingTable;

EditingTable.propTypes = {
  className: PropTypes.string,
  naked: PropTypes.bool,
  children: PropTypes.node.isRequired,
};


export const Row = ({ className, name = '', label, required, children }) => (
  <tr className={classNames(styles.row, className)}>
    <td className={styles.label}>
      { label ? (
        <label htmlFor={name}>
          { label }
          { required ? '*' : null }
        </label>
      ) : null}
    </td>
    <td>
      { children }
    </td>
  </tr>
);

Row.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string,
  label: PropTypes.string,
  required: PropTypes.bool,
  children: PropTypes.node.isRequired,
};


export const RowSeparator = ({ className }) => (
  <Row className={classNames(styles.separatorRow, className)}>
    <div className={styles.separator} />
  </Row>
);

RowSeparator.propTypes = {
  className: PropTypes.string,
};

