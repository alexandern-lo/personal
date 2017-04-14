import styles from './buttons.module.css';

export const Button = ({ className, withIcon, wide, children, ...props }) => (
  <button
    className={classNames(className, styles.button, {
      [styles.withIcon]: withIcon,
      [styles.wide]: wide,
    })}
    {...props}
  >
    { children }
  </button>
);
Button.propTypes = {
  className: PropTypes.string,
  onClick: PropTypes.func.isRequired,
  withIcon: PropTypes.bool,
  wide: PropTypes.bool,
  children: PropTypes.node,
};

export const CreateButton = ({ className, ...props }) => (
  <Button
    className={classNames(className, styles.create)}
    {...props}
  />
);
CreateButton.propTypes = {
  className: PropTypes.string,
};

export const SaveButton = ({ className, ...props }) => (
  <Button
    className={classNames(className, styles.create)}
    {...props}
    wide
  />
);
SaveButton.propTypes = {
  className: PropTypes.string,
};

export const CancelButton = ({ ...props }) => <Button {...props} wide />;

export const ExportButton = ({ className, ...props }) => (
  <Button
    className={classNames(className, styles.export)}
    {...props}
    withIcon
  />
);
ExportButton.propTypes = {
  className: PropTypes.string,
};

export const ImportButton = ({ className, ...props }) => (
  <Button
    className={classNames(className, styles.import)}
    {...props}
    withIcon
  />
);
ImportButton.propTypes = {
  className: PropTypes.string,
};
