
const getKeys = type => (typeof type === 'object') && Object.keys(type);
const keysEmpty = keys => keys && keys.length === 0;

export const isAnyType = type => keysEmpty(getKeys(type));

const customProps = {
  type: true,
  read: true,
  from: true,
  required: true,
};

export const isCustomType = (type) => {
  const keys = getKeys(type);
  if (!keys) return false;
  return keys.reduce((custom, key) => custom && customProps[key], true);
};

export const getCustomProp = type => type.from;
export const getCustomReader = type => type.read;
export const getCustomType = type => type.type;
export const isRequired = type => type.required;
