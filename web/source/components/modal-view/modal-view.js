import styles from './modal-view.module.css';

const ModalButton = ({ text, onClick, active = true }) => (
  <button
    onClick={(e) => { e.stopPropagation(); onClick(); }}
    disabled={!active}
  >
    {text}
  </button>
);

ModalButton.propTypes = {
  text: PropTypes.string.isRequired,
  onClick: PropTypes.func.isRequired,
  active: PropTypes.bool,
};

const ModalView = ({ className, title, onClose, children, leftButton, button }) => (
  <div className={classNames(className, styles.moire)} onClick={onClose}>
    <div className={styles.container}>
      <div className={styles.form} onClick={e => e.stopPropagation()}>
        <div className={styles.header} >
          {typeof title === 'string' ? title : title()}
          {onClose && <button className={styles.close} onClick={onClose} />}
        </div>
        {children}
      </div>
      {button && (
        <div className={styles.footer}>
          {leftButton && ModalButton(leftButton)}
          <div className={styles.placeholder} />
          {button && ModalButton(button)}
        </div>
      )}
    </div>
  </div>
);

export default ModalView;

ModalView.propTypes = {
  className: PropTypes.string,
  title: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.func,
  ]),
  onClose: PropTypes.func,
  children: PropTypes.node,
  leftButton: PropTypes.shape({
    text: PropTypes.string.isRequired,
    onClick: PropTypes.func.isRequired,
    active: PropTypes.bool,
  }),
  button: PropTypes.shape({
    text: PropTypes.string.isRequired,
    onClick: PropTypes.func.isRequired,
    active: PropTypes.bool,
  }),
};
