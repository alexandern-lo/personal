import styles from './pagination.module.css';

export default class Pagination extends PureComponent {
  static propTypes = {
    className: PropTypes.string,
    page: PropTypes.number.isRequired,
    totalPages: PropTypes.number.isRequired,
    perPage: PropTypes.number.isRequired,
    perPageOptions: PropTypes.arrayOf(PropTypes.number).isRequired,
    onChangePage: PropTypes.func.isRequired,
    onChangePerPage: PropTypes.func.isRequired,
  };

  onNextPage = () => {
    const { page, totalPages } = this.props;
    this.props.onChangePage(Math.min(totalPages - 1, page + 1));
  };
  onPrevPage = () => this.props.onChangePage(Math.max(0, this.props.page - 1));
  onLastPage = () => this.props.onChangePage(Math.max(0, this.props.totalPages - 1));
  onFirstPage = () => this.props.onChangePage(0);

  onChangePerPage = event => this.props.onChangePerPage(parseInt(event.target.value, 10));

  render() {
    const { className, page, totalPages, perPage, perPageOptions } = this.props;
    const isMin = page <= 0;
    const isMax = page >= totalPages - 1;
    const options = perPageOptions.map(pp => <option key={pp} value={pp}>{pp}</option>);
    return (
      <div className={classNames(className, styles.wrapper)}>
        <div className={styles.container}>
          <button className={styles.firstPage} disabled={isMin} onClick={this.onFirstPage} />
          <button className={styles.prevPage} disabled={isMin} onClick={this.onPrevPage} />
          <span className={styles.indicator}>
            <span>{page + 1}</span> of {totalPages}
          </span>
          <button className={styles.nextPage} disabled={isMax} onClick={this.onNextPage} />
          <button className={styles.lastPage} disabled={isMax} onClick={this.onLastPage} />
          <select className={styles.perPage} value={perPage} onChange={this.onChangePerPage}>
            {options}
          </select>
          <span className={styles.perPageHint}>Records<br />per page</span>
        </div>
      </div>
    );
  }
}
