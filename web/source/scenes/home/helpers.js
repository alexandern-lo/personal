export const numberCommaFormat = number => number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
