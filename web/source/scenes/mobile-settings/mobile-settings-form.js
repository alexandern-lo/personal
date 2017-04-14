import { reduxForm, validate } from 'components/forms';

import EditingForm from 'components/editing/editing-form';
import EditingTable from 'components/editing/editing-table';
import { InputRow, ErrorsRow } from 'components/editing/editing-rows';

const fields = {
  label: {
    required: true,
    minLength: 2,
    maxLength: 20,
  },
  url: {
    required: true,
    maxLength: 200,
    url: true,
  },
};

@reduxForm({
  form: 'mobile-settings',
  ...validate(fields, { validateOnBlur: true }),
})
// eslint-disable-next-line react/prefer-stateless-function
export default class MobileSettingsForm extends PureComponent {
  render() {
    return (
      <EditingForm {...this.props}>
        <EditingTable>
          <InputRow label='Nav item name' name='label' required maxLength={fields.label.maxLength} />
          <ErrorsRow name='label' />

          <InputRow label='URL' name='url' required maxLength={fields.url.maxLength} />
          <ErrorsRow name='url' />
        </EditingTable>
      </EditingForm>
    );
  }
}
