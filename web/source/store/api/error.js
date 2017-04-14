import Error from 'es6-error';

export const isValidationError = error => (
  error.fields && error.fields.length && error.code
);

export default class ApiError extends Error {
  constructor(message, errors = []) {
    super(message);
    this.errors = errors.filter(e => !isValidationError(e));
    this.validationErrors = errors.filter(isValidationError)
      .map(({ fields, ...error }) => ({ ...error, field: fields[0] }));
  }
}
