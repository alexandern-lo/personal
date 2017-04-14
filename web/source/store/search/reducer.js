import _ from 'lodash';
import { handleActions, isApiResultAction } from 'store/helpers/actions';
import { nonEmpty } from 'store/helpers/data';

import { readEvent } from 'store/data/event';
import { readLead } from 'store/data/lead';
import { readAttendee } from 'store/data/attendee';

import {
  search,
  clearError,
  previewItem,
  changeTab,
  changePage,
  changePerPage,
  sortItems,
} from './actions';

const initialState = {
  term: '',
  loading: false,
  error: null,
  activeTab: 'events',
  events: null, // collection
  leads: null, // collection
  attendees: null, // collection
};

const initialCollection = {
  items: null, // []
  total: 0,
  page: 0,
  perPage: 10,
  totalPages: 1,
  preview: null,
  sortField: null,
  sortOrder: null,
};

export const getSearchTerm = ({ term }) => term;
export const getActiveTab = ({ activeTab }) => activeTab;
export const isLoading = ({ loading }) => loading;
export const getError = ({ error }) => error;

export const getEvents = ({ events }) => events || initialCollection;
export const getLeads = ({ leads }) => leads || initialCollection;
export const getAttendees = ({ attendees }) => attendees || initialCollection;

export const getFetchParams = (state, collection) => {
  const coll = state[collection] || initialCollection;
  const { page, perPage, sortField, sortOrder } = coll;
  const params = {
    q: state.term,
    page,
    per_page: perPage,
  };
  if (sortField && sortField.length > 0 && sortOrder && sortOrder.length > 0) {
    params.sort_field = sortField;
    params.sort_order = sortOrder;
  }
  return _.pickBy(params, nonEmpty);
};

const readCollection = (payload, reader, coll) => {
  const collection = coll || initialCollection;
  if (!payload) return collection;
  const items = _.map(payload.data, reader);
  const total = payload.total_filtered_records;
  const totalPages = total > 0 ? Math.ceil(total / collection.perPage) : 1;
  const previewUID = collection.preview && collection.preview.uid;
  const preview = previewUID ? _.find(items, item => item.uid === previewUID) : null;
  return {
    ...collection,
    items,
    total,
    totalPages,
    preview,
    sortField: payload.sort_field || collection.sortField,
    sortOrder: payload.sort_order || collection.sortOrder,
  };
};

const onSearch = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) return { ...state, loading: false, error: payload };
    const { events, leads, attendees } = payload;
    return {
      ...state,
      events: readCollection(events, readEvent, state.events),
      leads: readCollection(leads, readLead, state.leads),
      attendees: readCollection(attendees, readAttendee, state.attendees),
      loading: false,
      error: null,
    };
  }
  const term = typeof action.payload === 'string' ? action.payload : state.term;
  const sameTerm = term === state.term;
  return { ...(sameTerm ? state : initialState), term, loading: true };
};

const updatePreview = (coll, item) => ({ ...coll, preview: item });

const onPreview = (state, { payload }) => {
  switch (getActiveTab(state)) {
    case 'events': return { ...state, events: updatePreview(state.events, payload) };
    case 'leads': return { ...state, leads: updatePreview(state.leads, payload) };
    case 'attendees': return { ...state, attendees: updatePreview(state.attendees, payload) };
    default: throw new Error('there is no collection in previewItem action');
  }
};

const updateSort = (coll, { sortField, sortOrder }) => ({ ...coll, page: 0, sortField, sortOrder });

const onSortItems = (state, { payload }) => {
  switch (getActiveTab(state)) {
    case 'events': return { ...state, events: updateSort(state.events, payload) };
    case 'leads': return { ...state, leads: updateSort(state.leads, payload) };
    case 'attendees': return { ...state, attendees: updateSort(state.attendees, payload) };
    default: throw new Error('there is no collection in sortItems action');
  }
};

const updatePage = (coll, page) => {
  const totalPages = coll.total > 0 ? Math.ceil(coll.total / coll.perPage) : 1;
  return { ...coll, totalPages, page: Math.min(page, totalPages - 1) };
};

const onChangePage = (state, { payload }) => {
  switch (getActiveTab(state)) {
    case 'events': return { ...state, events: updatePage(state.events, payload) };
    case 'leads': return { ...state, leads: updatePage(state.leads, payload) };
    case 'attendees': return { ...state, attendees: updatePage(state.attendees, payload) };
    default: throw new Error('there is no collection in changePage action');
  }
};

const onChangePerPage = (state, { payload }) => ({
  ...state,
  events: { ...state.events, page: 0, perPage: payload },
  leads: { ...state.leads, page: 0, perPage: payload },
  attendees: { ...state.attendees, page: 0, perPage: payload },
});

export default handleActions({
  [search]: onSearch,
  [clearError]: state => ({ ...state, error: null }),
  [previewItem]: onPreview,
  [changeTab]: (state, { payload }) => ({ ...state, activeTab: payload }),
  [changePage]: onChangePage,
  [changePerPage]: onChangePerPage,
  [sortItems]: onSortItems,
}, initialState);
