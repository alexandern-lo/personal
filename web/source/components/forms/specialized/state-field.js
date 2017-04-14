import { US_STATES } from 'config/geography';

import GuardField from '../guard-field';
import SelectField from '../select-field';
import InputField from '../input-field';
import { isUS } from './country-field';

const not = fn => (...args) => !fn(...args);

const StateField = ({ className, name, countryFieldName, ...props }) => (
  <div className={className}>
    <GuardField name={countryFieldName} predicate={isUS}>
      <SelectField
        name={name}
        placeholder='Choose state'
        options={US_STATES}
        {...props}
      />
    </GuardField>
    <GuardField name={countryFieldName} predicate={not(isUS)}>
      <InputField name={name} {...props} />
    </GuardField>
  </div>
);
export default StateField;

StateField.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  countryFieldName: PropTypes.string.isRequired,
  disabled: PropTypes.bool,
};
