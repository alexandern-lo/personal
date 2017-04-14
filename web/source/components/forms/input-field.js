import { Field } from 'redux-form';
import { RemoveButton } from './specialized/buttons';

import styles from './input-field.module.css';

const RenderInputField = ({
  input,
  meta,
  className,
  onRemove,
  ...props
}) => (onRemove ? (
  <div className={styles.container}>
    <input
      className={classNames(styles.input, className, {
        invalid: meta.invalid,
      })}
      {...input}
      {...props}
    />
    <RemoveButton onClick={onRemove} />
  </div>
) : (
  <input
    className={classNames(styles.input, className, {
      invalid: meta.invalid,
    })}
    {...input}
    {...props}
  />
));

RenderInputField.propTypes = {
  input: PropTypes.shape({
    value: PropTypes.any,
    onChange: PropTypes.func.isRequired,
  }).isRequired,
  meta: PropTypes.shape({
    invalid: PropTypes.bool,
  }).isRequired,
  className: PropTypes.string,
  type: PropTypes.string.isRequired,
  onRemove: PropTypes.func,
  disabled: PropTypes.bool,
};


const InputField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderInputField}
    {...props}
  />
);
export default InputField;

InputField.propTypes = {
  name: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
};

InputField.defaultProps = {
  type: 'text',
};
