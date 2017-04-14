import { connect } from 'store';
import { navigate } from 'store/navigate';

import { getProfile } from 'store/auth';
import { profileShape, isAnon } from 'store/data/profile';
import { login, signup } from 'store/auth/actions';

@connect({
  profile: getProfile,
}, {
  login,
  signup,
  navigate,
})
export default class LoginPage extends Component {

  static propTypes = {
    profile: profileShape,
    login: PropTypes.func.isRequired,
    signup: PropTypes.func.isRequired,
    navigate: PropTypes.func.isRequired,
  };

  componentWillMount() {
    this.checkAuth(this.props.profile);
  }

  componentWillReceiveProps(nextProps) {
    this.checkAuth(nextProps.profile);
  }

  checkAuth = (profile) => {
    if (!isAnon(profile)) {
      this.props.navigate('/');
    }
  };

  login = () => this.props.login();
  signup = () => this.props.signup();

  render() {
    return (
      <div className='login-container'>
        <button className='btn-login' onClick={this.login}>Login</button>
        <p>or</p>
        <button className='btn-create-account' onClick={this.signup}>Create account</button>
      </div>
    );
  }
}
