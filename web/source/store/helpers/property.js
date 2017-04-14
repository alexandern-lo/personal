const object = value => typeof value === 'object';

export const getPath = path => (
  path.length > 1
  ? (value) => {
    let res = value;
    for (let i = 0; i < path.length; i += 1) {
      if (!object(res)) return undefined;
      res = res[path[i]];
    }
    return res;
  }
  : value => value && value[path[0]]
);

export default name => getPath(Array.isArray(name) ? name : name.split('.'));
