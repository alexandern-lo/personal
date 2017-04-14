import typeReader from '../helpers/type_reader';
import typeShape, { required, optional } from '../helpers/type_shape';

export const ROLE_SUPER_ADMIN = 'super_admin';
export const ROLE_TENANT_ADMIN = 'tenant_admin';
export const ROLE_USER = 'seat_user';

export const UserType = {
  uid: required(String),
  role: optional(String), // admin, user
  status: optional(String), // enabled, disabled, invited
  tenant: optional(String),
  firstName: optional(String, 'first_name'),
  lastName: optional(String, 'last_name'),
  jobTitle: optional(String, 'job_title'),
  email: String,
  city: String,
  state: String,
  lastInvitation: optional(String, 'last_invitation'),
  updatedAt: optional(String, 'updated_at'),
};

export const readUser = typeReader(UserType);
export const userShape = typeShape(UserType);

const haveRole = role => user => user && user.role === role;

export const isSuperAdmin = haveRole(ROLE_SUPER_ADMIN);
export const isTenantAdmin = haveRole(ROLE_TENANT_ADMIN);
export const isUser = haveRole(ROLE_USER);
export const isAnon = user => !user || !user.role;

export const isFilled = user => !!(user && user.firstName && user.lastName && user.email);

export const isAdmin = user => isSuperAdmin(user) || isTenantAdmin(user);
export const isEnabled = user => user && user.status === 'enabled';
export const isInvited = users => users && users.status === 'invited';

export const getDisplayName = (user) => {
  if (!user) return null;
  const { firstName, lastName, email } = user;
  if (firstName || lastName) return [firstName, lastName].join(' ');
  return email;
};

export const editUser = user => user && {
  uid: user.uid,
  role: user.role,
  status: user.status,
  tenant: user.tenant,
  first_name: user.firstName,
  last_name: user.lastName,
  job_title: user.jobTitle,
  email: user.email,
  city: user.city,
  state: user.state,
};
