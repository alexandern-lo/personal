import { reduxForm, validate } from 'components/forms';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row } from 'components/editing/editing-table';
import FileField from 'components/forms/file-field';
import { InputRow, ErrorsRow, TextAreaRow } from 'components/editing/editing-rows';

const fields = {
  name: {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  file: {
    required: true,
  },
  description: {
    minLength: 2,
    maxLength: 200,
  },
};

const ResourceEditForm = props => (
  <EditingForm {...props}>
    <EditingTable>
      <InputRow name='name' label='Title' required />
      <ErrorsRow name='name' />

      <Row label='Resource' required >
        <FileField name='file' withMimeType />
      </Row>
      <ErrorsRow name='file' />

      <TextAreaRow name='description' label='Description' maxLength={200} />
      <ErrorsRow name='description' />
    </EditingTable>
  </EditingForm>
);

export default reduxForm({
  form: 'edit-resource',
  ...validate(fields, { validateOnBlur: true }),
})(ResourceEditForm);
