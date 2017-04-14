
export const nonEmpty = (o) => {
  if (!o && o !== false) return false;
  if (o.length && o.length === 0) return false;
  if (typeof o === 'object' && Object.keys(o).length === 0) return false;
  return true;
};
