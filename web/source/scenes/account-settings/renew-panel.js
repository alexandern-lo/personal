import { timeLeft } from 'helpers/dates';
import { RedMessagePanel, BlueMessagePanel } from 'components/message-panel/message-panel';
import styles from './renew-panel.module.css';

const RenewPanel = ({ expire, onClear }) => {
  const daysLeft = timeLeft(expire, 'days');
  const Panel = daysLeft <= 7 ? RedMessagePanel : BlueMessagePanel;
  return (
    <Panel
      message={daysLeft > 0 ?
        (
          <span>
            Your next subscription renew is in: <b className={styles.days}>
              {daysLeft} days
            </b>
          </span>
        ) :
        (<span>Your subscription is expired</span>)
      }
      onClear={onClear}
    />
  );
};

export default RenewPanel;

RenewPanel.propTypes = {
  expire: PropTypes.string.isRequired,
  onClear: PropTypes.func.isRequired,
};
