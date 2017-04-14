import {
  lengthConf,
  digitsConf,
  minNumberConf,
  maxNumberConf,
} from './validations_config';

// eslint-disable-next-line max-len
const emailRegex = /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$/;
const urlRegex = /^(https?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$/;

const present = value => value && (
  value.length > 0 ||
  typeof value === 'number' ||
  typeof value === 'boolean' ||
  typeof value === 'object'
);

export const required = (field, value) => !present(value);

export const length = (field, value, config) => {
  if (!present(value)) return false;
  const { min, max, exact } = lengthConf(config);
  const actual = value.length;
  if (exact) return actual !== exact;
  return (min > 0 && actual < min) || (max > 0 && actual > max);
};

export const minLength = (field, value, config) => (
  present(value) && value.length < config
);

export const maxLength = (field, value, config) => (
  present(value) && value.length > config
);

export const email = (field, value) => (
  present(value) && !(emailRegex.test(value))
);

export const url = (field, value) => (
  present(value) && !(urlRegex.test(value))
);

export const words = (field, value) => (
  present(value) && !(/^[\w\s]+$/.test(value))
);

export const digits = (field, value, config) => {
  if (!present(value)) return false;
  const { min, max } = digitsConf(config);
  if (!min && !max) return !/^\d+$/.test(value);
  const actual = value.replace(/\D/g, '').length;
  return (min > 0 && actual < min) || (max > 0 && actual > max);
};

export const phone = (field, value) => (
  present(value) && (!/^\+?[\d()-]+$/.test(value) || value.match(/\d/g).length < 10)
);

export const alnum = (field, value) => (
  present(value) && !(/^[\w\d]+$/.test(value))
);

export const min = (field, value, config) => {
  const num = Number(value);
  return num && (num < minNumberConf(config));
};

export const max = (field, value, config) => {
  const num = Number(value);
  return num && (num > maxNumberConf(config));
};

export const requiredOneOfFields = (field, value, config, allValues) => (
  config.reduce((all, f) => (all && required(f, allValues[f])), true)
);

export const matchField = (field, value, config, allValues) => (
  present(value) && value !== allValues[config]
);

const getConfigValue = (field, prop, conf, allValues, props) => {
  if (field && present(allValues[field])) return allValues[field];
  if (prop && present(props[prop])) return props[prop];
  return present(conf) ? conf : null;
};

export const between = (field, value, config, allValues, props) => {
  if (!present(value)) return false;
  const less = config.less || ((x, y) => x < y);
  const lower = getConfigValue(config.minField, config.minProp, config.min, allValues, props);
  const upper = getConfigValue(config.maxField, config.maxProp, config.max, allValues, props);
  if (present(lower) && less(value, lower)) return true;
  if (present(upper) && less(upper, value)) return true;
  return false;
};
