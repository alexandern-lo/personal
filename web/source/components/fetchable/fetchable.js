import _ from 'lodash';
import { connect } from 'react-redux';
import { diff } from './helper';

const loadPage = (dispatch, action, reader, params) => dispatch(action(params))
  .then(payload => ({
    total: payload.total_filtered_records,
    items: _.map(payload.data, reader),
  }));

const optionalCallback = (callback, ...args) => {
  if (callback) {
    return callback(...args);
  }
  return undefined;
};

export default () => (Base) => {
  const displayName = `Fetchable(${Base.displayName || Base.name || 'Component'})`;

  @connect(null, null, null, { withRef: true })
  class Fetchable extends Component {
    static WrappedComponent = Base;
    static displayName = displayName;

    static propTypes = {
      fetchPageAction: PropTypes.func.isRequired,
      fetchParams: PropTypes.shape({}),
      reader: PropTypes.func.isRequired,
      searchable: PropTypes.bool,
      dispatch: PropTypes.func.isRequired,
      perPage: PropTypes.number.isRequired,
      onFetching: PropTypes.func,
      onFetched: PropTypes.func,
      onError: PropTypes.func,
    };

    static defaultProps = {
      perPage: 10,
    };

    constructor(props) {
      super(props);
      this.state = {
        items: null,
        search: '',
        error: null,
        page: 0,
        total: 0,
        fetching: false,
      };
    }

    componentWillReceiveProps(nextProps) {
      if (this.propsDiff(nextProps)) {
        this.setState({
          items: null,
          search: '',
          error: null,
          page: 0,
          total: 0,
          fetching: false,
        });
      }
    }

    onSearch = (value) => {
      this.setState({
        ...this.state,
        page: 0,
        total: 0,
        items: null,
        search: value,
        error: null,
        fetching: false,
      });
    };

    propsDiff = nextProps => diff(this.props, nextProps,
      ['fetchPageAction', 'reader', 'perPage', 'searchable']);

    hasMore = () => {
      if (!this.state.items) {
        return true;
      }
      return this.state.page < this.totalPages();
    };
    totalPages = () => Math.ceil(this.state.total / this.props.perPage);

    fetchParams = () => {
      const params = {
        per_page: this.props.perPage,
        page: this.state.page,
        ...this.props.fetchParams,
      };
      if (this.props.searchable) {
        params.q = this.state.search;
      }


      return params;
    };

    loadMore = () => {
      const {
        dispatch, fetchPageAction, reader,
        onFetching, onFetched, onError,
      } = this.props;
      optionalCallback(onFetching);
      this.setState({ ...this.state, fetching: true });
      const search = this.state.search;
      return loadPage(dispatch, fetchPageAction, reader, this.fetchParams())
        .then(({ items, total }) => {
          if (this.state.search === search) {
            this.setState({
              ...this.state,
              page: this.state.page + 1,
              total,
              items: (this.state.items || []).concat(items),
              fetching: false,
            });
            optionalCallback(onFetched);
          }
        })
        .catch((error) => {
          if (this.state.search === search) {
            this.setState({ ...this.state, error, fetching: false });
            optionalCallback(onError, error);
          }
        });
    };

    registerRef = (ref) => { this.wrappedInstance = ref; };

    render() {
      const { search, items, error, fetching } = this.state;

      return (
        <Base
          {...this.props}
          ref={this.registerRef}
          search={this.props.searchable ? search : null}
          items={items || []}
          error={error}
          fetching={fetching}
          hasMore={this.hasMore()}
          onSearch={this.props.searchable ? this.onSearch : null}
          loadMoreRows={this.loadMore}
        />
      );
    }
  }
  return Fetchable;
};
