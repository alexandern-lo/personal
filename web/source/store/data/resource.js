import typeReader from '../helpers/type_reader';
import typeShape, { optional, required } from '../helpers/type_shape';

import { UserType } from './user';
import { TenantType } from './tenant';

const ResourceType = {
  uid: required(String, 'resource_uid'),
  name: required(String, 'name'),
  type: required(String, 'type'),
  url: required(String, 'url'),
  description: optional(String, 'description'),
  createdAt: required(String, 'created_at'),
  user: UserType,
  tenant: TenantType,
};

export const resourceShape = typeShape(ResourceType);
export const readResource = typeReader(ResourceType);

const urlType = (url) => {
  const match = url ? url.match(/\.\w+$/) : null;
  return match ? match[0] : null;
};
export const getDisplayType = resource => resource && (urlType(resource.url) || resource.type);

export const editResource = resource => (resource && {
  uid: resource.uid,
  name: resource.name,
  description: resource.description,
  file: {
    url: resource.url,
    mimeType: resource.type,
  },
  type: resource.type,
});

export const dataResource = resource => (resource && {
  uid: resource.uid,
  name: resource.name,
  description: resource.description,
  url: resource.file.url,
  type: resource.file.mimeType,
});
