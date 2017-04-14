import { createFetchAction, createAction } from '../helpers/actions';

export const search = createFetchAction('search/SEARCH');
export const clearError = createAction('search/CLEAR_ERROR');

export const previewItem = createAction('search/PREVIEW');

export const changeTab = createAction('search/CHANGE_TAB');
export const changePage = createAction('search/CHANGE_PAGE');
export const changePerPage = createAction('search/CHANGE_PER_PAGE');

export const sortItems = createAction('search/SORT');
