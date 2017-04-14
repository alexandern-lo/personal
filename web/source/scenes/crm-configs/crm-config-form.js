import { reduxForm, validate, Field, FormSection } from 'components/forms';

import { SALESFORCE_TYPE, DYNAMICS365_TYPE } from 'store/data/crm-config';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row } from 'components/editing/editing-table';
import { InputRow, ErrorsRow } from 'components/editing/editing-rows';
import CheckBoxField from 'components/forms/checkbox-field';

import styles from './crm-config-form.module.css';

const fields = {
  name: {
    required: true,
  },
};

@reduxForm({
  form: 'edit-crm-config',
  ...validate(fields, { validateOnBlur: true }),
})
// eslint-disable-next-line react/prefer-stateless-function
export default class CrmConfigEditForm extends PureComponent {
  render() {
    return (
      <EditingForm {...this.props}>
        <EditingTable>
          <Row className={styles.crmSystem} label='CRM system'>
            <Field
              name='type'
              component={RenderCrmTypeField} // eslint-disable-line no-use-before-define
            />
          </Row>
          <ErrorsRow name='type' />

          <InputRow name='name' label='Name' required />
          <ErrorsRow name='name' />

          <InputRow name='url' label='URL' />
          <ErrorsRow name='url' />

          <FormSection name='sync_fields'>
            <Row className={styles.syncFields} label='Sync fields'>
              <CheckBoxField name='first_name' label='First name' />
              <CheckBoxField name='last_name' label='Last name' />
              <CheckBoxField name='company_name' label='Company name' />
              <CheckBoxField name='company_url' label='Company url' />
              <CheckBoxField name='job_title' label='Job title' />
              <CheckBoxField name='zip_code' label='Zip code' />
              <CheckBoxField name='address' label='Address' />
              <CheckBoxField name='city' label='City' />
              <CheckBoxField name='state' label='State' />
              <CheckBoxField name='country' label='Country' />
              <CheckBoxField name='qualification' label='Qualification' />
              <CheckBoxField name='email1' label='Email' />
              <CheckBoxField name='work_phone1' label='Work phone' />
              <CheckBoxField name='mobile_phone1' label='Mobile phone' />
              <CheckBoxField name='notes' label='Notes' />
            </Row>
          </FormSection>
          <ErrorsRow name='sync_fields' />

        </EditingTable>
      </EditingForm>
    );
  }
}

const crmButton = (type, className, value, onChange) => (
  <button
    className={classNames(styles.crmButton, className, { [styles.selected]: value === type })}
    onClick={() => onChange(type)}
  >
    { type }
  </button>
);

const RenderCrmTypeField = ({ input: { value, onChange } }) => (
  <div>
    { crmButton(SALESFORCE_TYPE, styles.salesforce, value, onChange) }
    { crmButton(DYNAMICS365_TYPE, styles.dynamics, value, onChange) }
  </div>
);
RenderCrmTypeField.propTypes = {
  input: PropTypes.shape({
    value: PropTypes.string,
    onChange: PropTypes.func.isRequired,
  }).isRequired,
};
