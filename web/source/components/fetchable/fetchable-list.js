import _ from 'lodash';
import { InfiniteLoader } from 'react-virtualized/dist/commonjs/InfiniteLoader';
import { List } from 'react-virtualized/dist/commonjs/List';
import fetchable from './fetchable';
import { diff } from './helper';

import styles from './fetchable-list.module.css';

@fetchable()
export default class SearchableList extends Component {
  static propTypes = {
    className: PropTypes.string,

    search: PropTypes.string,
    items: PropTypes.arrayOf(PropTypes.any).isRequired,
    hasMore: PropTypes.bool.isRequired,
    onSearch: PropTypes.func,
    loadMoreRows: PropTypes.func.isRequired,

    selected: PropTypes.arrayOf(PropTypes.shape({})),
    onSelect: PropTypes.func,

    renderItem: PropTypes.func.isRequired,
    renderPlaceholder: PropTypes.func.isRequired,
    renderEmpty: PropTypes.func.isRequired,

    fetching: PropTypes.bool.isRequired,

    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    rowHeight: PropTypes.oneOfType([PropTypes.number, PropTypes.func]).isRequired,
  };

  static defaultProps = {
    renderPlaceholder: () => null,
    renderEmpty: () => null,
  };

  componentWillReceiveProps(nextProps) {
    if (this.propsDiff(nextProps)) {
      this.forceUpdateList();
    }
  }

  onSearch = (e) => {
    const { value } = e.target;
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => this.props.onSearch(value), 500);
  }

  propsDiff = nextProps => diff(
    this.props,
    nextProps,
    ['items', 'hasMore', 'selected',
      'renderItem', 'renderPlaceholder',
      'error',
    ]);

  forceUpdateList = () => {
    if (this.listRef) {
      this.listRef.forceUpdateGrid();
    }
  };

  rowCount = () => this.props.items.length + (this.props.hasMore ? 1 : 0);

  isRowLoaded = ({ index }) => !!this.props.items[index];

  registerList = registerChild => (ref) => {
    this.listRef = ref;
    registerChild(ref);
  };

  isSelected = item => this.props.selected && _.indexOf(this.props.selected, item) >= 0;

  renderRow = ({ index, key, style }) => {
    const { items, onSelect, renderPlaceholder, renderItem } = this.props;
    if (index < items.length) {
      const item = items[index];
      return (
        <div
          onClick={() => onSelect && onSelect(item)}
          className={styles.rowWrapper}
          key={key}
          style={style}
        >
          {renderItem(item, this.isSelected(item))}
        </div>
      );
    }
    return (
      <div className={styles.rowWrapper} key={key} style={style}>
        {renderPlaceholder()}
      </div>
    );
  };

  renderList = ({ onRowsRendered, registerChild }) => {
    const { width, height, rowHeight, renderEmpty } = this.props;
    return (
      <List
        className={styles.list}
        width={width}
        height={height}
        rowHeight={rowHeight}
        rowCount={this.rowCount()}
        rowRenderer={this.renderRow}
        onRowsRendered={onRowsRendered}
        noRowsRenderer={renderEmpty}
        ref={this.registerList(registerChild)}
      />
    );
  };

  renderLoader = () => (
    <div className={styles.loaderContainer}>
      <span className={classNames(styles.loader)} />
    </div>
  );

  render() {
    const { className, onSearch, search, loadMoreRows, fetching } = this.props;

    return (
      <div className={classNames(styles.container, className)}>
        {onSearch && (
          <div className={styles.header}>
            <input className={styles.search} onChange={this.onSearch} />
          </div>
        )}
        {fetching && this.renderLoader()}
        <InfiniteLoader
          key={search}
          className={classNames(styles.loader)}
          isRowLoaded={this.isRowLoaded}
          loadMoreRows={loadMoreRows}
          minimumBatchSize={1}
          rowCount={this.rowCount()}
        >
          {this.renderList}
        </InfiniteLoader>
      </div>
    );
  }
}
