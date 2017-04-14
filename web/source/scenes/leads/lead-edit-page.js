import { connect } from 'store';
import { redirect } from 'store/navigate';

import { getPinnedLead } from 'store/leads';
import { updateLead } from 'store/leads/actions';

import {
  editLead,
  leadShape,
  leadDisplayName,
} from 'store/data/lead';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './lead-edit-form';

@connect({
  lead: getPinnedLead,
}, {
  updateLead,
  redirect,
})
export default class LeadEditPage extends Component {

  static propTypes = {
    lead: leadShape,
    updateLead: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onUpdate = data => this.props.updateLead(data);

  onSuccess = (r, d, { values: lead }) => this.props.redirect({
    pathname: '/leads',
    state: { updated: leadDisplayName(lead) },
  });

  onCancel = () => this.props.redirect('/leads');

  render() {
    const { lead } = this.props;
    return (
      <EditForm
        title={<Breadcrumbs lead={lead} action='edit' />}
        actionTitle='Save'
        ownerId={lead.ownerId}
        initialValues={editLead(lead)}
        onSubmit={this.onUpdate}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
