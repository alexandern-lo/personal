import typeReader from '../helpers/type_reader';
import typeShape, { required } from '../helpers/type_shape';

import {
  TenantType,
  isFilled as isTenantDataFilled,
} from './tenant';
import { SubscriptionType } from './subscription';
import { TermsType } from './terms';
import { CrmConfigType } from './crm-config';

import {
  UserType,
  isFilled as isUserDataFilled,
  isSuperAdmin as isUserSuperAdmin,
  isTenantAdmin as isUserTenantAdmin,
  isUser as isSeatUser,
  isAnon as isAnonUser,
} from './user';

const ProfileType = {
  uid: required(String, 'user.uid'),
  user: {
    required: true,
    type: UserType,
  },
  tenant: TenantType,
  subscription: SubscriptionType,
  terms: {
    type: TermsType,
    from: 'accepted_terms',
  },
  crm: {
    type: CrmConfigType,
    from: 'default_crm',
  },
};

export const readProfile = typeReader(ProfileType);
export const profileShape = typeShape(ProfileType);
export const SIZES = ['1-10', '11-50', '51-100', '101-500', '500-1000', '1000+'];

export const isSuperAdmin = prof => prof && isUserSuperAdmin(prof.user);
export const isTenantAdmin = prof => prof && isUserTenantAdmin(prof.user);
export const isUser = prof => prof && isSeatUser(prof.user);
export const isAnon = prof => !prof || isAnonUser(prof.user);

export const shouldAcceptTerms = prof => prof && (!prof.terms || prof.terms.outdated);

export const shouldSelectPlan = prof => prof && !prof.subscription;

export const shouldFillProfile = prof => prof && !isSuperAdmin(prof) &&
    (!isUserDataFilled(prof.user) || !isTenantDataFilled(prof.tenant));
