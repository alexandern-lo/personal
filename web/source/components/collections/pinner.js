import invariant from 'invariant';
import { connect } from 'store';
import { collectionShape } from 'store/collections';
import buildActions from 'store/collections/actions_builder';

import Spinner from 'components/loading-spinner/loading-spinner';
import ErrorPanel from 'components/errors/error-panel';

const itemPinner = ({
  name, getCollectionState, getItemIdFromParams, getFetchParams = getItemIdFromParams,
}) => {
  invariant(typeof name === 'string' && name.length > 0, 'please provide collection name');
  invariant(typeof getCollectionState === 'function', 'please provide getCollectionState');
  invariant(typeof getItemIdFromParams === 'function', 'please provide getItemIdFromParams');
  invariant(typeof getFetchParams === 'function', 'please provide getFetchParams');

  const { pinItemById } = buildActions(name);

  @connect({
    collection: getCollectionState,
  }, {
    pinItemById,
  })
  class CollectionItemPinner extends Component {
    static propTypes = {
      collection: collectionShape.isRequired,
      params: PropTypes.objectOf(PropTypes.string).isRequired,
      pinItemById: PropTypes.func.isRequired,
      children: PropTypes.node,
    };

    static displayName = `CollectionItemPinner(${name})`;

    componentWillMount() {
      const fetchParams = getFetchParams(this.props.params);
      this.props.pinItemById(fetchParams);
    }

    render() {
      const {
        collection: { pinnedItem, pinnedError },
        params,
        children,
      } = this.props;
      if (pinnedError) return <ErrorPanel error={pinnedError} />;
      const itemId = getItemIdFromParams(params);
      const isPinned = pinnedItem && pinnedItem.uid === itemId;
      return isPinned ? children : <Spinner />;
    }
  }
  return CollectionItemPinner;
};
export default itemPinner;
