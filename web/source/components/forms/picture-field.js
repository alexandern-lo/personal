import { Field } from 'redux-form';
import { updateSyncErrors } from 'redux-form/lib/actions';

import PictureUpload from '../file-upload/picture-upload';

export default class PictureField extends Component {
  static propTypes = {
    name: PropTypes.string.isRequired,
  };

  onError = (error, upload) => {
    const { name } = this.props;
    const { dispatch, form } = upload.props.meta;
    const errors = { [name]: error ? { error } : null };
    dispatch(updateSyncErrors(form, errors));
  }

  renderPictureField = ({ className, input, meta, ...props }) => (
    <PictureUpload
      className={classNames(className, {
        invalid: meta.invalid,
      })}
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
        component={this.renderPictureField}
        {...props}
      />
    );
  }
}
