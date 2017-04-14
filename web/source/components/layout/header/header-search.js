import styles from './header.module.css';

export default class HeaderSearch extends Component {
  static propTypes = {
    className: PropTypes.string,
    searchTerm: PropTypes.string,
    onSearch: PropTypes.func.isRequired,
  };

  componentWillMount() {
    this.setState({ searchTerm: this.props.searchTerm });
  }

  componentWillReceiveProps(nextProps) {
    const { searchTerm } = this.state || {};
    if (searchTerm === this.props.searchTerm) {
      this.setState({ searchTerm: nextProps.searchTerm });
    }
  }

  onKeyPress = (e) => {
    if (e.key === 'Enter') {
      this.props.onSearch(e.target.value);
      e.target.blur();
    }
  };

  onChange = e => this.setState({ searchTerm: e.target.value });

  render() {
    const { className } = this.props;
    const { searchTerm = '' } = this.state || {};
    return (
      <div className={classNames(className, styles.search)}>
        <input
          type='text'
          value={searchTerm}
          onChange={this.onChange}
          onKeyPress={this.onKeyPress}
        />
      </div>
    );
  }
}
