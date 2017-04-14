import styles from './message-panel.module.css';

export const MessagePanel = ({ className, message, onClear }) => (
  <div className={classNames(styles.panel, className)}>
    <span>{message}</span>
    <div className={styles.close}>
      <button onClick={onClear}>
        <i className='material-icons'>close</i>
      </button>
    </div>
  </div>
);

MessagePanel.propTypes = {
  className: PropTypes.string.isRequired,
  message: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.node,
  ]).isRequired,
  onClear: PropTypes.func.isRequired,
};

export const RedMessagePanel = ({ message, onClear }) =>
  <MessagePanel className={styles.red} message={message} onClear={onClear} />;

RedMessagePanel.propTypes = {
  message: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.node,
  ]).isRequired,
  onClear: PropTypes.func.isRequired,
};

export const BlueMessagePanel = ({ message, onClear }) =>
  <MessagePanel className={styles.blue} message={message} onClear={onClear} />;

BlueMessagePanel.propTypes = {
  message: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.node,
  ]).isRequired,
  onClear: PropTypes.func.isRequired,
};
