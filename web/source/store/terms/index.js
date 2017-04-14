import bind from '../helpers/bind';
import reducer, * as getters from './reducer';

const { binder, wrapAll } = bind(reducer);
export default binder;

export const {

  getLatestTerms,
  getError,
  isLoading,

} = wrapAll(getters);
