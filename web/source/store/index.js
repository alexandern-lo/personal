import invariant from 'invariant';
import { bindActionCreators } from 'redux';
import { connect as reduxConnect } from 'react-redux';

export function bindProps(props) {
  invariant(typeof props === 'object', '[connect]: getters should be an object');
  return function bindStateToProps(state) {
    const boundProps = {};
    Object.keys(props).forEach((prop) => {
      const getter = props[prop];
      invariant(typeof getter === 'function', `[connect]: getter '${prop}' should be function`);
      boundProps[prop] = getter.length > 1
        ? (arg, ...args) => getter(state, arg, ...args)
        : getter(state);
    });
    return boundProps;
  };
}

export function bindActions(actions) {
  invariant(typeof actions === 'object', '[connect]: actions should be an object');
  return function bindActionsToDispatch(dispatch) {
    return bindActionCreators(actions, dispatch);
  };
}

export function connect(props, actions, options) {
  return reduxConnect(bindProps(props), actions && bindActions(actions), undefined, options);
}

export function connectActions(actions, options) {
  return reduxConnect(null, bindActions(actions), undefined, options);
}
