import { Field } from 'redux-form';

const RenderGuardField = ({ input, predicate, children }) => (
  predicate(input.value) ? children : null
);

const GuardField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderGuardField}
    {...props}
  />
);
export default GuardField;

GuardField.propTypes = {
  name: PropTypes.string.isRequired,
  predicate: PropTypes.func.isRequired,
  children: PropTypes.node.isRequired,
};
