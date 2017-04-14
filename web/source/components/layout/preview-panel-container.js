import styles from './preview-panel-container.module.css';

const PreviewPanelContainer = ({ className, children, previewPanel }) => (
  <div className={classNames(styles.wrapper, className)}>
    <div className={styles.content}>
      { children }
    </div>
    <div className={styles.previewPanel}>
      { previewPanel }
    </div>
  </div>
);
export default PreviewPanelContainer;

PreviewPanelContainer.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  previewPanel: PropTypes.node,
};
