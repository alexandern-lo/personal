import { Field } from 'redux-form';

import validationMessages from './validations/validation_messages';

import styles from './error-field.module.css';

export const ErrorMessages = ({ className, input, meta }) => {
  const messages = validationMessages(meta.error || {}, input.value, input.name);
  if (messages.length === 0) return null;
  return (
    <p className={classNames(styles.errors, className)}>
      { messages.map(({ key, message }) => (
        <span key={key}>{message}</span>
      ))}
    </p>
  );
};

ErrorMessages.propTypes = {
  className: PropTypes.string,
  input: PropTypes.shape({
    name: PropTypes.string,
    value: PropTypes.any,
  }).isRequired,
  meta: PropTypes.shape({
    error: PropTypes.objectOf(PropTypes.any),
  }).isRequired,
};

export const ErrorField = ({ name, ...props }) => (
  <Field
    name={name}
    component={ErrorMessages}
    {...props}
  />
);
export default ErrorField;

ErrorField.propTypes = {
  name: PropTypes.string.isRequired,
};


const RenderErrorGuard = ({ meta, children }) => (meta.error ? children : null);

export const ErrorGuard = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderErrorGuard}
    {...props}
  />
);

ErrorGuard.propTypes = {
  name: PropTypes.string.isRequired,
  children: PropTypes.node.isRequired,
};
