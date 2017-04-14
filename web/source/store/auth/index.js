import bind from '../helpers/bind';
import reducer, * as getters from './reducer';

const { binder, wrapAll } = bind(reducer);
export default binder;

export const {

  getProfile,
  getProfileUID,
  isLoading,
  getError,

  getInvite,
  isInviteLoading,
  getInviteError,

  isSuperAdmin,
  isTenantAdmin,
  isUser,
  isAnon,

  getSelectedCrmConfig,

} = wrapAll(getters);
