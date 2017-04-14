import BasicDialog from './basic-dialog';
import styles from './success-dialog.module.css';

const SuccessDialog = ({ className, message, onOk }) => (
  <BasicDialog
    className={className}
    iconClass={styles.successIcon}
    message={message}
    onOk={onOk}
  />
);

export default SuccessDialog;

SuccessDialog.propTypes = {
  className: PropTypes.string,
  message: PropTypes.string.isRequired,
  onOk: PropTypes.func.isRequired,
};
