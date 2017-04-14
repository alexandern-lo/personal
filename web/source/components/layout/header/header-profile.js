import { profileShape, isSuperAdmin, isTenantAdmin } from 'store/data/profile';
import { getDisplayName } from 'store/data/user';

import Moire from '../moire';
import HeaderMenu from './header-menu';

import styles from './header.module.css';

export default class HeaderProfile extends Component {
  static propTypes = {
    className: PropTypes.string,
    profile: profileShape.isRequired,
    onLogout: PropTypes.func.isRequired,
    onEditProfile: PropTypes.func,
    onEditAccountSettings: PropTypes.func,
  };

  onToggleMenu = (e) => {
    if (e) e.stopPropagation();
    this.setState(({ displayMenu }) => ({ displayMenu: !displayMenu }));
  };

  onLogout = (e) => {
    e.preventDefault();
    this.props.onLogout();
  };

  onEditProfile = (e) => {
    e.preventDefault();
    this.props.onEditProfile();
  };

  onAccountSettingsClick = (e) => {
    e.preventDefault();
    this.props.onEditAccountSettings();
  };

  getUserName = () => {
    const { profile } = this.props;
    if (isSuperAdmin(profile)) return 'Admin';
    return getDisplayName(profile.user);
  };

  getRole = () => {
    const { profile } = this.props;
    if (isSuperAdmin(profile)) return null;
    if (isTenantAdmin(profile)) return 'Tenant Admin';
    return 'Seat User';
  };

  render() {
    const { className, onEditAccountSettings, onEditProfile } = this.props;
    const { displayMenu } = this.state || {};
    const userName = this.getUserName();
    const role = this.getRole();
    return (
      <div
        className={classNames(className, styles.profile)}
        onClick={this.onToggleMenu}
      >
        <ul>
          <li className={styles.profileName}>{userName}</li>
          <li className={styles.profileRole}>{role}</li>
        </ul>
        {displayMenu && (
          <div>
            <Moire onClick={this.onToggleMenu} />
            <HeaderMenu className={styles.profileMenu}>
              {onEditAccountSettings &&
                <button onClick={this.onAccountSettingsClick}>Account Settings</button>
              }
              {onEditProfile &&
                <button onClick={this.onEditProfile}>Edit Profile</button>
              }
              <button onClick={this.onLogout}>Sign out</button>
            </HeaderMenu>
          </div>
        )}
      </div>
    );
  }
}
