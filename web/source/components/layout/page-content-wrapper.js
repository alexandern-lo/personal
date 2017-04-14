import styles from './page-content-wrapper.module.css';

const PageContentWrapper = ({ className, children }) => (
  <div className={classNames(className, styles.wrapper)}>
    { children }
  </div>
);
export default PageContentWrapper;

PageContentWrapper.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};
