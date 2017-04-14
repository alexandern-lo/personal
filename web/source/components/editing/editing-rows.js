import { Row } from './editing-table';

import ErrorField, { ErrorGuard } from '../forms/error-field';
import InputField from '../forms/input-field';
import DateField from '../forms/date-field';
import SelectField from '../forms/select-field';
import TextAreaField from '../forms/text-area-field';
import TimeField from '../forms/time-field';
import PictureField from '../forms/picture-field';
import styles from './editing-table.module.css';

export const ErrorsRow = ({ className, name }) => (
  <ErrorGuard name={name}>
    <Row className={classNames(styles.errorRow, className)}>
      <ErrorField name={name} />
    </Row>
  </ErrorGuard>
);
ErrorsRow.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
};

export const HintRow = ({ className, children }) => (
  <Row className={classNames(className, styles.hintRow)}>
    <div>{ children }</div>
  </Row>
);
HintRow.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

export const InputRow = ({ className, name, label, required, ...props }) => (
  <Row className={className} name={name} label={label} required={required}>
    <InputField name={name} {...props} />
  </Row>
);
InputRow.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  required: PropTypes.bool,
};


export const DateRow = ({ className, name, label, required, ...props }) => (
  <Row className={className} name={name} label={label} required={required}>
    <DateField name={name} {...props} />
  </Row>
);
DateRow.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  required: PropTypes.bool,
};


export const SelectRow = ({ className, name, label, required, ...props }) => (
  <Row className={className} name={name} label={label} required={required}>
    <SelectField name={name} {...props} />
  </Row>
);
SelectRow.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  required: PropTypes.bool,
};

export const TextAreaRow = ({
  className, name, label, required, textAreaClassName, ...props
}) => (
  <Row className={className} name={name} label={label} required={required}>
    <TextAreaField name={name} className={textAreaClassName} {...props} />
  </Row>
);
TextAreaRow.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  textAreaClassName: PropTypes.string,
  required: PropTypes.bool,
};

export const TimeRow = ({ className, name, label, required, ...props }) => (
  <Row className={className} name={name} label={label} required={required}>
    <TimeField name={name} {...props} />
  </Row>
);
TimeRow.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  required: PropTypes.bool,
};

export const PictureRow = ({ className, name, label, required, ...props }) => (
  <Row className={className} name={name} label={label} required={required}>
    <PictureField name={name} {...props} />
  </Row>
);
PictureRow.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  required: PropTypes.bool,
};
