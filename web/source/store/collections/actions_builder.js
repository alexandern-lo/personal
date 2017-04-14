import invariant from 'invariant';
import { createFetchAction, createAction } from '../helpers/actions';

export const SAME_CONTEXT = '__COLLECTION__SAME__CONTEXT__';

export default (name) => {
  invariant(name && name.length > 0, 'you should provide collection name to generate actions');
  return {
    fetchItems: createFetchAction(`${name}/COLLECTION/FETCH`, (payload = SAME_CONTEXT) => payload),
    clearFetchError: createAction(`${name}/COLLECTION/CLEAR_FETCH_ERROR`),
    previewItem: createAction(`${name}/COLLECTION/PREVIEW`),
    selectItems: createAction(`${name}/COLLECTION/SELECT`),
    sortItems: createAction(`${name}/COLLECTION/SORT`),
    changePage: createAction(`${name}/COLLECTION/CHANGE_PAGE`),
    changePerPage: createAction(`${name}/COLLECTION/CHANGE_PER_PAGE`),
    search: createAction(`${name}/COLLECTION/SEARCH`),
    changeFilter: createAction(`${name}/COLLECTION/CHANGE_FILTER`),

    pinItemById: createFetchAction(`${name}/COLLECTION/PIN_BY_UID`),
  };
};
