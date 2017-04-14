import typeReader from '../helpers/type_reader';
import typeShape, { required } from '../helpers/type_shape';

export const SubscriptionType = {
  billingPeriod: required(String, 'billing_period'),
  activeUsers: required(Number, 'active_users'),
  maxUsers: required(Number, 'max_users'),
  expiresAt: required(String, 'expires_at'),
  expired: required(Boolean),
  trial: Boolean,
};

export const readSubscription = typeReader(SubscriptionType);
export const subscriptionShape = typeShape(SubscriptionType);
