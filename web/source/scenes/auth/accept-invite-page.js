import qs from 'query-string';
import { connect } from 'store';
import { navigate } from 'store/navigate';

import { getInvite, isInviteLoading, getInviteError } from 'store/auth';
import { fetchInvite, signup } from 'store/auth/actions';

import InfoMessage from 'components/layout/info-message';

@connect({
  invite: getInvite,
  loading: isInviteLoading,
  error: getInviteError,
}, {
  fetchInvite,
  signup,
  navigate,
})
export default class LoginPage extends Component {

  static propTypes = {
    location: PropTypes.shape({
      search: PropTypes.string,
    }).isRequired,
    invite: PropTypes.shape({}),
    loading: PropTypes.bool,
    error: PropTypes.shape({
      message: PropTypes.string,
    }),
    fetchInvite: PropTypes.func.isRequired,
    signup: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
  };

  componentWillMount() {
    const code = this.getInviteCode();
    if (!code) {
      this.props.navigate('/');
    } else {
      this.props.fetchInvite(code);
    }
  }

  componentWillReceiveProps(nextProps) {
    const { invite } = nextProps;
    if (invite) {
      const code = this.getInviteCode();
      this.props.signup({ ...invite, code });
    }
  }

  getInviteCode = () => qs.parse(this.props.location.search).invite;

  render() {
    const { invite, loading, error } = this.props;
    if (error) {
      return (
        <InfoMessage>
          Error: {error.message}. Please retry in several minutes.
          <br />
          If the problem persists, please contact our support team.
        </InfoMessage>
      );
    }
    if (!invite && !loading) {
      return (
        <InfoMessage>
          Server error occurred. Please retry in several minutes.
          <br />
          If the problem persists, please contact our support team.
        </InfoMessage>
      );
    }
    return (
      <InfoMessage>
        Processing invite...
      </InfoMessage>
    );
  }
}
