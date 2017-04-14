import { reduxForm, Field, validate } from 'components/forms';

import _ from 'lodash';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row } from 'components/editing/editing-table';

import {
  ErrorsRow,
  InputRow,
  PictureRow,
} from 'components/editing/editing-rows';

import ErrorField from 'components/forms/error-field';
import GuardField from 'components/forms/guard-field';
import MultiselectSelect from 'components/forms/multiselect-field';
import ArrayField from 'components/forms/array-field';
import CountryField from 'components/forms/specialized/country-field';
import StateField from 'components/forms/specialized/state-field';

import styles from './attendee-edit-form.module.css';

const fields = {
  first_name: {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  last_name: {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  title: {
    minLength: 2,
    maxLength: 200,
  },
  company: {
    minLength: 2,
    maxLength: 200,
  },
  email: {
    email: true,
  },
  phone: {
    phone: true,
  },
  city: {
    minLength: 2,
    maxLength: 200,
  },
  state: {
    minLength: 2,
    maxLength: 200,
  },
  country: {
    minLength: 2,
    maxLength: 200,
  },
  zip_code: {
    minLength: 2,
    maxLength: 200,
  },
};

@reduxForm({
  form: 'edit-attendee',
  ...validate(fields, { validateOnBlur: true }),
})
// eslint-disable-next-line react/prefer-stateless-function
export default class AttendeesEditForm extends PureComponent {
  render() {
    return (
      <EditingForm {...this.props}>
        <EditingTable>
          <PictureRow className={styles.imageRow} label='Photo' name='avatarUrl' info='png, jpg' />

          <InputRow name='first_name' label='First Name' required />
          <ErrorsRow name='first_name' />

          <InputRow name='last_name' label='Last Name' required />
          <ErrorsRow name='last_name' />

          <InputRow name='title' label='Job Name' />
          <ErrorsRow name='title' />

          <InputRow name='company' label='Company' />
          <ErrorsRow name='company' />

          <InputRow name='email' label='E-mail' />
          <ErrorsRow name='email' />

          <InputRow name='phone' label='Phone number' />
          <ErrorsRow name='phone' />

          <InputRow name='city' label='City' />
          <ErrorsRow name='city' />

          <Row label='State'>
            <StateField name='state' countryFieldName='country' />
          </Row>
          <ErrorsRow name='state' />

          <Row label='Country'>
            <CountryField name='country' stateFieldName='state' />
          </Row>
          <ErrorsRow name='country' />

          <InputRow name='zip_code' label='Zip Code' />
          <ErrorsRow name='zip_code' />
        </EditingTable>
        <GuardField name='categories' predicate={categories => categories && categories.length > 0}>
          <div className={styles.categoryList}>
            <div className={styles.categoryListTitle}>Category list</div>
            <ArrayField className={styles.categoryListItems} name='categories' fixed>
              <Field
                name=''
                component={RenderCategoryField} // eslint-disable-line no-use-before-define
              />
              <ErrorField name='' />
            </ArrayField>
          </div>
        </GuardField>
      </EditingForm>
    );
  }
}

const RenderCategoryField = ({ input }) => (
  <div>
    <div className={styles.categoryName}>{input.value.name}</div>
    <MultiselectSelect
      className={styles.categorySelect}
      name={`${input.name}.option_uid`}
      placeholder='Pick a value'
      options={_.map(input.value.options,
       o => ({ label: o.name, value: o.option_uid }))}
    />
  </div>
);

RenderCategoryField.propTypes = {
  input: PropTypes.shape({
    name: PropTypes.string,
    value: PropTypes.shape({
      name: PropTypes.string.isRequired,
      options: PropTypes.arrayOf(PropTypes.shape({
        name: PropTypes.string,
        option_uid: PropTypes.string,
      })),
      option_uid: PropTypes.arrayOf(PropTypes.string),
    }).isRequired,
  }).isRequired,
};
