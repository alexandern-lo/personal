import invariant from 'invariant';
import { SubmissionError, stopAsyncValidation } from 'redux-form';
import ApiError from 'store/api/error';

export {
  reduxForm,
  Field,
  FormSection,
  SubmissionError,
} from 'redux-form';

export validate from './validations/validate';

export * as validations from './validations/validations';

export validationMessages from './validations/validation_messages';

const arrayPropRegex = /^(\w+)(\[\d+\])$/;

const getPropErrors = (errors, path) => {
  if (!path.length) return errors;
  const prop = path.shift();
  const match = prop.match(arrayPropRegex);
  if (match) {
    const name = match[1];
    const id = parseInt(match[2].replace(/[[\]]/g, ''), 10);
    const array = errors[name] || (errors[name] = []); // eslint-disable-line no-param-reassign
    const item = array[id] || (array[id] = {});
    return getPropErrors(item, path);
  }
  const item = errors[prop] || (errors[prop] = {}); // eslint-disable-line no-param-reassign
  return getPropErrors(item, path);
};

export const processSubmitError = (error, options = {}) => {
  if (!error) throw new SubmissionError();
  const errors = {};
  if (error instanceof ApiError) {
    const codeMapping = options.codeMapping || {};
    error.validationErrors.forEach((err) => {
      const { field, code } = err;
      const propErrors = getPropErrors(errors, field.split('.'));
      const actualCode = codeMapping[code] || code;
      propErrors[actualCode] = err;
    });
    if (error.errors && error.errors.length > 0) {
      errors._error = error; // eslint-disable-line no-underscore-dangle
    }
  } else {
    errors._error = error; // eslint-disable-line no-underscore-dangle
  }
  throw new SubmissionError(errors);
};

export const handleSubmitError = (promise, options) => {
  invariant(promise && typeof promise.catch === 'function', 'handleSubmitError should be called with promise');
  return promise.catch(error => processSubmitError(error, options));
};

export const clearSubmitError = props => props.dispatch(stopAsyncValidation(props.form));
