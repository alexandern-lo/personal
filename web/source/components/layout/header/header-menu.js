import styles from './header-menu.module.css';

const HeaderMenu = ({ className, children }) => (
  <ul className={classNames(className, styles.menu)}>
    { React.Children.map(children, (el, key) => (
      <li key={key}>{el}</li>
    ))}
  </ul>
);
export default HeaderMenu;

HeaderMenu.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};
