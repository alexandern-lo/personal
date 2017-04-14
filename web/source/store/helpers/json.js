export const parseJson = (json) => {
  try {
    return JSON.parse(json);
  } catch (_) {
    return null;
  }
};
