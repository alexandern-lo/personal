import { reduxForm, handleSubmitError } from 'components/forms';
import BasicDialog from './basic-dialog';
import styles from './delete-dialog.module.css';

@reduxForm({
  form: 'dialog',
})
export default class DeleteDialog extends Component {
  static propTypes = {
    className: PropTypes.string,
    message: PropTypes.string.isRequired,
    onSubmit: PropTypes.func.isRequired,
    onCancel: PropTypes.func.isRequired,
    submitting: PropTypes.bool,
    handleSubmit: PropTypes.func.isRequired,
  };

  componentDidMount() {
    window.addEventListener('keyup', this.onKeyUp);
  }

  componentWillUnmount() {
    window.removeEventListener('keyup', this.onKeyUp);
  }

  onKeyUp = (event) => {
    if (event.keyCode === 27) {
      this.props.onCancel();
    }
  }

  onSubmit = (event) => {
    event.preventDefault();
    const { onSubmit, handleSubmit } = this.props;
    const submitter = () => {
      const promise = onSubmit();
      if (promise && typeof promise.then === 'function') {
        return handleSubmitError(promise);
      }
      return null;
    };
    return handleSubmit(submitter)(event);
  }

  render() {
    const {
      className,
      message,
      onCancel,
      submitting,
    } = this.props;
    return (
      <BasicDialog
        loading={submitting}
        className={className}
        iconClass={styles.deleteIcon}
        message={message}
        onDelete={this.onSubmit}
        onCancel={onCancel}
      />
    );
  }
}
