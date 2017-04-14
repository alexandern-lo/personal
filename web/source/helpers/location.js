
export const isDevelopment = process.env.NODE_ENV === 'development'
  ? window.location.hostname.indexOf('localhost') >= 0
  : false;
