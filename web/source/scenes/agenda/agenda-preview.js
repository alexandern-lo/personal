import PreviewPanel from 'components/preview/preview-panel';
import PreviewTable, { PreviewRow } from 'components/preview/preview-table';
import {
  GroupRow,
  TextRow,
  DateRow,
  TimeRow,
} from 'components/preview/preview-rows';

import styles from './agenda-preview.module.css';

export default class AgendaPreview extends PureComponent {
  static propTypes = {
    agenda: PropTypes.shape({
      title: PropTypes.string,
      date: PropTypes.string,
      startTime: PropTypes.string,
      endTime: PropTypes.string,
      location: PropTypes.string,
      description: PropTypes.string,
    }),
    onClose: PropTypes.func,
    onEdit: PropTypes.func,
    onDelete: PropTypes.func,
  };

  onEdit = () => this.props.onEdit(this.props.agenda);

  onDelete = () => this.props.onDelete(this.props.agenda);

  render() {
    const { agenda, onClose, onEdit, onDelete } = this.props;
    return (
      <PreviewPanel
        title={agenda.name}
        onClose={onClose}
        onEdit={onEdit ? this.onEdit : null}
        onDelete={onDelete ? this.onDelete : null}
      >
        <PreviewTable>
          <GroupRow label='General' />
          <DateRow label='Date' date={agenda.date} />
          <TimeRow label='Start time' time={agenda.startTime} />
          <TimeRow label='End time' time={agenda.endTime} />
          <TextRow label='Website' text={agenda.website} />
          <GroupRow label='Location' />
          <TextRow label='Address' text={agenda.location} />
          <GroupRow label='' />
          <PreviewRow label='Description'>
            {agenda.description && (
              <div className={styles.description}>
                {agenda.description}
              </div>
            )}
          </PreviewRow>
        </PreviewTable>
      </PreviewPanel>
    );
  }
}
