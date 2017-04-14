import { change } from 'redux-form/lib/actions';
import { COUNTRIES } from 'config/geography';

import SelectField from '../select-field';

export const isUS = country => country === 'United States';

const watcher = stateFieldName => (country, input, meta) => {
  if (isUS(input.value) !== isUS(country)) {
    const { dispatch, form } = meta;
    dispatch(change(form, stateFieldName, null));
  }
};

// eslint-disable-next-line no-unused-vars
const CountryField = ({ stateFieldName, ...props }) => (
  <SelectField
    placeholder='Choose country'
    options={COUNTRIES}
    watcher={stateFieldName ? watcher(stateFieldName) : null}
    {...props}
  />
);
export default CountryField;

CountryField.propTypes = {
  stateFieldName: PropTypes.string,
  disabled: PropTypes.bool,
};
