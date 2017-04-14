import qs from 'query-string';

const host = 'https://login.microsoftonline.com/';

export default class Adal {
  constructor(config) {
    this.config = { ...config };
    if (!this.config.tenant) throw new Error('tenant is required');
    if (!this.config.clientId) throw new Error('clientId is required');
    if (!this.config.scope) {
      this.config.scope = ['openid'];
    }
  }

  getLoginURL = ({ state, params: moreParams } = {}) => {
    const {
      tenant,
      clientId,
      redirectUri,
      scope,
      policy,
      params,
    } = this.config;

    const query = qs.stringify({
      response_type: 'id_token',
      client_id: clientId,
      scope: scope.join(' '),
      redirect_uri: redirectUri,
      state,
      p: policy,
      nux: 1,
      ...params,
      ...moreParams,
    });
    return `${host}${tenant}/oauth2/v2.0/authorize?${query}`;
  };

  getLogOutURL = ({ redirectUri } = {}) => {
    const query = qs.stringify({
      post_logout_redirect_uri: redirectUri,
    });
    return `${host}${this.config.tenant}/oauth2/v2.0/logout?${query}`;
  };
}
