import invariant from 'invariant';

export const isApiCallAction = action => typeof action.apiCall === 'string';
export const isApiResultAction = action => !!action.apiCallResult;
export const isErrorAction = action => !!action.error;

const id = v => v;
const makeCreator = (creator, def) => {
  if (typeof creator === 'function') return creator;
  if (typeof def === 'function') return def;
  return () => def;
};


export const createApiCallAction = (type, apiCall, payloadCreator_, meta) => {
  invariant(typeof type === 'string', 'action type for createApiCallAction should be string');
  invariant(typeof apiCall === 'string' || typeof apiCall === 'function',
    'apiCall for createApiCallAction should be string or function');
  const payloadCreator = makeCreator(payloadCreator_, id);
  const metaCreator = makeCreator(meta, meta);
  const creator = payload => ({
    type,
    apiCall,
    payload: payloadCreator(payload),
    meta: metaCreator(payload),
  });
  creator.toString = () => type;
  return creator;
};


export const buildResultActionData = (type, payload, meta) => ({
  type,
  apiCallResult: true,
  error: payload instanceof Error,
  payload,
  meta,
});

export const createApiCallResultAction = (type, payloadCreator_, meta) => {
  const typeString = type.toString();
  const payloadCreator = makeCreator(payloadCreator_, id);
  const metaCreator = makeCreator(meta, meta);
  const creator = payload =>
    buildResultActionData(typeString, payloadCreator(payload), metaCreator(payload));
  creator.toString = () => typeString;
  return creator;
};
