import { PropTypes } from 'react';

import {
  isAnyType,
  isCustomType,
  isRequired,
  getCustomType,
} from './type_info';

export const optional = (type, from) => {
  const data = typeof type === 'object'
    ? { ...type }
    : { type };
  if (typeof from === 'string') {
    data.from = from;
  }
  return data;
};

export const required = (type, from) => {
  const data = typeof type === 'object'
    ? { ...type, required: true }
    : { required: true, type };
  if (typeof from === 'string') {
    data.from = from;
  }
  return data;
};

const makeTypeShape = (type) => {
  if (type === String) {
    return PropTypes.string;
  }
  if (type === Boolean) {
    return PropTypes.bool;
  }
  if (type === Number) {
    return PropTypes.number;
  }
  if (Array.isArray(type) && type.length === 1) {
    return PropTypes.arrayOf(makeTypeShape(type[0]));
  }
  if (isAnyType(type)) {
    return PropTypes.object;
  }
  if (isCustomType(type)) {
    const typeShape = makeTypeShape(getCustomType(type));
    return isRequired(type) ? typeShape.isRequired : typeShape;
  }
  if (typeof type === 'object') {
    const shape = {};
    Object.keys(type).forEach((field) => {
      shape[field] = makeTypeShape(type[field]);
    });
    return PropTypes.shape(shape);
  }
  console.log('Unsupported type', type); // eslint-disable-line no-console
  throw new Error(`Unsupported type ${type}`);
};

export default makeTypeShape;
