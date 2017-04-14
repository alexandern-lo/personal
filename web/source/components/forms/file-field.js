import { Field } from 'redux-form';
import { updateSyncErrors } from 'redux-form/lib/actions';

import FileUpload from '../file-upload/file-upload';
import { imageTypes } from '../file-upload/picture-upload';

export default class FileField extends Component {
  static propTypes = {
    name: PropTypes.string.isRequired,
    image: PropTypes.bool,
  };

  onError = (error, upload) => {
    const { name } = this.props;
    const { dispatch, form } = upload.props.meta;
    const errors = { [name]: error ? { error } : null };
    dispatch(updateSyncErrors(form, errors));
  }

  renderFileField = ({ className, image, input, meta, ...props }) => (
    <FileUpload
      className={classNames(className, {
        invalid: meta.invalid,
      })}
      accept={image ? imageTypes : null}
      {...input}
      {...props}
      meta={meta}
      error={meta.error}
      onError={this.onError}
    />
  )

  render() {
    const { name, ...props } = this.props;
    return (
      <Field
        name={name}
        component={this.renderFileField}
        {...props}
      />
    );
  }
}
