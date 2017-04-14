import { planShape } from 'store/data/subscription-plan';

import styles from './plan-card.module.css';

export default class PlanCard extends Component {
  static propTypes = {
    plan: planShape.isRequired,
    onSubscribe: PropTypes.func.isRequired,
  };

  onSubscribe = () => this.props.onSubscribe(this.props.plan);

  render() {
    const { name, description, priceInCents } = this.props.plan;
    return (
      <div className={styles.plan}>
        <div className={styles.header}>
          <h3>{name}</h3>
          <span>Per user</span>
        </div>
        <div className={styles.price}>
          <sup>$</sup>
          <span>{priceInCents / 100}</span>
        </div>
        <div className={styles.description}>
          { description }
        </div>
        <div className={styles.action}>
          <button onClick={this.onSubscribe}>
            Subscribe
          </button>
        </div>
      </div>
    );
  }
}
