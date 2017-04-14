import { connectActions } from 'store';
import { redirect } from 'store/navigate';

import { leadDisplayName, makeBlankLead } from 'store/data/lead';
import { createLead } from 'store/leads/actions';

import Breadcrumbs from 'components/sub-header/breadcrumbs';
import EditForm from './lead-edit-form';

@connectActions({
  createLead,
  redirect,
})
export default class LeadCreatePage extends Component {
  static propTypes = {
    createLead: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onCreate = data => this.props.createLead(data);

  onSuccess = (r, d, { values: lead }) => this.props.redirect({
    pathname: '/leads',
    state: { created: leadDisplayName(lead) },
  });

  onCancel = () => this.props.redirect('/leads');

  render() {
    return (
      <EditForm
        title={<Breadcrumbs lead action='Create new lead' />}
        actionTitle='Create'
        initialValues={makeBlankLead()}
        onCancel={this.onCancel}
        onSubmit={this.onCreate}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}
