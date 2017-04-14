import { connect } from 'store';
import { redirect, goBack } from 'store/navigate';
import Breadcrumbs from 'components/sub-header/breadcrumbs';
import { reduxForm, validate } from 'components/forms';
import { Field } from 'redux-form';

import { getPinnedEvent } from 'store/events';
import { importAttendees } from 'store/attendees/actions';

import { extractFileName } from 'helpers/url';

import EditingForm from 'components/editing/editing-form';
import EditingTable, { Row } from 'components/editing/editing-table';
import { ErrorsRow } from 'components/editing/editing-rows';

import styles from './attendees-import.module.css';

@connect({
  event: getPinnedEvent,
}, {
  importAttendees,
  redirect,
  goBack,
})
export default class AttendeesImportPage extends Component {
  static propTypes = {
    event: PropTypes.shape({ uid: PropTypes.string }),
    redirect: PropTypes.func.isRequired,
    goBack: PropTypes.func.isRequired,
    importAttendees: PropTypes.func.isRequired,
  }

  onImport= data => this.props.importAttendees({ eventUid: this.props.event.uid, ...data })
  onCancel = () => this.props.goBack();
  onSuccess = () => this.props.redirect({
    pathname: `/events/${this.props.event.uid}/attendees`,
    state: { imported: true },
  });

  render() {
    const { event } = this.props;
    return (
      <ResourceEditForm
        form='import-attendee'
        title={<Breadcrumbs event={event} action='import' />}
        actionTitle='Import'
        onSubmit={this.onImport}
        onCancel={this.onCancel}
        onSubmitSuccess={this.onSuccess}
      />
    );
  }
}

const fields = {
  file: {
    required: true,
  },
};

const RenderResourceEditForm = props => (
  <EditingForm {...props}>
    <EditingTable>
      <Row label='Attendees List' required >
        <FileField name='file' />
      </Row>
      <ErrorsRow name='file' />
      <Row>
        <div className={styles.help}>
          Here will be written some Excel formats and other thigns about format
        </div>
        <div className={styles.example}>
          <span />
          <a href='/example.xls' rel='noopener noreferrer' target='_blank'>
            Download Excel sample
          </a>
        </div>
      </Row>
    </EditingTable>
  </EditingForm>
);

const ResourceEditForm = reduxForm({
  form: 'import-attendees',
  ...validate(fields, { validateOnBlur: true }),
})(RenderResourceEditForm);
// eslint-disable-next-line react/no-multi-comp
class RenderFileField extends Component {
  static propTypes = {
    input: PropTypes.shape({
      value: PropTypes.any,
      onChange: PropTypes.func,
    }).isRequired,
  }
  onFileSelected = () => {
    const file = this.inputFile.files[0];
    const data = new FormData();
    data.append('attendees', file);
    this.inputText.value = extractFileName(file.name);
    this.props.input.onChange(data);
  }

  render() {
    return (
      <div className={styles.fileField}>
        <input ref={(input) => { this.inputText = input; }} type='text' disabled />
        <button
          className={styles.uploadButton}
          onClick={() => this.inputFile.click()}
        >
          Upload excel file
        </button>
        <input
          ref={(input) => { this.inputFile = input; }}
          onChange={this.onFileSelected}
          className={styles.invisible}
          type='file'
          accept='.csv'
        />
      </div>
    );
  }
}

const FileField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderFileField}
    {...props}
  />
);

FileField.propTypes = {
  name: PropTypes.string,
};
