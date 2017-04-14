import { reduxForm, validate } from 'components/forms';

import { INDUSTRIES } from 'store/data/event';
import { SIZES } from 'store/data/profile';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row } from 'components/editing/editing-table';

import CountryField from 'components/forms/specialized/country-field';
import StateField from 'components/forms/specialized/state-field';

import {
  ErrorsRow,
  InputRow,
  SelectRow,
  TextAreaRow,
} from 'components/editing/editing-rows';

import styles from './profile-form.module.css';

const fields = {
  'user.first_name': {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  'user.last_name': {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  'user.email': {
    required: true,
    email: true,
    minLength: 2,
    maxLength: 200,
  },
  'user.job_title': {
    minLength: 2,
    maxLength: 200,
  },
  'tenant.company_name': {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  'tenant.industry': {
    required: true,
  },
  'tenant.company_size': {
    minLength: 2,
    maxLength: 200,
  },
  'tenant.website_url': {
    minLength: 2,
    maxLength: 200,
  },
  'tenant.address': {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  'tenant.country': {
    required: true,
  },
  'tenant.city': {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  'tenant.zip_code': {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  'tenant.state': {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  'tenant.phone': {
    phone: true,
  },
  'tenant.description': {
    minLength: 2,
    maxLength: 200,
  },
};

@reduxForm({
  form: 'profile',
  ...validate(fields, { validateOnBlur: true }),
})
// eslint-disable-next-line react/prefer-stateless-function
export default class ProfileEditForm extends PureComponent {
  static propTypes = {
    onResetPassword: PropTypes.func.isRequired,
    disableCompanyDetails: PropTypes.bool,
  }

  render() {
    const { disableCompanyDetails } = this.props;
    return (
      <EditingForm {...this.props}>
        <div className={styles.column}>
          <div className={styles.moduleTitle}>
            User Details
          </div>
          <EditingTable>
            <InputRow label='First name' name='user.first_name' required />
            <ErrorsRow name='user.first_name' />

            <InputRow label='Last name' name='user.last_name' required />
            <ErrorsRow name='user.last_name' />

            <InputRow label='E-mail' name='user.email' required />
            <ErrorsRow name='user.email' />

            <InputRow label='Job title' name='user.job_title' />
            <ErrorsRow name='user.job_title' />
          </EditingTable>
          <div className={classNames(styles.moduleTitle, styles.passwordModule)}>
            Password
          </div>
          <div className={styles.passwordButton} onClick={this.props.onResetPassword}>
            Reset password
          </div>
        </div>
        <div className={classNames(styles.column, { [styles.disabled]: disableCompanyDetails })}>
          <div className={styles.moduleTitle}>
            <span>Company Details</span>
            {disableCompanyDetails &&
              <span className={styles.disabledTitleAddition}>
                &nbsp;(can be edited only by Tenant Admin)
              </span>}
          </div>
          <EditingTable>
            <InputRow
              label='Company name'
              name='tenant.company_name'
              required
              disabled={disableCompanyDetails}
            />
            <ErrorsRow name='tenant.company_name' />

            <SelectRow
              label='Industry'
              name='tenant.industry'
              placeholder='Choose industry'
              options={INDUSTRIES}
              disabled={disableCompanyDetails}
              required
            />
            <ErrorsRow name='tenant.industry' />

            <SelectRow
              label='Company size'
              name='tenant.company_size'
              placeholder='Choose size'
              disabled={disableCompanyDetails}
              options={SIZES}
            />
            <ErrorsRow name='tenant.company_size' />

            <InputRow
              label='Company website'
              name='tenant.website_url'
              disabled={disableCompanyDetails}
            />
            <ErrorsRow name='tenant.website_url' />

            <InputRow
              label='Address'
              name='tenant.address'
              required
              disabled={disableCompanyDetails}
            />
            <ErrorsRow name='tenant.address' />

            <Row label='Country' required>
              <CountryField
                name='tenant.country'
                stateFieldName='tenant.state'
                disabled={disableCompanyDetails}
              />
            </Row>
            <ErrorsRow name='tenant.country' />

            <InputRow
              label='City'
              name='tenant.city'
              required
              disabled={disableCompanyDetails}
            />
            <ErrorsRow name='tenant.city' />

            <InputRow
              label='Zipcode'
              name='tenant.zip_code'
              required
              disabled={disableCompanyDetails}
            />
            <ErrorsRow name='tenant.zip_code' />

            <Row label='State' required>
              <StateField
                name='tenant.state'
                countryFieldName='tenant.country'
                disabled={disableCompanyDetails}
              />
            </Row>
            <ErrorsRow name='tenant.state' />

            <InputRow
              label='Phone'
              name='tenant.phone'
              disabled={disableCompanyDetails}
            />
            <ErrorsRow name='tenant.phone' />

            <TextAreaRow
              label='Company description'
              name='tenant.description'
              maxLength={200}
              disabled={disableCompanyDetails}
            />
            <ErrorsRow name='tenant.description' />
          </EditingTable>
        </div>
      </EditingForm>
    );
  }
}
