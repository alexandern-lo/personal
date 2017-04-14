import _ from 'lodash';
import qs from 'query-string';
import { connect } from 'store';
import { navigate } from 'store/navigate';
import { Link } from 'react-router';

import { profileShape } from 'store/data/profile';
import { planShape } from 'store/data/subscription-plan';

import { getProfile } from 'store/auth';
import { getPlans, getError, isLoading } from 'store/plans';
import { isDevelopment } from 'helpers/location';

import { fetchPlans } from 'store/plans/actions';
import { refreshSubscription, startTrial } from 'store/auth/actions';

import FullPage from 'components/layout/full-page';
import InfoMessage from 'components/layout/info-message';
import PlanCard from './plan-card';

import styles from './select-plan-page.module.css';

@connect({
  profile: getProfile,
  loading: isLoading,
  error: getError,
  plans: getPlans,
}, {
  fetchPlans,
  refreshSubscription,
  startTrial,
  navigate,
})
export default class SelectPlanPage extends Component {
  static propTypes = {
    location: PropTypes.shape({
      hash: PropTypes.string,
    }).isRequired,
    profile: profileShape,
    loading: PropTypes.bool,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    plans: PropTypes.arrayOf(planShape),

    fetchPlans: PropTypes.func.isRequired,
    refreshSubscription: PropTypes.func.isRequired,
    startTrial: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
  };

  componentWillMount() {
    this.props.refreshSubscription();
    this.props.fetchPlans(isDevelopment ? { env: 'local' } : null);
  }

  onRefPaymentFrame = (frame) => {
    this.paymentFrame = frame;
    if (!frame) return;
    if (frame.attachEvent) {
      frame.attachEvent('onload', this.onFrameLoaded);
    } else {
      frame.onload = this.onFrameLoaded; // eslint-disable-line no-param-reassign
    }
  };

  onFrameLoaded = () => {
    const { document: doc } = this.paymentFrame.contentWindow;
    const { hostname } = doc.location;
    if (hostname === window.location.hostname) {
      const selectedPlan = this.getSelectedPlan();
      this.applyPlan(selectedPlan);
    }
  };

  getSelectedPlan = () => {
    const { plan: name } = qs.parse(this.props.location.hash);
    return _.find(this.props.plans, p => p.name === name);
  };

  getError = () => this.props.error || (this.state || {}).error;

  applyPlan = (plan) => {
    const promise = plan.trial ? this.props.startTrial() : this.props.refreshSubscription();
    this.setState({ promise, success: null, error: null });
    promise
      .then(() => this.setState({ promise: null, success: true }))
      .catch(error => this.setState({ promise: null, error }));
  };

  selectPlan = (plan) => {
    if (plan.trial) {
      this.applyPlan(plan);
    } else {
      this.props.navigate({
        ...this.props.location,
        hash: `#${qs.stringify({ plan: plan.name })}`,
      });
    }
  };

  renderPayment = (plan) => {
    const { profile } = this.props;
    const { firstName, lastName, email } = profile.user;
    const url = new URL(plan.activationUrl);
    const params = url.searchParams;
    if (firstName) params.set('first_name', firstName);
    if (lastName) params.set('last_name', lastName);
    if (email) params.set('email', email);
    return (
      <FullPage>
        <iframe
          sandbox='allow-forms allow-same-origin allow-scripts'
          src={url.toString()}
          frameBorder='0'
          width='700px'
          height='700px'
          ref={this.onRefPaymentFrame}
        />
      </FullPage>
    );
  }

  render() {
    const { loading, plans } = this.props;
    const selectedPlan = this.getSelectedPlan();
    const { promise, success } = this.state || {};
    const error = this.getError();
    if (error) {
      return (
        <InfoMessage>
          Error occured: {error.message}
          <br />
          Please retry in several minutes.
          If the problem persists, please contact our support team.
        </InfoMessage>
      );
    }
    if (loading || !plans) {
      return <InfoMessage>Loading plans...</InfoMessage>;
    }
    if (promise) {
      return <InfoMessage>Applying plan...</InfoMessage>;
    }
    if (success) {
      return (
        <InfoMessage>
          Thank you for choosing Avend!
          <br />
          Please <Link to='/'>proceed to the website</Link> now.
        </InfoMessage>
      );
    }
    if (selectedPlan) return this.renderPayment(selectedPlan);
    const haveTrial = plans && plans.length === 3;
    return (
      <FullPage>
        <h1 className={styles.header}>Subscription plans</h1>
        <div className={classNames(styles.plansContainer, { [styles.withTrial]: haveTrial })}>
          { _.map(plans, plan => (
            <PlanCard
              key={plan.name}
              plan={plan}
              onSubscribe={() => this.selectPlan(plan)}
            />
          ))}
        </div>
      </FullPage>
    );
  }
}
