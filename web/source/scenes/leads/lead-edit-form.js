import { reduxForm, validate } from 'components/forms';
import { createApiCallAction } from 'store/helpers/actions';
import {
  phoneDesignations,
  blankPhone,
  emailDesignations,
  blankEmail,
} from 'store/data/lead';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row, RowSeparator } from 'components/editing/editing-table';
import {
  InputRow,
  TextAreaRow,
  ErrorsRow,
} from 'components/editing/editing-rows';

import ErrorField from 'components/forms/error-field';
import FileField from 'components/forms/file-field';
import ArrayField from 'components/forms/array-field';
import DesignatedField from 'components/forms/specialized/designated-field';
import CountryField from 'components/forms/specialized/country-field';
import StateField from 'components/forms/specialized/state-field';
import AutocompleteField from 'components/forms/autocomplete-field';

import styles from './lead-edit-form.module.css';

const stdLength = {
  minLength: 2,
  maxLength: 200,
};

const fields = {
  event_uid: { required: true },
  first_name: { required: true, ...stdLength },
  last_name: { ...stdLength },
  job_title: { ...stdLength },
  company_name: { ...stdLength },
  company_url: { url: true },
  address: { ...stdLength },
  city: { ...stdLength },
  state: { ...stdLength },
  zip_code: { ...stdLength },
  notes: { maxLength: 180 },
};

const fetchEvents = createApiCallAction('events/AUTOCOMPLETE', 'fetchEvents');
const readEventOption = data => ({ label: data.name, value: data.event_uid });

@reduxForm({
  form: 'edit-lead',
  ...validate(fields, { validateOnBlur: true }),
})
export default class ResourceEditForm extends Component {
  static propTypes = {
    ownerId: PropTypes.string,
  };

  fetchEvents = (params) => {
    const { ownerId } = this.props;
    return fetchEvents({ ...params, scope: 'selectable', for_user: ownerId });
  };

  renderPreview = url => (
    <div className={styles.preview}>
      {url
        ? <img src={url} alt='' />
        : <div className={styles.placeholder} />
      }
    </div>
  );

  render() {
    return (
      <EditingForm {...this.props}>
        <EditingTable>
          <Row label='Event' required>
            <AutocompleteField
              name='event_uid'
              placeholder='Select event...'
              fetcher={this.fetchEvents}
              reader={readEventOption}
            />
          </Row>
          <ErrorsRow name='event_uid' />

          <InputRow name='first_name' label='First name' required maxLength={fields.first_name.maxLength} />
          <ErrorsRow name='first_name' />

          <InputRow name='last_name' label='Last name' maxLength={fields.last_name.maxLength} />
          <ErrorsRow name='last_name' />

          <InputRow name='company_name' label='Company' maxLength={fields.company_name.maxLength} />
          <ErrorsRow name='company_name' />

          <InputRow name='company_url' label='Company URL' />
          <ErrorsRow name='company_url' />

          <InputRow name='job_title' label='Job title' maxLength={fields.job_title.maxLength} />
          <ErrorsRow name='job_title' />

          <RowSeparator />

          <Row label='Phone numbers'>
            <ArrayField name='phones' max={4} initialValue={blankPhone}>
              <DesignatedField name='' valueProp='phone' designations={phoneDesignations} />
              <ErrorField name='' />
            </ArrayField>
          </Row>

          <RowSeparator />

          <Row label='Emails' required>
            <ArrayField name='emails' max={4} initialValue={blankEmail}>
              <DesignatedField name='' valueProp='email' designations={emailDesignations} />
              <ErrorField name='' />
            </ArrayField>
          </Row>
        </EditingTable>

        <EditingTable>
          <InputRow name='address' label='Address' maxLength={fields.address.maxLength} />
          <ErrorsRow name='address' />

          <InputRow name='city' label='City' maxLength={fields.city.maxLength} />
          <ErrorsRow name='city' />

          <Row label='State'>
            <StateField name='state' countryFieldName='country' maxLength={fields.state.maxLength} />
          </Row>
          <ErrorsRow name='state' />

          <InputRow name='zip_code' label='Zip code' maxLength={fields.zip_code.maxLength} />
          <ErrorsRow name='zip_code' />

          <Row label='Country'>
            <CountryField name='country' stateFieldName='state' />
          </Row>
          <ErrorsRow name='country' />

          <Row label='Photo'>
            <FileField name='photo_url' image renderPreview={this.renderPreview} />
          </Row>

          <Row label='Business card'>
            <div className={styles.businessCard}>
              <FileField name='business_card_front_url' image renderPreview={this.renderPreview} />
              <FileField name='business_card_back_url' image renderPreview={this.renderPreview} />
            </div>
          </Row>

          <TextAreaRow name='notes' label='Notes' maxLength={fields.notes.maxLength} />
          <ErrorsRow name='notes' />
        </EditingTable>
      </EditingForm>
    );
  }
}
