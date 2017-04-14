import { reduxForm, handleSubmitError, clearSubmitError } from 'components/forms';
import { leadShape } from 'store/data/lead';

import Spinner from 'components/loading-spinner/loading-spinner';
import ErrorPanel from 'components/errors/error-panel';

import styles from './lead-preview-qualification.module.css';

const QUALIFICATIONS = ['cold', 'warm', 'hot'];

@reduxForm({
  form: 'lead-preview-qualification',
})
export default class LeadPreviewQualification extends Component {
  static propTypes = {
    className: PropTypes.string,
    lead: leadShape.isRequired,
    onChangeQualification: PropTypes.func,

    handleSubmit: PropTypes.func.isRequired,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    submitting: PropTypes.bool,
  };

  componentWillReceiveProps(nextProps) {
    const { uid: nextUid } = nextProps.lead;
    const { uid } = this.props.lead;
    if (uid !== nextUid) {
      this.onClearError();
    }
  }

  onChangeQualification = (qualification) => {
    const { lead, onChangeQualification, handleSubmit } = this.props;
    if (!onChangeQualification) return;
    const submit = () => handleSubmitError(onChangeQualification(lead, qualification));
    handleSubmit(submit)();
  };

  onClearError = () => clearSubmitError(this.props);

  render() {
    const {
      className,
      lead,
      submitting, error,
      onChangeQualification,
    } = this.props;
    return (
      <div>
        { submitting && <Spinner /> }
        { error && <ErrorPanel error={error} onClear={this.onClearError} /> }
        <div className={classNames(className, styles.container)}>
          {QUALIFICATIONS.map((q) => {
            const isActive = q === lead.qualification;
            return (
              <button
                key={q}
                className={classNames(styles.item, styles[q], {
                  [styles.active]: isActive,
                })}
                onClick={isActive ? null : () => this.onChangeQualification(q)}
                disabled={!onChangeQualification}
              >
                { q }
              </button>
            );
          })}
        </div>
      </div>
    );
  }
}
