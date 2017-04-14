import { ellipsis } from 'helpers/strings';
import { crmConfigShape } from 'store/data/crm-config';
import { eventShape } from 'store/data/event';
import { leadShape, leadDisplayName } from 'store/data/lead';
import { resourceShape } from 'store/data/resource';
import { agendaShape } from 'store/data/agenda';
import { categoryShape } from 'store/data/category';
import { questionShape } from 'store/data/question';
import { attendeeShape } from 'store/data/attendee';
import { userShape } from 'store/data/user';

import { Link } from 'react-router';

import styles from './sub-header.module.css';

const crmConfigName = crmConfig => (crmConfig && crmConfig !== true ? crmConfig.name : null);
const eventName = event => (event && event !== true ? event.name : null);
const leadName = lead => (lead && lead !== true ? leadDisplayName(lead) : null);
const resourceName = res => (res && res !== true ? res.name : null);
const agendaName = agenda => (agenda && agenda !== true ? agenda.name : null);
const categoryName = category => (category && category !== true ? category.name : null);
const questionName = question => (question && question !== true ? question.text : null);
const attendeeName = attendee => (
  attendee && attendee !== true ? `${attendee.firstName} ${attendee.lastName}` : null
);
const userName = user => (user && user !== true ? user.email : null);

export default class Breadcrumbs extends Component {
  static propTypes = {
    crmConfig: PropTypes.oneOfType([
      PropTypes.bool,
      crmConfigShape,
    ]),
    event: PropTypes.oneOfType([
      PropTypes.bool,
      eventShape,
    ]),
    lead: PropTypes.oneOfType([
      PropTypes.bool,
      leadShape,
    ]),
    resource: PropTypes.oneOfType([
      PropTypes.bool,
      resourceShape,
    ]),

    agenda: PropTypes.oneOfType([
      PropTypes.bool,
      agendaShape,
    ]),
    category: PropTypes.oneOfType([
      PropTypes.bool,
      categoryShape,
    ]),
    question: PropTypes.oneOfType([
      PropTypes.bool,
      questionShape,
    ]),
    attendee: PropTypes.oneOfType([
      PropTypes.bool,
      attendeeShape,
    ]),
    eventUser: PropTypes.oneOfType([
      PropTypes.bool,
      userShape,
    ]),
    eventUserInvite: PropTypes.oneOfType([
      PropTypes.bool,
      userShape,
    ]),
    action: PropTypes.string,
  };

  firstChunk = (url, list, name, hasSecond) => {
    const { action } = this.props;
    const chunk = [];
    if (name || action || hasSecond) {
      chunk.push(<Link key='flink' to={url}>{list}</Link>);
    } else {
      chunk.push(<span key='flist'>{list}</span>);
    }
    if (name) chunk.push(<span key='fname'>{ellipsis(name, 15)}</span>);
    return chunk;
  };

  firstLine = (hasSecond) => {
    const { crmConfig, event, lead, resource } = this.props;
    if (crmConfig) {
      return this.firstChunk('/crm', 'CRM Configuration', crmConfigName(crmConfig), hasSecond);
    }
    if (event) {
      return this.firstChunk('/events', 'Events', eventName(event), hasSecond);
    }
    if (lead) {
      return this.firstChunk('/leads', 'Leads', leadName(lead), hasSecond);
    }
    if (resource) {
      return this.firstChunk('/resources', 'Resources', resourceName(resource), hasSecond);
    }

    return [];
  };

  prefix = () => {
    const { event, lead } = this.props;
    if (event) return `/events/${event.uid}`;
    if (lead) return `/leads/${lead.uid}`;
    return '';
  };

  secondChunk = (url, list, name) => {
    const { action } = this.props;
    const chunk = [];
    if (name || action) {
      chunk.push(<Link key='slink' to={`${this.prefix()}${url}`}>{list}</Link>);
    } else {
      chunk.push(<span key='slist'>{list}</span>);
    }
    if (name) chunk.push(<span key='sname'>{ellipsis(name, 15)}</span>);
    return chunk;
  };

  secondLine = () => {
    const { agenda, category, question, attendee, eventUser, eventUserInvite } = this.props;
    if (agenda) {
      return this.secondChunk('/agenda_items', 'Agenda', agendaName(agenda));
    }
    if (category) {
      return this.secondChunk('/categories', 'Categories', categoryName(category));
    }
    if (question) {
      return this.secondChunk('/questions', 'Questionnaire', questionName(question));
    }
    if (attendee) {
      return this.secondChunk('/attendees', 'Attendees', attendeeName(attendee));
    }
    if (eventUser) {
      return this.secondChunk('/users', 'Users', userName(attendee));
    }
    if (eventUserInvite) {
      return this.secondChunk('/users/invite', 'Invite users', userName(attendee));
    }
    return [];
  }

  render() {
    const { action } = this.props;
    const second = this.secondLine();
    const first = this.firstLine(second.length > 0);
    return (
      <div className={styles.breadcrumbs}>
        { first }
        { second }
        <span>{action}</span>
      </div>
    );
  }
}
