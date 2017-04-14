import { reduxForm, validate } from 'components/forms';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row, RowSeparator } from 'components/editing/editing-table';
import { InputRow, ErrorsRow } from 'components/editing/editing-rows';
import ArrayField from 'components/forms/array-field';
import InputField from 'components/forms/input-field';
import ErrorField from 'components/forms/error-field';

const fields = {
  text: {
    required: true,
  },
};

const QuestionEditForm = props => (
  <EditingForm {...props}>
    <EditingTable>
      <InputRow name='text' label='Question' required />
      <ErrorsRow name='text' />
      <RowSeparator />
      <Row label='Answers' required>
        <ArrayField name='answers' min={2} max={6} prop='text' initialValue={{ text: '' }}>
          <InputField name='' />
          <ErrorField name='' />
        </ArrayField>
      </Row>
    </EditingTable>
  </EditingForm>
);

export default reduxForm({
  form: 'edit-question',
  ...validate(fields, { validateOnBlur: true }),
})(QuestionEditForm);
