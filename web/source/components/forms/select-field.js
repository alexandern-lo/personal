import _ from 'lodash';
import { Field } from 'redux-form';

import styles from './select-field.module.css';

export const optionsShape = PropTypes.arrayOf(PropTypes.oneOfType([
  PropTypes.string,
  PropTypes.objectOf(PropTypes.string),
]));

const changeWatcher = (watcher, input, meta) => (e) => {
  watcher(e.target.value, input, meta);
  input.onChange(e);
};

const RenderSelectField = ({
  input,
  meta,
  className,
  placeholder,
  disablePlaceholder,
  watcher,
  options,
  ...props
}) => (
  <select
    className={classNames(styles.input, className, {
      invalid: meta.invalid,
    })}
    {...input}
    {...props}
    onChange={watcher ? changeWatcher(watcher, input, meta) : input.onChange}
  >
    { placeholder ? <option value='' disabled={disablePlaceholder}>{placeholder}</option> : null }
    { _.map(options, item => (
      item.label || item.value
        ? <option key={item.value} value={item.value}>{item.label || item.value}</option>
        : <option key={item} value={item}>{item}</option>
    ))}
  </select>
);

RenderSelectField.propTypes = {
  input: PropTypes.shape({
    value: PropTypes.any,
    onChange: PropTypes.func.isRequired,
  }).isRequired,
  meta: PropTypes.shape({
    invalid: PropTypes.bool,
  }).isRequired,
  className: PropTypes.string,
  placeholder: PropTypes.string,
  disablePlaceholder: PropTypes.bool,
  watcher: PropTypes.func,
  options: optionsShape.isRequired,
  debugger: PropTypes.bool,
};

const SelectField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderSelectField}
    {...props}
  />
);
export default SelectField;

SelectField.propTypes = {
  name: PropTypes.string.isRequired,
};
