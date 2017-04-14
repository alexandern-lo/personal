import styles from './preview-table.module.css';

const PreviewTable = ({ className, children }) => (
  <div className={classNames(className, styles.tableContainer)}>
    <table className={styles.table}>
      <tbody>
        { children }
      </tbody>
    </table>
  </div>
);
export default PreviewTable;

PreviewTable.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

export const PreviewRow = ({ className, label, children }) => (
  <tr className={classNames(className, styles.row)}>
    <td className={styles.label}>{label}</td>
    <td>{children}</td>
  </tr>
);
PreviewRow.propTypes = {
  className: PropTypes.string,
  label: PropTypes.string,
  children: PropTypes.node,
};

export const PreviewSpan = ({ className, children }) => (
  <tr className={classNames(className, styles.row, styles.span)}>
    <td colSpan={2}>{children}</td>
  </tr>
);
PreviewSpan.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};
