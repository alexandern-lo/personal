import bind from '../helpers/bind';
import reducer, * as getters from './reducer';

const { binder, wrapAll } = bind(reducer);
export default binder;

export const {

  getSearchTerm,
  getActiveTab,
  isLoading,
  getError,

  getEvents,
  getLeads,
  getAttendees,

  getFetchParams,

} = wrapAll(getters);
