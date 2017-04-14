import { connect } from 'store';
import { redirect } from 'store/navigate';

import { profileShape, shouldAcceptTerms } from 'store/data/profile';
import { termsShape } from 'store/data/terms';

import { getProfile } from 'store/auth';
import { getLatestTerms, getError, isLoading } from 'store/terms';
import { fetchTerms, acceptTerms } from 'store/terms/actions';

import { logout } from 'store/auth/actions';

import InfoMessage from 'components/layout/info-message';

@connect({
  profile: getProfile,
  terms: getLatestTerms,
  error: getError,
  loading: isLoading,
}, {
  fetchTerms,
  acceptTerms,
  logout,
  redirect,
})
export default class TermsPage extends Component {
  static propTypes = {
    profile: profileShape.isRequired,
    terms: termsShape,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    loading: PropTypes.bool,

    fetchTerms: PropTypes.func.isRequired,
    acceptTerms: PropTypes.func.isRequired,
    logout: PropTypes.func.isRequired,
    redirect: PropTypes.func.isRequired,
  };

  componentWillMount() {
    this.props.fetchTerms();
  }

  getError = () => this.props.error || (this.state || {}).error;

  accept = () => {
    const promise = this.props.acceptTerms(this.props.terms.uid);
    this.setState({ promise, error: null });
    promise
      .then(() => this.props.redirect('/'))
      .catch(error => this.setState({ promise: null, error }));
  };

  decline = () => this.props.logout();

  render() {
    const { profile, terms, loading } = this.props;
    const { promise } = this.state || {};
    const error = this.getError();
    if (error && !terms) {
      return (
        <InfoMessage>
          Error occured: {error.message}
          <br />
          Please retry in several minutes.
          <br />
          If the problem persists, please contact our support team.
        </InfoMessage>
      );
    }
    if (loading || !terms) {
      return <InfoMessage>Loading terms...</InfoMessage>;
    }
    if (promise) {
      return <InfoMessage>Accepting terms...</InfoMessage>;
    }
    return (
      <div className='terms-container'>
        <div
          className='terms-text'
          dangerouslySetInnerHTML={{ __html: terms.text }} // eslint-disable-line react/no-danger
        />
        { (shouldAcceptTerms(profile) || true) && (
          <div className='terms-buttons-container'>
            <button className='btn-accept-terms' onClick={this.accept}>Accept</button>
            <button className='btn-decline-terms' onClick={this.decline}>Decline</button>
          </div>
        )}
        { error && <p>{error.message}</p> }
      </div>
    );
  }
}
