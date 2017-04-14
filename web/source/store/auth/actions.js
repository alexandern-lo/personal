import { createFetchAction, createAction, createApiCallAction } from '../helpers/actions';

export const login = createAction('profile/LOGIN');
export const signup = createAction('profile/SIGNUP');

export const logout = createAction('profile/LOGOUT');
export const resetPassword = createAction('profile/RESET_PASSWORD');

export const loggedIn = createFetchAction('profile/LOGGED_IN');
export const signedUp = createFetchAction('profile/SIGNED_UP');

export const fetchProfile = createFetchAction('profile/FETCH');
export const updateProfile = createApiCallAction('profile/UPDATE', 'updateProfile');

export const fetchInvite = createFetchAction('invite/FETCH');

export const refreshSubscription = createApiCallAction('profile/REFRESH_SUBSCRIPTION', 'refreshSubscription');
export const startTrial = createApiCallAction('profile/START_TRIAL', 'startTrialSubscription');
