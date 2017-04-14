import typeReader from '../helpers/type_reader';
import typeShape, { required } from '../helpers/type_shape';

export const SALESFORCE_TYPE = 'salesforce';
export const DYNAMICS365_TYPE = 'dynamics_365';

export const CrmConfigType = {
  uid: required(String),
  name: required(String),
  type: required(String), // salesforce, dynamics_365
  url: String,
  authorized: required(Boolean),

  // internals
  authorization_url: required(String),
  sync_fields: {},
};

export const readCrmConfig = typeReader(CrmConfigType);
export const crmConfigShape = typeShape(CrmConfigType);

const responsePaths = {
  [SALESFORCE_TYPE]: '/salesforce_response',
  [DYNAMICS365_TYPE]: '/dynamics365_response',
};

export const getAuthorizationUrl = (crm) => {
  const url = crm && crm.authorization_url ? new URL(crm.authorization_url) : null;
  if (!url) return null;
  const params = url.searchParams;
  params.set('state', crm.uid);
  const { protocol, host, hostname } = window.location;
  if (hostname === 'localhost') {
    params.set('redirect_uri', `${protocol}//${host}${responsePaths[crm.type]}`);
  }
  return url.toString();
};
