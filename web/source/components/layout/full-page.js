import styles from './full-page.module.css';

const FullPage = ({ className, children }) => (
  <div>
    <div className={classNames(className, styles.container)}>
      { children }
    </div>
  </div>
);
export default FullPage;

FullPage.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node.isRequired,
};
