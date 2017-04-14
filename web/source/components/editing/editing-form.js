import { handleSubmitError, clearSubmitError } from 'components/forms';

import PageContentWrapper from 'components/layout/page-content-wrapper';
import Spinner from 'components/loading-spinner/loading-spinner';
import SubHeader from 'components/sub-header/sub-header';
import ErrorPanel from 'components/errors/error-panel';
import { CancelButton, SaveButton } from 'components/sub-header/buttons';
import { SuccessDialog } from 'components/dialogs';

import styles from './editing-form.module.css';

export default class EditingForm extends Component {
  static propTypes = {
    title: PropTypes.node.isRequired,
    actionTitle: PropTypes.string.isRequired,
    children: PropTypes.node.isRequired,

    loading: PropTypes.bool,
    loadingError: PropTypes.shape({
      message: PropTypes.string,
    }),
    clearLoadingError: PropTypes.func,

    onSubmit: PropTypes.func.isRequired, // eslint-disable-line react/no-unused-prop-types
    onCancel: PropTypes.func.isRequired,
    handleSubmit: PropTypes.func.isRequired,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    pristine: PropTypes.bool,
    submitting: PropTypes.bool,

    panel: PropTypes.node,

    notification: PropTypes.string,
    clearNotification: PropTypes.func,
  };

  onSubmit = (event) => {
    const { onSubmit, handleSubmit } = this.props;
    const submit = (...args) => handleSubmitError(onSubmit(...args));
    return handleSubmit(submit)(event);
  };

  onClearError = () => clearSubmitError(this.props);

  render() {
    const {
      title, actionTitle, onCancel, panel, children,
      pristine, submitting, error,
      notification, clearNotification,
      loading, loadingError, clearLoadingError,
    } = this.props;
    return (
      <PageContentWrapper>
        { (submitting || loading) && !notification && <Spinner /> }
        { notification && <SuccessDialog message={notification} onOk={clearNotification} /> }
        { error && <ErrorPanel error={error} onClear={this.onClearError} /> }
        { loadingError && <ErrorPanel error={loadingError} onClear={clearLoadingError} /> }
        <SubHeader>
          { title }
          <CancelButton onClick={onCancel}>Cancel</CancelButton>
          <SaveButton disabled={pristine} onClick={this.onSubmit}>{actionTitle}</SaveButton>
        </SubHeader>
        {panel}
        <div className={styles.tableWrapper}>
          { children }
        </div>
      </PageContentWrapper>
    );
  }
}
