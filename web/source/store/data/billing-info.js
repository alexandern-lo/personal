import typeReader from '../helpers/type_reader';
import typeShape, { required } from '../helpers/type_shape';

export const BillingInfo = {
  fullName: required(String, 'full_name'),
  creditCard: required(String, 'cc_last_four'),
  expYear: required(Number, 'cc_exp_year'),
  expMonth: required(Number, 'cc_exp_month'),
  ipAddress: required(String, 'ip_address'),
  address: required(String, 'address'),
  editUrl: required(String, 'edit_url'),
};

export const readBillingInfo = typeReader(BillingInfo);
export const billingInfoShape = typeShape(BillingInfo);
