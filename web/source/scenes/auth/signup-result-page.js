import qs from 'query-string';
import { connect } from 'store';
import { navigate } from 'store/navigate';

import { login, signup, signedUp } from 'store/auth/actions';

import { getProfile, getError, isLoading } from 'store/auth';
import {
  profileShape,
  shouldAcceptTerms,
  shouldSelectPlan,
} from 'store/data/profile';

import Spinner from 'components/loading-spinner/loading-spinner';
import FullPage from 'components/layout/full-page';
import InfoMessage from 'components/layout/info-message';

@connect({
  profile: getProfile,
  loading: isLoading,
  error: getError,
}, {
  signedUp,
  login,
  signup,
  navigate,
})
export default class SignupResultPage extends Component {
  static propTypes = {
    location: PropTypes.shape({
      hash: PropTypes.string,
    }),
    profile: profileShape,
    loading: PropTypes.bool,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),

    signedUp: PropTypes.func.isRequired,
    login: PropTypes.func.isRequired,
    signup: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
  };

  componentWillMount() {
    const {
      id_token: token,
      state,
      error,
    } = qs.parse(this.props.location.hash);
    if (token) {
      return this.props.signedUp({ token, state });
    }
    if (error) {
      return this.setState({ error });
    }
    const { profile } = this.props;
    return profile && this.proceedWithProfile(profile);
  }

  componentWillReceiveProps(nextProps) {
    const { profile } = nextProps;
    return profile && this.proceedWithProfile(profile);
  }

  getError = () => this.props.error || (this.state || {}).error;

  proceedWithProfile = (profile) => {
    if (shouldAcceptTerms(profile)) {
      return this.props.navigate('/terms');
    }
    if (shouldSelectPlan(profile)) {
      return this.props.navigate('/select_plan');
    }
    return this.props.navigate('/');
  };

  login = () => this.props.login();

  signup = () => this.props.signup();

  render() {
    const { loading } = this.props;
    if (loading) return <FullPage><Spinner /></FullPage>;
    const error = this.getError();
    if (error) {
      return (
        <InfoMessage>
          The sign up failed or was cancelled.
          Please <a onClick={this.signup}>click here</a> to
          try again or <a onClick={this.login}>proceed to log in</a>.
        </InfoMessage>
      );
    }
    return (
      <InfoMessage>
        Profile loaded, proceed to next page.
      </InfoMessage>
    );
  }
}
