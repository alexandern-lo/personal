export function diff(obj1, obj2, keys) {
  for (let i = 0; i < keys.length; i += 1) {
    const k = keys[i];
    if (obj1[k] !== obj2[k]) {
      return true;
    }
  }
  return false;
}
