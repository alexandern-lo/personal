import { ellipsis } from 'helpers/strings';

import { DeleteButton } from 'components/forms/specialized/buttons';

import styles from './preview-panel.module.css';

export const PreviewButton = ({ className, children, ...props }) => (
  <button
    className={classNames(className, styles.button)}
    {...props}
  >
    { children }
  </button>
);
PreviewButton.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

const PreviewPanel = ({
  className,
  title,
  actions,
  onClose,
  onEdit,
  onDelete,
  topPanel,
  bottomPanel,
  children,
}) => (
  <div className={classNames(className, styles.container)}>
    <div className={styles.top}>
      <button className={styles.closeButton} onClick={onClose} />
      <div className={styles.header}>
        <span className={styles.title}>{ellipsis(title, 35)}</span>
        { actions }
        { onEdit && (
          <PreviewButton className={styles.editButton} onClick={onEdit}>
            edit
          </PreviewButton>
        )}
      </div>
    </div>
    <div className={styles.content}>
      { topPanel }
      { children }
      <div className={styles.contentActions}>
        { onDelete && <DeleteButton onClick={onDelete} /> }
      </div>
      { bottomPanel }
    </div>
  </div>
);
export default PreviewPanel;

PreviewPanel.propTypes = {
  className: PropTypes.string,
  title: PropTypes.string,
  actions: PropTypes.arrayOf(PropTypes.node),
  onClose: PropTypes.func.isRequired,
  onEdit: PropTypes.func,
  onDelete: PropTypes.func,
  topPanel: PropTypes.node,
  bottomPanel: PropTypes.node,
  children: PropTypes.node,
};
