import { handleSubmitError, clearSubmitError } from 'components/forms';

import ModalView from 'components/modal-view/modal-view';
import Spinner from 'components/loading-spinner/loading-spinner';
import ErrorPanel from 'components/errors/error-panel';

export default class EditingView extends Component {
  static propTypes = {
    submitButtonText: PropTypes.string.isRequired,

    children: PropTypes.node.isRequired,

    onSubmit: PropTypes.func.isRequired, // eslint-disable-line react/no-unused-prop-types
    handleSubmit: PropTypes.func.isRequired,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    submitting: PropTypes.bool,
    pristine: PropTypes.bool,
  };

  onSubmit = (event) => {
    const { onSubmit, handleSubmit } = this.props;
    const submit = (...args) => handleSubmitError(onSubmit(...args));
    return handleSubmit(submit)(event);
  };

  onClearError = () => clearSubmitError(this.props);

  render() {
    const { submitButtonText, children, submitting, error, pristine } = this.props;
    return (
      <ModalView
        button={{
          text: submitButtonText,
          onClick: this.onSubmit,
          active: !submitting && !pristine,
        }}
        {...this.props}
      >
        { submitting && <Spinner /> }
        { children }
        { error && <ErrorPanel error={error} onClear={this.onClearError} /> }
      </ModalView>
    );
  }
}
