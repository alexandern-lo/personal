import { extractFileName } from 'helpers/url';
import FileUploadForm from './file-upload-form';

import styles from './file-upload.module.css';


export default class FileUpload extends Component {

  static propTypes = {
    className: PropTypes.string,

    value: PropTypes.oneOfType([
      PropTypes.string,
      PropTypes.objectOf(PropTypes.string),
    ]),
    onChange: PropTypes.func.isRequired,

    withMimeType: PropTypes.bool,

    renderPreview: PropTypes.oneOfType([
      PropTypes.bool,
      PropTypes.func,
    ]),

    error: PropTypes.any, // eslint-disable-line react/forbid-prop-types
    onError: PropTypes.func.isRequired,

    accept: PropTypes.oneOfType([
      PropTypes.string,
      PropTypes.arrayOf(PropTypes.string),
    ]),
    validateFile: PropTypes.func,
  };

  onRefUpload = (upload) => { this.upload = upload; }

  onSelectFile = (e) => {
    e.preventDefault();
    this.upload.getWrappedInstance().selectFile();
  };

  onDeleteFile = (e) => {
    e.preventDefault();
    this.upload.getWrappedInstance().deleteFile();
  };

  onPreload = url => this.setState({ url });

  onStart = file => this.setState({ file, progress: 0 });

  onProgress = progress => this.setState({ progress });

  onComplete = (url, file) => {
    const { withMimeType } = this.props;
    const value = url && withMimeType ? { url, mimeType: file.type || '' } : url;
    this.setState({ file: null, url: null, progress: 0 }, () => {
      this.props.onChange(value);
    });
  };

  onDelete = () => {
    this.setState({ file: null, url: null, progress: 0 }, () => {
      this.props.onError(null, this);
      this.props.onChange(null);
    });
  };

  onError = error => this.props.onError(error, this);

  getFileName = () => {
    const { value } = this.props;
    const { file } = this.state || {};
    return extractFileName(value && (value.url || value)) || (file && file.name);
  };

  renderPreview = () => {
    const { value, renderPreview } = this.props;
    const { url } = this.state || {};
    const previewUrl = url || (value && (value.url || value));
    if (typeof renderPreview === 'function') return renderPreview(previewUrl);
    return null;
  };

  render() {
    const { renderPreview, accept, validateFile, error } = this.props;
    const { file, progress } = this.state || {};
    const fileName = this.getFileName();
    return (
      <div>
        <div className={classNames(this.props.className, styles.inputBlock)}>
          <FileUploadForm
            onStart={this.onStart}
            onPreload={renderPreview ? this.onPreload : null}
            onProgress={this.onProgress}
            onComplete={this.onComplete}
            onDelete={this.onDelete}
            onError={this.onError}
            accept={accept}
            validateFile={validateFile}
            ref={this.onRefUpload}
          />
          <div className={styles.input}>
            <input value={fileName || ''} type='text' disabled />
            { (file || error) && (
              <div className={classNames(styles.progress, { [styles.progressError]: error })}>
                <div style={{ width: `${Math.round(progress * 100)}%` }} />
              </div>
            )}
          </div>
          <div>
            { fileName ? (
              <button className={styles.buttonDelete} onClick={this.onDeleteFile}>
                Delete
              </button>
            ) : (
              <button className={styles.buttonAdd} onClick={this.onSelectFile}>
                Upload
              </button>
            )}
          </div>
        </div>
        { renderPreview && this.renderPreview() }
      </div>
    );
  }
}
