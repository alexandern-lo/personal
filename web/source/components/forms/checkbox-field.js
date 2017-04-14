import { Field } from 'redux-form';

const RenderCheckboxField = ({
  input: { value, ...input },
  meta,
  className,
  label,
  inputChildren,
}) => (
  <label // eslint-disable-line jsx-a11y/label-has-for
    className={classNames(className, {
      invalid: meta.invalid,
    })}
  >
    <input type='checkbox' checked={value} {...input} />
    { label && <span>{label}</span> }
    { inputChildren }
  </label>
);

RenderCheckboxField.propTypes = {
  input: PropTypes.shape({
    value: PropTypes.any,
    onChange: PropTypes.func.isRequired,
  }).isRequired,
  meta: PropTypes.shape({
    invalid: PropTypes.bool,
  }).isRequired,
  className: PropTypes.string,
  label: PropTypes.string,
  inputChildren: PropTypes.node,
};


const CheckboxField = ({ name, children, ...props }) => (
  <Field
    name={name}
    component={RenderCheckboxField}
    inputChildren={children}
    {...props}
  />
);
export default CheckboxField;

CheckboxField.propTypes = {
  name: PropTypes.string.isRequired,
  children: PropTypes.node,
};
