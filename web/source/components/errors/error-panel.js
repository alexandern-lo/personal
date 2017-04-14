import _ from 'lodash';
import ApiError from 'store/api/error';

import styles from './error-panel.module.css';

export default class ErrorPanel extends Component {

  static propTypes = {
    className: PropTypes.string,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    timeout: PropTypes.number,
    onClear: PropTypes.func,
  };

  static defaultProps = {
    message: 'Unknown error',
  }

  componentDidMount() {
    const { timeout, onClear } = this.props;
    if (timeout > 0 && onClear) {
      this.timer = setTimeout(() => {
        this.props.onClear();
      }, timeout);
    }
  }

  componentWillUnmount() {
    clearTimeout(this.timer);
  }

  getMessage = (error) => {
    if (error instanceof ApiError) {
      return _.map(error.errors, err => <span>{err.message}</span>);
    }
    return error.message;
  };

  render() {
    const { className, error, onClear } = this.props;
    if (!error) return null;
    const message = this.getMessage(error);
    if (!message || !message.length) return null;
    return (
      <div className={classNames(className, styles.panel)}>
        <i className='material-icons'>&#xE888;</i>
        <p className={styles.message}>{message}</p>
        { onClear && (
          <div className={styles.close}>
            <button onClick={onClear}>
              <i className='material-icons'>&#xE5CD;</i>
            </button>
          </div>
        )}
      </div>
    );
  }
}
