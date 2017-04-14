import { reduxForm, validate } from 'components/forms';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row } from 'components/editing/editing-table';
import { InputRow, TextAreaRow, ErrorsRow } from 'components/editing/editing-rows';
import AutocompleteField from 'components/forms/autocomplete-field';

import { connect } from 'store';
import { redirect } from 'store/navigate';
import { inviteUser } from 'store/users/actions';
import { isSuperAdmin } from 'store/auth';
import { searchTenants } from 'store/tenants/actions';
import Breadcrumbs from 'components/sub-header/breadcrumbs';

const fields = {
  first_name: {
    required: true,
  },
  last_name: {
    required: true,
  },
  email: {
    required: true,
  },
  text: {
    required: true,
  },
};

const tenantsReader = data => ({ label: data.company_name, value: data.tenant_uid });

const UserInvitationForm = reduxForm({
  form: 'invite-user',
  initialValues: {
    text: 'Default invitation',
  },
  ...validate(fields, { validateOnBlur: true }),
})(props => (
  <EditingForm {...props}>
    <EditingTable>
      <InputRow name='first_name' label='First name' required />
      <ErrorsRow name='first_name' />
      <InputRow name='last_name' label='Last name' required />
      <ErrorsRow name='last_name' />
      <InputRow name='email' label='Email' required />
      <ErrorsRow name='email' />
      {props.superAdmin &&
        <Row label='Tenant' required>
          <AutocompleteField
            name='suid'
            placeholder='Select tenant...'
            fetcher={searchTenants}
            reader={tenantsReader}
          />
        </Row>
      }
      <TextAreaRow
        name='text'
        label='Invitation'
        required
      />
      <ErrorsRow name='text' />
    </EditingTable>
  </EditingForm>
));

@connect({
  superAdmin: isSuperAdmin,
}, {
  inviteUser,
  redirect,
})
export default class UserInvitationPage extends Component {
  static propTypes = {
    superAdmin: PropTypes.bool.isRequired,
    inviteUser: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  onCancel = () => this.props.redirect(this.redirectPathname());
  onInvite = data => this.props.inviteUser(data);
  onSuccess = user => this.props.redirect({
    pathname: this.redirectPathname(),
    state: { invited: user.data.email },
  });
  redirectPathname = () => (this.props.superAdmin ? '/users' : '/account_settings');

  render() {
    const { superAdmin } = this.props;
    return (
      <UserInvitationForm
        title={<Breadcrumbs action='Invite new user' />}
        actionTitle='Invite'
        onCancel={this.onCancel}
        onSubmit={this.onInvite}
        onSubmitSuccess={this.onSuccess}
        superAdmin={superAdmin}
      />
    );
  }
}
