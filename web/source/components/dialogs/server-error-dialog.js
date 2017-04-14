import BasicDialog from './basic-dialog';
import styles from './server-error-dialog.module.css';

const ServerErrorDialog = ({ className, message, onOk }) => (
  <BasicDialog
    className={className}
    iconClass={styles.errorIcon}
    message={message}
    onOk={onOk}
  />
);

export default ServerErrorDialog;

ServerErrorDialog.propTypes = {
  className: PropTypes.string,
  message: PropTypes.string.isRequired,
  onOk: PropTypes.func.isRequired,
};
