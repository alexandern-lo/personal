import { defaultCountry } from 'config/geography';
import typeReader from '../helpers/type_reader';
import typeShape, { optional, required } from '../helpers/type_shape';

const EventType = {
  uid: required(String, 'event_uid'),
  type: required(String, 'event_type'), // conference, personal
  name: required(String, 'name'),
  startDate: required(String, 'start_date'),
  endDate: optional(String, 'end_date'),
  recurring: optional(Boolean, 'recurring'),
  venueName: optional(String, 'venue_name'),
  industry: optional(String, 'industry'),
  address: optional(String, 'address'),
  city: optional(String, 'city'),
  state: optional(String, 'state'),
  zipCode: optional(String, 'zip_code'),
  country: optional(String, 'country'),
  logo: optional(String, 'logo_url'),
  url: optional(String, 'website_url'),
  tenant: optional(String, 'tenant'),
  detailsOnly: required(Boolean, 'details_only'),
  owner: {
    uid: required(String, 'uid'),
    email: optional(String, 'email'),
    role: optional(String, 'role'),
  },
  categories: [
    {
      category_uid: required(String),
      name: required(String, 'name'),
      options: [
        {
          option_uid: required(String),
          name: required(String, 'name'),
        },
      ],
    },
  ],
};

export const readEvent = typeReader(EventType);
export const eventShape = typeShape(EventType);

export const makeBlankEvent = () => ({ country: defaultCountry });

export const INDUSTRIES = ['Aerospace & Defense', 'Agency', 'Agriculture', 'Automotive', 'Business & Professional Services', 'Chemicals', 'Conference & Event Services', 'Construction', 'Consumer Goods & Services', 'Electric Power Industry', 'Energy Industry', 'Financial Services', 'Firearms & Explosives', 'Food & Beverage', 'Government', 'Health Care', 'Housing & Real Estate', 'IT consulting', 'IT services', 'Manufacturing', 'Mining & Drilling', 'Nuclear Power Industry', 'Oil and Gas Industry', 'Pharmaceuticals & Biotechnology', 'Printing & Publishing', 'Software', 'Technology (other)', 'Telecommunications & Media', 'Transportation & Logistics', 'Other'];

export const isConference = event => event && event.type === 'conference';
export const isPersonal = event => event && event.type === 'personal';

export const isOwnedBy = (event, userId) => event && event.owner && event.owner.uid === userId;

export const editEvent = event => (event && {
  event_uid: event.uid,
  event_type: event.type,
  name: event.name,
  start_date: event.startDate,
  end_date: event.endDate,
  recurring: event.recurring,
  venue_name: event.venueName,
  industry: event.industry,
  address: event.address,
  city: event.city,
  state: event.state,
  country: event.country,
  zip_code: event.zipCode,
  logo_url: event.logo,
  website_url: event.url,
});
