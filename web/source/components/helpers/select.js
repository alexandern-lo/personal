import _ from 'lodash';

export const select = (array = [], item) => _.union(array, [item]);
export const deselect = (array = [], item) => _.without(array, item);
export const isSelected = (array = [], item) => _.indexOf(array, item) >= 0;
export const toggle = (array = [], item) => {
  if (isSelected(array, item)) {
    return deselect(array, item);
  }
  return select(array, item);
};
