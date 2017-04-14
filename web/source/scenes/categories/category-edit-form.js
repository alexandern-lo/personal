import { reduxForm, validate } from 'components/forms';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row, RowSeparator } from 'components/editing/editing-table';
import { InputRow, ErrorsRow } from 'components/editing/editing-rows';
import ArrayField from 'components/forms/array-field';
import InputField from 'components/forms/input-field';
import ErrorField from 'components/forms/error-field';

const fields = {
  name: {
    required: true,
  },
};

const CategoryEditForm = props => (
  <EditingForm {...props}>
    <EditingTable>
      <InputRow name='name' label='Name' required />
      <ErrorsRow name='name' />
      <RowSeparator />
      <Row label='Possible values' required>
        <ArrayField name='options' min={1} prop='name' initialValue={{ name: '' }}>
          <InputField name='' />
          <ErrorField name='' />
        </ArrayField>
      </Row>
    </EditingTable>
  </EditingForm>
);

export default reduxForm({
  form: 'edit-category',
  ...validate(fields, { validateOnBlur: true }),
})(CategoryEditForm);
