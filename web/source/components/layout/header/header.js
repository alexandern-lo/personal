import { connect } from 'store';
import { navigate } from 'store/navigate';
import { Link } from 'react-router';

import { getProfile } from 'store/auth';
import { profileShape, isTenantAdmin, isSuperAdmin } from 'store/data/profile';
import { getSearchTerm } from 'store/search';

import { logout } from 'store/auth/actions';
import { search } from 'store/search/actions';

import HeaderSearch from './header-search';
import HeaderProfile from './header-profile';

import styles from './header.module.css';

@connect({
  profile: getProfile,
  searchTerm: getSearchTerm,
}, {
  search,
  logout,
  navigate,
})
export default class Header extends Component {
  static propTypes = {
    className: PropTypes.string,
    profile: profileShape.isRequired,
    searchTerm: PropTypes.string,
    onToggleSidebar: PropTypes.func.isRequired,
    onToggleHelp: PropTypes.func.isRequired,
    search: PropTypes.func.isRequired,
    logout: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
  };

  onSearch = (query) => {
    this.props.search(query);
    this.props.navigate('/search');
  };

  onSettingsClick = () => this.props.navigate('/crm');

  onNotificationsClick = () => {};

  onEditProfile = () => this.props.navigate('/profile');

  onAccountSettingsClick = () => this.props.navigate('/account_settings');

  render() {
    const { className, profile, searchTerm, onToggleSidebar, onToggleHelp } = this.props;
    const isTA = isTenantAdmin(profile);
    const isSA = isSuperAdmin(profile);
    return (
      <div className={classNames(className, styles.header)}>
        <div className={styles.brand}>
          <button onClick={onToggleSidebar}>X</button>
          <Link to='/'>AVEND</Link>
        </div>
        <HeaderSearch searchTerm={searchTerm} onSearch={this.onSearch} />
        <HeaderButton
          className={styles.notifications}
          onClick={this.onNotificationsClick}
          text='Notifications'
        />
        <div className={styles.separator} />
        <HeaderButton
          className={styles.settings}
          onClick={this.onSettingsClick}
          text='Settings'
        />
        <div className={styles.separator} />
        <HeaderButton
          className={styles.help}
          onClick={onToggleHelp}
          text='Help'
        />
        <HeaderProfile
          profile={profile}
          onEditAccountSettings={isTA ? this.onAccountSettingsClick : null}
          onEditProfile={!isSA ? this.onEditProfile : null}
          onLogout={this.props.logout}
        />
      </div>
    );
  }
}

const HeaderButton = ({ className, onClick, text }) => (
  <div className={classNames(className, styles.button)}>
    <button onClick={onClick}>
      { text }
    </button>
  </div>
);
HeaderButton.propTypes = {
  className: PropTypes.string,
  onClick: PropTypes.func.isRequired,
  text: PropTypes.string,
};
