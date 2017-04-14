import { defaultCountry } from 'config/geography';
import typeReader from '../helpers/type_reader';
import typeShape, { required, optional } from '../helpers/type_shape';

export const TenantType = {
  uid: optional(String, 'tenant_uid'),
  name: required(String, 'company_name'),
  companySize: optional(String, 'company_size'),
  description: optional(String, 'description'),
  websiteUrl: optional(String, 'website_url'),
  phone: optional(String, 'phone'),
  industry: optional(String, 'industry'),
  country: optional(String, 'country'),
  state: optional(String, 'state'),
  city: optional(String, 'city'),
  address: optional(String, 'address'),
  zipCode: optional(String, 'zip_code'),
};

export const readTenant = typeReader(TenantType);
export const tenantShape = typeShape(TenantType);

export const isFilled = tenant => !!(tenant && tenant.name && tenant.industry &&
    tenant.address && tenant.country && tenant.city);

export const editTenant = tenant => tenant && {
  tenant_uid: tenant.uid,
  company_name: tenant.name,
  company_size: tenant.companySize,
  description: tenant.description,
  website_url: tenant.websiteUrl,
  phone: tenant.phone,
  industry: tenant.industry,
  country: tenant.country || defaultCountry,
  state: tenant.state,
  city: tenant.city,
  address: tenant.address,
  zip_code: tenant.zipCode,
};
