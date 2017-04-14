import typeReader from '../helpers/type_reader';
import typeShape, { required, optional } from '../helpers/type_shape';

const SubscriptionPlanType = {
  name: required(String),
  trial: required(Boolean),
  description: String,
  priceInCents: required(Number, 'cents_per_user'),
  activationUrl: optional(String, 'activation_url'),
};

export const readPlan = typeReader(SubscriptionPlanType);
export const planShape = typeShape(SubscriptionPlanType);
