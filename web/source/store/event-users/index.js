import bind from '../helpers/bind';
import reducer, * as getters from './reducer';

const { binder, wrapAll } = bind(reducer);
export default binder;

export const {

  getCollectionState,
  getFetchParams,

} = wrapAll(getters);

export const EVENT_USER_INVITED_CONTEXT = 'EVENT-USER-INVITED';
export const EVENT_USER_NOT_INVITED_CONTEXT = 'EVENT-USER-NOT-INVITED';
