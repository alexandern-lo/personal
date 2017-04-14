import ModalView from 'components/modal-view/modal-view';
import FetchableList from 'components/fetchable/fetchable-list';
import { searchEvents } from 'store/events/actions';
import { readEvent } from 'store/data/event';

import ErrorPanel from 'components/errors/error-panel';

import styles from './event-list-dialog.module.css';

export default class EventListDialog extends Component {
  static propTypes = {
    onClose: PropTypes.func.isRequired,
    onSelect: PropTypes.func.isRequired,
    filters: PropTypes.shape({
      start_after: PropTypes.string,
    }),
  }

  constructor(props) {
    super(props);

    this.state = { error: null };
  }

  onError = error => this.setState({ error });

  renderEvent = event => (
    <div className={styles.row}>
      <span>{event.name}</span>
    </div>
  );

  render() {
    const { filters } = this.props;
    return (
      <ModalView
        title='Сhoose an event'
        onClose={this.props.onClose}
      >
        <FetchableList
          width={480}
          height={234}
          rowHeight={42}
          renderItem={this.renderEvent}
          selected={[]}
          onSelect={this.props.onSelect}
          fetchPageAction={searchEvents}
          fetchParams={filters}
          reader={readEvent}
          searchable
          onError={this.onError}
        />
        {this.state.error &&
          <ErrorPanel
            error={this.state.error}
            onClear={() => this.setState({ error: null })}
          />}
      </ModalView>
    );
  }
}
