
export const ellipsis = (str, max) => (
  (str && str.length >= max) ? `${str.slice(0, max - 1)}…` : str
);
