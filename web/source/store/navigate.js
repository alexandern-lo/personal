import invariant from 'invariant';
import { push, replace, go } from 'react-router-redux';

export {
  push,
  replace,
  go,
  goBack,
  goForward,
} from 'react-router-redux';

export default push;
export const navigate = push;
export const redirect = replace;
export const goTo = go;

export const refresh = (location, state) => {
  invariant(location, 'please provide current location to refresh');
  return replace({ ...location, state });
};
