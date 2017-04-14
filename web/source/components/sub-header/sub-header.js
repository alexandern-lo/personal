import styles from './sub-header.module.css';

const SubHeader = ({ children }) => (
  <div className={styles.header}>
    { children }
  </div>
);
export default SubHeader;

SubHeader.propTypes = {
  children: PropTypes.node,
};
