import moment from 'moment';
import { Field } from 'redux-form';

import styles from './time-field.module.css';

export class TimeInput extends Component {
  static propTypes = {
    className: PropTypes.string,
    format: PropTypes.string,
    value: PropTypes.string,
    onBlur: PropTypes.func,
    onChange: PropTypes.func.isRequired,
  };

  static defaultProps = {
    format: 'HH:mm:ss',
  };

  onRef = container => (this.container = container);
  onRefMeridiem = meridiem => (this.meridiem = meridiem);

  onFocus = () => {
    const time = this.getTime();
    if (!time.isValid()) {
      this.setTime(this.getInitial());
    }
  };

  onBlur = (event) => {
    if (!this.container.contains(event.relatedTarget)) {
      const { value, onBlur } = this.props;
      onBlur(value);
    }
  };

  onHoursChange = (event) => {
    const time = this.getTime();
    const parsed = parseInt(event.target.value, 10) || 0;
    const hours = Math.max(0, Math.min(parsed, 12));
    this.setTime(this.updateHours(time, hours, this.meridiem.value));
  };

  onMinutesChange = (event) => {
    const time = this.getTime();
    const parsed = parseInt(event.target.value, 10) || 0;
    const minutes = Math.max(0, Math.min(parsed, 59));
    this.setTime(time.minutes(minutes));
  };

  onMeridiemChange = (event) => {
    const time = this.getTime();
    this.setTime(this.updateHours(time, null, event.target.value));
  };

  getTime = () => moment.utc(this.props.value, this.props.format);
  setTime = (time) => {
    if (time.isValid()) {
      this.props.onChange(time.format(this.props.format));
    }
  };

  getInitial = () => moment.utc({ h: 6, m: 0, s: 0, ms: 0 });

  updateHours = (time, h, a) => {
    const parts = time.format('h:a').split(':');
    if (h) parts[0] = h;
    if (a) parts[1] = a;
    const newTime = moment.utc(parts.join(':'), 'h:a');
    time.hours(newTime.hours());
    return time;
  }

  render() {
    const time = this.getTime();
    const valid = time.isValid();
    return (
      <div
        className={classNames(this.props.className, styles.container, { [styles.blank]: !valid })}
        onFocus={this.onFocus}
        onBlur={this.onBlur}
        ref={this.onRef}
      >
        <span className={styles.timeInput}>
          <input
            type='number' min={1} max={12}
            value={time.isValid() ? time.format('hh') : '00'}
            onChange={this.onHoursChange}
          />
          <i>h</i>
        </span>
        <span className={styles.separator}>:</span>
        <span className={styles.timeInput}>
          <input
            type='number' min={0} max={59}
            value={time.isValid() ? time.format('mm') : '00'}
            onChange={this.onMinutesChange}
          />
          <i>m</i>
        </span>
        <select
          className={styles.meridiemSelect}
          value={time.isValid() ? time.format('a') : 'am'}
          onChange={this.onMeridiemChange}
          ref={this.onRefMeridiem}
        >
          <option value='am'>am</option>
          <option value='pm'>pm</option>
        </select>
      </div>
    );
  }
}

const RenderTimeInputField = ({ input, meta, ...props }) => (
  <TimeInput
    className={classNames({ [styles.invalid]: meta.invalid })}
    {...input}
    {...props}
  />
);

RenderTimeInputField.propTypes = {
  input: PropTypes.shape({
    value: PropTypes.string,
  }),
  meta: PropTypes.shape({
    invalid: PropTypes.bool,
  }),
};

const TimeInputField = ({ name, ...props }) => (
  <Field
    name={name}
    component={RenderTimeInputField}
    {...props}
  />
);
export default TimeInputField;

TimeInputField.propTypes = {
  name: PropTypes.string.isRequired,
};
