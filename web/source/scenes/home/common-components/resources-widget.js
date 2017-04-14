import { connect } from 'store';
import { navigate } from 'store/navigate';

import { getDashboardSummary } from 'store/home';
import { dashboardSummaryShape } from 'store/data/home';

import Spinner from 'components/loading-spinner/loading-spinner';

import styles from './resources-widget.module.css';

@connect({
  dashboardSummary: getDashboardSummary,
}, {
  navigate,
})
export default class ResourcesWidget extends Component {
  static propTypes = {
    dashboardSummary: dashboardSummaryShape,
    navigate: PropTypes.func,
    showFooter: PropTypes.bool,
  }

  getResourceIconClass = (type) => {
    if (type.match(/^image/)) {
      return styles.imageIcon;
    }

    switch (type) {
      case 'application/pdf':
        return styles.acrobatIcon;

      case 'application/msword':
        return styles.wordIcon;

      case 'application/vnd.openxmlformats-officedocument.wordprocessingml.document':
        return styles.wordIcon;

      case 'application/vnd.ms-excel':
        return styles.excelIcon;

      case 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet':
        return styles.excelIcon;

      case 'application/vnd.ms-powerpointtd>':
        return styles.powerPointIcon;

      case 'application/vnd.openxmlformats-officedocument.presentationml.presentation':
        return styles.powerPointIcon;

      case 'application/zip':
        return styles.archiveIcon;

      case 'pplication/x-rar-compressed':
        return styles.archiveIcon;

      default: break;
    }

    return styles.unknownIcon;
  }

  renderTableRow = resource => (
    <tr key={resource.uid}>
      <td className={classNames(styles.resourceInfoCell, this.getResourceIconClass(resource.type))}>
        <span>{resource.name}</span>
      </td>
      <td className={styles.sentOpenedCell}>
        <span>{resource.sentCount}</span>
        <span>{resource.openedCount}</span>
      </td>
    </tr>
  )

  renderFooter = () => (
    this.props.showFooter && (
      <div className={styles.footer} onClick={() => { this.props.navigate('/resources'); }}>
        <span>Show all resources</span>
        <span>&nbsp;▶</span>
      </div>
    )
  )

  render() {
    const { dashboardSummary } = this.props;
    if (!dashboardSummary) {
      return (
        <div className={styles.spinnerContainer}>
          <Spinner className={styles.spinner} />
        </div>
      );
    }

    const { resources = [] } = dashboardSummary;

    return (
      <div className={styles.rootContainer}>
        <table>
          <tbody>
            <tr>
              <th className={styles.resourceInfoCell}>
                Resource conversions
              </th>
              <th className={styles.sentOpenedCell}>
                Sent / Opened
              </th>
            </tr>
            {resources.map(r => this.renderTableRow(r))}
          </tbody>
          {this.renderFooter()}
        </table>
      </div>
    );
  }
}
