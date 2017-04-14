import { parseJson } from 'store/helpers/json';
import Adal from 'helpers/adal';
import { TOKEN_KEY } from 'config/api.config';
import {
  adalConfigSignIn,
  adalConfigSignUp,
  adalConfigResetPassword,
} from 'config/adal.config';

import {
  fromStore,
  apiCaller,
  dispatchApiCall,
} from '../helpers/sagas';

import {
  createApiCallResultAction,
} from '../helpers/actions';

import {
  login,
  signup,
  logout,
  resetPassword,
  fetchProfile,
  fetchInvite,
  loggedIn,
  signedUp,
} from './actions';

import { getProfile } from './index';

const adalSignin = new Adal(adalConfigSignIn);
const adalSignup = new Adal(adalConfigSignUp);
const adalResetPwd = new Adal(adalConfigResetPassword);

const profileResponse = createApiCallResultAction(fetchProfile);
const inviteResponse = createApiCallResultAction(fetchInvite);

function onLogin() {
  sessionStorage.removeItem(TOKEN_KEY);
  window.location = adalSignin.getLoginURL();
}

function* onLoggedIn({ payload: token }) {
  sessionStorage.setItem(TOKEN_KEY, token);
  yield dispatchApiCall(profileResponse, 'fetchProfile');
}

function onSignup({ payload }) {
  sessionStorage.removeItem(TOKEN_KEY);
  const { code, email } = payload || {};
  window.location = adalSignup.getLoginURL({
    state: code ? btoa(JSON.stringify({ invite: code })) : null,
    params: {
      login_hint: email,
    },
  });
}

function* onSignedUp({ payload }) {
  const { token, state } = payload || {};
  const { invite } = parseJson(atob(state || '')) || {};
  sessionStorage.setItem(TOKEN_KEY, token);
  yield dispatchApiCall(profileResponse, 'fetchProfile', { invite_code: invite });
}

function onLogout() {
  sessionStorage.removeItem(TOKEN_KEY);
  window.location = adalSignin.getLogOutURL();
}

function* onResetPassword() {
  const profile = yield fromStore(getProfile);
  const email = profile && profile.user ? profile.user.email : null;
  window.location = adalResetPwd.getLoginURL({
    params: { login_hint: email },
  });
}

export default {
  [fetchProfile]: apiCaller('fetchProfile', profileResponse),
  [fetchInvite]: apiCaller('fetchInvite', inviteResponse),
  [loggedIn]: onLoggedIn,
  [signedUp]: onSignedUp,
  [login]: onLogin,
  [signup]: onSignup,
  [logout]: onLogout,
  [resetPassword]: onResetPassword,
};
