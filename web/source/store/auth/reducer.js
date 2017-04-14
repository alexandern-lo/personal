import { handleActions, isApiResultAction } from '../helpers/actions';

import {
  readProfile,
  isSuperAdmin as isProfileSuperAdmin,
  isTenantAdmin as isProfileTenantAdmin,
  isUser as isProfileUser,
  isAnon as isProfileAnon,
} from '../data/profile';

import { readCrmConfig } from '../data/crm-config';
import { readSubscription } from '../data/subscription';

import {
  fetchProfile,
  updateProfile,
  fetchInvite,
  loggedIn,
  signedUp,
  startTrial,
  refreshSubscription,
} from './actions';
import { setDefaultCrm } from '../crm-configs/actions';
import { acceptTerms } from '../terms/actions';

import { enableUser, disableUser } from '../users/actions';

const initialState = {
  profile: null,
  loading: false,
  error: null,

  invite: null,
  inviteLoading: false,
  inviteError: null,
};

export const getProfile = ({ profile }) => profile;
export const getProfileUID = ({ profile }) => profile && profile.user && profile.user.uid;
export const isLoading = ({ loading }) => loading;
export const getError = ({ error }) => error;

export const getInvite = ({ invite }) => invite;
export const isInviteLoading = ({ inviteLoading }) => inviteLoading;
export const getInviteError = ({ inviteError }) => inviteError;


export const getSelectedCrmConfig = ({ profile }) => (profile ? profile.crm : null);

export const isSuperAdmin = ({ profile }) => isProfileSuperAdmin(profile);
export const isTenantAdmin = ({ profile }) => isProfileTenantAdmin(profile);
export const isUser = ({ profile }) => isProfileUser(profile);
export const isAnon = ({ profile }) => isProfileAnon(profile);

const onFetchProfile = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, error: payload, loading: false };
    }
    const profile = readProfile(payload.data);
    return { ...state, profile, error: null, loading: false };
  }
  return { ...state, error: null, loading: true };
};

const onUpdateProfile = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) return state;
    const profile = readProfile(payload.data);
    return { ...state, profile };
  }
  return state;
};

const onFetchInvite = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) {
      return { ...state, invite: null, inviteError: payload, inviteLoading: false };
    }
    const invite = payload.data;
    return { ...state, invite, inviteError: null, inviteLoading: false };
  }
  return { ...state, inviteError: null, inviteLoading: true };
};

const onUpdateCRM = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) return state;
    const crm = readCrmConfig(payload.data);
    return { ...state, profile: { ...state.profile, crm } };
  }
  return state;
};

const onUpdateSubscription = (state, action) => {
  if (isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) return state;
    const subscription = readSubscription(payload.data);
    return { ...state, profile: { ...state.profile, subscription } };
  }
  return state;
};

const onUpdateTenantSubscriptionFromUser = (state, action) => {
  if (isTenantAdmin(state) && isApiResultAction(action)) {
    const { error, payload } = action;
    if (error) return state;
    const subscription = payload.data && readSubscription(payload.data.subscription);
    return { ...state, profile: { ...state.profile, subscription } };
  }
  return state;
};

export default handleActions({
  [fetchProfile]: onFetchProfile,
  [updateProfile]: onUpdateProfile,
  [fetchInvite]: onFetchInvite,
  [loggedIn]: state => ({ ...state, loading: true }),
  [signedUp]: state => ({ ...state, loading: true }),
  [setDefaultCrm]: onUpdateCRM,
  [acceptTerms]: onUpdateProfile,
  [startTrial]: onUpdateSubscription,
  [refreshSubscription]: onUpdateSubscription,
  [enableUser]: onUpdateTenantSubscriptionFromUser,
  [disableUser]: onUpdateTenantSubscriptionFromUser,
}, initialState);
