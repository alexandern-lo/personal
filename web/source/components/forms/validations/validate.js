import { getPath } from 'store/helpers/property';
import * as validations from './validations';

const resolved = Promise.resolve();

class Validator {
  constructor(values, props) {
    this.values = values;
    this.props = props;
    this.promises = [resolved];
    this.errors = {};
  }

  validateField = (fieldName, validationFn, validationType, validationConfig) => {
    const value = getPath(fieldName.split('.'))(this.values);
    const hasError = validationFn(fieldName, value, validationConfig, this.values, this.props);
    if (hasError) {
      if (typeof hasError.then === 'function') {
        this.promises.push(hasError.catch(error => (
          this.addError(fieldName, validationConfig, validationType, error)
        )));
      } else {
        this.addError(fieldName, validationConfig, validationType, hasError);
      }
    }
  };

  addError = (fieldName, validationConfig, validationType, error) => {
    const path = fieldName.split('.');
    let errors = this.errors;
    while (path.length > 0) {
      const prop = path.shift();
      errors = errors[prop] || (errors[prop] = {});
    }
    errors[validationType] = error === true ? validationConfig || true : error;
  };

  errorPromise = () => (
    Promise.all(this.promises).then(() => (
      Object.keys(this.errors).length > 0 ? Promise.reject(this.errors) : resolved
    ))
  )
}

const collectFields = config => (
  Object.keys(config).map(name => ({ name, config: config[name] }))
);

const applyFieldValidations = (validator, { name, config }) => {
  Object.keys(config).forEach((validationType) => {
    const validationFn = validations[validationType];
    const validationConfig = config[validationType];
    if (typeof validationFn !== 'function') {
      throw new Error(`Unsupported validation: ${validationType}`);
    }
    validator.validateField(name, validationFn, validationType, validationConfig);
  });
};

export const asyncValidate = (fieldsConfig, { validateOnBlur } = {}) => {
  const fields = collectFields(fieldsConfig);
  return (values, dispatch, props, blurredField) => {
    const validator = new Validator(values, props, dispatch);
    if (blurredField) {
      const field = fields.find(f => f.name === blurredField);
      if (field && (field.config.validateOnBlur || validateOnBlur)) {
        applyFieldValidations(validator, field);
      }
    } else {
      fields.forEach(field => applyFieldValidations(validator, field));
    }
    return validator.errorPromise();
  };
};

export const asyncBlurFields = (fieldsConfig, { validateOnBlur } = {}) => {
  const fields = collectFields(fieldsConfig);
  const filtered = validateOnBlur
    ? fields
    : fields.filter(f => f.config.validateOnBlur);
  return filtered.map(f => f.name);
};

export default (config, options) => ({
  asyncValidate: asyncValidate(config, options),
  asyncBlurFields: asyncBlurFields(config, options),
});
