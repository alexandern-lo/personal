import FileUpload from './file-upload-form';

import styles from './picture-upload.module.css';

export const imageTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/svg+xml'];

export default class PictureUpload extends Component {

  static propTypes = {
    className: PropTypes.string,

    hint: PropTypes.string,

    value: PropTypes.string,
    onChange: PropTypes.func.isRequired,

    error: PropTypes.any, // eslint-disable-line react/forbid-prop-types
    onError: PropTypes.func.isRequired,

    accept: PropTypes.oneOfType([
      PropTypes.string,
      PropTypes.arrayOf(PropTypes.string),
    ]),
    validateFile: PropTypes.func,
  };

  static defaultProps = {
    accept: imageTypes,
  };

  onRefUpload = (upload) => { this.upload = upload; };

  onSelectFile = (e) => {
    e.preventDefault();
    this.upload.getWrappedInstance().selectFile();
  };

  onDeleteFile = (e) => {
    e.preventDefault();
    this.upload.getWrappedInstance().deleteFile();
  };

  onStart = () => {
    this.setState({ uploading: true, progress: 0 });
  };

  onPreload = (url) => {
    this.setState({ url });
  };

  onProgress = (progress) => {
    this.setState({ progress });
  };

  onComplete = (url) => {
    this.setState({ uploading: false, url: null, progress: 0 }, () => {
      this.props.onChange(url);
    });
  };

  onError = (error) => {
    this.props.onError(error, this);
  };

  getUrl = () => {
    const { value } = this.props;
    const { url } = this.state || {};
    return value && value.length > 0 ? value : url;
  };

  renderHint = () => {
    const { hint } = this.props;
    return hint && <div className={styles.hint} title={hint} />;
  };

  renderPreview = ({ url, uploading, progress, error }) => (
    <div className={styles.wrapper}>
      <div className={styles.preview}>
        <img src={url} alt='' />
        { (uploading || error) && (
          <div className={classNames(styles.progress, {
            [styles.progressError]: error,
          })}
          >
            <div style={{ width: `${Math.round(progress * 100)}%` }} />
          </div>
        )}
      </div>
      <div>
        <button className={styles.buttonDelete} onClick={this.onDeleteFile}>
          Delete
        </button>
      </div>
      {this.renderHint()}
    </div>
  );

  renderEmpty = () => (
    <div className={styles.wrapper}>
      <div className={styles.placeholder} />
      <div>
        <button className={styles.buttonAdd} onClick={this.onSelectFile}>
          Add photo
        </button>
      </div>
      {this.renderHint()}
    </div>
  );

  render() {
    const { className, accept, validateFile, error } = this.props;
    const { uploading, progress } = this.state || {};
    const url = this.getUrl();
    return (
      <div className={className}>
        { url
          ? this.renderPreview({ url, uploading, progress, error })
          : this.renderEmpty()
        }
        <FileUpload
          onStart={this.onStart}
          onComplete={this.onComplete}
          onError={this.onError}

          onPreload={this.onPreload}
          onProgress={this.onProgress}

          accept={accept}
          validateFile={validateFile}

          ref={this.onRefUpload}
        />
      </div>
    );
  }
}
