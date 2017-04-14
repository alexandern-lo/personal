import { connect } from 'store';
import { redirect } from 'store/navigate';

import { getPinnedLead } from 'store/leads';
import { updateLead } from 'store/leads/actions';
import {
  leadShape,
  leadDisplayName,
  startEditLeadQuestionnaire,
  endEditLeadQuestionnaire,
} from 'store/data/lead';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import Form from './lead-questionnaire-form';

@connect({
  lead: getPinnedLead,
}, {
  updateLead,
  redirect,
})
export default class LeadQuestionnairePage extends Component {
  static propTypes = {
    lead: leadShape.isRequired,
    updateLead: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onUpdateLeadQuestionnaire = data => this.props.updateLead(endEditLeadQuestionnaire(data));

  onSuccess = () => this.props.redirect({
    pathname: '/leads',
    state: { updated: leadDisplayName(this.props.lead) },
  });

  onCancel = () => this.props.redirect('/leads');

  render() {
    const { lead } = this.props;
    return (
      <Form
        title={<Breadcrumbs lead={lead} action='questionnaire' />}
        actionTitle='Save'
        initialValues={startEditLeadQuestionnaire(lead)}
        onSubmit={this.onUpdateLeadQuestionnaire}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
