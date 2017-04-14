import _ from 'lodash';
import { leadShape } from 'store/data/lead';

import PreviewPanel, { PreviewButton } from 'components/preview/preview-panel';
import PreviewTable, { PreviewRow } from 'components/preview/preview-table';
import {
  GroupRow,
  TextRow,
  ImageRow,
  UrlRow,
} from 'components/preview/preview-rows';

import PreviewQualification from './lead-preview-qualification';

import styles from './lead-preview.module.css';

export const renderEmail = ({ email, designation }, idx) => (
  <div key={idx}><a href={`mailto:${email}`}>{email}</a> ({designation})</div>
);
renderEmail.propTypes = {
  email: PropTypes.string.isRequired,
  designation: PropTypes.string.isRequired,
};

export const renderPhone = ({ phone, designation }, idx) => (
  <div key={idx}>{phone} ({designation})</div>
);
renderPhone.propTypes = {
  phone: PropTypes.string.isRequired,
  designation: PropTypes.string.isRequired,
};

const renderEvent = (event = {}) => event.name;

export default class LeadPreview extends Component {
  static propTypes = {
    className: PropTypes.string,
    lead: leadShape.isRequired,
    onClose: PropTypes.func.isRequired,
    onEdit: PropTypes.func,
    onDelete: PropTypes.func,
    onEditQuestionnaire: PropTypes.func,
    onChangeQualification: PropTypes.func,
  };

  onEdit = () => this.props.onEdit(this.props.lead);
  onEditQuestionnaire = () => this.props.onEditQuestionnaire(this.props.lead);

  onDelete = () => this.props.onDelete(this.props.lead);

  renderQualification = () => {
    const { lead, onChangeQualification } = this.props;
    return (
      <PreviewQualification
        lead={lead}
        onChangeQualification={onChangeQualification}
      />
    );
  };

  render() {
    const { className, lead, onClose, onEdit, onEditQuestionnaire, onDelete } = this.props;
    const moreActions = [];
    if (onEditQuestionnaire) {
      moreActions.push((
        <PreviewButton
          key='questionnaire'
          className={styles.questionnaireButton}
          onClick={this.onEditQuestionnaire}
        >
          Questionnaire
        </PreviewButton>
      ));
    }
    return (
      <PreviewPanel
        className={className}
        onClose={onClose}
        onDelete={onDelete ? this.onDelete : null}
        onEdit={onEdit ? this.onEdit : null}
        actions={moreActions}
        bottomPanel={this.renderQualification()}
      >
        <PreviewTable>
          <ImageRow imageUrl={lead.photoUrl} onClick={onEdit ? this.onEdit : null} />

          <GroupRow label='Personal' />
          <TextRow label='Event' text={renderEvent(lead.event)} />
          {lead.companyUrl
            ? <UrlRow label='Company' text={lead.companyName} url={lead.companyUrl} />
            : <TextRow label='Company' text={lead.companyName} />
          }
          <TextRow label='Job title' text={lead.jobTitle} />

          <GroupRow label='Contacts' />
          <PreviewRow label='Email'>{_.map(lead.emails, renderEmail)}</PreviewRow>
          <PreviewRow label='Phone'>{_.map(lead.phones, renderPhone)}</PreviewRow>

          <GroupRow label='Location' />
          <TextRow label='Address' text={lead.address} />
          <TextRow label='State' text={lead.state} />
          <TextRow label='City' text={lead.city} />
        </PreviewTable>
      </PreviewPanel>
    );
  }
}
