import bind from '../helpers/bind';
import reducer, * as getters from './reducer';

const { binder, get, wrapAll } = bind(reducer);
export default binder;

export const getApi = get;

export const {

  haveToken,

} = wrapAll(getters);
