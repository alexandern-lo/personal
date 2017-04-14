import Select from 'components/select/select';
import styles from './sub-header-filters.module.css';

export default class SubHeaderFilters extends PureComponent {

  static propTypes = {
    searchTerm: PropTypes.string,
    searchPlaceholder: PropTypes.string,
    searchDelay: PropTypes.number,
    onSearch: PropTypes.func,
    filtersConfig: PropTypes.objectOf(PropTypes.shape({
      title: PropTypes.string,
      options: PropTypes.objectOf(PropTypes.string),
      render: PropTypes.func,
    })),
    filters: PropTypes.objectOf(PropTypes.oneOfType([
      PropTypes.string,
      PropTypes.number,
    ])),
    onFilterChange: PropTypes.func,
    children: PropTypes.node,
  };

  static defaultProps = {
    searchPlaceholder: 'Search for',
    searchDelay: 300,
  };

  componentWillUnmount = () => {
    clearTimeout(this.timeout);
  };

  onSearch = (e) => {
    const { value } = e.target;
    if (value != this.props.searchTerm) { // eslint-disable-line eqeqeq
      clearTimeout(this.timeout);
      this.timeout = setTimeout(() => {
        this.props.onSearch(value);
      }, this.props.searchDelay);
    }
  };

  renderSearch = () => {
    const { searchTerm, searchPlaceholder } = this.props;
    return (
      <input
        type='text'
        className={styles.search}
        onChange={this.onSearch}
        defaultValue={searchTerm}
        placeholder={searchPlaceholder}
      />
    );
  }

  renderFilters = () => {
    const { filters, filtersConfig, onFilterChange } = this.props;
    const filterTypes = Object.keys(filtersConfig || {});
    return filterTypes.map((name) => {
      const value = filters[name];
      const { title, options = {}, render } = filtersConfig[name];
      const onChange = v => onFilterChange({ [name]: v });
      if (render) return render({ name, value, onChange, filters, onFilterChange });
      return (
        <Select
          key={name}
          className={styles.select}
          placeholder={title}
          onChange={onChange}
          value={value}
          options={Object.keys(options).map(opt => ({ value: opt, label: options[opt] }))}
          searchable={false}
          simpleValue
          clearable
        />
      );
    });
  }

  render() {
    const { onSearch, onFilterChange, children } = this.props;
    return (
      <div className={styles.header}>
        <div className={styles.filterGroup}>
          { onSearch && this.renderSearch() }
          { onFilterChange && this.renderFilters() }
        </div>
        { children }
      </div>
    );
  }
}
