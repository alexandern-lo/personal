import invariant from 'invariant';
import prop from './property';

const isString = obj => typeof obj === 'string';
const isObject = obj => typeof obj === 'object';
const isFunction = obj => typeof obj === 'function';

export default function bind(reducer) {
  let stateGetter = () => undefined;

  function binder(propName) {
    invariant(isString(propName) && propName.length > 0, '[bind]: state prop should be non empty string');
    stateGetter = prop(propName);
    return reducer;
  }

  function get(state) {
    return stateGetter(state);
  }

  function wrapper(getter, name) {
    invariant(isFunction(getter), `[bind]: state getter ${name} should be function`);
    return getter.length > 1
      ? (state, arg, ...args) => getter(stateGetter(state), arg, ...args)
      : state => getter(stateGetter(state));
  }

  function wrapAll(getters) {
    invariant(isObject(getters), '[bind]: state getters should be object');
    const wrapped = {};
    Object.keys(getters).forEach((key) => {
      wrapped[key] = wrapper(getters[key], key);
    });
    return wrapped;
  }

  return { binder, wrapper, wrapAll, get };
}

export function bindReducers(binders, prefix = '') {
  const reducers = {};
  Object.keys(binders).forEach((name) => {
    const binder = binders[name];
    invariant(isFunction(binder), `[bind]: reducer binder '${name}' should be function`);
    const reducer = binder(prefix + name);
    invariant(isFunction(reducer), `[bind]: reducer '${name}' should be function`);
    reducers[name] = reducer;
  });
  return reducers;
}
