import { crmConfigShape, getAuthorizationUrl } from 'store/data/crm-config';

import HeaderMenu from 'components/layout/header/header-menu';
import { ExportButton } from 'components/sub-header/buttons';

import styles from './leads-export.module.css';

export default class LeadsExport extends Component {
  static propTypes = {
    count: PropTypes.number,
    crmConfig: crmConfigShape,
    onExportToFile: PropTypes.func.isRequired,
    onSetupCRM: PropTypes.func.isRequired,
    onExportToCRM: PropTypes.func.isRequired,
  };

  onRef = container => (this.container = container);

  onBlur = (event) => {
    if (!this.container.contains(event.relatedTarget)) {
      this.setState({ showMenu: false });
    }
  };

  onToggleMenu = () => this.setState(({ showMenu }) => ({ showMenu: !showMenu }));

  onExportToCSV = () => {
    this.onToggleMenu();
    this.props.onExportToFile('csv');
  };

  onExportToExcel = () => {
    this.onToggleMenu();
    this.props.onExportToFile('excel');
  };

  onSetupCRM = () => {
    this.onToggleMenu();
    this.props.onSetupCRM();
  };

  onExportToCRM = () => {
    this.onToggleMenu();
    this.props.onExportToCRM();
  };

  onAuthorizeCRM = () => (window.location = getAuthorizationUrl(this.props.crmConfig));

  getExportMessage = () => {
    const { count } = this.props;
    if (count > 0) {
      return `Export ${count} lead${count > 1 ? 's' : ''}`;
    }
    return 'Export all leads';
  };

  renderCrmButton = () => {
    const { crmConfig } = this.props;
    if (!crmConfig) {
      return <button onClick={this.onSetupCRM}>Setup CRM account</button>;
    }
    const { name, authorized } = crmConfig;
    if (!authorized) {
      return <button onClick={this.onAuthorizeCRM}>Authorize {name}</button>;
    }
    return <button onClick={this.onExportToCRM}>Export to {name}</button>;
  };

  renderMenu = () => (
    <HeaderMenu>
      <button onClick={this.onExportToCSV}>Export to CSV</button>
      <button onClick={this.onExportToExcel}>Export to Excel</button>
      { this.renderCrmButton() }
    </HeaderMenu>
  );

  render() {
    const { showMenu } = this.state || {};
    return (
      <div
        className={styles.exportButtonContainer}
        ref={this.onRef}
        onBlur={this.onBlur}
      >
        <ExportButton onClick={this.onToggleMenu}>
          { this.getExportMessage() }
        </ExportButton>
        { showMenu && this.renderMenu() }
      </div>
    );
  }
}
