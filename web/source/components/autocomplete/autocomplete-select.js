import _ from 'lodash';
import Select from 'react-select';

import './autocomplete-select.styles.scss';
import styles from './autocomplete-select.module.css';

export default class AutocompleteSelect extends Component {
  static propTypes = {
    className: PropTypes.string,

    value: PropTypes.string,
    onChange: PropTypes.func.isRequired,
    invalid: PropTypes.bool,

    limit: PropTypes.number,
    fetcher: PropTypes.func.isRequired,
    reader: PropTypes.func.isRequired,

    clearOption: PropTypes.shape({
      label: PropTypes.string,
    }),
  };

  static defaultProps = {
    limit: 50,
  };

  onChange = option => this.props.onChange(option ? option.value : null);

  loadOptions = (query, callback) => this.props.fetcher({
    q: query,
    per_page: this.props.limit,
  })
  .then(resp => _.map(resp.data, this.props.reader))
  .then((options) => {
    const clearOpt = this.props.clearOption;
    callback(null, { options: clearOpt ? [clearOpt].concat(options) : options });
  })
  .catch(error => callback(error));

  render() {
    const {
      limit, fetcher, reader, // eslint-disable-line no-unused-vars
      className,
      invalid,
      ...props
    } = this.props;
    return (
      <Select.Async
        className={classNames(className, styles.select, {
          [styles.invalid]: invalid,
        })}
        {...props}
        onChange={this.onChange}
        loadOptions={this.loadOptions}
      />
    );
  }
}
