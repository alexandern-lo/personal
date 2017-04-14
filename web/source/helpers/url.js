
const fileUrlRegex = /(.*)(\/[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}-)/;

export const extractFileName = url => (url && url.length > 0 ? url.replace(fileUrlRegex, '') : null);
