import ProgressIndicator from 'components/loading-spinner/loading-spinner';
import styles from './basic-dialog.module.css';

const BasicDialog = ({ className, iconClass, message, onOk, onDelete, onCancel, loading }) => (
  <div className={classNames(styles.moire, className)}>
    {loading && <ProgressIndicator />}
    <div className={styles.container}>
      <div className={classNames(styles.icon, iconClass)} />
      <div className={styles.message}>{message}</div>
      <div className={styles.actions}>
        {onOk &&
        <button className={styles.okButton} onClick={onOk}>OK</button>}
        {onDelete &&
        <button className={styles.deleteButton} onClick={onDelete}>Delete</button>}
        {onCancel &&
        <button className={styles.cancelButton} onClick={onCancel}>Cancel</button>}
      </div>
    </div>
  </div>
);

export default BasicDialog;

BasicDialog.propTypes = {
  className: PropTypes.string,
  iconClass: PropTypes.string.isRequired,
  message: PropTypes.string.isRequired,
  onOk: PropTypes.func,
  onDelete: PropTypes.func,
  onCancel: PropTypes.func,
  loading: PropTypes.bool,
};
