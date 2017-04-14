import _ from 'lodash';
import moment from 'moment';
import { Field } from 'redux-form';
import { MonthView } from 'react-date-picker';

import 'css/components/datePickerInput/base.css';
import 'css/components/datePickerInput/theme/default/index.scss';

import styles from './date-field.module.css';

export const datePropType = PropTypes.oneOfType([
  PropTypes.string,
  PropTypes.number,
]);

class RenderDateField extends Component {
  static propTypes = {
    className: PropTypes.string,
    input: PropTypes.shape({
      value: datePropType,
      onChange: PropTypes.func.isRequired,
      onFocus: PropTypes.func.isRequired,
      onBlur: PropTypes.func.isRequired,
    }).isRequired,
    meta: PropTypes.shape({
      active: PropTypes.bool,
      invalid: PropTypes.bool,
    }).isRequired,
    minDate: datePropType,
    maxDate: datePropType,
    defaultValue: datePropType,
    format: PropTypes.string,
    isoString: PropTypes.bool,
    disabled: PropTypes.bool,
  };

  componentDidMount() {
    document.addEventListener('mousedown', this.onMouseDown);
  }

  componentWillUnmount() {
    document.removeEventListener('mousedown', this.onMouseDown);
  }

  onRef = container => (this.container = container);

  onChangeInput = event => event.preventDefault();

  onChange = (str, { dateMoment }) => {
    const { isoString } = this.props;
    const year = dateMoment.get('year');
    const month = dateMoment.get('month');
    const day = dateMoment.get('date');
    const date = new Date(Date.UTC(year, month, day));
    const value = isoString ? date.toISOString() : date.valueOf();
    this.props.input.onBlur(value);
  };

  onMouseDown = (event) => {
    const outside = !this.container.contains(event.target);
    const { active } = this.props.meta;
    const { value, onBlur } = this.props.input;
    if (active && outside) {
      onBlur(value);
    }
  };

  parseDate = (value, defaultValue) => {
    const date = value || defaultValue;
    if (!date) return null;
    return _.isNumber(date) ? moment.unix(date) : moment(date);
  };

  render() {
    const {
      className,
      input: { name, value, onFocus },
      meta: { active, invalid },
      defaultValue, minDate, maxDate,
      format = moment.localeData().longDateFormat('LL'),
      disabled,
    } = this.props;
    const date = this.parseDate(value, defaultValue);
    const str = date && date.isValid() ? date.format(format) : '';
    const min = this.parseDate(minDate);
    const max = this.parseDate(maxDate);
    return (
      <div
        className={classNames(styles.inputWrapper, className)}
        ref={this.onRef}
      >
        <input
          name={name}
          type='text'
          className={classNames(styles.input, {
            invalid,
          })}
          value={str}
          onFocus={onFocus}
          onChange={this.onChangeInput}
          disabled={disabled}
        />
        <div
          className={classNames(styles.calendarWrapper, {
            [styles.showCalendar]: active,
          })}
        >
          <MonthView
            value={date}
            minDate={min}
            maxDate={max}
            dateFormat={format}
            onChange={this.onChange}
            enableHistoryView={false}
            showClock={false}
            weekNumbers={false}
            footer={false}
            theme='input'
          />
        </div>
      </div>
    );
  }
}


const DateInputField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderDateField}
    {...props}
  />
);
export default DateInputField;

DateInputField.propTypes = {
  name: PropTypes.string.isRequired,
};
