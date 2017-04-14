import { connect } from 'react-redux';
import Select from 'components/autocomplete/autocomplete-select';

import styles from './autocomplete-filter.module.css';

const clearRenderer = () => null;

@connect(null, null)
export default class AutocompleteFilter extends Component {
  static propTypes = {
    name: PropTypes.string.isRequired,
    filters: PropTypes.objectOf(PropTypes.string).isRequired,
    onFilterChange: PropTypes.func.isRequired,
    fetchPageAction: PropTypes.func.isRequired,
    reader: PropTypes.func.isRequired,
    dispatch: PropTypes.func.isRequired,
  };

  onChange = (opt) => {
    const { onFilterChange, name } = this.props;
    onFilterChange({ [name]: opt });
  };

  fetcher = params => this.props.dispatch(this.props.fetchPageAction(params));

  render() {
    const { name, filters, reader, ...props } = this.props;
    return (
      <Select
        {...props}
        className={styles.select}
        value={filters[name]}
        onChange={this.onChange}
        fetcher={this.fetcher}
        reader={reader}
        arrowRenderer={() => null}
        clearRenderer={clearRenderer}
      />
    );
  }
}
