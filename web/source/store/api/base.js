import axios, { CancelToken, isCancel } from 'axios';
import { CANCEL } from 'redux-saga/lib/internal/utils';

import ApiError from './error';

export default class ApiBase {

  constructor({ baseURL, timeout }) {
    this.axios = axios.create({
      baseURL,
      timeout,
    });
  }

  get(url, options) {
    const { cancel, config } = this.processOptions(options);
    return this.processResponse(this.axios.get(url, config), cancel);
  }

  post(url, data, options) {
    const { cancel, config } = this.processOptions(options);
    return this.processResponse(this.axios.post(url, data, config), cancel);
  }

  patch(url, data, options) {
    const { cancel, config } = this.processOptions(options);
    return this.processResponse(this.axios.patch(url, data, config), cancel);
  }

  put(url, data, options) {
    const { cancel, config } = this.processOptions(options);
    return this.processResponse(this.axios.put(url, data, config), cancel);
  }

  delete(url, options) {
    const { cancel, config } = this.processOptions(options);
    return this.processResponse(this.axios.delete(url, config), cancel);
  }

  uploadFile({ url, file, ...options }) {
    const { cancel, config } = this.processOptions(options, { noAuth: true });
    config.headers = {
      ...config.headers,
      'x-ms-blob-type': 'BlockBlob',
      'Content-Type': file.type,
    };
    return this.processResponse(this.axios.put(url, file, config), cancel);
  }

  processOptions(options, { noAuth } = {}) {
    const config = { ...options };
    let cancel;
    if (!config.cancelToken) {
      const source = CancelToken.source();
      config.cancelToken = source.token;
      cancel = source.cancel;
    }
    if (!noAuth) {
      const authToken = this.getAuthToken ? this.getAuthToken() : undefined;
      if (authToken) {
        config.headers = {
          ...config.headers,
          Authorization: `Bearer ${authToken}`,
        };
      }
    }
    const { headers = {} } = config;
    if (!headers['Content-Type']) {
      config.headers = {
        ...config.headers,
        'Content-Type': 'application/json',
      };
    }
    return { cancel, config };
  }

  processResponse(promise, cancel) {
    const response = promise
      .then(reply => ({ reply }))
      .catch(error => (isCancel(error)
        ? {}
        : { error: this.processError(error) }
      ))
      .then(({ reply, error }) => {
        if (error) throw error;
        if (!reply) return null;
        return reply.config.fullResponse ? reply : reply.data;
      });
    if (cancel) {
      response[CANCEL] = cancel;
      response.cancel = cancel;
    }
    return response;
  }

  processError(error) {
    const response = error.response;
    const status = response && response.status;
    const data = response && response.data;
    const result =
        (data && this.errorFromData(data))
        || (status && this.errorFromStatus(status))
        || error;
    if (!result.status) {
      result.status = status;
    }
    return result;
  }

  errorFromData(data) { // eslint-disable-line class-methods-use-this
    if (typeof data === 'string') {
      return new ApiError(data);
    }
    return new ApiError(data.message || 'Server error', data.errors);
  }

  errorFromStatus(status) { // eslint-disable-line class-methods-use-this
    switch (status) {
      case 401: return new ApiError('Authentication failed');
      default: return null;
    }
  }
}
