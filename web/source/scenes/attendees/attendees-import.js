import HeaderMenu from 'components/layout/header/header-menu';
import { ImportButton } from 'components/sub-header/buttons';

import styles from './attendees-import.module.css';

export default class AttendeesImport extends Component {
  static propTypes = {
    onImportFromCSV: PropTypes.func.isRequired,
    onImportFromEventCore: PropTypes.func.isRequired,
  };

  onRef = container => (this.container = container);

  onBlur = (event) => {
    if (!this.container.contains(event.relatedTarget)) {
      this.setState({ showMenu: false });
    }
  };

  onToggleMenu = () => this.setState(({ showMenu }) => ({ showMenu: !showMenu }));

  onImportFromCSV = () => {
    this.onToggleMenu();
    this.props.onImportFromCSV();
  };

  onImportFromEventCore = () => {
    this.onToggleMenu();
    this.props.onImportFromEventCore();
  }

  renderMenu = () => (
    <HeaderMenu>
      <button onClick={this.onImportFromCSV}>Import from CSV</button>
      <button onClick={this.onImportFromEventCore}>Import from EventCore</button>
    </HeaderMenu>
  );

  render() {
    const { showMenu } = this.state || {};
    return (
      <div
        className={styles.importButtonContainer}
        ref={this.onRef}
        onBlur={this.onBlur}
      >
        <ImportButton onClick={this.onToggleMenu}>
          Import attendees list
        </ImportButton>
        { showMenu && this.renderMenu() }
      </div>
    );
  }
}
