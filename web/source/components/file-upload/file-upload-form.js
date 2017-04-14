import { CancelToken } from 'axios';
import { connectActions } from 'store';
import { requestUpload, uploadFile } from 'store/file-upload/actions';

import styles from './file-upload-form.module.css';

@connectActions({
  requestUpload,
  uploadFile,
}, {
  withRef: true,
})
export default class FileUpload extends Component {
  static propTypes = {
    onStart: PropTypes.func.isRequired, // (file)
    onDelete: PropTypes.func, // ()
    onComplete: PropTypes.func.isRequired, // (file_url, file)
    onError: PropTypes.func.isRequired, // (error)

    onPreload: PropTypes.func, // (data_url)
    onProgress: PropTypes.func, // (progress [0..1])

    accept: PropTypes.oneOfType([
      PropTypes.string,
      PropTypes.arrayOf(PropTypes.string),
    ]),
    validateFile: PropTypes.func, // (file) => true | error

    requestUpload: PropTypes.func.isRequired,
    uploadFile: PropTypes.func.isRequired,
  };

  constructor(props) {
    super(props);
    this.state = {};
  }

  componentWillUnmount() {
    this.cancelUpload();
  }

  onRefForm = (form) => { this.form = form; }
  onRefInput = (input) => { this.fileInput = input; }

  onFileSelected = (event) => {
    const file = event.target.files[0];
    this.form.reset(); // clear file input so user can select same file again
    const { validateFile = () => true } = this.props;
    const result = validateFile(file);
    if (result === true) {
      this.uploadFile(file);
    } else {
      this.props.onError(result, this);
    }
  }

  selectFile = () => this.fileInput.click()

  deleteFile = () => {
    this.cancelUpload();
    const notify = this.props.onDelete || this.props.onComplete;
    this.setState({ file: null, cancel: null }, () => notify(null));
  }

  cancelUpload = () => {
    const { cancel } = this.state;
    if (cancel) {
      cancel();
    }
  }

  uploadFile = (file) => {
    const check = () => file === this.state.file;
    const cancelSource = CancelToken.source();
    const onProgress = this.props.onProgress
      ? ({ loaded, total }) => (check() && this.props.onProgress(loaded / total))
      : undefined;
    this.props.requestUpload(file.name).then(({ data: url } = {}) => {
      if (!url || !check()) return null;
      return this.props.uploadFile({
        url,
        file,
        onUploadProgress: onProgress,
        cancelToken: cancelSource.token,
      }).then(res => (res === null ? res : url.split('?')[0]));
    })
    .then(url => check() && this.props.onComplete(url, file))
    .catch(error => check() && this.props.onError(error));
    if (this.props.onPreload) {
      const reader = new FileReader();
      reader.onload = e => (check() && this.props.onPreload(e.target.result));
      reader.readAsDataURL(file);
    }
    this.setState({ file, cancel: cancelSource.cancel }, () => {
      this.props.onStart(file);
    });
  }

  render() {
    const { accept } = this.props;
    const fileTypes = Array.isArray(accept) ? accept.join(',') : accept;
    return (
      <form
        className={styles.form}
        ref={this.onRefForm}
      >
        <input
          type='file'
          accept={fileTypes}
          onChange={this.onFileSelected}
          ref={this.onRefInput}
        />
      </form>
    );
  }
}
