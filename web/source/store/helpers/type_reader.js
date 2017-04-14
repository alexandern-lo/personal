import {
  isAnyType,
  isCustomType,
  getCustomType,
  getCustomProp,
  getCustomReader,
} from './type_info';

import { getPath } from './property';

const exists = value => value !== null && typeof value !== 'undefined';

const makeFieldReader = (field, type) => {
  const prop = isCustomType(type) ? getCustomProp(type) || field : field;
  const path = prop.split('.');
  const reader = isCustomType(type)
    // eslint-disable-next-line no-use-before-define
    ? getCustomReader(type) || makeTypeReader(type)
    // eslint-disable-next-line no-use-before-define
    : makeTypeReader(type);
  if (path.length > 1) {
    const pathReader = getPath(path);
    return value => reader(pathReader(value));
  }
  return value => reader(value[prop]);
};

const makeTypeReader = (type) => {
  if (type === String) {
    return value => (exists(value) ? value.toString() : value);
  }
  if (type === Boolean) {
    return value => !!value;
  }
  if (type === Number) {
    return value => Number(value);
  }
  if (Array.isArray(type) && type.length === 1) {
    const reader = makeTypeReader(type[0]);
    return value => (Array.isArray(value) ? value.map(reader) : []);
  }
  if (isAnyType(type)) {
    return value => value;
  }
  if (isCustomType(type)) {
    return makeTypeReader(getCustomType(type));
  }
  if (typeof type === 'object') {
    const readers = Object.keys(type).map(field => (
      { field, reader: makeFieldReader(field, type[field]) }
    ));
    return (value) => {
      if (!exists(value)) return value;
      const result = {};
      readers.forEach(item => (result[item.field] = item.reader(value)));
      return result;
    };
  }
  console.log('Unsupported type', type); // eslint-disable-line no-console
  throw new Error(`Unsupported type ${type}`);
};

export default makeTypeReader;
