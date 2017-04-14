import moment from 'moment';
import { DateField, MonthView } from 'react-date-picker';
import { format as formatDatetime } from 'helpers/dates';
import styles from './date-picker.module.css';

export default class DatePicker extends Component {
  static propTypes = {
    format: PropTypes.string.isRequired,
    value: PropTypes.string,
    onChange: PropTypes.func.isRequired,
    placeholder: PropTypes.string,
  };

  onRef = (ref) => {
    this.ref = ref;
  };

  onClick = () => {
    this.ref.setExpanded(true);
  };

  onChange = date =>
    this.props.onChange(moment(date, this.props.format).toISOString());

  render() {
    const { format, value, placeholder } = this.props;
    return (
      <DateField
        ref={this.onRef}
        dateFormat={format}
        value={value ? formatDatetime(value, format) : null}
        onChange={this.onChange}
        updateOnDateClick
        collapseOnDateClick
        showClock={false}
        clearIcon
        renderCalendarIcon={false}
        theme={'input'}
      >
        <input className={styles.fixIeCloseIcon} placeholder={placeholder} onClick={this.onClick} />
        <MonthView footer={false} />
      </DateField>
    );
  }
}
