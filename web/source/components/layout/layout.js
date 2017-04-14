import { connect } from 'store';
import { isSuperAdmin } from 'store/auth';

import Header from 'components/layout/header/header';
import Sidebar from './sidebar';
import HelpPanel from './help-panel';

import styles from './layout.module.css';

@connect({
  isSA: isSuperAdmin,
})
export default class Layout extends PureComponent {
  static propTypes = {
    isSA: PropTypes.bool,
    children: PropTypes.node,
  };

  onToggleSidebar = () => this.setState(({ sidebarCollapsed }) => ({
    sidebarCollapsed: !sidebarCollapsed,
  }));

  onToggleHelp = () => this.setState(({ helpOpen }) => ({ helpOpen: !helpOpen }));

  render() {
    const { isSA, children } = this.props;
    const { sidebarCollapsed, helpOpen } = this.state || {};
    return (
      <div>
        <Header
          className={styles.header}
          onToggleSidebar={this.onToggleSidebar}
          onToggleHelp={this.onToggleHelp}
        />
        <div className={styles.pageContainer}>
          <Sidebar
            className={styles.sidebar}
            isSuperAdmin={isSA}
            collapsed={sidebarCollapsed}
          />
          <HelpPanel isOpened={helpOpen} onClose={this.onToggleHelp} />
          <div className={styles.contentContainer}>
            { children }
          </div>
        </div>
      </div>
    );
  }
}
