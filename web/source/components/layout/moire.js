import styles from './moire.module.css';

const Moire = ({ className, children, ...props }) => (
  <div className={classNames(className, styles.moire)} {...props}>
    { children }
  </div>
);
export default Moire;

Moire.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};
