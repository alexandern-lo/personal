import styles from './buttons.module.css';

export const AddButton = ({ className, children, ...props }) => (
  <button
    className={classNames(className, styles.addButton)}
    {...props}
  >
    { children || 'Add new' }
  </button>
);
AddButton.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

export const RemoveButton = ({ className, ...props }) => (
  <button
    className={classNames(className, styles.removeButton)}
    {...props}
  >
    remove
  </button>
);
RemoveButton.propTypes = {
  className: PropTypes.string,
};

export const DeleteButton = ({ className, children, ...props }) => (
  <button
    className={classNames(className, styles.deleteButton)}
    {...props}
  >
    { children || 'Delete' }
  </button>
);
DeleteButton.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};
