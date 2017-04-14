import qs from 'query-string';

import { connectActions } from 'store';
import { readCrmConfig } from 'store/data/crm-config';
import { grantCrmConfig } from 'store/crm-configs/actions';

import { Link } from 'react-router';

import InfoMessage from 'components/layout/info-message';

export const SalesforceResponsePage = props => (
  <ResponsePage message='Salesforce authorizing...' {...props} />
);

export const Dynamics365ResponsePage = props => (
  <ResponsePage message='Dynamics365 authorizing...' {...props} />
);

@connectActions({
  grantCrmConfig,
})
export default class ResponsePage extends Component {
  static propTypes = {
    location: PropTypes.shape({
      search: PropTypes.string.isRequired,
    }).isRequired,
    message: PropTypes.string.isRequired,
    grantCrmConfig: PropTypes.func.isRequired,
  };

  componentWillMount() {
    const { code, state } = qs.parse(this.props.location.search);
    if (code && state) {
      const promise = this.props.grantCrmConfig({ uid: state, token: code });
      this.setState({ promise });
      promise
        .then(res => this.setState({ promise: null, crmConfig: readCrmConfig(res.data) }))
        .catch(error => this.setState({ promise: null, error }));
    }
  }

  componentWillUnmount() {
    const { promise } = this.state || {};
    if (promise) {
      promise.cancel();
    }
  }

  render() {
    const { message } = this.props;
    const { crmConfig, error } = this.state || {};
    const back = <Link to='/crm'>Click here to go back to settings</Link>;
    if (error) {
      return <InfoMessage>An error occured while processing crm response.<br />{back}</InfoMessage>;
    }
    if (crmConfig) {
      return <InfoMessage>{crmConfig.name} successfully authorized.<br />{back}</InfoMessage>;
    }
    return <InfoMessage>{message}</InfoMessage>;
  }
}
