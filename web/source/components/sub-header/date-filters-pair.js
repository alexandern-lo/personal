import DatePicker from 'components/date-picker/date-picker';

import styles from './date-filters-pair.module.css';

const DateFiltersPair = ({ filters, onFilterChange, start, end, format }) => (
  <div className={styles.container}>
    <div className={styles.dataPickerWrapper}>
      <DatePicker
        format={format}
        value={filters[start.field]}
        onChange={date => onFilterChange({ [start.field]: date })}
        placeholder={start.title}
      />
    </div>
    <div className={styles.dash} />
    <div className={styles.dataPickerWrapper}>
      <DatePicker
        format={format}
        value={filters[end.field]}
        onChange={date => onFilterChange({ [end.field]: date })}
        placeholder={end.title}
      />
    </div>
  </div>
);
export default DateFiltersPair;

DateFiltersPair.propTypes = {
  filters: PropTypes.objectOf(PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number,
  ])),
  onFilterChange: PropTypes.func.isRequired,
  start: PropTypes.shape({
    title: PropTypes.string.isRequired,
    field: PropTypes.string.isRequired,
  }).isRequired,
  end: PropTypes.shape({
    title: PropTypes.string.isRequired,
    field: PropTypes.string.isRequired,
  }).isRequired,
  format: PropTypes.string.isRequired,
};

DateFiltersPair.defaultProps = {
  format: 'LL',
};
