import styles from './loading-spinner.module.css';

const LoadingSpinner = ({ className, children }) => (
  <div className={classNames(className, styles.container)}>
    <span className={styles.loader} />
    <div className={styles.message}>{children}</div>
  </div>
);
export default LoadingSpinner;

LoadingSpinner.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

LoadingSpinner.defaultProps = {
  children: 'Please wait...',
};
