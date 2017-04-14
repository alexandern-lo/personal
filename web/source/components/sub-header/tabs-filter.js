import styles from './tabs-filter.module.css';

const TabsFilter = ({ className, filters, selected, onSelect }) => (
  <div className={classNames(className, styles.panel)}>
    { Object.keys(filters).map(name => (
      <a
        key={name}
        className={classNames(styles.tab, { [styles.selected]: name === selected })}
        onClick={() => onSelect(name)}
      >
        { filters[name] }
      </a>
    ))}
  </div>
);
export default TabsFilter;

TabsFilter.propTypes = {
  className: PropTypes.string,
  filters: PropTypes.objectOf(
    PropTypes.string.isRequired,
  ).isRequired,
  selected: PropTypes.string,
  onSelect: PropTypes.func.isRequired,
};
