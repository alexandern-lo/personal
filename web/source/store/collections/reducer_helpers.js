import _ from 'lodash';
import invariant from 'invariant';

import { isApiResultAction } from 'store/helpers/actions';
import { nonEmpty } from 'store/helpers/data';

import buildActions, { SAME_CONTEXT } from './actions_builder';

const initialState = {
  loading: false,
  items: null, // []
  error: null,
  context: null,

  total: 0,
  totalPages: 1,
  page: 0,
  perPage: 10,

  search: '',
  filters: {},
  sortField: null,
  sortOrder: null,

  preview: null, // item
  selected: [],

  pinnedItem: null,
  pinnedError: null,
};

export const collectionShape = PropTypes.shape({
  loading: PropTypes.bool,
  items: PropTypes.arrayOf(PropTypes.object),
  error: PropTypes.shape({ message: PropTypes.string }),
  context: PropTypes.string,

  total: PropTypes.number.isRequired,
  totalPages: PropTypes.number.isRequired,
  page: PropTypes.number.isRequired,
  perPage: PropTypes.number.isRequired,

  search: PropTypes.string,
  filters: PropTypes.objectOf(PropTypes.any),
  sortField: PropTypes.string,
  sortOrder: PropTypes.string,

  preview: PropTypes.object,
  selected: PropTypes.arrayOf(PropTypes.object),
});


export const getCollectionState = ({ collection = initialState } = {}) => collection;
export const updateCollectionState = (state, fn) => ({
  ...state,
  collection: fn(getCollectionState(state)),
});

export const getPinnedItem = state => getCollectionState(state).pinnedItem;
export const getPinnedError = state => getCollectionState(state).pinnedError;

const findCollectionItem = (collection, pred) => _.find(collection.items || [], pred);
const replaceCollectionItem = (collection, newItem, pred) => {
  const mapper = item => (pred(item) ? newItem : item);
  const items = _.map(collection.items, mapper);
  const preview = collection.preview ? mapper(collection.preview) : collection.preview;
  const selected = _.map(collection.selected, mapper);
  const pinnedItem = collection.pinnedItem ? mapper(collection.pinnedItem) : collection.pinnedItem;
  return { ...collection, items, selected, preview, pinnedItem };
};
export const findItem = (state, pred) => findCollectionItem(getCollectionState(state), pred);
export const replaceItem = (state, newItem, pred) => updateCollectionState(state, collection => (
  replaceCollectionItem(collection, newItem, pred)
));

export const getFetchParams = (state) => {
  const { search, filters, sortField, sortOrder, page, perPage } = getCollectionState(state);
  const params = {
    q: search,
    ...filters,
    page,
    per_page: perPage,
  };
  if (sortField && sortField.length > 0 && sortOrder && sortOrder.length > 0) {
    params.sort_field = sortField;
    params.sort_order = sortOrder;
  }
  return _.pickBy(params, nonEmpty);
};

const fixPage = (state) => {
  const totalPages = state.total > 0 ? Math.ceil(state.total / state.perPage) : 1;
  return {
    ...state,
    totalPages,
    page: Math.min(state.page, totalPages - 1),
  };
};

const updateItem = (state, item) => {
  const { items } = state;
  const i = _.findIndex(items, o => o.uid === item.uid);
  if (i >= 0) {
    const newItems = items.slice();
    newItems[i] = item;
    return { ...state, items: newItems };
  }
  return state;
};

export const onUpdateItem = (state, item) =>
  updateCollectionState(state, collection => updateItem(collection, item));

const onFetchItems = reader => (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, loading: false, error: payload };
    }
    const items = _.map(payload.data, reader);
    const total = payload.total_filtered_records;
    const previewUID = state.preview && state.preview.uid;
    const preview = previewUID ? _.find(items, item => item.uid === previewUID) : null;
    const pinnedUID = state.pinnedItem && state.pinnedItem.uid;
    const pinnedItem = pinnedUID
      ? (_.find(items, item => item.uid === pinnedUID) || state.pinnedItem)
      : null;
    return fixPage({
      ...state,
      items,
      total,
      preview,
      selected: [],
      pinnedItem,
      search: payload.q || state.search,
      filters: payload.filters || state.filters,
      sortField: payload.sort_field || state.sortField,
      sortOrder: payload.sort_order || state.sortOrder,
      loading: false,
      error: null,
    });
  }
  const { payload } = action;
  const context = payload === SAME_CONTEXT ? state.context : payload;
  return { ...state, context, loading: true, error: null };
};

const onPinItem = reader => (collection, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...collection, pinnedError: payload };
    }
    const item = reader(payload.data);
    return {
      ...replaceCollectionItem(collection, item, other => other.uid === item.uid),
      pinnedItem: item,
      pinnedError: null,
    };
  }
  const uid = action.payload;
  const current = collection.pinnedItem;
  const pinnedItem = current && current.uid === uid
    ? current
    : findCollectionItem(collection, item => item.uid === uid);
  return { ...collection, pinnedItem, pinnedError: null };
};

const handlers = h => _.mapValues(h, fn => (state, action) => (
  updateCollectionState(state, collection => fn(collection, action))
));


export const actionHandlers = (name, reader) => {
  invariant(typeof reader === 'function', 'you should provide reader function for collection');
  const {
    fetchItems, clearFetchError,
    previewItem, selectItems, sortItems,
    changePage, changePerPage, search,
    changeFilter, pinItemById,
  } = buildActions(name);
  return handlers({

    [fetchItems]: onFetchItems(reader),
    [clearFetchError]: state => ({ ...state, error: null }),

    [pinItemById]: onPinItem(reader),

    [previewItem]: (state, { payload }) => ({ ...state, preview: payload }),
    [selectItems]: (state, { payload }) => ({ ...state, selected: payload }),

    [changePage]: (state, { payload }) => fixPage({ ...state, page: payload }),
    [changePerPage]: (state, { payload }) => fixPage({ ...state, perPage: payload }),

    [search]: (state, { payload } = {}) => ({ ...state, search: payload }),
    [changeFilter]: (state, { payload } = {}) =>
      ({ ...state, filters: { ...state.filters, ...payload } }),

    [sortItems]: (state, { payload: { sortField, sortOrder } }) =>
      ({ ...state, sortField, sortOrder }),
  });
};
