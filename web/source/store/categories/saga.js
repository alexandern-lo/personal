import { actionDispatchers } from '../collections/saga_helpers';
import { getFetchParams } from './index';
import { getPinnedEvent } from '../events';

const getFetchCategoriesParams = store => ({
  eventUid: getPinnedEvent(store).uid,
  params: getFetchParams(store),
});

export default {
  ...actionDispatchers('categories', {
    fetch: {
      apiCall: 'fetchCategories',
      getFetchParams: getFetchCategoriesParams,
    },
    pin: {
      apiCall: 'fetchCategoryById',
    },
  }),
};
