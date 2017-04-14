import {
  digitsConf,
  lengthConf,
  minLengthConf,
  maxLengthConf,
  minNumberConf,
  maxNumberConf,
} from './validations_config';

// TODO: use i18n

const required = () => 'required';

const digits = (conf) => {
  const { min, max } = digitsConf(conf);
  if (min > 0 && max > min) {
    return `should contain from ${min} to ${max} digits`;
  }
  if (min > 0) {
    return `should contain at least ${min} digit${min > 1 ? 's' : ''}`;
  }
  return 'should contain only digits';
};

const minNumber = (conf) => {
  const min = minNumberConf(conf);
  return `should not be less than ${min}`;
};

const maxNumber = (conf) => {
  const max = maxNumberConf(conf);
  return `should not be more than ${max}`;
};

const actualLength = v => (v ? v.length : 0);
const minLengthMessage = (min, value) => {
  const actual = actualLength(value);
  const msg = `min length ${min}`;
  return actual > 0 ? `${msg}, now it's ${actual}` : msg;
};
const maxLengthMessage = (max, value) => (
  `max length ${max}, now it's ${actualLength(value)}`
);
const exactLengthMessage = (exact, value) => (
  `length should be ${exact}, now it's ${actualLength(value)}`
);
const lengthRangeMessage = (min, max, value) => (
  `length should be between ${min} and ${max}, now it's ${actualLength(value)}`
);

const length = (conf, value) => {
  const { min, max, exact } = lengthConf(conf);
  if (exact) return exactLengthMessage(exact, value);
  if (min > 0 && max > min) return lengthRangeMessage(min, max, value);
  if (min > 0) return minLengthMessage(min, value);
  if (max > 0) return maxLengthMessage(max, value);
  return 'bad length';
};

const minLength = (conf, value) => {
  const min = minLengthConf(conf);
  return minLengthMessage(min, value);
};

const maxLength = (conf, value) => {
  const max = maxLengthConf(conf);
  return maxLengthMessage(max, value);
};

const formatters = {
  required,
  requiredOneOfFields: required,
  not_empty: required,
  length,
  minLength,
  min_length: minLength,
  maxLength,
  max_length: maxLength,
  digits,
  words: () => 'may contain only letters and spaces',
  alnum: () => 'may contain only letters and numbers',
  email: () => 'should be valid email address',
  phone: () => 'should be phone number with at least 10 digits',
  url: () => 'should be valid url',
  min: minNumber,
  max: maxNumber,
};

const validationMessages = (error, value, name) => Object.keys(error).map((key) => {
  const errorData = error[key];
  const formatter = formatters[key];
  const message = formatter ? formatter(errorData, value, name) : (errorData && errorData.message);
  if (!message) {
    // eslint-disable-next-line no-console
    console.error('No validation message for error', key, errorData);
    return { key, message: key };
  }
  return { key, message };
});

export default validationMessages;
