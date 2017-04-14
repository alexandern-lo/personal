import { reduxForm, validate } from 'components/forms';

import EditingForm from 'components/editing/editing-form';
import EditingTable from 'components/editing/editing-table';

import {
  ErrorsRow,
  InputRow,
  DateRow,
  TimeRow,
  TextAreaRow,
  HintRow,
} from 'components/editing/editing-rows';

const timeComparator = (x, y) => Date.parse(`01/01/2017 ${x}`) <= Date.parse(`01/01/2017 ${y}`);

const fields = {
  name: {
    required: true,
    minLength: 2,
    maxLength: 200,
  },
  date: {
    required: true,
    between: {
      minProp: 'minDate',
      maxProp: 'maxDate',
      message: 'should be in event duration interval',
    },
  },
  start_time: {
    required: true,
    between: {
      maxField: 'end_time',
      message: 'should be lesser than end time',
      less: timeComparator,
    },
  },
  end_time: {
    required: true,
    between: {
      minField: 'start_time',
      message: 'should be greater than start time',
      less: timeComparator,
    },
  },
  location: {
    minLength: 2,
    maxLength: 200,
  },
};

const AgendaEditForm = ({ minDate, maxDate, ...props }) => (
  <EditingForm {...props}>
    <EditingTable>
      <InputRow name='name' label='Title' required />
      <ErrorsRow name='name' />

      <TextAreaRow name='description' label='Description' maxLength={200} />

      <DateRow
        name='date' label='Date'
        minDate={minDate}
        maxDate={maxDate || minDate}
        isoString
        required
      />
      <ErrorsRow name='date' />

      <TimeRow name='start_time' label='Start time' required />
      <ErrorsRow name='start_time' />

      <TimeRow name='end_time' label='End time' required />
      <ErrorsRow name='end_time' />

      <InputRow name='website' label='Website' />

      <InputRow name='location' label='Location' />
      <ErrorsRow name='location' />

      <InputRow name='location_url' label='Location URL' />
      <HintRow>
        if populated this will make the title of the Agenda item
        in the mobile client appear as a link that will open
        a separate browser window to view the URL
      </HintRow>
    </EditingTable>
  </EditingForm>
);

AgendaEditForm.propTypes = {
  minDate: PropTypes.string,
  maxDate: PropTypes.string,
};

export default reduxForm({
  form: 'edit-agenda',
  ...validate(fields, { validateOnBlur: true }),
})(AgendaEditForm);
