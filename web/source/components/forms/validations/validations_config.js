const positive = v => (v > 0 ? v : undefined);
const o = v => (typeof v === 'object' ? v : undefined);
const n = v => (isNaN(v) ? undefined : positive(Number(v)));

export const digitsConf = (conf) => {
  const min = o(conf) ? n(conf.min) : n(conf);
  const max = o(conf) ? n(conf.max) : undefined;
  return { min, max };
};

export const lengthConf = (conf) => {
  const exact = o(conf) ? n(conf.length) : n(conf);
  const min = o(conf) ? n(conf.min) || n(conf.minLength) : undefined;
  const max = o(conf) ? n(conf.max) || n(conf.maxLength) : undefined;
  return { exact, min, max };
};

export const minLengthConf = conf => (o(conf) ? n(conf.min) || n(conf.minLength) : n(conf));

export const maxLengthConf = conf => (o(conf) ? n(conf.max) || n(conf.maxLength) : n(conf));

export const minNumberConf = conf => (o(conf) ? n(conf.min) : n(conf));

export const maxNumberConf = conf => (o(conf) ? n(conf.max) : n(conf));
