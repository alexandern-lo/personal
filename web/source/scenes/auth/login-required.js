import qs from 'query-string';
import { connect } from 'store';
import { parseJson } from 'store/helpers/json';
import { navigate, redirect } from 'store/navigate';

import { haveToken } from 'store/api';
import { getProfile, getError, isLoading } from 'store/auth';
import { fetchProfile, loggedIn } from 'store/auth/actions';
import {
  profileShape,
  isAnon,
  shouldAcceptTerms,
  shouldSelectPlan,
  shouldFillProfile,
} from 'store/data/profile';

import InfoMessage from 'components/layout/info-message';

const ORIGINAL_LOCATION_KEY = '__original_url__';

const isAuthError = error => error && error.status === 401;

export default function loginRequired(Base, { preserveURL } = {}) {
  const displayName = `Authenticated(${Base.displayName || Base.name || 'Component'})`;

  @connect({
    haveToken,
    profile: getProfile,
    loading: isLoading,
    error: getError,
  }, {
    fetchProfile,
    loggedIn,
    navigate,
    redirect,
  })
  class AuthenticatedComponent extends Component {
    static WrappedComponent = Base;
    static displayName = displayName;
    static propTypes = {
      location: PropTypes.shape({
        pathname: PropTypes.string,
        hash: PropTypes.string,
      }),
      haveToken: PropTypes.bool,
      profile: profileShape,
      loading: PropTypes.bool,
      error: PropTypes.shape({
        message: PropTypes.string,
      }),

      fetchProfile: PropTypes.func.isRequired,
      loggedIn: PropTypes.func.isRequired,
      navigate: PropTypes.func.isRequired,
      redirect: PropTypes.func.isRequired,
    };

    componentWillMount() {
      const { location } = this.props;
      const { id_token: token } = qs.parse(location.hash);
      if (token) {
        this.props.loggedIn(token);
        return this.props.redirect({ ...location, hash: null });
      }
      if (!this.props.haveToken) return this.navigate('/login');
      const { profile, loading, error } = this.props;
      if (loading) return null;
      if (error) return this.checkError(error);
      if (profile) return this.checkProfile(profile);
      return this.props.fetchProfile();
    }

    componentWillReceiveProps(nextProps) {
      const { profile, loading, error } = nextProps;
      if (loading) return null;
      if (error) return this.checkError(error);
      return this.checkProfile(profile, nextProps);
    }

    checkError = error => isAuthError(error) && this.navigate('/login');

    checkProfile = (profile, nextProps = {}) => {
      if (isAnon(profile)) {
        return this.navigate('/login');
      }
      if (shouldAcceptTerms(profile)) {
        return this.navigate('/terms');
      }
      if (shouldSelectPlan(profile)) {
        return this.navigate('/select_plan');
      }
      const pathname = (nextProps.location && nextProps.location.pathname) || '';
      if (shouldFillProfile(profile) && pathname !== '/profile') {
        return this.props.navigate({
          pathname: '/profile',
          state: { newUser: true },
        });
      }
      if (preserveURL) {
        const originalLocation = this.loadOriginalLocation();
        if (originalLocation) {
          this.clearOriginalLocation();
          return this.props.redirect(originalLocation);
        }
      }
      return null;
    };

    navigate = (path) => {
      const { location } = this.props;
      if (preserveURL) {
        this.saveOriginalLocation(location);
      }
      if (location.pathname !== path) {
        this.props.navigate(path);
      }
    };

    saveOriginalLocation = location => (
      sessionStorage.setItem(ORIGINAL_LOCATION_KEY, JSON.stringify(location))
    );

    clearOriginalLocation = () => sessionStorage.removeItem(ORIGINAL_LOCATION_KEY);

    loadOriginalLocation = () => parseJson(sessionStorage.getItem(ORIGINAL_LOCATION_KEY));

    render() {
      const { profile, loading, error } = this.props;
      const pass = !isAnon(profile) && !shouldAcceptTerms(profile) && !shouldSelectPlan(profile);
      const inProgress = loading || (preserveURL && !pass);
      if (error) {
        return (
          <InfoMessage>
            Server error occurred. Please retry in several minutes.
            <br />
            If the problem persists, please contact our support team.
          </InfoMessage>
        );
      }
      if (inProgress) {
        return (
          <InfoMessage>
            Verifying your subscription, please wait...
          </InfoMessage>
        );
      }
      return Base
          ? <Base {...this.props} />
          : null;
    }
  }

  return AuthenticatedComponent;
}
