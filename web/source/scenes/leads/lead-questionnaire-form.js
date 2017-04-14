import _ from 'lodash';
import { reduxForm, Field } from 'components/forms';
import ArrayField from 'components/forms/array-field';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { RowSeparator } from 'components/editing/editing-table';
import { InputRow, SelectRow } from 'components/editing/editing-rows';

@reduxForm({
  form: 'edit-lead-questionnaire',
})
export default class ResourceEditForm extends Component {

  renderAnswerFields = array => (
    <EditingTable naked>
      {array.map(name => (
        // eslint-disable-next-line no-use-before-define
        <Field key={name} name={name} component={RenderAnswerField} />
      ))}
    </EditingTable>
  );

  render() {
    return (
      <EditingForm {...this.props}>
        <ArrayField name='question_answers' renderFields={this.renderAnswerFields} />
      </EditingForm>
    );
  }
}

const RenderAnswerField = ({ input }) => {
  const { question } = input.value;
  return (
    <tbody>
      <InputRow
        name={`${input.name}.question.text`}
        label='Question'
        readOnly
      />
      <SelectRow
        name={`${input.name}.answer_uid`}
        label='Answer'
        placeholder='Please select answer'
        options={_.map(question.answers, answer => (
          { label: answer.text, value: answer.uid }
        ))}
      />
      <RowSeparator />
    </tbody>
  );
};
RenderAnswerField.propTypes = {
  input: PropTypes.shape({
    name: PropTypes.string.isRequired,
    value: PropTypes.shape({}).isRequired,
  }).isRequired,
};

