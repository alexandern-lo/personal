const devApiServer = 'avend-dev-api.azurewebsites.net';
const envApiServer = {
  production: 'api.avend.co',
  stage: 'avend-stage-api.azurewebsites.net',
};

const apiServer = process.env.LO_SERVER === ''
  ? envApiServer[process.env.NODE_ENV] || devApiServer
  : process.env.LO_SERVER;

const apiUrl = `https://${apiServer}/api/v1`;

/**
 * API base URL
 */
export default apiUrl;
export const BASE_API_URL = apiUrl;

export const TOKEN_KEY = '___token___';

/**
 * API request timeout, ms
 */
export const DEFAULT_TIMEOUT = 15000;
