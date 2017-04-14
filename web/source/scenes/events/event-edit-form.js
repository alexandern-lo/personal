import { reduxForm, validate } from 'components/forms';

import { INDUSTRIES } from 'store/data/event';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row } from 'components/editing/editing-table';
import {
  ErrorsRow,
  InputRow,
  DateRow,
  SelectRow,
} from 'components/editing/editing-rows';

import DateField from 'components/forms/date-field';
import CheckboxField from 'components/forms/checkbox-field';
import PictureField from 'components/forms/picture-field';
import CountryField from 'components/forms/specialized/country-field';
import StateField from 'components/forms/specialized/state-field';
import ErrorField from 'components/forms/error-field';
import GuardField from 'components/forms/guard-field';

import styles from './event-edit-form.module.css';

const fields = {
  name: {
    required: true,
  },
  start_date: {
    required: true,
  },
  website_url: {
    url: true,
  },
};

@reduxForm({
  form: 'edit-event',
  ...validate(fields, { validateOnBlur: true }),
})
export default class EventEditForm extends PureComponent { // eslint-disable-line
  static propTypes = {
    isConference: PropTypes.bool,
  };

  render() {
    const { isConference } = this.props;
    return (
      <EditingForm {...this.props}>
        <EditingTable>

          <Row className={styles.imageRow} label='Event image'>
            <PictureField name='logo_url' />
          </Row>
          <ErrorsRow name='logo_url' />

          <InputRow name='name' label='Name' required maxLength={200} />
          <ErrorsRow name='name' />

          { isConference && <DateRow name='start_date' label='Start date' isoString required /> }
          { !isConference && (
            <Row name='start_date' label='Date' required>
              <div className={styles.dateWrapper}>
                <div>
                  <DateField name='start_date' isoString />
                </div>
                <div>
                  <CheckboxField name='recurring' label='Ongoing' />
                </div>
              </div>
            </Row>
          )}
          <ErrorsRow name='start_date' />

          { isConference && <DateRow name='end_date' label='End date' isoString required /> }
          { !isConference && (
            <GuardField name='recurring' predicate={v => v}>
              <DateRow name='end_date' label='End date' disabled isoString />
            </GuardField>
          )}
          { !isConference && (
            <GuardField name='recurring' predicate={v => !v}>
              <DateRow name='end_date' label='End date' isoString />
            </GuardField>
          )}
          <ErrorsRow name='end_date' />

          <SelectRow
            name='industry'
            label='Industry'
            placeholder='Choose industry'
            options={INDUSTRIES}
          />
          <ErrorsRow name='industry' />

          <InputRow name='venue_name' label='Venue name' />
          <ErrorsRow name='venue_name' />

          <InputRow name='address' label='Address' />
          <ErrorsRow name='address' />

          <InputRow name='city' label='City' />
          <ErrorsRow name='city' />

          <Row label='State'>
            <div className={styles.stateWrapper}>
              <div>
                <StateField name='state' countryFieldName='country' />
                <ErrorField name='state' />
              </div>
              <div className={styles.zipWrapper}>
                <EditingTable>
                  <InputRow name='zip_code' label='Zip code' />
                  <ErrorsRow name='zip_code' />
                </EditingTable>
              </div>
            </div>
          </Row>

          <Row label='Country'>
            <CountryField name='country' stateFieldName='state' />
          </Row>
          <ErrorsRow name='country' />

          <InputRow name='website_url' label='URL' />
          <ErrorsRow name='website_url' />

        </EditingTable>
      </EditingForm>
    );
  }
}
