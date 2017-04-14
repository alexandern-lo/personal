import Link from 'react-router/lib/Link';

import styles from './sidebar.module.css';

const menuItem = (name, url, className, { index } = {}) => (
  <li className={className}>
    <Link to={url} onlyActiveOnIndex={index} activeClassName={styles.active}>
      <span>{ name }</span>
    </Link>
  </li>
);

const Sidebar = ({ className, collapsed, isSuperAdmin }) => (
  <div className={classNames(className, styles.container)}>
    <div className={classNames(styles.sidebar, { [styles.collapsed]: collapsed })}>
      <ul>
        { menuItem('Home', '/', styles.home, { index: true }) }
        { menuItem('Events', '/events', styles.events) }
        { menuItem('Leads', '/leads', styles.leads) }
        { isSuperAdmin && menuItem('Users', '/users', styles.users) }
        { menuItem('Resources', '/resources', styles.resources) }
        { isSuperAdmin && menuItem('Mobile app settings', '/mobile_settings', styles.mobile) }
      </ul>
    </div>
    <div className={classNames(styles.placeholder, { [styles.collapsed]: collapsed })} />
  </div>
);
export default Sidebar;

Sidebar.propTypes = {
  className: PropTypes.string,
  collapsed: PropTypes.bool,
  isSuperAdmin: PropTypes.bool,
};
