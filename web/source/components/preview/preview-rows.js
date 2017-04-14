import { formatUtcDate, formatUtcTime } from 'helpers/dates';
import { extractFileName } from 'helpers/url';

import { PreviewRow, PreviewSpan } from './preview-table';
import styles from './preview-table.module.css';

export const GroupRow = ({ className, label }) => (
  <PreviewRow className={classNames(className, styles.groupRow)} label={label} />
);
GroupRow.propTypes = {
  className: PropTypes.string,
  label: PropTypes.string.isRequired,
};

export const TextRow = ({ className, label, text }) => (
  <PreviewRow className={className} label={label}>{text}</PreviewRow>
);
TextRow.propTypes = {
  className: PropTypes.string,
  label: PropTypes.string,
  text: PropTypes.string,
};

export const ImageRow = ({ className, imageUrl, onClick }) => (
  <PreviewSpan className={className}>
    <div className={styles.imageContainer} onClick={onClick}>
      {imageUrl
        ? (<img src={imageUrl} alt='' />)
        : (<div className={styles.imagePlaceholder} />)
      }
    </div>
  </PreviewSpan>
);
ImageRow.propTypes = {
  className: PropTypes.string,
  imageUrl: PropTypes.string,
  onClick: PropTypes.func,
};

export const DateRow = ({ className, label, date }) => (
  <PreviewRow className={className} label={label}>{formatUtcDate(date)}</PreviewRow>
);
DateRow.propTypes = {
  className: PropTypes.string,
  label: PropTypes.string,
  date: PropTypes.string,
};

export const TimeRow = ({ className, label, time }) => (
  <PreviewRow className={className} label={label}>{formatUtcTime(time)}</PreviewRow>
);
TimeRow.propTypes = {
  className: PropTypes.string,
  label: PropTypes.string,
  time: PropTypes.string,
};

export const FileUrlRow = ({ className, label, url }) => (
  <PreviewRow className={className} label={label}>
    {url && (
      <a href={url} download target='_blank' rel='noopener noreferrer'>
        { extractFileName(url) }
      </a>
    )}
  </PreviewRow>
);
FileUrlRow.propTypes = {
  className: PropTypes.string,
  label: PropTypes.string,
  url: PropTypes.string,
};

const fixUrlSchema = url => url && (url.indexOf('://') === -1 ? `http://${url}` : url);

export const UrlRow = ({ className, label, url, text }) => {
  const fixedUrl = fixUrlSchema(url);
  return (
    <PreviewRow className={className} label={label}>
      {fixedUrl && <a href={fixedUrl}>{text || fixedUrl}</a>}
    </PreviewRow>
  );
};
UrlRow.propTypes = {
  className: PropTypes.string,
  label: PropTypes.string,
  url: PropTypes.string,
  text: PropTypes.string,
};
