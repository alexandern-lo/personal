import bind from '../helpers/bind';
import reducer, * as getters from './reducer';

const { binder, wrapAll } = bind(reducer);
export default binder;

export const {

  getCollectionState,
  getFetchParams,

  getPinnedResource,
  getPinnedError,

} = wrapAll(getters);
