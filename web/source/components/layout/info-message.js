import FullPage from './full-page';

import styles from './info-message.module.css';

const InfoMessage = ({ children }) => (
  <FullPage className={styles.message}>
    <p>{children}</p>
  </FullPage>
);
export default InfoMessage;

InfoMessage.propTypes = {
  children: PropTypes.node,
};
