import { subscriptionShape } from 'store/data/subscription';
import { billingInfoShape } from 'store/data/billing-info';
import { timeLeft } from 'helpers/dates';
import styles from './account-settings-panel.module.css';

const AccountSettingsPanel = ({ subscription, billingInfo, onEditSubscription }) => (
  <div className={styles.settingsPanel}>
    <div className={styles.section}>
      <table className={styles.data}>
        <tr><td>Subscribtion type</td><td>{subscription.billingPeriod}</td></tr>
        <tr><td>Days left</td><td>{`${Math.max(timeLeft(subscription.expiresAt, 'days'), 0)} days`}</td></tr>
        <tr><td>Total user licenses</td><td>{subscription.maxUsers}</td></tr>
      </table>
      <button
        onClick={onEditSubscription}
        className={styles.button}
      >
        Edit subscription
      </button>
    </div>
    <div className={styles.title}>Payment information</div>
    <div className={styles.section}>
      <table className={styles.data}>
        <tr><td>Full name</td><td>{billingInfo.fullName}</td></tr>
        <tr><td>Credit card</td><td>{`xxxx-${billingInfo.creditCard}`}</td></tr>
        <tr className={styles.exp}><td>exp.</td><td>{`${billingInfo.expMonth}/${billingInfo.expYear}`}</td></tr>
        <tr><td>Address</td><td>{billingInfo.address}</td></tr>
        <tr><td>IP address</td><td>{billingInfo.ipAddress}</td></tr>
      </table>
    </div>
  </div>
);

export default AccountSettingsPanel;

AccountSettingsPanel.propTypes = {
  subscription: subscriptionShape.isRequired,
  billingInfo: billingInfoShape.isRequired,
  onEditSubscription: PropTypes.func.isRequired,
};
