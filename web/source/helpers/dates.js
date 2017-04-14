import moment from 'moment';

moment.locale('en-US'); // window.navigator.userLanguage || window.navigator.language ||

export const format = (string, fmt, parseFormat) => {
  if (!string) return null;
  const date = moment(string, parseFormat);
  return date.isValid() ? date.local().format(fmt) : null;
};

export const formatUtcDateTime = dateTimeString => format(dateTimeString, 'll LT') || '-';

export const formatUtcDate = dateString => format(dateString, 'll') || '-';

export const formatUtcTime = timeString => format(timeString, 'LT', 'HH:mm::ss') || '-';

export const formatUtcDateInterval = (startDate, endDate) => [
  format(startDate, 'LL') || '',
  format(endDate, 'LL') || '',
].join(' - ');

export const timeLeft = (expire, units = 'days') => (
  expire && moment.utc(expire).diff(moment(), units)
);

export const listOfDatesBetween = (startDate, endDate) => {
  const start = moment.utc(startDate);
  const end = moment.utc(endDate);
  const duration = end.diff(start, 'days');
  const dates = [];
  for (let i = 0; i < duration; i += 1) {
    dates.push(moment(start).add(i, 'day'));
  }
  return dates;
};
