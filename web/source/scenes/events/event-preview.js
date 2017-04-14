import { formatUtcDateInterval } from 'helpers/dates';
import { eventShape, isConference, isPersonal } from 'store/data/event';

import PreviewPanel from 'components/preview/preview-panel';
import PreviewTable from 'components/preview/preview-table';
import {
  GroupRow,
  TextRow,
  ImageRow,
  UrlRow,
} from 'components/preview/preview-rows';

import styles from './event-preview.module.css';

const MenuButton = ({ className, onClick, children }) => (
  <div className={classNames(className, styles.menuButton)} onClick={onClick}>
    <div className={styles.icon} />
    <span>{children}</span>
  </div>
);
MenuButton.propTypes = {
  className: PropTypes.string,
  onClick: PropTypes.func.isRequired,
  children: PropTypes.node,
};

export default class EventPreview extends Component {
  static propTypes = {
    className: PropTypes.string,
    isSuperAdmin: PropTypes.bool,
    event: eventShape.isRequired,
    onClose: PropTypes.func.isRequired,
    onEdit: PropTypes.func,
    onDelete: PropTypes.func,
    onNavigate: PropTypes.func,
  };

  onEdit = () => this.props.onEdit(this.props.event);
  onDelete = () => this.props.onDelete(this.props.event);

  onNavigate = entity => this.props.onNavigate(`/events/${this.props.event.uid}/${entity}`);

  onAttendeesClick = () => this.onNavigate('attendees');
  onAgendaClick = () => this.onNavigate('agenda_items');
  onQuestionsClick = () => this.onNavigate('questions');
  onUsersClick = () => this.onNavigate('users');
  onCategoriesClick = () => this.onNavigate('categories');

  renderMenu = () => {
    const { event, isSuperAdmin } = this.props;
    const conference = isConference(event);
    return (
      <div className={styles.menu}>
        {conference && !event.detailsOnly &&
          <MenuButton className={styles.attendees} onClick={this.onAttendeesClick}>
            Attendees
          </MenuButton>
        }
        {conference && !event.detailsOnly &&
          <MenuButton className={styles.agenda} onClick={this.onAgendaClick}>
            Agenda
          </MenuButton>
        }
        <MenuButton className={styles.questions} onClick={this.onQuestionsClick}>
          Questions
        </MenuButton>
        {!event.detailsOnly &&
          <MenuButton className={styles.users} onClick={this.onUsersClick}>
            Users
          </MenuButton>
        }
        {isSuperAdmin && conference && (
          <MenuButton className={styles.categories} onClick={this.onCategoriesClick}>
            Categories
          </MenuButton>
        )}
      </div>
    );
  };

  render() {
    const { className, isSuperAdmin, event, onClose, onEdit, onDelete } = this.props;
    const showTenant = isPersonal(event) && isSuperAdmin;
    return (
      <PreviewPanel
        className={className}
        title={event.name}
        onClose={onClose}
        onEdit={onEdit ? this.onEdit : null}
        onDelete={onDelete ? this.onDelete : null}
        topPanel={this.renderMenu()}
      >
        <PreviewTable>
          <ImageRow imageUrl={event.logo} />
          <GroupRow label='General' />
          <TextRow label='Type' text={event.type} />
          {showTenant &&
            <TextRow label='Tenant' text={event.tenant} />
          }
          <TextRow label='Industry' text={event.industry} />
          <TextRow label='Dates' text={formatUtcDateInterval(event.startDate, event.endDate)} />
          <GroupRow label='Location' />
          <TextRow label='Address' text={event.address} />
          <TextRow label='City' text={event.city} />
          <TextRow label='State' text={event.state} />
          <TextRow label='Country' text={event.country} />
          <GroupRow label='Contacts' />
          <UrlRow label='Website' url={event.url} />
        </PreviewTable>
      </PreviewPanel>
    );
  }
}
